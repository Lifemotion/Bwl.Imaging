Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Runtime.CompilerServices

Public Module ImageTools
    Public Function Resize(img As Bitmap, W As Integer, H As Integer,
                           Optional align4 As Boolean = True,
                           Optional interp As InterpolationMode = InterpolationMode.Bilinear) As Bitmap
        If align4 Then
            W = If(W Mod 4 <> 0, W + (4 - W Mod 4), W)
        End If
        Dim resized = New Bitmap(W, H, PixelFormat.Format24bppRgb)
        Using gr = Graphics.FromImage(resized)
            With gr
                .SmoothingMode = SmoothingMode.None
                .InterpolationMode = interp
                .PixelOffsetMode = PixelOffsetMode.Half
                .DrawImage(img, 0, 0, W, H)
            End With
        End Using
        Return resized
    End Function

    Public Function RectangleToRectangleF(rect As Rectangle, size As Size) As RectangleF
        Dim X = rect.X / CSng(size.Width)
        Dim Y = rect.Y / CSng(size.Height)
        Dim W = rect.Width / CSng(size.Width)
        Dim H = rect.Height / CSng(size.Height)
        Return New RectangleF(X, Y, W, H)
    End Function

    Public Function RectangleFToRectangle(rectF As RectangleF, size As Size) As Rectangle
        Dim X = CInt(Math.Floor(rectF.X * size.Width))
        Dim Y = CInt(Math.Floor(rectF.Y * size.Height))
        Dim W = CInt(Math.Floor(rectF.Width * size.Width))
        Dim H = CInt(Math.Floor(rectF.Height * size.Height))
        Return New Rectangle(X, Y, W, H)
    End Function

    Public Function PointsToRectangleF(points As PointF()) As RectangleF
        Dim left = points.Min(Function(item) item.X)
        Dim right = points.Max(Function(item) item.X)
        Dim top = points.Min(Function(item) item.Y)
        Dim bottom = points.Max(Function(item) item.Y)
        Return RectangleF.FromLTRB(left, top, right, bottom)
    End Function

    Public Function PointsToRectangle(points As Point()) As Rectangle
        Dim left = points.Min(Function(item) item.X)
        Dim right = points.Max(Function(item) item.X)
        Dim top = points.Min(Function(item) item.Y)
        Dim bottom = points.Max(Function(item) item.Y)
        Return Rectangle.FromLTRB(left, top, right, bottom)
    End Function

    <Extension()>
    Public Function ConvertTo(img As Image, targetPixelFormat As PixelFormat) As Bitmap
        Dim bmp = New Bitmap(img.Width, img.Height, targetPixelFormat)
        Using gr = Graphics.FromImage(bmp)
            gr.DrawImage(img, New Rectangle(0, 0, img.Width, img.Height))
        End Using
        Return bmp
    End Function
End Module
