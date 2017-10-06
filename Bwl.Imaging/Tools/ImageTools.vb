Imports System.Drawing.Drawing2D

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
End Module
