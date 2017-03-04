Imports System.Drawing

<TestClass()> Public Class MatrixTests

    <TestMethod()> Public Sub LoadAndAccessRgbMatrixTest()
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

    <TestMethod()> Public Sub SaveRgbMatrixTest()
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

    <TestMethod()> Public Sub RgbMatrixAccessTest()
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

    <TestMethod()> Public Sub GrayMatrixAccessTest()
        Dim bmp = New GrayMatrix(4, 4)
        bmp.GrayPixel(1, 1) = Byte.MaxValue
        Assert.AreEqual(255, bmp.GrayPixel(1, 1))
        Assert.AreEqual(255, bmp.Gray(1 + 4 * 1))
        Dim gr = CByte(bmp.GrayPixel(1, 1))
        Assert.AreEqual(CByte(255), gr)
    End Sub

    <TestMethod()> Public Sub BitmapToGrayMatrixTest1()
        Dim bmpRgb = My.Resources._4x3_rgb
        Dim bmpGray = My.Resources._4x3_gray
        Dim grayMatrixFromRgb_4x3 = BitmapConverter.BitmapToGrayMatrix(bmpRgb)
        Dim grayMatrixFromGray_4x3 = BitmapConverter.BitmapToGrayMatrix(bmpGray)

        Dim grayMatrixFromRgb_4x4 = New GrayMatrix(7, 3)
        Dim grayMatrixFromGray_4x4 = New GrayMatrix(7, 3)
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
                Assert.AreEqual(CInt(diffs.Average()), diff_4x3_1)
                Assert.IsTrue(diffs.Max() <= 1) 'Внешний редактор может рассчитывать переход RGB -> Gray немного иначе
            Next
        Next

        For y = 0 To grayMatrixFromRgb_4x3.Height - 1
            Dim offset = y * grayMatrixFromRgb_4x3.Width
            For x = 0 To grayMatrixFromRgb_4x3.Width - 1
                '<= 1: Внешний редактор может рассчитывать переход RGB -> Gray немного иначе
                Assert.IsTrue(Math.Abs(CInt(bmpGray.GetPixel(x, y).R) - grayMatrixFromRgb_4x3.GrayPixel(x, y)) <= 1)
                Assert.IsTrue(Math.Abs(CInt(bmpGray.GetPixel(x, y).G) - grayMatrixFromRgb_4x3.GrayPixel(x, y)) <= 1)
                Assert.IsTrue(Math.Abs(CInt(bmpGray.GetPixel(x, y).B) - grayMatrixFromRgb_4x3.GrayPixel(x, y)) <= 1)
            Next
        Next

        Dim bmpGray2_1 = grayMatrixFromRgb_4x4.ToBitmap()
        Dim bmpGray2_2 = grayMatrixFromGray_4x4.ToBitmap()
        For y = 0 To grayMatrixFromRgb_4x3.Height - 1
            Dim offset = y * grayMatrixFromRgb_4x3.Width
            For x = 0 To grayMatrixFromRgb_4x3.Width - 1
                '<= 1: Внешний редактор может рассчитывать переход RGB -> Gray немного иначе
                Assert.IsTrue(Math.Abs(CInt(bmpGray2_1.GetPixel(x, y).R) - CInt(bmpGray2_2.GetPixel(x, y).R)) <= 1)
                Assert.IsTrue(Math.Abs(CInt(bmpGray.GetPixel(x, y).R) - CInt(bmpGray2_1.GetPixel(x, y).R)) <= 1)
                Assert.IsTrue(Math.Abs(CInt(bmpGray.GetPixel(x, y).G) - CInt(bmpGray2_1.GetPixel(x, y).G)) <= 1)
                Assert.IsTrue(Math.Abs(CInt(bmpGray.GetPixel(x, y).B) - CInt(bmpGray2_1.GetPixel(x, y).B)) <= 1)
            Next
        Next
    End Sub

    <TestMethod()> Public Sub BitmapToGrayMatrixTest2()
        Dim bmpRgb = My.Resources.rgbw25
        Dim bmpGray = My.Resources.gray25
        Dim grayMatrixFromRgb = BitmapConverter.BitmapToGrayMatrix(bmpRgb)
        Dim grayMatrixFromGray = BitmapConverter.BitmapToGrayMatrix(bmpGray)

        For y = 0 To grayMatrixFromRgb.Height - 1
            Dim offset = y * grayMatrixFromRgb.Width
            For x = 0 To grayMatrixFromRgb.Width - 1
                Dim diff_1 = Math.Abs(grayMatrixFromRgb.GrayPixel(x, y) - grayMatrixFromGray.GrayPixel(x, y))
                Dim diff_2 = Math.Abs(grayMatrixFromRgb.Gray(x + offset) - grayMatrixFromGray.Gray(x + offset))
                Dim diffs = {diff_1, diff_2}
                Assert.AreEqual(CInt(diffs.Average()), diff_1)
                Assert.IsTrue(diffs.Max() <= 1) 'Внешний редактор может рассчитывать переход RGB -> Gray немного иначе
            Next
        Next

        For y = 0 To grayMatrixFromRgb.Height - 1
            Dim offset = y * grayMatrixFromRgb.Width
            For x = 0 To grayMatrixFromRgb.Width - 1
                '<= 1: Внешний редактор может рассчитывать переход RGB -> Gray немного иначе
                Assert.IsTrue(Math.Abs(CInt(bmpGray.GetPixel(x, y).R) - grayMatrixFromRgb.GrayPixel(x, y)) <= 1)
                Assert.IsTrue(Math.Abs(CInt(bmpGray.GetPixel(x, y).G) - grayMatrixFromRgb.GrayPixel(x, y)) <= 1)
                Assert.IsTrue(Math.Abs(CInt(bmpGray.GetPixel(x, y).B) - grayMatrixFromRgb.GrayPixel(x, y)) <= 1)
            Next
        Next
    End Sub

    <TestMethod()> Public Sub GrayMatrixResizeTest_4x3_rgb()
        GrayMatrixResizeTest(My.Resources._4x3_rgb)
    End Sub

    <TestMethod()> Public Sub GrayMatrixResizeTest_4x3_gray()
        GrayMatrixResizeTest(My.Resources._4x3_gray)
    End Sub

    <TestMethod()> Public Sub GrayMatrixResizeTest_rgbw24()
        GrayMatrixResizeTest(My.Resources.rgbw24)
    End Sub

    <TestMethod()> Public Sub GrayMatrixResizeTest_gray24()
        GrayMatrixResizeTest(My.Resources.gray24)
    End Sub

    <TestMethod()> Public Sub GrayMatrixResizeTest_rgbw25()
        GrayMatrixResizeTest(My.Resources.rgbw25)
    End Sub

    <TestMethod()> Public Sub GrayMatrixResizeTest_gray25()
        GrayMatrixResizeTest(My.Resources.gray25)
    End Sub

    Private Sub GrayMatrixResizeTest(bmp As Bitmap)
        Dim grayMatrixFromGray = BitmapConverter.BitmapToGrayMatrix(bmp)
        Dim grayMatrixFromGrayResize = grayMatrixFromGray.ResizeTwo().ResizeHalf()
        Assert.AreEqual(grayMatrixFromGray.Width, grayMatrixFromGrayResize.Width)

        For x = 0 To grayMatrixFromGray.Width - 1
            For y = 0 To grayMatrixFromGray.Height - 1
                Assert.AreEqual(grayMatrixFromGray.GrayPixel(x, y), grayMatrixFromGrayResize.GrayPixel(x, y))
            Next
        Next
    End Sub

    <TestMethod()> Public Sub RgbMatrixResizeTest_4x3_rgb()
        RgbMatrixResizeTest(My.Resources._4x3_rgb)
    End Sub

    <TestMethod()> Public Sub RgbMatrixResizeTest_4x3_gray()
        RgbMatrixResizeTest(My.Resources._4x3_gray)
    End Sub

    <TestMethod()> Public Sub RgbMatrixResizeTest_rgbw24()
        RgbMatrixResizeTest(My.Resources.rgbw24)
    End Sub

    <TestMethod()> Public Sub RgbMatrixResizeTest_gray24()
        RgbMatrixResizeTest(My.Resources.gray24)
    End Sub

    <TestMethod()> Public Sub RgbMatrixResizeTest_rgbw25()
        RgbMatrixResizeTest(My.Resources.rgbw25)
    End Sub

    <TestMethod()> Public Sub RgbMatrixResizeTest_gray25()
        RgbMatrixResizeTest(My.Resources.gray25)
    End Sub

    Private Sub RgbMatrixResizeTest(bmp As Bitmap)
        Dim grayMatrixFromRgb_4x3 = BitmapConverter.BitmapToRGBMatrix(bmp)
        Dim grayMatrixFromGray_4x3_Resize = grayMatrixFromRgb_4x3.ResizeTwo().ResizeHalf()
        Assert.AreEqual(grayMatrixFromRgb_4x3.Width, grayMatrixFromGray_4x3_Resize.Width)
        For x = 0 To grayMatrixFromRgb_4x3.Width - 1
            For y = 0 To grayMatrixFromRgb_4x3.Height - 1
                Assert.AreEqual(grayMatrixFromRgb_4x3.RedPixel(x, y), grayMatrixFromGray_4x3_Resize.RedPixel(x, y))
                Assert.AreEqual(grayMatrixFromRgb_4x3.GreenPixel(x, y), grayMatrixFromGray_4x3_Resize.GreenPixel(x, y))
                Assert.AreEqual(grayMatrixFromRgb_4x3.BluePixel(x, y), grayMatrixFromGray_4x3_Resize.BluePixel(x, y))
            Next
        Next
    End Sub

    <TestMethod()> Public Sub GrayFloatMatrixResizeTest_4x3_rgb()
        GrayFloatMatrixResizeTest(My.Resources._4x3_rgb)
    End Sub

    <TestMethod()> Public Sub GrayFloatMatrixResizeTest_4x3_gray()
        GrayFloatMatrixResizeTest(My.Resources._4x3_gray)
    End Sub

    <TestMethod()> Public Sub GrayFloatMatrixResizeTest_rgbw24()
        GrayFloatMatrixResizeTest(My.Resources.rgbw24)
    End Sub

    <TestMethod()> Public Sub GrayFloatMatrixResizeTest_gray24()
        GrayFloatMatrixResizeTest(My.Resources.gray24)
    End Sub

    <TestMethod()> Public Sub GrayFloatMatrixResizeTest_rgbw25()
        GrayFloatMatrixResizeTest(My.Resources.rgbw25)
    End Sub

    <TestMethod()> Public Sub GrayFloatMatrixResizeTest_gray25()
        GrayFloatMatrixResizeTest(My.Resources.gray25)
    End Sub

    Private Sub GrayFloatMatrixResizeTest(bmp As Bitmap)
        Dim matrixFromGray = BitmapConverter.BitmapToGrayMatrix(bmp)
        Dim floatMatrixFromGray = New GrayFloatMatrix(matrixFromGray.Gray, matrixFromGray.Width, matrixFromGray.Height)
        Dim floatMatrixFromGrayResize = floatMatrixFromGray.ResizeTwo().ResizeHalf()
        Assert.AreEqual(floatMatrixFromGrayResize.Width, floatMatrixFromGray.Width)
        For x = 0 To floatMatrixFromGray.Width - 1
            For y = 0 To floatMatrixFromGray.Height - 1
                Assert.AreEqual(floatMatrixFromGray.GrayPixel(x, y), floatMatrixFromGray.GrayPixel(x, y))
            Next
        Next
    End Sub

    <TestMethod()> Public Sub RgbFloatMatrixResizeTest_4x3_rgb()
        RgbFloatMatrixResizeTest(My.Resources._4x3_rgb)
    End Sub

    <TestMethod()> Public Sub RgbFloatMatrixResizeTest_4x3_gray()
        RgbFloatMatrixResizeTest(My.Resources._4x3_gray)
    End Sub

    <TestMethod()> Public Sub RgbFloatMatrixResizeTest_rgbw24()
        RgbFloatMatrixResizeTest(My.Resources.rgbw24)
    End Sub

    <TestMethod()> Public Sub RgbFloatMatrixResizeTest_gray24()
        RgbFloatMatrixResizeTest(My.Resources.gray24)
    End Sub

    <TestMethod()> Public Sub RgbFloatMatrixResizeTest_rgbw25()
        RgbFloatMatrixResizeTest(My.Resources.rgbw25)
    End Sub

    <TestMethod()> Public Sub RgbFloatMatrixResizeTest_gray25()
        RgbFloatMatrixResizeTest(My.Resources.gray25)
    End Sub

    Private Sub RgbFloatMatrixResizeTest(bmp As Bitmap)
        Dim matrixFromRgb = BitmapConverter.BitmapToRGBMatrix(bmp)
        Dim floatMatrixFromRgb = New RGBFloatMatrix(matrixFromRgb.Red, matrixFromRgb.Green, matrixFromRgb.Blue,
                                                    matrixFromRgb.Width, matrixFromRgb.Height)
        Dim floatMatrixFromRgbResize = floatMatrixFromRgb.ResizeTwo().ResizeHalf()
        Assert.AreEqual(floatMatrixFromRgbResize.Width, floatMatrixFromRgb.Width)
        For x = 0 To floatMatrixFromRgb.Width - 1
            For y = 0 To floatMatrixFromRgb.Height - 1
                Assert.AreEqual(floatMatrixFromRgb.RedPixel(x, y), floatMatrixFromRgbResize.RedPixel(x, y))
                Assert.AreEqual(floatMatrixFromRgb.GreenPixel(x, y), floatMatrixFromRgbResize.GreenPixel(x, y))
                Assert.AreEqual(floatMatrixFromRgb.BluePixel(x, y), floatMatrixFromRgbResize.BluePixel(x, y))
            Next
        Next
    End Sub

    <TestMethod()> Public Sub MatrixAlignAndCropTestGray_4x3_rgb()
        MatrixAlignAndCropTestGray(My.Resources._4x3_rgb, 3)
        MatrixAlignAndCropTestGray(My.Resources._4x3_rgb, 4)
        MatrixAlignAndCropTestGray(My.Resources._4x3_rgb, 5)
    End Sub

    <TestMethod()> Public Sub MatrixAlignAndCropTestGray_4x3_gray()
        MatrixAlignAndCropTestGray(My.Resources._4x3_gray, 3)
        MatrixAlignAndCropTestGray(My.Resources._4x3_gray, 4)
        MatrixAlignAndCropTestGray(My.Resources._4x3_gray, 5)
    End Sub

    <TestMethod()> Public Sub MatrixAlignAndCropTestGray_rgbw24()
        MatrixAlignAndCropTestGray(My.Resources.rgbw24, 3)
        MatrixAlignAndCropTestGray(My.Resources.rgbw24, 4)
        MatrixAlignAndCropTestGray(My.Resources.rgbw24, 5)
    End Sub

    <TestMethod()> Public Sub MatrixAlignAndCropTestGray_gray24()
        MatrixAlignAndCropTestGray(My.Resources.gray24, 3)
        MatrixAlignAndCropTestGray(My.Resources.gray24, 4)
        MatrixAlignAndCropTestGray(My.Resources.gray24, 5)
    End Sub

    <TestMethod()> Public Sub MatrixAlignAndCropTestGray_rgbw25()
        MatrixAlignAndCropTestGray(My.Resources.rgbw25, 3)
        MatrixAlignAndCropTestGray(My.Resources.rgbw25, 4)
        MatrixAlignAndCropTestGray(My.Resources.rgbw25, 5)
    End Sub

    <TestMethod()> Public Sub MatrixAlignAndCropTestGray_gray25()
        MatrixAlignAndCropTestGray(My.Resources.gray25, 3)
        MatrixAlignAndCropTestGray(My.Resources.gray25, 4)
        MatrixAlignAndCropTestGray(My.Resources.gray25, 5)
    End Sub

    Private Sub MatrixAlignAndCropTestGray(bmp As Bitmap, align As Integer)
        Dim matrixGray = BitmapConverter.BitmapToGrayMatrix(bmp)
        Dim matrixGrayAligned = MatrixTools.GrayMatrixAlign(matrixGray, align)
        Dim alignOffsetL = If(matrixGray.Width Mod align <> 0, (align - matrixGray.Width Mod align) \ 2, 0)
        Dim matrixGrayCropped = MatrixTools.GrayMatrixSubRect(matrixGrayAligned, New Rectangle(alignOffsetL, 0, matrixGray.Width, matrixGray.Height))
        If matrixGray.Width <> matrixGrayCropped.Width AndAlso matrixGray.Height <> matrixGrayCropped.Height Then
            Throw New Exception("matrixGray.Width <> matrixGrayCropped.Width AndAlso matrixGray.Height <> matrixGrayCropped.Height")
        End If
        For y = 0 To matrixGray.Height - 1
            For x = 0 To matrixGray.Width - 1
                Assert.AreEqual(matrixGray.GrayPixel(x, y), matrixGrayCropped.GrayPixel(x, y))
            Next
        Next
    End Sub

    <TestMethod()> Public Sub MatrixAlignAndCropTestRgb_4x3_rgb()
        MatrixAlignAndCropTestRgb(My.Resources._4x3_rgb, 3)
        MatrixAlignAndCropTestRgb(My.Resources._4x3_rgb, 4)
        MatrixAlignAndCropTestRgb(My.Resources._4x3_rgb, 5)
    End Sub

    <TestMethod()> Public Sub MatrixAlignAndCropTestRgb_4x3_gray()
        MatrixAlignAndCropTestRgb(My.Resources._4x3_gray, 3)
        MatrixAlignAndCropTestRgb(My.Resources._4x3_gray, 4)
        MatrixAlignAndCropTestRgb(My.Resources._4x3_gray, 5)
    End Sub

    <TestMethod()> Public Sub MatrixAlignAndCropTestRgb_rgbw24()
        MatrixAlignAndCropTestRgb(My.Resources.rgbw24, 3)
        MatrixAlignAndCropTestRgb(My.Resources.rgbw24, 4)
        MatrixAlignAndCropTestRgb(My.Resources.rgbw24, 5)
    End Sub

    <TestMethod()> Public Sub MatrixAlignAndCropTestRgb_gray24()
        MatrixAlignAndCropTestRgb(My.Resources.gray24, 3)
        MatrixAlignAndCropTestRgb(My.Resources.gray24, 4)
        MatrixAlignAndCropTestRgb(My.Resources.gray24, 5)
    End Sub

    <TestMethod()> Public Sub MatrixAlignAndCropTestRgb_rgbw25()
        MatrixAlignAndCropTestRgb(My.Resources.rgbw25, 3)
        MatrixAlignAndCropTestRgb(My.Resources.rgbw25, 4)
        MatrixAlignAndCropTestRgb(My.Resources.rgbw25, 5)
    End Sub

    <TestMethod()> Public Sub MatrixAlignAndCropTestRgb_gray25()
        MatrixAlignAndCropTestRgb(My.Resources.gray25, 3)
        MatrixAlignAndCropTestRgb(My.Resources.gray25, 4)
        MatrixAlignAndCropTestRgb(My.Resources.gray25, 5)
    End Sub

    Private Sub MatrixAlignAndCropTestRgb(bmp As Bitmap, align As Integer)
        Dim matrixRgb = BitmapConverter.BitmapToRGBMatrix(bmp)
        Dim matrixRgbAligned = MatrixTools.RGBMatrixAlign(matrixRgb, align)
        Dim alignOffsetL = If(matrixRgb.Width Mod align <> 0, (align - matrixRgb.Width Mod align) \ 2, 0)
        Dim matrixRGBCropped = MatrixTools.RGBMatrixSubRect(matrixRgbAligned, New Rectangle(alignOffsetL, 0, matrixRgb.Width, matrixRgb.Height))
        If matrixRgb.Width <> matrixRGBCropped.Width AndAlso matrixRgb.Height <> matrixRGBCropped.Height Then
            Throw New Exception("matrixGray.Width <> matrixGrayCropped.Width AndAlso matrixGray.Height <> matrixGrayCropped.Height")
        End If
        For y = 0 To matrixRgb.Height - 1
            For x = 0 To matrixRgb.Width - 1
                Assert.AreEqual(matrixRgb.RedPixel(x, y), matrixRGBCropped.RedPixel(x, y))
                Assert.AreEqual(matrixRgb.GreenPixel(x, y), matrixRGBCropped.GreenPixel(x, y))
                Assert.AreEqual(matrixRgb.BluePixel(x, y), matrixRGBCropped.BluePixel(x, y))
            Next
        Next
    End Sub

    <TestMethod()> Public Sub MatrixCropTestGray()
        Dim bmpGray = New Bitmap(My.Resources._4x3_gray, New Size(9, 7))
        Dim testRect = New Rectangle(3, 3, 4, 3)
        Dim bmpGrayCroppedEthalon = bmpGray.Clone(testRect, Drawing.Imaging.PixelFormat.DontCare)
        Dim bmpGrayCroppedEthalonMatrix = BitmapConverter.BitmapToGrayMatrix(bmpGrayCroppedEthalon)
        Dim matrixGray = BitmapConverter.BitmapToGrayMatrix(bmpGray)
        Dim matrixGrayCropped = MatrixTools.GrayMatrixSubRect(matrixGray, testRect)
        If matrixGrayCropped.Width <> bmpGrayCroppedEthalonMatrix.Width AndAlso matrixGrayCropped.Height <> bmpGrayCroppedEthalonMatrix.Height Then
            Throw New Exception("matrixGrayCropped.Width <> bmpGrayCroppedEthalonMatrix.Width AndAlso matrixGrayCropped.Height <> bmpGrayCroppedEthalonMatrix.Height")
        End If
        For y = 0 To matrixGrayCropped.Height - 1
            For x = 0 To matrixGrayCropped.Width - 1
                Assert.AreEqual(matrixGrayCropped.GrayPixel(x, y), bmpGrayCroppedEthalonMatrix.GrayPixel(x, y))
            Next
        Next
    End Sub

    <TestMethod()> Public Sub MatrixCropTestRgb()
        Dim bmpRgb = New Bitmap(My.Resources._4x3_rgb, New Size(9, 7))
        Dim testRect = New Rectangle(3, 3, 4, 3)
        Dim bmpRgbCroppedEthalon = bmpRgb.Clone(testRect, Drawing.Imaging.PixelFormat.DontCare)
        Dim bmpRgbCroppedEthalonMatrix = BitmapConverter.BitmapToRGBMatrix(bmpRgbCroppedEthalon)
        Dim matrixRgb = BitmapConverter.BitmapToRGBMatrix(bmpRgb)
        Dim matrixRgbCropped = MatrixTools.RGBMatrixSubRect(matrixRgb, testRect)
        If matrixRgbCropped.Width <> bmpRgbCroppedEthalonMatrix.Width AndAlso matrixRgbCropped.Height <> bmpRgbCroppedEthalonMatrix.Height Then
            Throw New Exception("matrixRgbCropped.Width <> bmpRgbCroppedEthalonMatrix.Width AndAlso matrixRgbCropped.Height <> bmpRgbCroppedEthalonMatrix.Height")
        End If
        For y = 0 To matrixRgbCropped.Height - 1
            For x = 0 To matrixRgbCropped.Width - 1
                Assert.AreEqual(matrixRgbCropped.RedPixel(x, y), bmpRgbCroppedEthalonMatrix.RedPixel(x, y))
                Assert.AreEqual(matrixRgbCropped.GreenPixel(x, y), bmpRgbCroppedEthalonMatrix.GreenPixel(x, y))
                Assert.AreEqual(matrixRgbCropped.BluePixel(x, y), bmpRgbCroppedEthalonMatrix.BluePixel(x, y))
            Next
        Next
    End Sub

    <TestMethod()> Public Sub MatrixInverseTestGray()
        Dim bmpGray = My.Resources._4x3_gray
        Dim grayMatrix1 = BitmapConverter.BitmapToGrayMatrix(bmpGray)
        Dim grayMatrix2 = grayMatrix1.Clone()
        MatrixTools.InverseGray(grayMatrix2)
        For y = 0 To grayMatrix1.Height - 1
            For x = 0 To grayMatrix1.Width - 1
                Assert.AreEqual(grayMatrix1.GrayPixel(x, y), Byte.MaxValue - grayMatrix2.GrayPixel(x, y))
            Next
        Next
        MatrixTools.InverseGray(grayMatrix2)
        For y = 0 To grayMatrix1.Height - 1
            For x = 0 To grayMatrix1.Width - 1
                Assert.AreEqual(grayMatrix1.GrayPixel(x, y), grayMatrix2.GrayPixel(x, y))
            Next
        Next
    End Sub

    <TestMethod()> Public Sub MatrixInverseTestRgb()
        Dim bmpRgb = My.Resources._4x3_rgb
        Dim rgbMatrix1 = BitmapConverter.BitmapToRGBMatrix(bmpRgb)
        Dim rgbMatrix2 = rgbMatrix1.Clone()
        MatrixTools.InverseRGB(rgbMatrix2)
        For y = 0 To rgbMatrix1.Height - 1
            For x = 0 To rgbMatrix1.Width - 1
                Assert.AreEqual(rgbMatrix1.RedPixel(x, y), Byte.MaxValue - rgbMatrix2.RedPixel(x, y))
                Assert.AreEqual(rgbMatrix1.GreenPixel(x, y), Byte.MaxValue - rgbMatrix2.GreenPixel(x, y))
                Assert.AreEqual(rgbMatrix1.BluePixel(x, y), Byte.MaxValue - rgbMatrix2.BluePixel(x, y))
            Next
        Next
        MatrixTools.InverseRGB(rgbMatrix2)
        For y = 0 To rgbMatrix1.Height - 1
            For x = 0 To rgbMatrix1.Width - 1
                Assert.AreEqual(rgbMatrix1.RedPixel(x, y), rgbMatrix2.RedPixel(x, y))
                Assert.AreEqual(rgbMatrix1.GreenPixel(x, y), rgbMatrix2.GreenPixel(x, y))
                Assert.AreEqual(rgbMatrix1.BluePixel(x, y), rgbMatrix2.BluePixel(x, y))
            Next
        Next
    End Sub
End Class
