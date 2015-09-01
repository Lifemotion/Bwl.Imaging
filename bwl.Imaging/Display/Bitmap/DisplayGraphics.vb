
Public Class DisplayGraphics
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
    Private _width As Integer
    Private _height As Integer

    Public Sub New(graphics As Graphics, width As Integer, height As Integer)
        If graphics Is Nothing Then Throw New ArgumentException("grpahics is nothing")
        SetGraphics(width, height, graphics)
        ComputeMultipliers()
    End Sub

    Public ReadOnly Property Graphics As Graphics
        Get
            Return _graphics
        End Get
    End Property

    Public ReadOnly Property Width As Integer
        Get
            Return _width
        End Get
    End Property

    Public ReadOnly Property Height As Integer
        Get
            Return _height
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

    Public Sub DrawBitmap(bitmap As Bitmap, x1 As Single, y1 As Single)
        _graphics.DrawImage(bitmap, x1 * _mulX, y1 * _mulY)
    End Sub

    Public Sub DrawBitmap(bitmap As Bitmap)
        _graphics.DrawImage(bitmap, 0, 0)
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
            _mulX *= _width
            _mulY *= _height
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

    Public Sub DrawVector(color As Color, x1 As Single, y1 As Single, x2 As Single, y2 As Single, Optional width As Single = 0)
        SyncLock Me
            If width <= 0 Then width = DefaultWidth
            If _pen.Color <> color Or _pen.Width <> width Then _pen = New Pen(color, width)
            _graphics.DrawLine(_pen, x1 * _mulX - 5, y1 * _mulY - 5,
                                     x2 * _mulX, y2 * _mulY)
            _graphics.DrawLine(_pen, x1 * _mulX + 5, y1 * _mulY + 5,
                                   x2 * _mulX, y2 * _mulY)
            _graphics.DrawLine(_pen, x1 * _mulX + 5, y1 * _mulY + 5,
                                   x1 * _mulX - 5, y1 * _mulY - 5)
        End SyncLock
    End Sub

    Public Sub DrawRectangle(color As Color, rect As RectangleF, Optional width As Single = 0)
        DrawRectangle(color, rect.Left, rect.Top, rect.Right, rect.Bottom, width)
    End Sub

    Public Sub DrawRectangle(color As Color, rect As Rectangle, Optional width As Single = 0)
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

    Public Sub DrawObject(color As Color, obj As Object, Optional lineWidth As Single = 0, Optional pointSize As Single = 0)
        If GetType(Vector).IsAssignableFrom(obj.GetType) Then
            Dim vector As Vector = obj
            DrawVector(color, vector.Point1.X, vector.Point1.Y, vector.Point2.X, vector.Point2.Y)
        ElseIf GetType(Polygon).IsAssignableFrom(obj.GetType)
            DrawPoligon(color, obj, lineWidth)
        ElseIf GetType(PointC).IsAssignableFrom(obj.GetType) Or GetType(PointF).IsAssignableFrom(obj.GetType) Or GetType(Point).IsAssignableFrom(obj.GetType)
            DrawPoint(color, obj.x, obj.y, pointSize)
        ElseIf GetType(RectangleF).IsAssignableFrom(obj.GetType) Or GetType(RectangleC).IsAssignableFrom(obj.GetType) Or GetType(Rectangle).IsAssignableFrom(obj.GetType)
            DrawRectangle(color, obj, lineWidth)
        ElseIf GetType(BitmapObject).IsAssignableFrom(obj.GetType)
            DrawBitmap(obj)
        ElseIf GetType(Bitmap).IsAssignableFrom(obj.GetType)
            DrawBitmap(obj, 0, 0, 1, 1)
        ElseIf GetType(TextObject).IsAssignableFrom(obj.GetType)
            DrawText(color, obj)
        End If
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
        If GetType(Polygon).IsAssignableFrom(obj.GetType) Then
            Dim bound = DirectCast(obj, Polygon).GetBoundRectangleF
            Return bound
        ElseIf GetType(PointC).IsAssignableFrom(obj.GetType) Or GetType(PointF).IsAssignableFrom(obj.GetType) Or GetType(Point).IsAssignableFrom(obj.GetType)
            Dim px As Single = obj.X
            Dim py As Single = obj.Y
            Dim sx = DefaultPointSize * 2 / _mulX
            Dim bound = New RectangleF(px - sx / 2, py - sx / 2, sx, sx)
            Return bound
        ElseIf GetType(RectangleF).IsAssignableFrom(obj.GetType)
            Dim bound = DirectCast(obj, RectangleF)
            Return bound
        ElseIf GetType(Rectangle).IsAssignableFrom(obj.GetType)
            Dim bound = DirectCast(obj, Rectangle)
            Return bound
        ElseIf GetType(BitmapObject).IsAssignableFrom(obj.GetType)
            Dim bound = DirectCast(obj, BitmapObject).RectangleF
            Return bound
        ElseIf GetType(Bitmap).IsAssignableFrom(obj.GetType)
            Dim bound = New RectangleF(0, 0, 1, 1)
            Return bound
        ElseIf GetType(TextObject).IsAssignableFrom(obj.GetType)
            Dim bound = New RectangleF(DirectCast(obj, TextObject).Point1, New SizeF(0.1, 0.1))
            Return bound
        End If
        Return New RectangleF(0, 0, 0, 0)
    End Function

    Public Sub DrawDisplayObject(dispObj As DisplayObject)
        DrawObject(dispObj.Color, dispObj.DrawObject, dispObj.LineWidth, dispObj.PointSize)
    End Sub

    Public Sub SetGraphics(width As Integer, height As Integer)
        SetGraphics(width, height, _graphics)
    End Sub

    Public Sub SetGraphics(width As Integer, height As Integer, graphics As Graphics)
        If width < 1 Then Throw New ArgumentException("width must be >0")
        If height < 1 Then Throw New ArgumentException("height must be >0")
        _graphics = graphics
        _width = width
        _height = height
        If Quality = QualityMode.fast Then
            _graphics.SmoothingMode = Drawing2D.SmoothingMode.None
            _graphics.InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
            _graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half

        End If
        ComputeMultipliers()
    End Sub

    Public Property Quality As QualityMode = QualityMode.fast

    Public Enum QualityMode
        normal
        fast
    End Enum
End Class
