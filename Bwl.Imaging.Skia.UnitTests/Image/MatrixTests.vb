Imports NUnit.Framework
Imports Bwl.Imaging.Skia
Imports SkiaSharp

<TestFixture>
<Parallelizable(ParallelScope.Self)>
Public Class MatrixTests

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub LoadAndAccessRgbMatrixTest()
        Dim bmp = ResourceLoader.Res_4x3_rgb
        Dim rgb = bmp.ToRgbMatrix()
        Assert.That(rgb.Width, [Is].EqualTo(4))
        Assert.That(rgb.Height, [Is].EqualTo(3))
        Assert.That(rgb.GetRedPixel(0, 0), [Is].EqualTo(0)) : Assert.That(rgb.GetGreenPixel(0, 0), [Is].EqualTo(0)) : Assert.That(rgb.GetBluePixel(0, 0), [Is].EqualTo(0))
        Assert.That(rgb.GetRedPixel(1, 0), [Is].EqualTo(255)) : Assert.That(rgb.GetGreenPixel(1, 0), [Is].EqualTo(0)) : Assert.That(rgb.GetBluePixel(1, 0), [Is].EqualTo(0))
        Assert.That(rgb.GetRedPixel(2, 0), [Is].EqualTo(0)) : Assert.That(rgb.GetGreenPixel(2, 0), [Is].EqualTo(255)) : Assert.That(rgb.GetBluePixel(2, 0), [Is].EqualTo(0))
        Assert.That(rgb.GetRedPixel(3, 0), [Is].EqualTo(0)) : Assert.That(rgb.GetGreenPixel(3, 0), [Is].EqualTo(0)) : Assert.That(rgb.GetBluePixel(3, 0), [Is].EqualTo(255))

        Assert.That(rgb.GetRedPixel(0, 1), [Is].EqualTo(255)) : Assert.That(rgb.GetGreenPixel(0, 1), [Is].EqualTo(255)) : Assert.That(rgb.GetBluePixel(0, 1), [Is].EqualTo(255))
        Assert.That(rgb.GetRedPixel(1, 1), [Is].EqualTo(255)) : Assert.That(rgb.GetGreenPixel(1, 1), [Is].EqualTo(255)) : Assert.That(rgb.GetBluePixel(1, 1), [Is].EqualTo(255))
        Assert.That(rgb.GetRedPixel(2, 1), [Is].EqualTo(255)) : Assert.That(rgb.GetGreenPixel(2, 1), [Is].EqualTo(255)) : Assert.That(rgb.GetBluePixel(2, 1), [Is].EqualTo(255))
        Assert.That(rgb.GetRedPixel(3, 1), [Is].EqualTo(255)) : Assert.That(rgb.GetGreenPixel(3, 1), [Is].EqualTo(255)) : Assert.That(rgb.GetBluePixel(3, 1), [Is].EqualTo(255))

        Assert.That(rgb.GetRedPixel(0, 2), [Is].EqualTo(0)) : Assert.That(rgb.GetGreenPixel(0, 2), [Is].EqualTo(0)) : Assert.That(rgb.GetBluePixel(0, 2), [Is].EqualTo(255))
        Assert.That(rgb.GetRedPixel(1, 2), [Is].EqualTo(0)) : Assert.That(rgb.GetGreenPixel(1, 2), [Is].EqualTo(255)) : Assert.That(rgb.GetBluePixel(1, 2), [Is].EqualTo(0))
        Assert.That(rgb.GetRedPixel(2, 2), [Is].EqualTo(255)) : Assert.That(rgb.GetGreenPixel(2, 2), [Is].EqualTo(0)) : Assert.That(rgb.GetBluePixel(2, 2), [Is].EqualTo(0))
        Assert.That(rgb.GetRedPixel(3, 2), [Is].EqualTo(0)) : Assert.That(rgb.GetGreenPixel(3, 2), [Is].EqualTo(0)) : Assert.That(rgb.GetBluePixel(3, 2), [Is].EqualTo(0))
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub LoadAndAccessRgbMatrixTest2()
        Dim bmp = ResourceLoader.Res_4x3_rgb
        Dim bmpArgb = RgbToArgb(bmp, 255)
        Dim rgb = bmpArgb.ToRgbMatrix()
        Assert.That(rgb.Width, [Is].EqualTo(4))
        Assert.That(rgb.Height, [Is].EqualTo(3))
        Assert.That(rgb.GetRedPixel(0, 0), [Is].EqualTo(0)) : Assert.That(rgb.GetGreenPixel(0, 0), [Is].EqualTo(0)) : Assert.That(rgb.GetBluePixel(0, 0), [Is].EqualTo(0))
        Assert.That(rgb.GetRedPixel(1, 0), [Is].EqualTo(255)) : Assert.That(rgb.GetGreenPixel(1, 0), [Is].EqualTo(0)) : Assert.That(rgb.GetBluePixel(1, 0), [Is].EqualTo(0))
        Assert.That(rgb.GetRedPixel(2, 0), [Is].EqualTo(0)) : Assert.That(rgb.GetGreenPixel(2, 0), [Is].EqualTo(255)) : Assert.That(rgb.GetBluePixel(2, 0), [Is].EqualTo(0))
        Assert.That(rgb.GetRedPixel(3, 0), [Is].EqualTo(0)) : Assert.That(rgb.GetGreenPixel(3, 0), [Is].EqualTo(0)) : Assert.That(rgb.GetBluePixel(3, 0), [Is].EqualTo(255))

        Assert.That(rgb.GetRedPixel(0, 1), [Is].EqualTo(255)) : Assert.That(rgb.GetGreenPixel(0, 1), [Is].EqualTo(255)) : Assert.That(rgb.GetBluePixel(0, 1), [Is].EqualTo(255))
        Assert.That(rgb.GetRedPixel(1, 1), [Is].EqualTo(255)) : Assert.That(rgb.GetGreenPixel(1, 1), [Is].EqualTo(255)) : Assert.That(rgb.GetBluePixel(1, 1), [Is].EqualTo(255))
        Assert.That(rgb.GetRedPixel(2, 1), [Is].EqualTo(255)) : Assert.That(rgb.GetGreenPixel(2, 1), [Is].EqualTo(255)) : Assert.That(rgb.GetBluePixel(2, 1), [Is].EqualTo(255))
        Assert.That(rgb.GetRedPixel(3, 1), [Is].EqualTo(255)) : Assert.That(rgb.GetGreenPixel(3, 1), [Is].EqualTo(255)) : Assert.That(rgb.GetBluePixel(3, 1), [Is].EqualTo(255))

        Assert.That(rgb.GetRedPixel(0, 2), [Is].EqualTo(0)) : Assert.That(rgb.GetGreenPixel(0, 2), [Is].EqualTo(0)) : Assert.That(rgb.GetBluePixel(0, 2), [Is].EqualTo(255))
        Assert.That(rgb.GetRedPixel(1, 2), [Is].EqualTo(0)) : Assert.That(rgb.GetGreenPixel(1, 2), [Is].EqualTo(255)) : Assert.That(rgb.GetBluePixel(1, 2), [Is].EqualTo(0))
        Assert.That(rgb.GetRedPixel(2, 2), [Is].EqualTo(255)) : Assert.That(rgb.GetGreenPixel(2, 2), [Is].EqualTo(0)) : Assert.That(rgb.GetBluePixel(2, 2), [Is].EqualTo(0))
        Assert.That(rgb.GetRedPixel(3, 2), [Is].EqualTo(0)) : Assert.That(rgb.GetGreenPixel(3, 2), [Is].EqualTo(0)) : Assert.That(rgb.GetBluePixel(3, 2), [Is].EqualTo(0))
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub SaveRgbMatrixTest()
        Dim bmp = ResourceLoader.Res_4x3_rgb
        Dim rgb = bmp.ToRgbMatrix()
        Dim newbmp = rgb.ToSKBitmap()
        Dim clr00 = newbmp.GetPixel(0, 0)
        Dim clr10 = newbmp.GetPixel(1, 0)
        Dim clr20 = newbmp.GetPixel(2, 0)
        Dim clr30 = newbmp.GetPixel(3, 0)
        ' newbmp.Save("C:\Users\heart\Repositories\test1.bmp")
        Assert.That(clr00, [Is].EqualTo(SKColors.Black))
        Assert.That(clr10, [Is].EqualTo(SKColors.Red))
        Assert.That(clr20, [Is].EqualTo(New SKColor(0, 255, 0)))
        Assert.That(clr30, [Is].EqualTo(SKColors.Blue))
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub RgbMatrixAccessTest()
        Dim bmp = New RGBMatrix(4, 4)
        bmp.SetColorPixel(1, 1, SKColors.Red)
        Assert.That(bmp.GetRedPixel(1, 1), [Is].EqualTo(255))
        Assert.That(bmp.GetGreenPixel(1, 1), [Is].EqualTo(0))
        Assert.That(bmp.GetBluePixel(1, 1), [Is].EqualTo(0))
        Assert.That(bmp.Red(1 + 4 * 1), [Is].EqualTo(255))
        Assert.That(bmp.Green(1 + 4 * 1), [Is].EqualTo(0))
        Assert.That(bmp.Blue(1 + 4 * 1), [Is].EqualTo(0))

        Dim clr = bmp.GetColorPixel(1, 1)
        Assert.That(clr.Red, [Is].EqualTo(CByte(255)))
        Assert.That(clr.Green, [Is].EqualTo(CByte(0)))
        Assert.That(clr.Blue, [Is].EqualTo(CByte(0)))
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub GrayMatrixAccessTest()
        Dim bmp = New GrayMatrix(4, 4)
        bmp.SetGrayPixel(1, 1, Byte.MaxValue)
        Assert.That(bmp.GetGrayPixel(1, 1), [Is].EqualTo(255))
        Assert.That(bmp.Gray(1 + 4 * 1), [Is].EqualTo(255))
        Dim gr = CByte(bmp.GetGrayPixel(1, 1))
        Assert.That(gr, [Is].EqualTo(CByte(255)))
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapToGrayMatrixTest1()
        Dim bmpRgb = ResourceLoader.Res_4x3_rgb
        Dim bmpGray = ResourceLoader.Res_4x3_gray
        Dim grayMatrixFromRgb_4x3 = bmpRgb.ToGrayMatrix()
        Dim grayMatrixFromGray_4x3 = bmpGray.ToGrayMatrix()

        Dim grayMatrixFromRgb_4x4 = New GrayMatrix(7, 3)
        Dim grayMatrixFromGray_4x4 = New GrayMatrix(7, 3)
        For y = 0 To grayMatrixFromRgb_4x3.Height - 1
            For x = 0 To grayMatrixFromRgb_4x3.Width - 1
                grayMatrixFromRgb_4x4.SetGrayPixel(x, y, grayMatrixFromRgb_4x3.GetGrayPixel(x, y))
                grayMatrixFromGray_4x4.SetGrayPixel(x, y, grayMatrixFromGray_4x3.GetGrayPixel(x, y))
            Next
        Next

        For y = 0 To grayMatrixFromRgb_4x3.Height - 1
            Dim offset = y * grayMatrixFromRgb_4x3.Width
            For x = 0 To grayMatrixFromRgb_4x3.Width - 1
                Dim diff_4x3_1 = Math.Abs(grayMatrixFromRgb_4x3.GetGrayPixel(x, y) - grayMatrixFromGray_4x3.GetGrayPixel(x, y))
                Dim diff_4x3_2 = Math.Abs(grayMatrixFromRgb_4x3.Gray(x + offset) - grayMatrixFromGray_4x3.Gray(x + offset))
                Dim diff_4x4_1 = Math.Abs(grayMatrixFromRgb_4x4.GetGrayPixel(x, y) - grayMatrixFromGray_4x4.GetGrayPixel(x, y))
                Dim diff_4x4_2 = Math.Abs(grayMatrixFromRgb_4x4.Gray(x + offset) - grayMatrixFromGray_4x4.Gray(x + offset))
                Dim diffs = {diff_4x3_1, diff_4x3_2, diff_4x4_1, diff_4x4_2}
                Assert.That(CInt(diffs.Average()), [Is].EqualTo(diff_4x3_1))
                Assert.That(diffs.Max(), [Is].LessThanOrEqualTo(1)) 'Внешний редактор может рассчитывать переход RGB -> Gray немного иначе
            Next
        Next

        For y = 0 To grayMatrixFromRgb_4x3.Height - 1
            Dim offset = y * grayMatrixFromRgb_4x3.Width
            For x = 0 To grayMatrixFromRgb_4x3.Width - 1
                '<= 1: Внешний редактор может рассчитывать переход RGB -> Gray немного иначе
                Assert.That(Math.Abs(CInt(bmpGray.GetPixel(x, y).Red) - grayMatrixFromRgb_4x3.GetGrayPixel(x, y)), [Is].LessThanOrEqualTo(1))
                Assert.That(Math.Abs(CInt(bmpGray.GetPixel(x, y).Green) - grayMatrixFromRgb_4x3.GetGrayPixel(x, y)), [Is].LessThanOrEqualTo(1))
                Assert.That(Math.Abs(CInt(bmpGray.GetPixel(x, y).Blue) - grayMatrixFromRgb_4x3.GetGrayPixel(x, y)), [Is].LessThanOrEqualTo(1))
            Next
        Next

        Dim bmpGray2_1 = grayMatrixFromRgb_4x4.ToSKBitmap()
        Dim bmpGray2_2 = grayMatrixFromGray_4x4.ToSKBitmap()
        For y = 0 To grayMatrixFromRgb_4x3.Height - 1
            Dim offset = y * grayMatrixFromRgb_4x3.Width
            For x = 0 To grayMatrixFromRgb_4x3.Width - 1
                '<= 1: Внешний редактор может рассчитывать переход RGB -> Gray немного иначе
                Assert.That(Math.Abs(CInt(bmpGray2_1.GetPixel(x, y).Red) - CInt(bmpGray2_2.GetPixel(x, y).Red)), [Is].LessThanOrEqualTo(1))
                Assert.That(Math.Abs(CInt(bmpGray.GetPixel(x, y).Red) - CInt(bmpGray2_1.GetPixel(x, y).Red)), [Is].LessThanOrEqualTo(1))
                Assert.That(Math.Abs(CInt(bmpGray.GetPixel(x, y).Green) - CInt(bmpGray2_1.GetPixel(x, y).Green)), [Is].LessThanOrEqualTo(1))
                Assert.That(Math.Abs(CInt(bmpGray.GetPixel(x, y).Blue) - CInt(bmpGray2_1.GetPixel(x, y).Blue)), [Is].LessThanOrEqualTo(1))
            Next
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapToGrayMatrixTest2()
        Dim bmpRgb = ResourceLoader.Resrgbw25
        Dim bmpGray = ResourceLoader.Resgray25
        Dim grayMatrixFromRgb = bmpRgb.ToGrayMatrix()
        Dim grayMatrixFromGray = bmpGray.ToGrayMatrix()

        For y = 0 To grayMatrixFromRgb.Height - 1
            Dim offset = y * grayMatrixFromRgb.Width
            For x = 0 To grayMatrixFromRgb.Width - 1
                Dim diff_1 = Math.Abs(grayMatrixFromRgb.GetGrayPixel(x, y) - grayMatrixFromGray.GetGrayPixel(x, y))
                Dim diff_2 = Math.Abs(grayMatrixFromRgb.Gray(x + offset) - grayMatrixFromGray.Gray(x + offset))
                Dim diffs = {diff_1, diff_2}
                Assert.That(CInt(diffs.Average()), [Is].EqualTo(diff_1))
                Assert.That(diffs.Max(), [Is].LessThanOrEqualTo(1)) 'Внешний редактор может рассчитывать переход RGB -> Gray немного иначе
            Next
        Next

        For y = 0 To grayMatrixFromRgb.Height - 1
            Dim offset = y * grayMatrixFromRgb.Width
            For x = 0 To grayMatrixFromRgb.Width - 1
                '<= 1: Внешний редактор может рассчитывать переход RGB -> Gray немного иначе
                Assert.That(Math.Abs(CInt(bmpGray.GetPixel(x, y).Red) - grayMatrixFromRgb.GetGrayPixel(x, y)), [Is].LessThanOrEqualTo(1))
                Assert.That(Math.Abs(CInt(bmpGray.GetPixel(x, y).Green) - grayMatrixFromRgb.GetGrayPixel(x, y)), [Is].LessThanOrEqualTo(1))
                Assert.That(Math.Abs(CInt(bmpGray.GetPixel(x, y).Blue) - grayMatrixFromRgb.GetGrayPixel(x, y)), [Is].LessThanOrEqualTo(1))
            Next
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub GrayMatrixResizeTest_4x3_rgb()
        GrayMatrixResizeTest(ResourceLoader.Res_4x3_rgb)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub GrayMatrixResizeTest_4x3_gray()
        GrayMatrixResizeTest(ResourceLoader.Res_4x3_gray)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub GrayMatrixResizeTest_rgbw24()
        GrayMatrixResizeTest(ResourceLoader.Resrgbw24)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub GrayMatrixResizeTest_gray24()
        GrayMatrixResizeTest(ResourceLoader.Resgray24)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub GrayMatrixResizeTest_rgbw25()
        GrayMatrixResizeTest(ResourceLoader.Resrgbw25)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub GrayMatrixResizeTest_gray25()
        GrayMatrixResizeTest(ResourceLoader.Resgray25)
    End Sub

    Private Sub GrayMatrixResizeTest(bmp As SKBitmap)
        Dim grayMatrixFromGray = bmp.ToGrayMatrix()
        Dim grayMatrixFromGrayResize = grayMatrixFromGray.ResizeTwo().ResizeHalf()
        Assert.That(grayMatrixFromGray.Width, [Is].EqualTo(grayMatrixFromGrayResize.Width))

        For x = 0 To grayMatrixFromGray.Width - 1
            For y = 0 To grayMatrixFromGray.Height - 1
                Assert.That(grayMatrixFromGray.GetGrayPixel(x, y), [Is].EqualTo(grayMatrixFromGrayResize.GetGrayPixel(x, y)))
            Next
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub RgbMatrixResizeTest_4x3_rgb()
        RgbMatrixResizeTest(ResourceLoader.Res_4x3_rgb)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub RgbMatrixResizeTest_4x3_gray()
        RgbMatrixResizeTest(ResourceLoader.Res_4x3_gray)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub RgbMatrixResizeTest_rgbw24()
        RgbMatrixResizeTest(ResourceLoader.Resrgbw24)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub RgbMatrixResizeTest_gray24()
        RgbMatrixResizeTest(ResourceLoader.Resgray24)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub RgbMatrixResizeTest_rgbw25()
        RgbMatrixResizeTest(ResourceLoader.Resrgbw25)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub RgbMatrixResizeTest_gray25()
        RgbMatrixResizeTest(ResourceLoader.Resgray25)
    End Sub

    Private Sub RgbMatrixResizeTest(bmp As SKBitmap)
        Dim grayMatrixFromRgb_4x3 = bmp.ToRgbMatrix()
        Dim grayMatrixFromGray_4x3_Resize = grayMatrixFromRgb_4x3.ResizeTwo().ResizeHalf()
        Assert.That(grayMatrixFromRgb_4x3.Width, [Is].EqualTo(grayMatrixFromGray_4x3_Resize.Width))
        For x = 0 To grayMatrixFromRgb_4x3.Width - 1
            For y = 0 To grayMatrixFromRgb_4x3.Height - 1
                Assert.That(grayMatrixFromRgb_4x3.GetRedPixel(x, y), [Is].EqualTo(grayMatrixFromGray_4x3_Resize.GetRedPixel(x, y)))
                Assert.That(grayMatrixFromRgb_4x3.GetGreenPixel(x, y), [Is].EqualTo(grayMatrixFromGray_4x3_Resize.GetGreenPixel(x, y)))
                Assert.That(grayMatrixFromRgb_4x3.GetBluePixel(x, y), [Is].EqualTo(grayMatrixFromGray_4x3_Resize.GetBluePixel(x, y)))
            Next
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub GrayFloatMatrixResizeTest_4x3_rgb()
        GrayFloatMatrixResizeTest(ResourceLoader.Res_4x3_rgb)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub GrayFloatMatrixResizeTest_4x3_gray()
        GrayFloatMatrixResizeTest(ResourceLoader.Res_4x3_gray)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub GrayFloatMatrixResizeTest_rgbw24()
        GrayFloatMatrixResizeTest(ResourceLoader.Resrgbw24)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub GrayFloatMatrixResizeTest_gray24()
        GrayFloatMatrixResizeTest(ResourceLoader.Resgray24)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub GrayFloatMatrixResizeTest_rgbw25()
        GrayFloatMatrixResizeTest(ResourceLoader.Resrgbw25)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub GrayFloatMatrixResizeTest_gray25()
        GrayFloatMatrixResizeTest(ResourceLoader.Resgray25)
    End Sub

    Private Sub GrayFloatMatrixResizeTest(bmp As SKBitmap)
        Dim matrixFromGray = bmp.ToGrayMatrix()
        Dim floatMatrixFromGray = New GrayFloatMatrix(matrixFromGray.Gray, matrixFromGray.Width, matrixFromGray.Height)
        Dim floatMatrixFromGrayResize = floatMatrixFromGray.ResizeTwo().ResizeHalf()
        Assert.That(floatMatrixFromGray.Width, [Is].EqualTo(floatMatrixFromGrayResize.Width))
        For x = 0 To floatMatrixFromGray.Width - 1
            For y = 0 To floatMatrixFromGray.Height - 1
                Assert.That(floatMatrixFromGray.GetGrayPixel(x, y), [Is].EqualTo(floatMatrixFromGrayResize.GetGrayPixel(x, y)))
            Next
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub RgbFloatMatrixResizeTest_4x3_rgb()
        RgbFloatMatrixResizeTest(ResourceLoader.Res_4x3_rgb)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub RgbFloatMatrixResizeTest_4x3_gray()
        RgbFloatMatrixResizeTest(ResourceLoader.Res_4x3_gray)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub RgbFloatMatrixResizeTest_rgbw24()
        RgbFloatMatrixResizeTest(ResourceLoader.Resrgbw24)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub RgbFloatMatrixResizeTest_gray24()
        RgbFloatMatrixResizeTest(ResourceLoader.Resgray24)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub RgbFloatMatrixResizeTest_rgbw25()
        RgbFloatMatrixResizeTest(ResourceLoader.Resrgbw25)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub RgbFloatMatrixResizeTest_gray25()
        RgbFloatMatrixResizeTest(ResourceLoader.Resgray25)
    End Sub

    Private Sub RgbFloatMatrixResizeTest(bmp As SKBitmap)
        Dim matrixFromRgb = bmp.ToRgbMatrix()
        Dim floatMatrixFromRgb = New RGBFloatMatrix(matrixFromRgb.Red, matrixFromRgb.Green, matrixFromRgb.Blue,
                                                    matrixFromRgb.Width, matrixFromRgb.Height)
        Dim floatMatrixFromRgbResize = floatMatrixFromRgb.ResizeTwo().ResizeHalf()
        Assert.That(floatMatrixFromRgb.Width, [Is].EqualTo(floatMatrixFromRgbResize.Width))
        For x = 0 To floatMatrixFromRgb.Width - 1
            For y = 0 To floatMatrixFromRgb.Height - 1
                Assert.That(floatMatrixFromRgb.GetRedPixel(x, y), [Is].EqualTo(floatMatrixFromRgbResize.GetRedPixel(x, y)))
                Assert.That(floatMatrixFromRgb.GetGreenPixel(x, y), [Is].EqualTo(floatMatrixFromRgbResize.GetGreenPixel(x, y)))
                Assert.That(floatMatrixFromRgb.GetBluePixel(x, y), [Is].EqualTo(floatMatrixFromRgbResize.GetBluePixel(x, y)))
            Next
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub MatrixAlignAndCropTestGray_4x3_rgb()
        MatrixAlignAndCropTestGray(ResourceLoader.Res_4x3_rgb, 3)
        MatrixAlignAndCropTestGray(ResourceLoader.Res_4x3_rgb, 4)
        MatrixAlignAndCropTestGray(ResourceLoader.Res_4x3_rgb, 5)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub MatrixAlignAndCropTestGray_4x3_gray()
        MatrixAlignAndCropTestGray(ResourceLoader.Res_4x3_gray, 3)
        MatrixAlignAndCropTestGray(ResourceLoader.Res_4x3_gray, 4)
        MatrixAlignAndCropTestGray(ResourceLoader.Res_4x3_gray, 5)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub MatrixAlignAndCropTestGray_rgbw24()
        MatrixAlignAndCropTestGray(ResourceLoader.Resrgbw24, 3)
        MatrixAlignAndCropTestGray(ResourceLoader.Resrgbw24, 4)
        MatrixAlignAndCropTestGray(ResourceLoader.Resrgbw24, 5)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub MatrixAlignAndCropTestGray_gray24()
        MatrixAlignAndCropTestGray(ResourceLoader.Resgray24, 3)
        MatrixAlignAndCropTestGray(ResourceLoader.Resgray24, 4)
        MatrixAlignAndCropTestGray(ResourceLoader.Resgray24, 5)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub MatrixAlignAndCropTestGray_rgbw25()
        MatrixAlignAndCropTestGray(ResourceLoader.Resrgbw25, 3)
        MatrixAlignAndCropTestGray(ResourceLoader.Resrgbw25, 4)
        MatrixAlignAndCropTestGray(ResourceLoader.Resrgbw25, 5)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub MatrixAlignAndCropTestGray_gray25()
        MatrixAlignAndCropTestGray(ResourceLoader.Resgray25, 3)
        MatrixAlignAndCropTestGray(ResourceLoader.Resgray25, 4)
        MatrixAlignAndCropTestGray(ResourceLoader.Resgray25, 5)
    End Sub

    Private Sub MatrixAlignAndCropTestGray(bmp As SKBitmap, align As Integer)
        Dim matrixGray = bmp.ToGrayMatrix()
        Dim matrixGrayAligned = MatrixTools.GrayMatrixAlign(matrixGray, align)
        Dim alignOffsetL = If(matrixGray.Width Mod align <> 0, (align - matrixGray.Width Mod align) \ 2, 0)
        Dim matrixGrayCropped = MatrixTools.GrayMatrixSubRect(matrixGrayAligned, SKExtensions.SKRectIFromXYWH(alignOffsetL, 0, matrixGray.Width, matrixGray.Height))
        If matrixGray.Width <> matrixGrayCropped.Width AndAlso matrixGray.Height <> matrixGrayCropped.Height Then
            Throw New Exception("matrixGray.Width <> matrixGrayCropped.Width AndAlso matrixGray.Height <> matrixGrayCropped.Height")
        End If
        For y = 0 To matrixGray.Height - 1
            For x = 0 To matrixGray.Width - 1
                Assert.That(matrixGray.GetGrayPixel(x, y), [Is].EqualTo(matrixGrayCropped.GetGrayPixel(x, y)))
            Next
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub MatrixAlignAndCropTestRgb_4x3_rgb()
        MatrixAlignAndCropTestRgb(ResourceLoader.Res_4x3_rgb, 3)
        MatrixAlignAndCropTestRgb(ResourceLoader.Res_4x3_rgb, 4)
        MatrixAlignAndCropTestRgb(ResourceLoader.Res_4x3_rgb, 5)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub MatrixAlignAndCropTestRgb_4x3_gray()
        MatrixAlignAndCropTestRgb(ResourceLoader.Res_4x3_gray, 3)
        MatrixAlignAndCropTestRgb(ResourceLoader.Res_4x3_gray, 4)
        MatrixAlignAndCropTestRgb(ResourceLoader.Res_4x3_gray, 5)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub MatrixAlignAndCropTestRgb_rgbw24()
        MatrixAlignAndCropTestRgb(ResourceLoader.Resrgbw24, 3)
        MatrixAlignAndCropTestRgb(ResourceLoader.Resrgbw24, 4)
        MatrixAlignAndCropTestRgb(ResourceLoader.Resrgbw24, 5)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub MatrixAlignAndCropTestRgb_gray24()
        MatrixAlignAndCropTestRgb(ResourceLoader.Resgray24, 3)
        MatrixAlignAndCropTestRgb(ResourceLoader.Resgray24, 4)
        MatrixAlignAndCropTestRgb(ResourceLoader.Resgray24, 5)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub MatrixAlignAndCropTestRgb_rgbw25()
        MatrixAlignAndCropTestRgb(ResourceLoader.Resrgbw25, 3)
        MatrixAlignAndCropTestRgb(ResourceLoader.Resrgbw25, 4)
        MatrixAlignAndCropTestRgb(ResourceLoader.Resrgbw25, 5)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub MatrixAlignAndCropTestRgb_gray25()
        MatrixAlignAndCropTestRgb(ResourceLoader.Resgray25, 3)
        MatrixAlignAndCropTestRgb(ResourceLoader.Resgray25, 4)
        MatrixAlignAndCropTestRgb(ResourceLoader.Resgray25, 5)
    End Sub

    Private Sub MatrixAlignAndCropTestRgb(bmp As SKBitmap, align As Integer)
        Dim matrixRgb = bmp.ToRgbMatrix()
        Dim matrixRgbAligned = MatrixTools.RGBMatrixAlign(matrixRgb, align)
        Dim alignOffsetL = If(matrixRgb.Width Mod align <> 0, (align - matrixRgb.Width Mod align) \ 2, 0)
        Dim matrixRGBCropped = MatrixTools.RGBMatrixSubRect(matrixRgbAligned, SKExtensions.SKRectIFromXYWH(alignOffsetL, 0, matrixRgb.Width, matrixRgb.Height))
        If matrixRgb.Width <> matrixRGBCropped.Width AndAlso matrixRgb.Height <> matrixRGBCropped.Height Then
            Throw New Exception("matrixGray.Width <> matrixGrayCropped.Width AndAlso matrixGray.Height <> matrixGrayCropped.Height")
        End If
        For y = 0 To matrixRgb.Height - 1
            For x = 0 To matrixRgb.Width - 1
                Assert.That(matrixRgb.GetRedPixel(x, y), [Is].EqualTo(matrixRGBCropped.GetRedPixel(x, y)))
                Assert.That(matrixRgb.GetGreenPixel(x, y), [Is].EqualTo(matrixRGBCropped.GetGreenPixel(x, y)))
                Assert.That(matrixRgb.GetBluePixel(x, y), [Is].EqualTo(matrixRGBCropped.GetBluePixel(x, y)))
            Next
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub MatrixCropTestGray()
        Dim bmpGray = ResourceLoader.Res_4x3_gray.CloneResized(New SKSize(9, 7))
        Dim testRect = SKExtensions.SKRectIFromXYWH(3, 3, 4, 3)
        Dim bmpGrayCroppedEthalon = UnsafeFunctions.Crop(bmpGray, testRect)
        Dim bmpGrayCroppedEthalonMatrix = bmpGrayCroppedEthalon.ToGrayMatrix()
        Dim matrixGray = bmpGray.ToGrayMatrix()
        Dim matrixGrayCropped = MatrixTools.GrayMatrixSubRect(matrixGray, testRect)
        If matrixGrayCropped.Width <> bmpGrayCroppedEthalonMatrix.Width AndAlso matrixGrayCropped.Height <> bmpGrayCroppedEthalonMatrix.Height Then
            Throw New Exception("matrixGrayCropped.Width <> bmpGrayCroppedEthalonMatrix.Width AndAlso matrixGrayCropped.Height <> bmpGrayCroppedEthalonMatrix.Height")
        End If
        For y = 0 To matrixGrayCropped.Height - 1
            For x = 0 To matrixGrayCropped.Width - 1
                Assert.That(matrixGrayCropped.GetGrayPixel(x, y), [Is].EqualTo(bmpGrayCroppedEthalonMatrix.GetGrayPixel(x, y)))
            Next
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub MatrixCropTestRgb()
        Dim bmpRgb = ResourceLoader.Res_4x3_rgb.CloneResized(New SKSize(9, 7))
        Dim testRect = SKExtensions.SKRectIFromXYWH(3, 3, 4, 3)
        Dim bmpRgbCroppedEthalon = UnsafeFunctions.Crop(bmpRgb, testRect)
        Dim bmpRgbCroppedEthalonMatrix = bmpRgbCroppedEthalon.ToRgbMatrix()
        Dim matrixRgb = bmpRgb.ToRgbMatrix()
        Dim matrixRgbCropped = MatrixTools.RGBMatrixSubRect(matrixRgb, testRect)
        If matrixRgbCropped.Width <> bmpRgbCroppedEthalonMatrix.Width AndAlso matrixRgbCropped.Height <> bmpRgbCroppedEthalonMatrix.Height Then
            Throw New Exception("matrixRgbCropped.Width <> bmpRgbCroppedEthalonMatrix.Width AndAlso matrixRgbCropped.Height <> bmpRgbCroppedEthalonMatrix.Height")
        End If
        For y = 0 To matrixRgbCropped.Height - 1
            For x = 0 To matrixRgbCropped.Width - 1
                Assert.That(matrixRgbCropped.GetRedPixel(x, y), [Is].EqualTo(bmpRgbCroppedEthalonMatrix.GetRedPixel(x, y)))
                Assert.That(matrixRgbCropped.GetGreenPixel(x, y), [Is].EqualTo(bmpRgbCroppedEthalonMatrix.GetGreenPixel(x, y)))
                Assert.That(matrixRgbCropped.GetBluePixel(x, y), [Is].EqualTo(bmpRgbCroppedEthalonMatrix.GetBluePixel(x, y)))
            Next
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub MatrixInverseTestGray()
        Dim bmpGray = ResourceLoader.Res_4x3_gray
        Dim grayMatrix1 = bmpGray.ToGrayMatrix()
        Dim grayMatrix2 = grayMatrix1.Clone()
        MatrixTools.InverseGray(grayMatrix2)
        For y = 0 To grayMatrix1.Height - 1
            For x = 0 To grayMatrix1.Width - 1
                Assert.That(grayMatrix1.GetGrayPixel(x, y), [Is].EqualTo(Byte.MaxValue - grayMatrix2.GetGrayPixel(x, y)))
            Next
        Next
        MatrixTools.InverseGray(grayMatrix2)
        For y = 0 To grayMatrix1.Height - 1
            For x = 0 To grayMatrix1.Width - 1
                Assert.That(grayMatrix1.GetGrayPixel(x, y), [Is].EqualTo(grayMatrix2.GetGrayPixel(x, y)))
            Next
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub MatrixInverseTestRgb()
        Dim bmpRgb = ResourceLoader.Res_4x3_rgb
        Dim rgbMatrix1 = bmpRgb.ToRgbMatrix()
        Dim rgbMatrix2 = rgbMatrix1.Clone()
        MatrixTools.InverseRGB(rgbMatrix2)
        For y = 0 To rgbMatrix1.Height - 1
            For x = 0 To rgbMatrix1.Width - 1
                Assert.That(rgbMatrix1.GetRedPixel(x, y), [Is].EqualTo(Byte.MaxValue - rgbMatrix2.GetRedPixel(x, y)))
                Assert.That(rgbMatrix1.GetGreenPixel(x, y), [Is].EqualTo(Byte.MaxValue - rgbMatrix2.GetGreenPixel(x, y)))
                Assert.That(rgbMatrix1.GetBluePixel(x, y), [Is].EqualTo(Byte.MaxValue - rgbMatrix2.GetBluePixel(x, y)))
            Next
        Next
        MatrixTools.InverseRGB(rgbMatrix2)
        For y = 0 To rgbMatrix1.Height - 1
            For x = 0 To rgbMatrix1.Width - 1
                Assert.That(rgbMatrix1.GetRedPixel(x, y), [Is].EqualTo(rgbMatrix2.GetRedPixel(x, y)))
                Assert.That(rgbMatrix1.GetGreenPixel(x, y), [Is].EqualTo(rgbMatrix2.GetGreenPixel(x, y)))
                Assert.That(rgbMatrix1.GetBluePixel(x, y), [Is].EqualTo(rgbMatrix2.GetBluePixel(x, y)))
            Next
        Next
    End Sub

    Private Function RgbToArgb(bmp As SKBitmap, Optional alpha As Byte = 0) As SKBitmap
        Dim bmpArgb = New SKBitmap(bmp.Width, bmp.Height, SKColorType.Bgra8888, SKAlphaType.Opaque)
        For x = 0 To bmp.Width - 1
            For y = 0 To bmp.Height - 1
                Dim px = bmp.GetPixel(x, y)
                Dim pxArgb = New SKColor(px.Red, px.Green, px.Blue, alpha)
                bmpArgb.SetPixel(x, y, pxArgb)
            Next
        Next
        Return bmpArgb
    End Function
End Class
