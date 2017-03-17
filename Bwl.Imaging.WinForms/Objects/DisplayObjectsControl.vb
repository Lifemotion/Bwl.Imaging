Public Class DisplayObjectsControl
    Inherits DisplayBitmapControl
    Private _displayObjects As New List(Of DisplayObject)

    Private _bkgX1F As Single = 0
    Private _bkgY1F As Single = 0
    Private _bkgX2F As Single = 1.0
    Private _bkgY2F As Single = 1.0

    Private _clickPointF As PointF
    Private _showClickPoint As Boolean = False

    Public ReadOnly Property ClickPoint
        Get
            Return _clickPointF
        End Get
    End Property

    Public ReadOnly Property DisplayObjects As List(Of DisplayObject)
        Get
            Return _displayObjects
        End Get
    End Property

    Public Property BackgroundBitmap As Bitmap
    Public Property BackgroundColor As Color
    Public Property KeepBackgroundAspectRatio As Boolean = True
    Public Property RedrawObjectsWhenCollectionChanged As Boolean = False
    Public Property SelectedObject As DisplayObject
    Public Property SelectedObjectBorderStyle As New Pen(New SolidBrush(Color.Blue), 1) With {.DashPattern = {2.0F, 2.0F}}
    Public Property MovingObjectBorderStyle As New Pen(New SolidBrush(Color.Red), 2) With {.DashPattern = {2.0F, 2.0F}}
    Public Property MoveModePointColor As Color = Color.Red
    Public Property MoveMode As Boolean
    Public Property MovePoints As New List(Of PointF)
    Public Property ShowStatusBar As Boolean = True
    Public Property ShowClickPoint As Boolean
        Get
            Return _showClickPoint
        End Get
        Set(value As Boolean)
            If value Then
                _clickPointFLabel.Visible = True
            Else
                _clickPointFLabel.Visible = False
            End If
            _showClickPoint = value
        End Set
    End Property

    Public Event ObjectSelected(sender As Object, selected As DisplayObject, e As MouseEventArgs)
    Public Event DisplayObjectMoved(sender As DisplayObjectsControl, displayObject As DisplayObject)
    Public Event DisplayObjectSelected(sender As DisplayObjectsControl, displayObject As DisplayObject)
    Public Event DisplayRightBtnMouseClick(sender As DisplayObjectsControl, ByRef needClearFeedback As Boolean)
    Public Event MoveModeChanged(sender As DisplayObjectsControl, moveMode As Boolean)
    Public Event MouseClickF(sender As Object, e As MouseEventArgs, clickPointF As PointF)
    Public Event MouseClickOnBackgroundF(sender As Object, e As MouseEventArgs, clickPointF As PointF)

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
        If ShowStatusBar Then
            _pictureBox.Dock = DockStyle.None
        Else
            _pictureBox.Dock = DockStyle.Fill
        End If
        RedrawObjects()
        MyBase.Refresh()
    End Sub

    Public Sub RedrawObjects()
        DisplayBitmap.BackgroundColor = Me.BackgroundColor
        If BackgroundBitmap Is Nothing OrElse KeepBackgroundAspectRatio Then
            DisplayBitmap.Clear()
        End If
        If Not BackgroundBitmap Is Nothing Then
            If KeepBackgroundAspectRatio Then
                DisplayBitmap.KeepAspectRatio(BackgroundBitmap.Width, BackgroundBitmap.Height)
            Else
                DisplayBitmap.KeepAspectRatio()
            End If
            DisplayBitmap.DrawBitmap(BackgroundBitmap, 0, 0, 1, 1)
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

    Public Sub ClearSelectedObjectAndPoints()
        MoveMode = Not MoveMode
        If SelectedObject IsNot Nothing AndAlso SelectedObject.IsMoveable = False Then MoveMode = False
        MovePoints.Clear()
        Me.Refresh()
    End Sub

    Private Sub _pictureBox_Click(sender As Object, e As MouseEventArgs) Handles _pictureBox.MouseClick
        Dim clickPointF = DisplayBitmap.GetObjectPoint(New PointF(e.X, e.Y))
        If ShowClickPoint Then
            _clickPointFLabel.Text = String.Format("X:{0}; Y:{1}", clickPointF.X.ToString("F1"), clickPointF.Y.ToString("F1"))
        End If
        _clickPointF = clickPointF
        RaiseEvent MouseClickF(sender, e, clickPointF)
        If clickPointF.X >= _bkgX1F AndAlso clickPointF.X <= _bkgX2F AndAlso clickPointF.Y >= _bkgY1F AndAlso clickPointF.Y <= _bkgY2F Then
            RaiseEvent MouseClickOnBackgroundF(sender, e, clickPointF)
            If e.Button = Windows.Forms.MouseButtons.Right Then
                Dim needClearFeedback = True
                RaiseEvent DisplayRightBtnMouseClick(Me, needClearFeedback)
                If needClearFeedback Then
                    ClearSelectedObjectAndPoints()
                End If
            End If
            If e.Button = Windows.Forms.MouseButtons.Left Then
                If MoveMode AndAlso SelectedObject IsNot Nothing Then
                    MovePoints.Add(clickPointF)

                    If GetType(Line).IsAssignableFrom(SelectedObject.DrawObject.GetType) AndAlso MovePoints.Count > 1 Then
                        With DirectCast(SelectedObject.DrawObject, Line)
                            .Point1 = MovePoints(0)
                            .Point2 = MovePoints(1)
                        End With
                        MoveMode = False
                        RaiseEvent DisplayObjectMoved(Me, SelectedObject)
                    End If

                    If GetType(TextObject).IsAssignableFrom(SelectedObject.DrawObject.GetType) AndAlso MovePoints.Count > 0 Then
                        With DirectCast(SelectedObject.DrawObject, TextObject)
                            .Point1 = MovePoints(0)
                        End With
                        MoveMode = False
                        RaiseEvent DisplayObjectMoved(Me, SelectedObject)
                    End If

                    If GetType(BitmapObject).IsAssignableFrom(SelectedObject.DrawObject.GetType) AndAlso MovePoints.Count > 1 Then
                        With DirectCast(SelectedObject.DrawObject, BitmapObject)
                            .RectangleF = RectangleF.FromLTRB(MovePoints(0).X, MovePoints(0).Y, MovePoints(1).X, MovePoints(1).Y)
                        End With
                        MoveMode = False
                        RaiseEvent DisplayObjectMoved(Me, SelectedObject)
                    End If

                    If GetType(Tetragon).IsAssignableFrom(SelectedObject.DrawObject.GetType) AndAlso MovePoints.Count > 3 Then
                        With DirectCast(SelectedObject.DrawObject, Tetragon)
                            .Point1 = MovePoints(0)
                            .Point2 = MovePoints(1)
                            .Point3 = MovePoints(2)
                            .Point4 = MovePoints(3)
                        End With
                        MoveMode = False
                        RaiseEvent DisplayObjectMoved(Me, SelectedObject)
                    End If

                    If GetType(PointC).IsAssignableFrom(SelectedObject.DrawObject.GetType) AndAlso MovePoints.Count > 0 Then
                        With DirectCast(SelectedObject.DrawObject, PointC)
                            .X = MovePoints(0).X
                            .Y = MovePoints(0).Y
                        End With
                        MoveMode = False
                        RaiseEvent DisplayObjectMoved(Me, SelectedObject)
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
                        RaiseEvent ObjectSelected(sender, SelectedObject, e)
                        _selectedObjectID.Text = SelectedObject.ID
                        If SelectedObject.Caption > "" Then _selectedObjectID.Text = SelectedObject.Caption
                        Me.Refresh()
                        RaiseEvent DisplayObjectSelected(Me, SelectedObject)
                    Else
                        SelectedObject = Nothing
                        _selectedObjectID.Text = "-"
                        Me.Refresh()
                        RaiseEvent DisplayObjectSelected(Me, SelectedObject)
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub DisplayControl_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Try
            Me.Refresh()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub _selectedObjectID_Click(sender As Object, e As EventArgs) Handles _selectedObjectID.Click
        If SelectedObject IsNot Nothing AndAlso SelectedObject.IsMoveable Then MoveMode = True
        MovePoints.Clear()
        Me.Refresh()
    End Sub
End Class
