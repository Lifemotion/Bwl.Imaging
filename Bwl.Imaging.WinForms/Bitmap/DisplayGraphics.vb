
Public Class DisplayGraphics
    Private _width As Integer
    Private _height As Integer
    Private _offsetX As Single
    Private _offsetY As Single
    Private _mulX As Single
    Private _mulY As Single
    Private _baseMulX As Single
    Private _baseMulY As Single
    Private _borders As RectangleF()
    Private _multiplyOnBitmapSize As Boolean = True

    Protected _bkgX1F As Single = 0
    Protected _bkgY1F As Single = 0
    Protected _bkgX2F As Single = 1.0
    Protected _bkgY2F As Single = 1.0

    Protected _graphics As Graphics
    Protected _pen As New Pen(Brushes.Black)
    Protected _brush As New SolidBrush(Color.Black)
    Protected _font As New Font(FontFamily.GenericSansSerif, 21)

    Public Property BackgroundColor As Color = Color.White
    Public Property DefaultWidth As Single = 1.0
    Public Property DefaultPointSize As Single = 5.0

    Public Sub New(graphics As Graphics, width As Integer, height As Integer)
        If graphics Is Nothing Then Throw New ArgumentException("graphics is nothing")
        SetGraphics(graphics, width, height)
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

    Public Sub KeepAspectRatio()
        KeepAspectRatio(_baseMulX, _baseMulY)
    End Sub

    Public Sub KeepAspectRatio(bkgWidth As Integer, bkgHeight As Integer)
        _mulX = _baseMulX
        _mulY = _baseMulY
        _offsetX = 0
        _offsetY = 0
        Dim aspectRatioControl = _baseMulX / _baseMulY
        Dim aspectRatioBitmap = bkgWidth / bkgHeight
        If aspectRatioControl > aspectRatioBitmap Then
            Dim ardivF = aspectRatioBitmap / aspectRatioControl
            _bkgX1F = 0.5 - ardivF * 0.5
            _bkgY1F = 0
            _bkgX2F = 0.5 + ardivF * 0.5
            _bkgY2F = 1
            _borders = {RectangleF.FromLTRB(0, 0, _bkgX1F, 1), RectangleF.FromLTRB(_bkgX2F, 0, 1, 1)}
            Dim bkgWF = _bkgX2F - _bkgX1F
            Dim bkgW = If(MultiplyOnBitmapSize, bkgWF * _width, bkgWF)
            _mulX = bkgW
            _offsetX = If(MultiplyOnBitmapSize, _bkgX1F * _width, _bkgX1F)
        End If
        If aspectRatioControl < aspectRatioBitmap Then
            Dim ardivF = aspectRatioControl / aspectRatioBitmap
            _bkgX1F = 0
            _bkgY1F = 0.5 - ardivF * 0.5
            _bkgX2F = 1
            _bkgY2F = 0.5 + ardivF * 0.5
            _borders = {RectangleF.FromLTRB(0, 0, 1, _bkgY1F), RectangleF.FromLTRB(0, _bkgY2F, 1, 1)}
            Dim bkgHF = _bkgY2F - _bkgY1F
            Dim bkgH = If(MultiplyOnBitmapSize, bkgHF * _height, bkgHF)
            _mulY = bkgH
            _offsetY = If(MultiplyOnBitmapSize, _bkgY1F * _height, _bkgY1F)
        End If
    End Sub

    Public Sub DrawBorders()
        DrawBorders(Color.Black)
    End Sub

    Public Sub DrawBorders(color As Color)
        If _borders IsNot Nothing AndAlso _borders.Any() Then
            FillRectanglesBase(color, _borders)
        End If
    End Sub

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
        _graphics.DrawImage(bitmap,
                            x1 * _mulX + _offsetX,
                            y1 * _mulY + _offsetY,
                            (x2 - x1) * _mulX,
                            (y2 - y1) * _mulY)
    End Sub

    Public Sub DrawBitmap(bitmap As Bitmap, x1 As Single, y1 As Single)
        _graphics.DrawImage(bitmap,
                            x1 * _mulX + _offsetX,
                            y1 * _mulY + _offsetY)
    End Sub

    Public Sub DrawBitmap(bitmap As Bitmap)
        _graphics.DrawImage(bitmap, 0, 0)
    End Sub

    Public Sub FillRectanglesBase(color As Color, rects As RectangleF())
        _graphics.FillRectangles(New SolidBrush(color), rects.Select(Function(item)
                                                                         Return New RectangleF(item.X * _baseMulX,
                                                                                               item.Y * _baseMulY,
                                                                                               item.Width * _baseMulX,
                                                                                               item.Height * _baseMulY)
                                                                     End Function).ToArray())
    End Sub

    Public Sub FillRectangles(color As Color, rects As RectangleF())
        _graphics.FillRectangles(New SolidBrush(color), rects.Select(Function(item)
                                                                         Return New RectangleF(item.X * _mulX + _offsetX,
                                                                                               item.Y * _mulY + _offsetY,
                                                                                               item.Width * _mulX,
                                                                                               item.Height * _mulY)
                                                                     End Function).ToArray())
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
        _baseMulX = 1.0
        _baseMulY = 1.0
        If MultiplyOnBitmapSize Then
            _baseMulX *= _width
            _baseMulY *= _height
        End If
        _mulX = _baseMulX
        _mulY = _baseMulY
    End Sub

    Public Sub DrawLine(color As Color, x1 As Single, y1 As Single, x2 As Single, y2 As Single, Optional width As Single = 0)
        SyncLock Me
            If width <= 0 Then width = DefaultWidth
            If _pen.Color <> color Or _pen.Width <> width Then _pen = New Pen(color, width)
            _graphics.DrawLine(_pen,
                               x1 * _mulX + _offsetX,
                               y1 * _mulY + _offsetY,
                               x2 * _mulX + _offsetX,
                               y2 * _mulY + _offsetY)
        End SyncLock
    End Sub

    Public Sub DrawVector(color As Color, x1 As Single, y1 As Single, x2 As Single, y2 As Single, Optional width As Single = 0)
        SyncLock Me
            If width <= 0 Then width = DefaultWidth
            If _pen.Color <> color Or _pen.Width <> width Then _pen = New Pen(color, width)
            Dim dx = x2 - x1
            Dim dy = y2 - y1
            Dim length = Math.Sqrt(dx ^ 2 + dy ^ 2)
            If length > 0 Then
                Dim angle As Single = Math.Atan2(dy, dx)
                Dim sz = 5
                _graphics.DrawLine(_pen,
                                   x1 * _mulX + _offsetX + CSng(Math.Cos(angle - Math.PI / 2) * sz),
                                   y1 * _mulY + _offsetY + CSng(Math.Sin(angle - Math.PI / 2) * sz),
                                   x2 * _mulX + _offsetX,
                                   y2 * _mulY + _offsetY)
                _graphics.DrawLine(_pen,
                                   x1 * _mulX + _offsetX + CSng(Math.Cos(angle + Math.PI / 2) * sz),
                                   y1 * _mulY + _offsetY + CSng(Math.Sin(angle + Math.PI / 2) * sz),
                                   x2 * _mulX + _offsetX,
                                   y2 * _mulY + _offsetY)
                _graphics.DrawLine(_pen,
                                   x1 * _mulX + _offsetX + CSng(Math.Cos(angle - Math.PI / 2) * sz),
                                   y1 * _mulY + _offsetY + CSng(Math.Sin(angle - Math.PI / 2) * sz),
                                   x1 * _mulX + _offsetX + CSng(Math.Cos(angle + Math.PI / 2) * sz),
                                   y1 * _mulY + _offsetY + CSng(Math.Sin(angle + Math.PI / 2) * sz))
            End If
            '  x1 * _mulX - 5, y1 * _mulY - 5)
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
            _graphics.DrawRectangle(_pen,
                                    x1 * _mulX + _offsetX,
                                    y1 * _mulY + _offsetY,
                                    (x2 - x1) * _mulX,
                                    (y2 - y1) * _mulY)
        End SyncLock
    End Sub

    Public Sub DrawPoint(color As Color, x1 As Single, y1 As Single, Optional size As Single = 0)
        SyncLock Me
            If size <= 0 Then size = DefaultPointSize
            If _brush.Color <> color Then _brush = New SolidBrush(color)
            _graphics.FillEllipse(_brush,
                                  x1 * _mulX + _offsetX - size / 2,
                                  y1 * _mulY + _offsetY - size / 2,
                                  size,
                                  size)
        End SyncLock
    End Sub

    Public Sub DrawCircle(color As Color, x1 As Single, y1 As Single, radius As Single, Optional width As Single = 0)
        SyncLock Me
            If width <= 0 Then width = DefaultWidth
            If _pen.Color <> color Or _pen.Width <> width Then _pen = New Pen(color, width)
            _graphics.DrawEllipse(_pen,
                                  x1 * _mulX + _offsetX,
                                  y1 * _mulY + _offsetY,
                                  radius * _mulX,
                                  radius * _mulX)
        End SyncLock
    End Sub

    Public Sub DrawText(color As Color, x1 As Single, y1 As Single, size As Single, text As String)
        SyncLock Me
            If _brush.Color <> color Then _brush = New SolidBrush(color)
            If _font.Size <> size * _mulX Then _font = New Font(FontFamily.GenericSansSerif, size * _mulX)
            _graphics.DrawString(text, _font, _brush,
                                 x1 * _mulX + _offsetX,
                                 y1 * _mulY + _offsetY)
        End SyncLock
    End Sub

    Public Sub DrawText(color As Color, textObj As TextObject)
        SyncLock Me
            If _brush.Color <> color Then _brush = New SolidBrush(color)
            If _font.Size <> textObj.Size * _mulX Then _font = New Font(FontFamily.GenericSansSerif, textObj.Size * _mulX)
            _graphics.DrawString(textObj.Text, _font, _brush,
                                 textObj.Point1.X * _mulX + _offsetX,
                                 textObj.Point1.Y * _mulY + _offsetY)
        End SyncLock
    End Sub

    Public Sub DrawPoligon(color As Color, poligon As Polygon, Optional width As Single = 0)
        SyncLock Me
            If width <= 0 Then width = DefaultWidth
            If _pen.Color <> color Or _pen.Width <> width Then _pen = New Pen(color, width)
            For i = 0 To poligon.Points.Count - 2
                _graphics.DrawLine(_pen,
                                   poligon.Points(i).X * _mulX + _offsetX,
                                   poligon.Points(i).Y * _mulY + _offsetY,
                                   poligon.Points(i + 1).X * _mulX + _offsetX,
                                   poligon.Points(i + 1).Y * _mulY + _offsetY)
            Next
            If poligon.Points.Count > 2 And poligon.IsClosed Then
                Dim last = poligon.Points.Count - 1
                _graphics.DrawLine(_pen,
                                   poligon.Points(last).X * _mulX + _offsetX,
                                   poligon.Points(last).Y * _mulY + _offsetY,
                                   poligon.Points(0).X * _mulX + _offsetX,
                                   poligon.Points(0).Y * _mulY + _offsetY)
            End If
        End SyncLock
    End Sub

    Public Sub DrawObject(color As Color, obj As Object, Optional lineWidth As Single = 0, Optional pointSize As Single = 0)
        If GetType(Vector).IsAssignableFrom(obj.GetType) Then
            Dim vector As Vector = obj
            DrawVector(color, vector.Point1.X, vector.Point1.Y, vector.Point2.X, vector.Point2.Y)
        ElseIf GetType(Polygon).IsAssignableFrom(obj.GetType) Then
            DrawPoligon(color, obj, lineWidth)
        ElseIf GetType(PointC).IsAssignableFrom(obj.GetType) Or GetType(PointF).IsAssignableFrom(obj.GetType) Or GetType(Point).IsAssignableFrom(obj.GetType) Then
            DrawPoint(color, obj.x, obj.y, pointSize)
        ElseIf GetType(RectangleF).IsAssignableFrom(obj.GetType) Or GetType(RectangleC).IsAssignableFrom(obj.GetType) Or GetType(Rectangle).IsAssignableFrom(obj.GetType) Then
            DrawRectangle(color, obj, lineWidth)
        ElseIf GetType(BitmapObject).IsAssignableFrom(obj.GetType) Then
            DrawBitmap(obj)
        ElseIf GetType(Bitmap).IsAssignableFrom(obj.GetType) Then
            DrawBitmap(obj, 0, 0, 1, 1)
        ElseIf GetType(TextObject).IsAssignableFrom(obj.GetType) Then
            DrawText(color, obj)
        End If
    End Sub

    Public Function GetBitmapRectangle(objectRecrtangle As RectangleF) As RectangleF
        Return New RectangleF(objectRecrtangle.Left * _mulX + _offsetX,
                              objectRecrtangle.Top * _mulY + _offsetY,
                              objectRecrtangle.Width * _mulX,
                              objectRecrtangle.Height * _mulY)
    End Function

    Public Function GetObjectRectangle(bitmapRectangle As RectangleF) As RectangleF
        Return New RectangleF((bitmapRectangle.Left - _offsetX) / _mulX,
                              (bitmapRectangle.Top - _offsetY) / _mulY,
                              bitmapRectangle.Width / _mulX,
                              bitmapRectangle.Height / _mulY)
    End Function

    Public Function GetBitmapPoint(objectPoint As PointF) As PointF
        Return New PointF(objectPoint.X * _mulX + _offsetX, objectPoint.Y * _mulY + _offsetY)
    End Function

    Public Function GetObjectPoint(bitmapPoint As PointF) As PointF
        Return New PointF((bitmapPoint.X - _offsetX) / _mulX, (bitmapPoint.Y - _offsetY) / _mulY)
    End Function

    Public Function IsBitmapPointInsideBound(obj As Object, bitmapX As Integer, bitmapY As Integer)
        Dim bound = GetBoundRectangeF(obj)
        Dim scrp = GetObjectPoint(New PointF(bitmapX, bitmapY))
        If bound.Contains(scrp) Then Return True
        Return False
    End Function

    Public Function ExtendRectangleAtLineWidth(rect As RectangleF) As RectangleF
        Dim width = DefaultWidth
        If width < 1 Then width = 1
        Dim offset = width / _mulX
        rect = rect.ToPositiveSized
        Dim newrect As New RectangleF(rect.Left - offset, rect.Top - offset, rect.Width + offset, rect.Height + offset)
        Return newrect
    End Function

    Public Function GetBoundRectangeF(obj As Object) As RectangleF
        If GetType(PointC).IsAssignableFrom(obj.GetType) Or GetType(PointF).IsAssignableFrom(obj.GetType) Or GetType(Point).IsAssignableFrom(obj.GetType) Then
            Dim px As Single = obj.X
            Dim py As Single = obj.Y
            Dim sx = (DefaultPointSize * 2) / _mulX
            Dim bound = New RectangleF(px - sx / 2, py - sx / 2, sx, sx)
            Return ExtendRectangleAtLineWidth(bound)
        ElseIf GetType(RectangleF).IsAssignableFrom(obj.GetType) Then
            Dim bound = DirectCast(obj, RectangleF)
            Return ExtendRectangleAtLineWidth(bound)
        ElseIf GetType(Rectangle).IsAssignableFrom(obj.GetType) Then
            Dim bound = DirectCast(obj, Rectangle)
            Return ExtendRectangleAtLineWidth(bound)
        ElseIf GetType(Polygon).IsAssignableFrom(obj.GetType) Then
            Dim bound = DirectCast(obj, Polygon).GetBoundRectangleF
            Return ExtendRectangleAtLineWidth(bound)
        ElseIf GetType(BitmapObject).IsAssignableFrom(obj.GetType) Then
            Dim bound = DirectCast(obj, BitmapObject).RectangleF
            Return ExtendRectangleAtLineWidth(bound)
        ElseIf GetType(Bitmap).IsAssignableFrom(obj.GetType) Then
            Dim bound = New RectangleF(0, 0, 1, 1)
            Return ExtendRectangleAtLineWidth(bound)
        ElseIf GetType(TextObject).IsAssignableFrom(obj.GetType) Then
            Dim bound = New RectangleF(DirectCast(obj, TextObject).Point1, New SizeF(0.1, 0.1))
            Return ExtendRectangleAtLineWidth(bound)
        End If
        Return New RectangleF(0, 0, 0, 0)
    End Function

    Public Sub DrawDisplayObject(dispObj As DisplayObject)
        DrawObject(dispObj.Color, dispObj.DrawObject, dispObj.LineWidth, dispObj.PointSize)
    End Sub

    Public Sub SetGraphics(width As Integer, height As Integer)
        SetGraphics(_graphics, width, height)
    End Sub

    Public Sub SetGraphics(graphics As Graphics, width As Integer, height As Integer)
        If width < 1 Then Throw New ArgumentException("width must be >0")
        If height < 1 Then Throw New ArgumentException("height must be >0")
        _graphics = graphics
        _width = width
        _height = height
        If Quality = QualityMode.Fast Then
            _graphics.SmoothingMode = Drawing2D.SmoothingMode.None
            _graphics.InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
            _graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half
        End If
        ComputeMultipliers()
    End Sub

    Public Property Quality As QualityMode = QualityMode.Fast

    Public Enum QualityMode
        Normal
        Fast
    End Enum
End Class
