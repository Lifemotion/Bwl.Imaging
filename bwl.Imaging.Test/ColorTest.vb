﻿Public Module ColorTest
    Public Sub Main()
        Dim mtr1 As New StructureMatrixRGB(10, 20)
        Dim mtr2 As New StructureMatrixHSV(10, 20)
        For i = 0 To 19
            mtr1.Pixel(0, i) = New RGB(200, 30, 10)
            mtr2.Pixel(0, i) = New HSV(200, 230, 240)
        Next
        mtr1.ToRGBMatrix.ToBitmap.Save("rgb1.bmp")
        mtr2.ToRGBMatrix.ToBitmap.Save("hsv1.bmp")

        Dim src = New Bitmap("mOABkQ1wC1Q.jpg")
        src.Save("1.bmp")
        Dim rgb = New StructureMatrixRGB(src.BitmapToRgbMatrix)
        rgb.ToRGBMatrix.ToBitmap.Save("2.bmp")
        Dim hsv = New StructureMatrixHSV(src.BitmapToRgbMatrix)
        For i = 0 To hsv.Matrix.Length - 1
            Dim pix = hsv.Matrix(i)
            ' pix.H += 180
            hsv.Matrix(i) = pix
        Next
        hsv.ToRGBMatrix.ToBitmap.Save("3.bmp")
        'mOABkQ1wC1Q.jpg
    End Sub
End Module
