Public Module ColorTest
    Public Sub Main()
        Dim mtr1 As New ColorMatrixRGB(10, 20)
        Dim mtr2 As New ColorMatrixHSV(10, 20)
        For i = 0 To 19
            mtr1.Pixel(0, i) = New RGB(200, 30, 10)
            mtr2.Pixel(0, i) = New HSV(200, 230, 240)
        Next
        mtr1.ToRGBMatrix.ToBitmap.Save("rgb1.bmp")
        mtr2.ToRGBMatrix.ToBitmap.Save("hsv1.bmp")

        Dim src = New Bitmap("mOABkQ1wC1Q.jpg")
        src.Save("1.bmp")
        Dim rgb = New ColorMatrixRGB(src.BitmapToRgbMatrix)
        rgb.ToRGBMatrix.ToBitmap.Save("2.bmp")
        Dim hsv = New ColorMatrixHSV(src.BitmapToRgbMatrix)
        hsv.ToRGBMatrix.ToBitmap.Save("3.bmp")
        'mOABkQ1wC1Q.jpg
    End Sub
End Module
