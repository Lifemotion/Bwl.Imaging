Imports System.Drawing
Imports System.Drawing.Imaging
Imports Bwl.Imaging

Public Class BitmapInfoTestBase
    Protected Function GetTestBmp() As Bitmap
        Return GetTestBmp(New Size(49, 100), PixelFormat.Format32bppArgb)
    End Function

    Protected Function GetTestBmp(pixelFormat As PixelFormat) As Bitmap
        Return GetTestBmp(New Size(49, 100), pixelFormat)
    End Function

    Protected Function GetTestBmp(size As Size, pixelFormat As PixelFormat) As Bitmap
        'Отрисовка тестового изображения
        Dim mtrx = New RGBMatrix(size.Width, size.Height)
        Dim w3 = size.Width \ 3
        For y = 0 To mtrx.Height - 1
            'Reg
            For x = 0 To w3
                Dim red = If(w3 > 0, (x / w3) * 255, 255)
                mtrx.RedPixel(x, y) = red
            Next
            'Green
            For x = w3 To 2 * w3
                Dim green = If(w3 > 0, ((x - w3) / w3) * 255, 255)
                mtrx.GreenPixel(x, y) = green
            Next
            'Blue
            For x = 2 * w3 + 1 To size.Width - 1
                Dim blue = If(w3 > 0, ((x - (2 * w3 + 1)) / w3) * 255, 255)
                mtrx.BluePixel(x, y) = blue
            Next
        Next
        'Обеспечение нужного формата Bitmap-а
        If pixelFormat = PixelFormat.Format24bppRgb Then
            Return mtrx.ToBitmap()
        Else
            Dim bmp32 = New Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb)
            Using gr = Graphics.FromImage(bmp32)
                gr.DrawImage(mtrx.ToBitmap(), 0, 0)
            End Using
            Return bmp32
        End If
    End Function

    Protected Function GetResourceFileData(fileName As String) As Byte()
        Dim exePath = IO.Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly().Location)
        Dim dataPath = IO.Path.Combine(exePath, "..", "..", "Resources")
        Dim data = IO.File.ReadAllBytes(IO.Path.Combine(dataPath, fileName))
        Return data
    End Function
End Class
