Imports Bwl.Imaging.Skia
Imports SkiaSharp

Public Class BitmapInfoTestBase
    Protected Function GetTestBmp() As SKBitmap
        Return GetTestBmp(New SKSizeI(49, 100), SKColorType.Bgra8888, SKAlphaType.Premul)
    End Function

    Protected Function GetTestBmp(colorType As SKColorType, alphaType As SKAlphaType) As SKBitmap
        Return GetTestBmp(New SKSizeI(49, 100), colorType, alphaType)
    End Function

    Protected Function GetTestBmp(size As SKSizeI, colorType As SKColorType, alphaType As SKAlphaType) As SKBitmap
        'Отрисовка тестового изображения
        Dim mtrx = New RGBMatrix(size.Width, size.Height)
        Dim w3 = size.Width \ 3
        For y = 0 To mtrx.Height - 1
            'Reg
            For x = 0 To w3
                Dim red = If(w3 > 0, (x / w3) * 255, 255)
                mtrx.SetRedPixel(x, y, red)
            Next
            'Green
            For x = w3 To 2 * w3
                Dim green = If(w3 > 0, ((x - w3) / w3) * 255, 255)
                mtrx.SetGreenPixel(x, y, green)
            Next
            'Blue
            For x = 2 * w3 + 1 To size.Width - 1
                Dim blue = If(w3 > 0, ((x - (2 * w3 + 1)) / w3) * 255, 255)
                mtrx.SetBluePixel(x, y, blue)
            Next
        Next
        'Обеспечение нужного формата Bitmap-а
        If colorType = SKColorType.Bgra8888 AndAlso alphaType = SKAlphaType.Premul Then
            Return mtrx.ToSKBitmap()
        Else
            Dim bmp32 = New SKBitmap(size.Width, size.Height, colorType, alphaType)
            Using gr = New SKCanvas(bmp32)
                gr.DrawBitmap(mtrx.ToSKBitmap(), 0, 0)
            End Using
            Return bmp32
        End If
    End Function
End Class
