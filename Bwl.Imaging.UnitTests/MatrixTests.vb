Imports System.Drawing

<TestClass()> Public Class MatrixTests

    <TestMethod()> Public Sub LoadAndAccessRgbMatrix()
        Dim bmp = My.Resources._4x3_rgb
        Dim rgb = BitmapConverter.BitmapToRGBMatrix(bmp)
        Assert.AreEqual(4, rgb.Width)
        Assert.AreEqual(3, rgb.Height)
        Assert.AreEqual(0, rgb.RedPixel(0, 0)) : Assert.AreEqual(0, rgb.GreenPixel(0, 0)) : Assert.AreEqual(0, rgb.BluePixel(0, 0))
        Assert.AreEqual(255, rgb.RedPixel(1, 0)) : Assert.AreEqual(0, rgb.GreenPixel(1, 0)) : Assert.AreEqual(0, rgb.BluePixel(1, 0))
        Assert.AreEqual(0, rgb.RedPixel(2, 0)) : Assert.AreEqual(255, rgb.GreenPixel(2, 0)) : Assert.AreEqual(0, rgb.BluePixel(2, 0))
        Assert.AreEqual(0, rgb.RedPixel(3, 0)) : Assert.AreEqual(0, rgb.GreenPixel(3, 0)) : Assert.AreEqual(255, rgb.BluePixel(3, 0))

        Assert.AreEqual(255, rgb.RedPixel(0, 1)) : Assert.AreEqual(255, rgb.GreenPixel(0, 1)) : Assert.AreEqual(255, rgb.BluePixel(0, 1))
        Assert.AreEqual(255, rgb.RedPixel(1, 1)) : Assert.AreEqual(255, rgb.GreenPixel(1, 1)) : Assert.AreEqual(255, rgb.BluePixel(1, 1))
        Assert.AreEqual(255, rgb.RedPixel(2, 1)) : Assert.AreEqual(255, rgb.GreenPixel(2, 1)) : Assert.AreEqual(255, rgb.BluePixel(2, 1))
        Assert.AreEqual(255, rgb.RedPixel(3, 1)) : Assert.AreEqual(255, rgb.GreenPixel(3, 1)) : Assert.AreEqual(255, rgb.BluePixel(3, 1))

        Assert.AreEqual(0, rgb.RedPixel(0, 2)) : Assert.AreEqual(0, rgb.GreenPixel(0, 2)) : Assert.AreEqual(255, rgb.BluePixel(0, 2))
        Assert.AreEqual(0, rgb.RedPixel(1, 2)) : Assert.AreEqual(255, rgb.GreenPixel(1, 2)) : Assert.AreEqual(0, rgb.BluePixel(1, 2))
        Assert.AreEqual(255, rgb.RedPixel(2, 2)) : Assert.AreEqual(0, rgb.GreenPixel(2, 2)) : Assert.AreEqual(0, rgb.BluePixel(2, 2))
        Assert.AreEqual(0, rgb.RedPixel(3, 2)) : Assert.AreEqual(0, rgb.GreenPixel(3, 2)) : Assert.AreEqual(0, rgb.BluePixel(3, 2))
    End Sub

    <TestMethod()> Public Sub SaveRgbMatrix()
        Dim bmp = My.Resources._4x3_rgb
        Dim rgb = BitmapConverter.BitmapToRGBMatrix(bmp)
        Dim newbmp = rgb.ToBitmap
        Dim clr00 = newbmp.GetPixel(0, 0)
        Dim clr10 = newbmp.GetPixel(1, 0)
        Dim clr20 = newbmp.GetPixel(2, 0)
        Dim clr30 = newbmp.GetPixel(3, 0)
        ' newbmp.Save("C:\Users\heart\Repositories\test1.bmp")
        Assert.AreEqual(Color.Black.ToArgb, clr00.ToArgb)
        Assert.AreEqual(Color.Red.ToArgb, clr10.ToArgb)
        Assert.AreEqual(Color.FromArgb(0, 255, 0).ToArgb, clr20.ToArgb)
        Assert.AreEqual(Color.Blue.ToArgb, clr30.ToArgb)
    End Sub

    <TestMethod()> Public Sub RgbMatrixAccess()
        Dim bmp = New RGBMatrix(4, 4)
        bmp.ColorPixel(1, 1) = Color.Red
        Assert.AreEqual(255, bmp.RedPixel(1, 1))
        Assert.AreEqual(0, bmp.GreenPixel(1, 1))
        Assert.AreEqual(0, bmp.BluePixel(1, 1))
        Assert.AreEqual(255, bmp.Red(1 + 4 * 1))
        Assert.AreEqual(0, bmp.Green(1 + 4 * 1))
        Assert.AreEqual(0, bmp.Blue(1 + 4 * 1))

        Dim clr = bmp.ColorPixel(1, 1)
        Assert.AreEqual(CByte(255), clr.R)
        Assert.AreEqual(CByte(0), clr.G)
        Assert.AreEqual(CByte(0), clr.B)
    End Sub

    <TestMethod()> Public Sub MatrixConvertTest()
        Dim bmpRgb = My.Resources._4x3_rgb
        Dim bmpGray = My.Resources._4x3_gray
        Dim grayMatrixFromRgb_4x3 = BitmapConverter.BitmapToGrayMatrix(bmpRgb)
        Dim grayMatrixFromGray_4x3 = BitmapConverter.BitmapToGrayMatrix(bmpGray)

        Dim grayMatrixFromRgb_4x4 = New GrayMatrix(4, 4)
        Dim grayMatrixFromGray_4x4 = New GrayMatrix(4, 4)
        For y = 0 To grayMatrixFromRgb_4x3.Height - 1
            For x = 0 To grayMatrixFromRgb_4x3.Width - 1
                grayMatrixFromRgb_4x4.GrayPixel(x, y) = grayMatrixFromRgb_4x3.GrayPixel(x, y)
                grayMatrixFromGray_4x4.GrayPixel(x, y) = grayMatrixFromGray_4x3.GrayPixel(x, y)
            Next
        Next

        For y = 0 To grayMatrixFromRgb_4x3.Height - 1
            Dim offset = y * grayMatrixFromRgb_4x3.Width
            For x = 0 To grayMatrixFromRgb_4x3.Width - 1
                Dim diff_4x3_1 = Math.Abs(grayMatrixFromRgb_4x3.GrayPixel(x, y) - grayMatrixFromGray_4x3.GrayPixel(x, y))
                Dim diff_4x3_2 = Math.Abs(grayMatrixFromRgb_4x3.Gray(x + offset) - grayMatrixFromGray_4x3.Gray(x + offset))
                Dim diff_4x4_1 = Math.Abs(grayMatrixFromRgb_4x4.GrayPixel(x, y) - grayMatrixFromGray_4x4.GrayPixel(x, y))
                Dim diff_4x4_2 = Math.Abs(grayMatrixFromRgb_4x4.Gray(x + offset) - grayMatrixFromGray_4x4.Gray(x + offset))
                Dim diffs = {diff_4x3_1, diff_4x3_2, diff_4x4_1, diff_4x4_2}
                If diffs.Average() <> diff_4x3_1 Then
                    Throw New Exception("diffs.Average() <> diff_4x3_1")
                End If
                If diffs.Max() <> 0 Then
                    Throw New Exception("diffs.Max() <> 0")
                End If
            Next
        Next

        For y = 0 To grayMatrixFromRgb_4x3.Height - 1
            Dim offset = y * grayMatrixFromRgb_4x3.Width
            For x = 0 To grayMatrixFromRgb_4x3.Width - 1
                If bmpGray.GetPixel(x, y).R <> grayMatrixFromRgb_4x3.GrayPixel(x, y) Then
                    Throw New Exception("bmpGray.GetPixel(x, y).R <> gm1_4x3.GrayPixel(x, y)")
                End If
                If bmpGray.GetPixel(x, y).G <> grayMatrixFromRgb_4x3.GrayPixel(x, y) Then
                    Throw New Exception("bmpGray.GetPixel(x, y).G <> gm1_4x3.GrayPixel(x, y)")
                End If
                If bmpGray.GetPixel(x, y).B <> grayMatrixFromRgb_4x3.GrayPixel(x, y) Then
                    Throw New Exception("bmpGray.GetPixel(x, y).B <> gm1_4x3.GrayPixel(x, y)")
                End If
            Next
        Next

        Dim bmpGray2_1 = grayMatrixFromRgb_4x4.ToBitmap()
        Dim bmpGray2_2 = grayMatrixFromGray_4x4.ToBitmap()
        For y = 0 To grayMatrixFromRgb_4x3.Height - 1
            Dim offset = y * grayMatrixFromRgb_4x3.Width
            For x = 0 To grayMatrixFromRgb_4x3.Width - 1
                If bmpGray2_1.GetPixel(x, y).R <> bmpGray2_2.GetPixel(x, y).R Then
                    Throw New Exception("bmpGray2_1.GetPixel(x, y).R <> bmpGray2_2.GetPixel(x, y).R")
                End If
                If bmpGray.GetPixel(x, y).R <> bmpGray2_1.GetPixel(x, y).R Then
                    Throw New Exception("bmpGray.GetPixel(x, y).R <> bmpGray2_1.GetPixel(x, y).R")
                End If
                If bmpGray.GetPixel(x, y).G <> bmpGray2_1.GetPixel(x, y).G Then
                    Throw New Exception("bmpGray.GetPixel(x, y).G <> bmpGray2_1.GetPixel(x, y).G")
                End If
                If bmpGray.GetPixel(x, y).B <> bmpGray2_1.GetPixel(x, y).B Then
                    Throw New Exception("bmpGray.GetPixel(x, y).B <> bmpGray2_1.GetPixel(x, y).B")
                End If
            Next
        Next
    End Sub

    <TestMethod()> Public Sub GrayMatrixResizeTest()
        Dim bmpGray = My.Resources._4x3_gray
        Dim grayMatrixFromGray_4x3 = BitmapConverter.BitmapToGrayMatrix(bmpGray)
        Dim grayMatrixFromGray_4x3_Resize = grayMatrixFromGray_4x3.ResizeTwo().ResizeHalf()
        If grayMatrixFromGray_4x3.Width <> grayMatrixFromGray_4x3_Resize.Width Then
            Throw New Exception("grayMatrixFromGray_4x3.Width <> grayMatrixFromGray_4x3_Resize.Width")
        End If
        For x = 0 To grayMatrixFromGray_4x3.Width - 1
            For y = 0 To grayMatrixFromGray_4x3.Height - 1
                If grayMatrixFromGray_4x3.GrayPixel(x, y) <> grayMatrixFromGray_4x3_Resize.GrayPixel(x, y) Then
                    Throw New Exception("grayMatrixFromGray_4x3.GrayPixel(x, y) <> grayMatrixFromGray_4x3_Resize.GrayPixel(x, y)")
                End If
            Next
        Next
    End Sub

    <TestMethod()> Public Sub RgbMatrixResizeTest()
        Dim bmpGray = My.Resources._4x3_rgb
        Dim grayMatrixFromRgb_4x3 = BitmapConverter.BitmapToRGBMatrix(bmpGray)
        Dim grayMatrixFromGray_4x3_Resize = grayMatrixFromRgb_4x3.ResizeTwo().ResizeHalf()
        If grayMatrixFromRgb_4x3.Width <> grayMatrixFromGray_4x3_Resize.Width Then
            Throw New Exception("grayMatrixFromGray_4x3.Width <> grayMatrixFromGray_4x3_Resize.Width")
        End If
        For x = 0 To grayMatrixFromRgb_4x3.Width - 1
            For y = 0 To grayMatrixFromRgb_4x3.Height - 1
                If grayMatrixFromRgb_4x3.RedPixel(x, y) <> grayMatrixFromGray_4x3_Resize.RedPixel(x, y) Then
                    Throw New Exception("grayMatrixFromRgb_4x3.RedPixel(x, y) <> grayMatrixFromGray_4x3_Resize.RedPixel(x, y)")
                End If
                If grayMatrixFromRgb_4x3.GreenPixel(x, y) <> grayMatrixFromGray_4x3_Resize.GreenPixel(x, y) Then
                    Throw New Exception("grayMatrixFromRgb_4x3.GreenPixel(x, y) <> grayMatrixFromGray_4x3_Resize.GreenPixel(x, y)")
                End If
                If grayMatrixFromRgb_4x3.BluePixel(x, y) <> grayMatrixFromGray_4x3_Resize.BluePixel(x, y) Then
                    Throw New Exception("grayMatrixFromRgb_4x3.BluePixel(x, y) <> grayMatrixFromGray_4x3_Resize.BluePixel(x, y)")
                End If
            Next
        Next
    End Sub

    <TestMethod()> Public Sub GrayFloatMatrixResizeTest()
        Dim bmpGray = My.Resources._4x3_gray
        Dim matrixFromGray_4x3 = BitmapConverter.BitmapToGrayMatrix(bmpGray)
        Dim floatMatrixFromGray_4x3 = New GrayFloatMatrix(matrixFromGray_4x3.Gray, matrixFromGray_4x3.Width, matrixFromGray_4x3.Height)
        Dim floatMatrixFromGray_4x3_Resize = floatMatrixFromGray_4x3.ResizeTwo().ResizeHalf()
        If floatMatrixFromGray_4x3_Resize.Width <> floatMatrixFromGray_4x3.Width Then
            Throw New Exception("floatMatrixFromGray_4x3_Resize.Width <> floatMatrixFromGray_4x3.Width")
        End If
        For x = 0 To floatMatrixFromGray_4x3.Width - 1
            For y = 0 To floatMatrixFromGray_4x3.Height - 1
                If floatMatrixFromGray_4x3.GrayPixel(x, y) <> floatMatrixFromGray_4x3.GrayPixel(x, y) Then
                    Throw New Exception("floatMatrixFromGray_4x3.GrayPixel(x, y) <> floatMatrixFromGray_4x3.GrayPixel(x, y)")
                End If
            Next
        Next
    End Sub

    <TestMethod()> Public Sub RGBFloatMatrixResizeTest()
        Dim bmpRgb = My.Resources._4x3_rgb
        Dim matrixFromRgb_4x3 = BitmapConverter.BitmapToRGBMatrix(bmpRgb)
        Dim floatMatrixFromRgb_4x3 = New RGBFloatMatrix(matrixFromRgb_4x3.Red, matrixFromRgb_4x3.Green, matrixFromRgb_4x3.Blue,
                                                        matrixFromRgb_4x3.Width, matrixFromRgb_4x3.Height)
        Dim floatMatrixFromRgb_4x3_Resize = floatMatrixFromRgb_4x3.ResizeTwo().ResizeHalf()
        If floatMatrixFromRgb_4x3_Resize.Width <> floatMatrixFromRgb_4x3.Width Then
            Throw New Exception("floatMatrixFromRgb_4x3_Resize.Width <> floatMatrixFromRgb_4x3.Width")
        End If
        For x = 0 To floatMatrixFromRgb_4x3.Width - 1
            For y = 0 To floatMatrixFromRgb_4x3.Height - 1
                If floatMatrixFromRgb_4x3.RedPixel(x, y) <> floatMatrixFromRgb_4x3_Resize.RedPixel(x, y) Then
                    Throw New Exception("floatMatrixFromRgb_4x3.RedPixel(x, y) <> floatMatrixFromRgb_4x3_Resize.RedPixel(x, y)")
                End If
                If floatMatrixFromRgb_4x3.GreenPixel(x, y) <> floatMatrixFromRgb_4x3_Resize.GreenPixel(x, y) Then
                    Throw New Exception("floatMatrixFromRgb_4x3.GreenPixel(x, y) <> floatMatrixFromRgb_4x3_Resize.GreenPixel(x, y)")
                End If
                If floatMatrixFromRgb_4x3.BluePixel(x, y) <> floatMatrixFromRgb_4x3_Resize.BluePixel(x, y) Then
                    Throw New Exception("floatMatrixFromRgb_4x3.BluePixel(x, y) <> floatMatrixFromRgb_4x3_Resize.BluePixel(x, y)")
                End If
            Next
        Next
    End Sub
End Class
