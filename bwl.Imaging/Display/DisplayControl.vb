Public Class DisplayControl
    Inherits DrawBitmapControl
    Private _displayObjects As New List(Of DisplayObject)
    Public Property AutoRedraw As Boolean = True

    Event ObjectSelect(sender As Object, selected As DisplayObject, e As MouseEventArgs)

    Public ReadOnly Property DisplayObjects As List(Of DisplayObject)
        Get
            Return _displayObjects
        End Get
    End Property

    Public Sub Add(displayObject As DisplayObject)
        SyncLock Me
            _displayObjects.Add(displayObject)
            Refresh()
        End SyncLock
    End Sub

    Public Sub AddRange(displayObjects As DisplayObject())
        SyncLock Me
            _displayObjects.AddRange(displayObjects)
            If AutoRedraw Then Refresh()
        End SyncLock
    End Sub

    Public Sub Replace(displayObjects As DisplayObject())
        SyncLock Me
            _displayObjects.Clear()
            _displayObjects.AddRange(displayObjects)
            If AutoRedraw Then Refresh()
        End SyncLock
    End Sub

    Public Sub Clear()
        SyncLock Me
            _displayObjects.Clear()
            If AutoRedraw Then Refresh()
        End SyncLock
    End Sub

    Public Sub Remove(id As String)
        SyncLock Me
            Dim copy As New List(Of DisplayObject)
            For Each obj In DisplayObjects
                If obj.ID <> id Then copy.Add(obj)
            Next
            _displayObjects.Clear()
            _displayObjects.AddRange(copy)
            If AutoRedraw Then Refresh()
        End SyncLock
    End Sub

    Public Overrides Sub Refresh()
        Draw()
        MyBase.Refresh()
    End Sub

    Private Sub Draw()
        DrawBitmap.Clear()
        For Each obj In _displayObjects
            If obj.IsVisible Then DrawBitmap.DrawDisplayObject(obj)
            If Object.Equals(obj, SelectedObject) Then
                Dim bound = DrawBitmap.GetBoundRectangeF(obj.DrawObject)
                Dim bbound = DrawBitmap.GetBitmapRectangle(bound)
                bbound.Inflate(2, 2)
                If Not MoveMode Then
                    DrawBitmap.Graphics.DrawRectangle(SelectedObjectBorderStyle, bbound.Left, bbound.Top, bbound.Width, bbound.Height)
                Else
                    DrawBitmap.Graphics.DrawRectangle(MovingObjectBorderStyle, bbound.Left, bbound.Top, bbound.Width, bbound.Height)
                End If
            End If
        Next
        If MoveMode Then
            For Each mp In MovePoints
                DrawBitmap.DrawPoint(MoveModePointColor, mp.X, mp.Y)
            Next
        End If
    End Sub

    Public Function Items(id As String) As DisplayObject()
        SyncLock Me
            Dim copy As New List(Of DisplayObject)
            For Each obj In DisplayObjects
                If obj.ID = id Then copy.Add(obj)
            Next
            Return copy.ToArray
        End SyncLock
    End Function

    Public Function Item(id As String) As DisplayObject
        SyncLock Me
            Dim copy As New List(Of DisplayObject)
            For Each obj In DisplayObjects
                Return obj
            Next
            Return Nothing
        End SyncLock
    End Function

    Public Property SelectedObject As DisplayObject
    Public Property SelectedObjectBorderStyle As New Pen(New SolidBrush(Color.Blue), 1) With {.DashPattern = {2.0F, 2.0F}}
    Public Property MovingObjectBorderStyle As New Pen(New SolidBrush(Color.Red), 2) With {.DashPattern = {2.0F, 2.0F}}

    Public Property MoveModePointColor As Color = Color.Red

    Public Property MoveMode As Boolean
    Public Property MovePoints As New List(Of PointF)

    Private Sub _pictureBox_Click(sender As Object, e As MouseEventArgs) Handles _pictureBox.MouseClick
        If e.Button = Windows.Forms.MouseButtons.Right Then
            MoveMode = Not MoveMode
            MovePoints.Clear()
            Me.Refresh()
        End If
        If e.Button = Windows.Forms.MouseButtons.Left Then
            If MoveMode Then
                MovePoints.Add(DrawBitmap.GetObjectPoint(e.Location))
                If TypeOf SelectedObject.DrawObject Is Line AndAlso MovePoints.Count > 1 Then
                    With DirectCast(SelectedObject.DrawObject, Line)
                        .Point1 = MovePoints(0)
                        .Point2 = MovePoints(1)
                        MoveMode = False
                    End With
                End If
                If TypeOf SelectedObject.DrawObject Is Rectangle AndAlso MovePoints.Count > 1 Then
                    SelectedObject.DrawObject = Rectangle.FromLTRB(MovePoints(0).X, MovePoints(0).Y, MovePoints(1).X, MovePoints(1).Y)
                    MoveMode = False
                End If
                If TypeOf SelectedObject.DrawObject Is RectangleF AndAlso MovePoints.Count > 1 Then
                    SelectedObject.DrawObject = RectangleF.FromLTRB(MovePoints(0).X, MovePoints(0).Y, MovePoints(1).X, MovePoints(1).Y)
                    MoveMode = False
                End If
                If TypeOf SelectedObject.DrawObject Is BitmapObject AndAlso MovePoints.Count > 1 Then
                    DirectCast(SelectedObject.DrawObject, BitmapObject).RectangleF = RectangleF.FromLTRB(MovePoints(0).X, MovePoints(0).Y, MovePoints(1).X, MovePoints(1).Y)
                    MoveMode = False
                End If
                If TypeOf SelectedObject.DrawObject Is Tetragon AndAlso MovePoints.Count > 3 Then
                    With DirectCast(SelectedObject.DrawObject, Tetragon)
                        .Point1 = MovePoints(0)
                        .Point2 = MovePoints(1)
                        .Point3 = MovePoints(2)
                        .Point4 = MovePoints(3)
                        MoveMode = False
                    End With
                End If
                If TypeOf SelectedObject.DrawObject Is PointF AndAlso MovePoints.Count > 0 Then
                    SelectedObject.DrawObject = MovePoints(0)
                    MoveMode = False
                End If
                If TypeOf SelectedObject.DrawObject Is Point AndAlso MovePoints.Count > 0 Then
                    SelectedObject.DrawObject = New Point(MovePoints(0).X, MovePoints(0).Y)
                    MoveMode = False
                End If
                If TypeOf SelectedObject.DrawObject Is PointC AndAlso MovePoints.Count > 0 Then
                    DirectCast(SelectedObject.DrawObject, PointC).PointF = New PointF(MovePoints(0).X, MovePoints(0).Y)
                    MoveMode = False
                End If
                Me.Refresh()
            Else
                Dim list As New List(Of DisplayObject)
                For Each obj In _displayObjects
                    If DrawBitmap.IsBitmapPointInsideBound(obj.DrawObject, e.X, e.Y) Then
                        list.Add(obj)
                    End If
                Next
                Static index As Integer
                If list.Count > 0 Then
                    index += 1
                    If index > list.Count - 1 Then index = 0
                    SelectedObject = list(index)
                    RaiseEvent ObjectSelect(sender, SelectedObject, e)
                    Label1.Text = SelectedObject.ID
                    If SelectedObject.Caption > "" Then Label1.Text = SelectedObject.Caption
                    Refresh()
                Else
                    SelectedObject = Nothing
                    Label1.Text = "-"
                    Refresh()
                End If
            End If
        End If
    End Sub

    Private Sub DisplayControl_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Try
            If AutoRedraw Then Refresh()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click
        If SelectedObject IsNot Nothing AndAlso SelectedObject.IsMoveable Then MoveMode = True
        MoveMode = True
        MovePoints.Clear()
        Me.Refresh()
    End Sub
End Class
