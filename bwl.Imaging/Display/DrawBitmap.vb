Public Class DrawBitmap
    Protected _bitmap As Bitmap
    Protected _graphics As Graphics
    Protected _pen As New Pen(Brushes.Black)
    Protected _brush As New SolidBrush(Color.Black)
    Protected _font As New Font(FontFamily.GenericSansSerif, 21)

    Private _mulX As Single
    Private _mulY As Single
    Private _multiplyOnBitmapSize As Boolean = True
    Public Property BackgroundColor As Color = Color.White
    Public Property DefaultWidth As Single = 1.0
    Public Property DefaultPointSize As Single = 5.0

    Public Sub New(bitmap As Bitmap)
        If bitmap Is Nothing Then Throw New ArgumentException("bitmap is nothing")
        _bitmap = bitmap
        _graphics = Graphics.FromImage(_bitmap)
        ComputeMultipliers()
    End Sub

    Public Sub New(width As Integer, height As Integer)
        Me.New(New Bitmap(width, height))
        If width < 1 Then Throw New ArgumentException("width must be >0")
        If height < 1 Then Throw New ArgumentException("height must be >0")
    End Sub

    Public ReadOnly Property Bitmap As Bitmap
        Get
            Return _bitmap
        End Get
    End Property

    Public ReadOnly Property Graphics As Graphics
        Get
            Return _graphics
        End Get
    End Property

    Public ReadOnly Property Width As Integer
        Get
            Return _bitmap.Width
        End Get
    End Property

    Public ReadOnly Property Height As Integer
        Get
            Return _bitmap.Height
        End Get
    End Property

    Public Sub Clear()
        _graphics.Clear(BackgroundColor)
    End Sub

    Public Sub DrawBitmap(bo As BitmapObject)
        DrawBitmap(bo.Bitmap, bo.RectangleF)
    End Sub

    Public Sub DrawBitmap(bitmap As Bitmap, rect As RectangleF)
        DrawBitmap(bitmap, rect.Left, rect.Top, rect.Right, rect.Bottom)
    End Sub
    Public Sub DrawBitmap(bitmap As Bitmap, x1 As Single, y1 As Single, x2 As Single, y2 As Single)
        _graphics.DrawImage(bitmap, x1 * _mulX, y1 * _mulY, (x2 - x1) * _mulX, (y2 - y1) * _mulY)
    End Sub

    Public Property MultiplyOnBitmapSize As Boolean
        Set(value As Boolean)
            _multiplyOnBitmapSize = value
            ComputeMultipliers()
        End Set
        Get
            Return _multiplyOnBitmapSize
        End Get
    End Property

    Private Sub ComputeMultipliers()
        _mulX = 1.0
        _mulY = 1.0
        If MultiplyOnBitmapSize Then
            _mulX *= _bitmap.Width
            _mulY *= _bitmap.Height
        End If
    End Sub

    Public Sub DrawLine(color As Color, x1 As Single, y1 As Single, x2 As Single, y2 As Single, Optional width As Single = 0)
        SyncLock Me
            If width <= 0 Then width = DefaultWidth
            If _pen.Color <> color Or _pen.Width <> width Then _pen = New Pen(color, width)
            _graphics.DrawLine(_pen, x1 * _mulX, y1 * _mulY,
                                     x2 * _mulX, y2 * _mulY)
        End SyncLock
    End Sub

    Public Sub DrawRectangle(color As Color, rect As RectangleF, Optional width As Single = 0)
        DrawRectangle(color, rect.Left, rect.Top, rect.Right, rect.Bottom, width)
    End Sub

    Public Sub DrawRectangle(color As Color, x1 As Single, y1 As Single, x2 As Single, y2 As Single, Optional width As Single = 0)
        SyncLock Me
            If width <= 0 Then width = DefaultWidth
            If _pen.Color <> color Or _pen.Width <> width Then _pen = New Pen(color, width)
            Dim tmp As Single
            If x1 > x2 Then tmp = x2 : x2 = x1 : x1 = tmp
            If y1 > y2 Then tmp = y2 : y2 = y1 : y1 = tmp
            _graphics.DrawRectangle(_pen, x1 * _mulX, y1 * _mulY,
                                         (x2 - x1) * _mulX, (y2 - y1) * _mulY)
        End SyncLock
    End Sub

    Public Sub DrawPoint(color As Color, x1 As Single, y1 As Single, Optional size As Single = 0)
        SyncLock Me
            If size <= 0 Then size = DefaultPointSize
            If _brush.Color <> color Then _brush = New SolidBrush(color)
            _graphics.FillEllipse(_brush, x1 * _mulX - size / 2, y1 * _mulY - size / 2,
                                         size, size)
        End SyncLock
    End Sub

    Public Sub DrawCircle(color As Color, x1 As Single, y1 As Single, radius As Single, Optional width As Single = 0)
        SyncLock Me
            If width <= 0 Then width = DefaultWidth
            If _pen.Color <> color Or _pen.Width <> width Then _pen = New Pen(color, width)
            _graphics.DrawEllipse(_pen, x1 * _mulX, y1 * _mulY,
                                         radius * _mulX, radius * _mulX)
        End SyncLock
    End Sub

    Public Sub DrawText(color As Color, x1 As Single, y1 As Single, size As Single, text As String)
        SyncLock Me
            If _brush.Color <> color Then _brush = New SolidBrush(color)
            If _font.Size <> size * _mulX Then _font = New Font(FontFamily.GenericSansSerif, size * _mulX)
            _graphics.DrawString(text, _font, _brush, x1 * _mulX, y1 * _mulY)
        End SyncLock
    End Sub

    Public Sub DrawText(color As Color, textObj As TextObject)
        SyncLock Me

            If _brush.Color <> color Then _brush = New SolidBrush(color)
            If _font.Size <> textObj.Size * _mulX Then _font = New Font(FontFamily.GenericSansSerif, textObj.Size * _mulX)
            _graphics.DrawString(textObj.Text, _font, _brush, textObj.Point1.X * _mulX, textObj.Point1.Y * _mulY)
        End SyncLock
    End Sub

    Public Sub DrawPoligon(color As Color, poligon As Polygon, Optional width As Single = 0)
        SyncLock Me
            If width <= 0 Then width = DefaultWidth
            If _pen.Color <> color Or _pen.Width <> width Then _pen = New Pen(color, width)
            For i = 0 To poligon.Points.Count - 2
                _graphics.DrawLine(_pen, poligon.Points(i).X * _mulX, poligon.Points(i).Y * _mulY,
                                         poligon.Points(i + 1).X * _mulX, poligon.Points(i + 1).Y * _mulY)
            Next
            If poligon.Points.Count > 2 And poligon.IsClosed Then
                Dim last = poligon.Points.Count - 1
                _graphics.DrawLine(_pen, poligon.Points(last).X * _mulX, poligon.Points(last).Y * _mulY,
                                      poligon.Points(0).X * _mulX, poligon.Points(0).Y * _mulY)
            End If
        End SyncLock
    End Sub

    Public Sub DrawObject(color As Color, obj As Object, Optional width As Single = 0, Optional size As Single = 0)
        Select Case obj.GetType()
            Case GetType(Polygon), GetType(Line), GetType(Tetragon)
                DrawPoligon(color, obj, width)
            Case GetType(PointC), GetType(PointF), GetType(Point)
                DrawPoint(color, obj.x, obj.y, size)
            Case GetType(RectangleF)
                DrawRectangle(color, obj, width)
            Case GetType(Bitmap)
                DrawBitmap(obj, 0, 0, 1, 1)
            Case GetType(BitmapObject)
                DrawBitmap(obj)
            Case GetType(TextObject)
                DrawText(color, obj)
        End Select
    End Sub

    Public Function GetBitmapRectangle(objectRecrtangle As RectangleF) As RectangleF
        Return New RectangleF(objectRecrtangle.Left * _mulX, objectRecrtangle.Top * _mulY, objectRecrtangle.Width * _mulX, objectRecrtangle.Height * _mulY)
    End Function

    Public Function GetObjectRectangle(bitmapRecrtangle As RectangleF) As RectangleF
        Return New RectangleF(bitmapRecrtangle.Left / _mulX, bitmapRecrtangle.Top / _mulY, bitmapRecrtangle.Width / _mulX, bitmapRecrtangle.Height / _mulY)
    End Function

    Public Function GetBitmapPoint(objectPoint As PointF) As PointF
        Return New PointF(objectPoint.X * _mulX, objectPoint.Y * _mulY)
    End Function

    Public Function GetObjectPoint(bitmpaPoint As PointF) As PointF
        Return New PointF(bitmpaPoint.X / _mulX, bitmpaPoint.Y / _mulY)
    End Function

    Public Function IsBitmapPointInsideBound(obj As Object, bitmapX As Integer, bitmapY As Integer)
        Dim bound = GetBoundRectangeF(obj)
        Dim scrp = GetObjectPoint(New PointF(bitmapX, bitmapY))
        If bound.Contains(scrp) Then Return True
        Return False
    End Function

    Public Function GetBoundRectangeF(obj As Object) As RectangleF
        Select Case obj.GetType()
            Case GetType(Polygon), GetType(Line), GetType(Tetragon)
                Dim bound = DirectCast(obj, Polygon).GetBoundRectangleF
                Return bound
            Case GetType(PointC), GetType(PointF), GetType(Point)
                Dim px As Single = obj.X
                Dim py As Single = obj.Y
                Dim sx = DefaultPointSize * 2 / _mulX
                Dim bound = New RectangleF(px - sx / 2, py - sx / 2, sx, sx)
                Return bound
            Case GetType(RectangleF)
                Dim bound = DirectCast(obj, RectangleF)
                Return bound
            Case GetType(Rectangle)
                Dim bound = DirectCast(obj, Rectangle)
                Return bound
            Case GetType(Bitmap)
                Dim bound = New RectangleF(0, 0, 1, 1)
                Return bound
            Case GetType(BitmapObject)
                Dim bound = DirectCast(obj, BitmapObject).RectangleF
                Return bound
            Case GetType(TextObject)
                Dim bound = New RectangleF(DirectCast(obj, TextObject).Point1, New SizeF(0.1, 0.1))
                Return bound
        End Select
        Return New RectangleF(0, 0, 0, 0)
    End Function

    Public Sub DrawDisplayObject(dispObj As DisplayObject)
        DrawObject(dispObj.Color, dispObj.DrawObject, dispObj.Width, dispObj.Size)
    End Sub

End Class
