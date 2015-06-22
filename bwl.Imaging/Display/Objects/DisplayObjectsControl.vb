Public Class DisplayObjectsControl
    Inherits DisplayBitmapControl
    Private _displayObjects As New List(Of DisplayObject)
    Public Property RedrawObjectsWhenCollectionChanged As Boolean = False

    Event ObjectSelect(sender As Object, selected As DisplayObject, e As MouseEventArgs)

    Public ReadOnly Property DisplayObjects As List(Of DisplayObject)
        Get
            Return _displayObjects
        End Get
    End Property

    Public Sub Add(displayObject As DisplayObject)
        SyncLock Me
            _displayObjects.Add(displayObject)
            If RedrawObjectsWhenCollectionChanged Then Refresh()
        End SyncLock
    End Sub

    Public Sub AddRange(displayObjects As IEnumerable(Of DisplayObject))
        SyncLock Me
            _displayObjects.AddRange(displayObjects)
            If RedrawObjectsWhenCollectionChanged Then Refresh()
        End SyncLock
    End Sub

    Public Sub Replace(displayObjects As DisplayObject())
        SyncLock Me
            _displayObjects.Clear()
            _displayObjects.AddRange(displayObjects)
            If RedrawObjectsWhenCollectionChanged Then Refresh()
        End SyncLock
    End Sub

    Public Sub Clear()
        SyncLock Me
            _displayObjects.Clear()
            If RedrawObjectsWhenCollectionChanged Then Refresh()
        End SyncLock
    End Sub

    Public Property BackgroundBitmap As Bitmap

    Public Sub Remove(id As String)
        SyncLock Me
            Dim copy As New List(Of DisplayObject)
            For Each obj In DisplayObjects
                If obj.ID <> id Then copy.Add(obj)
            Next
            _displayObjects.Clear()
            _displayObjects.AddRange(copy)
            If RedrawObjectsWhenCollectionChanged Then Refresh()
        End SyncLock
    End Sub

    Public Overrides Sub Refresh()
        RedrawObjects()
        MyBase.Refresh()
    End Sub

    Public Sub RedrawObjects()
        If BackgroundBitmap Is Nothing Then
            DisplayBitmap.Clear()
        Else
            Me.DisplayBitmap.DrawBitmap(BackgroundBitmap, 0, 0, 1, 1)
            ' _pictureBox.BackgroundImage = BackgroundBitmap
            ' _pictureBox.BackgroundImageLayout = ImageLayout.Stretch

            'Dim bmp = BackgroundBitmap 'New Bitmap(BackgroundBitmap, _pictureBox.Width, _pictureBox.Height)
            'Me.DisplayBitmap.Graphics.DrawImage(bmp, 0, 0)
        End If

        For Each obj In _displayObjects
            If obj.IsVisible Then DisplayBitmap.DrawDisplayObject(obj)
            If Object.Equals(obj, SelectedObject) Then
                Dim bound = DisplayBitmap.GetBoundRectangeF(obj.DrawObject)
                Dim bbound = DisplayBitmap.GetBitmapRectangle(bound)
                bbound.Inflate(2, 2)
                If Not MoveMode Then
                    DisplayBitmap.Graphics.DrawRectangle(SelectedObjectBorderStyle, bbound.Left, bbound.Top, bbound.Width, bbound.Height)
                Else
                    DisplayBitmap.Graphics.DrawRectangle(MovingObjectBorderStyle, bbound.Left, bbound.Top, bbound.Width, bbound.Height)
                End If
            End If
        Next
        If MoveMode Then
            For Each mp In MovePoints
                DisplayBitmap.DrawPoint(MoveModePointColor, mp.X, mp.Y)
            Next
        End If
        '  Me.Refresh()
    End Sub

    Public Function Find(idPart As String, groupPart As String) As DisplayObject()
        SyncLock Me
            Dim copy As New List(Of DisplayObject)
            For Each obj In DisplayObjects
                If obj.ID.Contains(idPart) And obj.Group.Contains(groupPart) Then copy.Add(obj)
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

    ' Public Event MoveModeChanged(sender As DisplayControl, moveMode As Boolean)
    Public Event DisplayObjectMoved(sender As DisplayObjectsControl, displayObject As DisplayObject)

    Private Sub _pictureBox_Click(sender As Object, e As MouseEventArgs) Handles _pictureBox.MouseClick
        If e.Button = Windows.Forms.MouseButtons.Right Then
            MoveMode = Not MoveMode
            If SelectedObject IsNot Nothing AndAlso SelectedObject.IsMoveable = False Then MoveMode = False
            MovePoints.Clear()
            Me.Refresh()
        End If
        If e.Button = Windows.Forms.MouseButtons.Left Then
            If MoveMode AndAlso SelectedObject IsNot Nothing Then
                MovePoints.Add(DisplayBitmap.GetObjectPoint(e.Location))
                If TypeOf SelectedObject.DrawObject Is Line AndAlso MovePoints.Count > 1 Then
                    With DirectCast(SelectedObject.DrawObject, Line)
                        .Point1 = MovePoints(0)
                        .Point2 = MovePoints(1)
                        MoveMode = False
                    End With
                    RaiseEvent DisplayObjectMoved(Me, SelectedObject.DrawObject)
                End If
                If TypeOf SelectedObject.DrawObject Is Rectangle AndAlso MovePoints.Count > 1 Then
                    SelectedObject.DrawObject = Rectangle.FromLTRB(MovePoints(0).X, MovePoints(0).Y, MovePoints(1).X, MovePoints(1).Y).ToPositiveSized
                    MoveMode = False
                    RaiseEvent DisplayObjectMoved(Me, SelectedObject)
                End If
                If TypeOf SelectedObject.DrawObject Is RectangleF AndAlso MovePoints.Count > 1 Then
                    SelectedObject.DrawObject = RectangleF.FromLTRB(MovePoints(0).X, MovePoints(0).Y, MovePoints(1).X, MovePoints(1).Y).ToPositiveSized
                    MoveMode = False
                    RaiseEvent DisplayObjectMoved(Me, SelectedObject)
                End If
                If TypeOf SelectedObject.DrawObject Is BitmapObject AndAlso MovePoints.Count > 1 Then
                    DirectCast(SelectedObject.DrawObject, BitmapObject).RectangleF = RectangleF.FromLTRB(MovePoints(0).X, MovePoints(0).Y, MovePoints(1).X, MovePoints(1).Y)
                    MoveMode = False
                    RaiseEvent DisplayObjectMoved(Me, SelectedObject)
                End If
                If TypeOf SelectedObject.DrawObject Is Tetragon AndAlso MovePoints.Count > 3 Then
                    With DirectCast(SelectedObject.DrawObject, Tetragon)
                        .Point1 = MovePoints(0)
                        .Point2 = MovePoints(1)
                        .Point3 = MovePoints(2)
                        .Point4 = MovePoints(3)
                        MoveMode = False
                        RaiseEvent DisplayObjectMoved(Me, SelectedObject)
                    End With
                End If
                If TypeOf SelectedObject.DrawObject Is PointF AndAlso MovePoints.Count > 0 Then
                    SelectedObject.DrawObject = MovePoints(0)
                    MoveMode = False
                    RaiseEvent DisplayObjectMoved(Me, SelectedObject)
                End If
                If TypeOf SelectedObject.DrawObject Is Point AndAlso MovePoints.Count > 0 Then
                    SelectedObject.DrawObject = New Point(MovePoints(0).X, MovePoints(0).Y)
                    MoveMode = False
                    RaiseEvent DisplayObjectMoved(Me, SelectedObject)
                End If
                If TypeOf SelectedObject.DrawObject Is PointC AndAlso MovePoints.Count > 0 Then
                    DirectCast(SelectedObject.DrawObject, PointC).PointF = New PointF(MovePoints(0).X, MovePoints(0).Y)
                    MoveMode = False
                    RaiseEvent DisplayObjectMoved(Me, SelectedObject)
                End If
                Me.Refresh()
            Else
                Dim list As New List(Of DisplayObject)
                For Each obj In _displayObjects
                    If DisplayBitmap.IsBitmapPointInsideBound(obj.DrawObject, e.X, e.Y) Then
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
            Refresh()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click
        If SelectedObject IsNot Nothing AndAlso SelectedObject.IsMoveable Then MoveMode = True
        MovePoints.Clear()
        Me.Refresh()
    End Sub

    Private Sub DisplayControl_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class
