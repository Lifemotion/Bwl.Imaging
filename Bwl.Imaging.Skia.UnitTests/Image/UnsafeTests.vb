Imports NUnit.Framework
Imports Bwl.Imaging.Skia
Imports SkiaSharp

<TestFixture>
<Parallelizable(ParallelScope.Self)>
Public Class UnsafeTests

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub UnsafeCropGray24Test()
        UnsafeCropGrayTest(ResourceLoader.Resgray24)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub UnsafeCropGray25Test()
        UnsafeCropGrayTest(ResourceLoader.Resgray25)
    End Sub

    Private Sub UnsafeCropGrayTest(bmpGray As SKBitmap)
        Dim S = bmpGray.Height \ 2

        Dim rectRedRow1 = SKExtensions.SKRectIFromXYWH(0, 0, S, S)
        Dim rectGreenRow1 = SKExtensions.SKRectIFromXYWH(S, 0, S, S)
        Dim rectBlueRow1 = SKExtensions.SKRectIFromXYWH(2 * S, 0, S, S)
        Dim rectWhiteRow1 = SKExtensions.SKRectIFromXYWH(3 * S, 0, S, S)

        Dim rectRedRow2 = SKExtensions.SKRectIFromXYWH(3 * S, S, S, S)
        Dim rectGreenRow2 = SKExtensions.SKRectIFromXYWH(2 * S, S, S, S)
        Dim rectBlueRow2 = SKExtensions.SKRectIFromXYWH(1 * S, S, S, S)
        Dim rectWhiteRow2 = SKExtensions.SKRectIFromXYWH(0, S, S, S)

        Dim redFromRow1 = UnsafeFunctions.Crop(bmpGray, rectRedRow1)
        Dim greenFromRow1 = UnsafeFunctions.Crop(bmpGray, rectGreenRow1)
        Dim blueFromRow1 = UnsafeFunctions.Crop(bmpGray, rectBlueRow1)
        Dim whiteFromRow1 = UnsafeFunctions.Crop(bmpGray, rectWhiteRow1)

        Dim redFromRow2 = UnsafeFunctions.Crop(bmpGray, rectRedRow2)
        Dim greenFromRow2 = UnsafeFunctions.Crop(bmpGray, rectGreenRow2)
        Dim blueFromRow2 = UnsafeFunctions.Crop(bmpGray, rectBlueRow2)
        Dim whiteFromRow2 = UnsafeFunctions.Crop(bmpGray, rectWhiteRow2)

        Dim redMatrixFromRow1 = BitmapConverter.BitmapToGrayMatrix(redFromRow1)
        Dim greenMatrixFromRow1 = BitmapConverter.BitmapToGrayMatrix(greenFromRow1)
        Dim blueMatrixFromRow1 = BitmapConverter.BitmapToGrayMatrix(blueFromRow1)
        Dim whiteMatrixFromRow1 = BitmapConverter.BitmapToGrayMatrix(whiteFromRow1)

        Dim redMatrixFromRow2 = BitmapConverter.BitmapToGrayMatrix(redFromRow2)
        Dim greenMatrixFromRow2 = BitmapConverter.BitmapToGrayMatrix(greenFromRow2)
        Dim blueMatrixFromRow2 = BitmapConverter.BitmapToGrayMatrix(blueFromRow2)
        Dim whiteMatrixFromRow2 = BitmapConverter.BitmapToGrayMatrix(whiteFromRow2)

        For row = 0 To S - 1
            For col = 0 To S - 1
                Assert.That(redMatrixFromRow1.GetGrayPixel(row, col), [Is].EqualTo(77))
                Assert.That(greenMatrixFromRow1.GetGrayPixel(row, col), [Is].EqualTo(151))
                Assert.That(blueMatrixFromRow1.GetGrayPixel(row, col), [Is].EqualTo(29))
                Assert.That(whiteMatrixFromRow1.GetGrayPixel(row, col), [Is].EqualTo(255))

                Assert.That(whiteMatrixFromRow2.GetGrayPixel(row, col), [Is].EqualTo(255))
                Assert.That(blueMatrixFromRow2.GetGrayPixel(row, col), [Is].EqualTo(29))
                Assert.That(greenMatrixFromRow2.GetGrayPixel(row, col), [Is].EqualTo(151))
                Assert.That(redMatrixFromRow2.GetGrayPixel(row, col), [Is].EqualTo(77))
            Next
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub UnsafePatchGray24Test()
        UnsafePatchGrayTest(ResourceLoader.Resgray24)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub UnsafePatchGray25Test()
        UnsafePatchGrayTest(ResourceLoader.Resgray25)
    End Sub

    Private Sub UnsafePatchGrayTest(bmpGray As SKBitmap)
        Dim S = bmpGray.Height \ 2

        Dim rectRedRow1 = SKExtensions.SKRectIFromXYWH(0, 0, S, S)
        Dim rectGreenRow1 = SKExtensions.SKRectIFromXYWH(S, 0, S, S)
        Dim rectBlueRow1 = SKExtensions.SKRectIFromXYWH(2 * S, 0, S, S)
        Dim rectWhiteRow1 = SKExtensions.SKRectIFromXYWH(3 * S, 0, S, S)

        Dim rectRedRow2 = SKExtensions.SKRectIFromXYWH(3 * S, S, S, S)
        Dim rectGreenRow2 = SKExtensions.SKRectIFromXYWH(2 * S, S, S, S)
        Dim rectBlueRow2 = SKExtensions.SKRectIFromXYWH(1 * S, S, S, S)
        Dim rectWhiteRow2 = SKExtensions.SKRectIFromXYWH(0, S, S, S)

        Dim redFromRow1 = UnsafeFunctions.Crop(bmpGray, rectRedRow1)
        Dim greenFromRow1 = UnsafeFunctions.Crop(bmpGray, rectGreenRow1)
        Dim blueFromRow1 = UnsafeFunctions.Crop(bmpGray, rectBlueRow1)
        Dim whiteFromRow1 = UnsafeFunctions.Crop(bmpGray, rectWhiteRow1)

        Dim redFromRow2 = UnsafeFunctions.Crop(bmpGray, rectRedRow2)
        Dim greenFromRow2 = UnsafeFunctions.Crop(bmpGray, rectGreenRow2)
        Dim blueFromRow2 = UnsafeFunctions.Crop(bmpGray, rectBlueRow2)
        Dim whiteFromRow2 = UnsafeFunctions.Crop(bmpGray, rectWhiteRow2)

        '---

        UnsafeFunctions.Patch(whiteFromRow1, bmpGray, rectRedRow1)
        UnsafeFunctions.Patch(blueFromRow1, bmpGray, rectGreenRow1)
        UnsafeFunctions.Patch(greenFromRow1, bmpGray, rectBlueRow1)
        UnsafeFunctions.Patch(redFromRow1, bmpGray, rectWhiteRow1)

        UnsafeFunctions.Patch(redFromRow2, bmpGray, rectWhiteRow2)
        UnsafeFunctions.Patch(greenFromRow2, bmpGray, rectBlueRow2)
        UnsafeFunctions.Patch(blueFromRow2, bmpGray, rectGreenRow2)
        UnsafeFunctions.Patch(whiteFromRow2, bmpGray, rectRedRow2)

        bmpGray.RotateFlip(SKRotateFlipType.RotateNoneFlipY)
        UnsafeCropGrayTest(bmpGray)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub UnsafeCropRgb24Test()
        UnsafeCropRgbTest(ResourceLoader.Resrgbw24)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub UnsafeCropRgb25Test()
        UnsafeCropRgbTest(ResourceLoader.Resrgbw25)
    End Sub

    Private Sub UnsafeCropRgbTest(bmpRgb As SKBitmap)
        Dim S = bmpRgb.Height \ 2

        Dim rectRedRow1 = SKExtensions.SKRectIFromXYWH(0, 0, S, S)
        Dim rectGreenRow1 = SKExtensions.SKRectIFromXYWH(S, 0, S, S)
        Dim rectBlueRow1 = SKExtensions.SKRectIFromXYWH(2 * S, 0, S, S)
        Dim rectWhiteRow1 = SKExtensions.SKRectIFromXYWH(3 * S, 0, S, S)

        Dim rectRedRow2 = SKExtensions.SKRectIFromXYWH(3 * S, S, S, S)
        Dim rectGreenRow2 = SKExtensions.SKRectIFromXYWH(2 * S, S, S, S)
        Dim rectBlueRow2 = SKExtensions.SKRectIFromXYWH(1 * S, S, S, S)
        Dim rectWhiteRow2 = SKExtensions.SKRectIFromXYWH(0, S, S, S)

        Dim redFromRow1 = UnsafeFunctions.Crop(bmpRgb, rectRedRow1)
        Dim greenFromRow1 = UnsafeFunctions.Crop(bmpRgb, rectGreenRow1)
        Dim blueFromRow1 = UnsafeFunctions.Crop(bmpRgb, rectBlueRow1)
        Dim whiteFromRow1 = UnsafeFunctions.Crop(bmpRgb, rectWhiteRow1)

        Dim redFromRow2 = UnsafeFunctions.Crop(bmpRgb, rectRedRow2)
        Dim greenFromRow2 = UnsafeFunctions.Crop(bmpRgb, rectGreenRow2)
        Dim blueFromRow2 = UnsafeFunctions.Crop(bmpRgb, rectBlueRow2)
        Dim whiteFromRow2 = UnsafeFunctions.Crop(bmpRgb, rectWhiteRow2)

        Dim redMatrixFromRow1 = BitmapConverter.BitmapToRGBMatrix(redFromRow1)
        Dim greenMatrixFromRow1 = BitmapConverter.BitmapToRGBMatrix(greenFromRow1)
        Dim blueMatrixFromRow1 = BitmapConverter.BitmapToRGBMatrix(blueFromRow1)
        Dim whiteMatrixFromRow1 = BitmapConverter.BitmapToRGBMatrix(whiteFromRow1)

        Dim redMatrixFromRow2 = BitmapConverter.BitmapToRGBMatrix(redFromRow2)
        Dim greenMatrixFromRow2 = BitmapConverter.BitmapToRGBMatrix(greenFromRow2)
        Dim blueMatrixFromRow2 = BitmapConverter.BitmapToRGBMatrix(blueFromRow2)
        Dim whiteMatrixFromRow2 = BitmapConverter.BitmapToRGBMatrix(whiteFromRow2)

        For row = 0 To S - 1
            For col = 0 To S - 1
                Assert.That(redMatrixFromRow1.GetColorPixel(row, col).Red, [Is].EqualTo(CByte(255)))
                Assert.That(redMatrixFromRow1.GetColorPixel(row, col).Green, [Is].EqualTo(CByte(0)))
                Assert.That(redMatrixFromRow1.GetColorPixel(row, col).Blue, [Is].EqualTo(CByte(0)))

                Assert.That(greenMatrixFromRow1.GetColorPixel(row, col).Red, [Is].EqualTo(CByte(0)))
                Assert.That(greenMatrixFromRow1.GetColorPixel(row, col).Green, [Is].EqualTo(CByte(255)))
                Assert.That(greenMatrixFromRow1.GetColorPixel(row, col).Blue, [Is].EqualTo(CByte(0)))
                Assert.That(blueMatrixFromRow1.GetColorPixel(row, col).Red, [Is].EqualTo(CByte(0)))
                Assert.That(blueMatrixFromRow1.GetColorPixel(row, col).Green, [Is].EqualTo(CByte(0)))
                Assert.That(blueMatrixFromRow1.GetColorPixel(row, col).Blue, [Is].EqualTo(CByte(255)))

                Assert.That(whiteMatrixFromRow1.GetColorPixel(row, col).Red, [Is].EqualTo(CByte(255)))
                Assert.That(whiteMatrixFromRow1.GetColorPixel(row, col).Green, [Is].EqualTo(CByte(255)))
                Assert.That(whiteMatrixFromRow1.GetColorPixel(row, col).Blue, [Is].EqualTo(CByte(255)))
                Assert.That(whiteMatrixFromRow2.GetColorPixel(row, col).Red, [Is].EqualTo(CByte(255)))
                Assert.That(whiteMatrixFromRow2.GetColorPixel(row, col).Green, [Is].EqualTo(CByte(255)))
                Assert.That(whiteMatrixFromRow2.GetColorPixel(row, col).Blue, [Is].EqualTo(CByte(255)))
                Assert.That(redMatrixFromRow2.GetColorPixel(row, col).Red, [Is].EqualTo(CByte(255)))
                Assert.That(redMatrixFromRow2.GetColorPixel(row, col).Green, [Is].EqualTo(CByte(0)))
                Assert.That(redMatrixFromRow2.GetColorPixel(row, col).Blue, [Is].EqualTo(CByte(0)))

                Assert.That(greenMatrixFromRow2.GetColorPixel(row, col).Red, [Is].EqualTo(CByte(0)))
                Assert.That(greenMatrixFromRow2.GetColorPixel(row, col).Green, [Is].EqualTo(CByte(255)))
                Assert.That(greenMatrixFromRow2.GetColorPixel(row, col).Blue, [Is].EqualTo(CByte(0)))
                Assert.That(blueMatrixFromRow2.GetColorPixel(row, col).Red, [Is].EqualTo(CByte(0)))
                Assert.That(blueMatrixFromRow2.GetColorPixel(row, col).Green, [Is].EqualTo(CByte(0)))
                Assert.That(blueMatrixFromRow2.GetColorPixel(row, col).Blue, [Is].EqualTo(CByte(255)))
            Next
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub UnsafePatchRgb24Test()
        UnsafePatchRgbTest(ResourceLoader.Resrgbw24)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub UnsafePatchRgb25Test()
        UnsafePatchRgbTest(ResourceLoader.Resrgbw25)
    End Sub

    Private Sub UnsafePatchRgbTest(bmpRgb As SKBitmap)
        Dim S = bmpRgb.Height \ 2

        Dim rectRedRow1 = SKExtensions.SKRectIFromXYWH(0, 0, S, S)
        Dim rectGreenRow1 = SKExtensions.SKRectIFromXYWH(S, 0, S, S)
        Dim rectBlueRow1 = SKExtensions.SKRectIFromXYWH(2 * S, 0, S, S)
        Dim rectWhiteRow1 = SKExtensions.SKRectIFromXYWH(3 * S, 0, S, S)

        Dim rectRedRow2 = SKExtensions.SKRectIFromXYWH(3 * S, S, S, S)
        Dim rectGreenRow2 = SKExtensions.SKRectIFromXYWH(2 * S, S, S, S)
        Dim rectBlueRow2 = SKExtensions.SKRectIFromXYWH(1 * S, S, S, S)
        Dim rectWhiteRow2 = SKExtensions.SKRectIFromXYWH(0, S, S, S)

        Dim redFromRow1 = UnsafeFunctions.Crop(bmpRgb, rectRedRow1)
        Dim greenFromRow1 = UnsafeFunctions.Crop(bmpRgb, rectGreenRow1)
        Dim blueFromRow1 = UnsafeFunctions.Crop(bmpRgb, rectBlueRow1)
        Dim whiteFromRow1 = UnsafeFunctions.Crop(bmpRgb, rectWhiteRow1)

        Dim redFromRow2 = UnsafeFunctions.Crop(bmpRgb, rectRedRow2)
        Dim greenFromRow2 = UnsafeFunctions.Crop(bmpRgb, rectGreenRow2)
        Dim blueFromRow2 = UnsafeFunctions.Crop(bmpRgb, rectBlueRow2)
        Dim whiteFromRow2 = UnsafeFunctions.Crop(bmpRgb, rectWhiteRow2)

        '---

        UnsafeFunctions.Patch(whiteFromRow1, bmpRgb, rectRedRow1)
        UnsafeFunctions.Patch(blueFromRow1, bmpRgb, rectGreenRow1)
        UnsafeFunctions.Patch(greenFromRow1, bmpRgb, rectBlueRow1)
        UnsafeFunctions.Patch(redFromRow1, bmpRgb, rectWhiteRow1)

        UnsafeFunctions.Patch(redFromRow2, bmpRgb, rectWhiteRow2)
        UnsafeFunctions.Patch(greenFromRow2, bmpRgb, rectBlueRow2)
        UnsafeFunctions.Patch(blueFromRow2, bmpRgb, rectGreenRow2)
        UnsafeFunctions.Patch(whiteFromRow2, bmpRgb, rectRedRow2)

        bmpRgb.RotateFlip(SKRotateFlipType.RotateNoneFlipY)
        UnsafeCropRgbTest(bmpRgb)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub UnsafeNormalizeRGBTest()
        Dim bmpRgb = ResourceLoader.ResRGB
        Dim bmp2 = New SKBitmap(bmpRgb.Width, bmpRgb.Height, bmpRgb.ColorType, bmpRgb.AlphaType)
        UnsafeFunctions.Normalize(bmpRgb, bmp2, 0)
        'bmpRgb.Save("D:\1.bmp")
        'bmp2.Save("D:\2.bmp")
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub UnsafeRgbToGray24Test()
        Dim bmpRgb = ResourceLoader.Resrgbw24
        Dim matrixGray = BitmapConverter.BitmapToGrayMatrix(bmpRgb)
        Dim bmpGray2 = UnsafeFunctions.RgbToGray(bmpRgb)
        Dim matrixGray2 = BitmapConverter.BitmapToGrayMatrix(bmpGray2)
        Assert.That(matrixGray.Width, [Is].EqualTo(matrixGray2.Width))
        Assert.That(matrixGray.Height, [Is].EqualTo(matrixGray2.Height))
        For i = 0 To matrixGray.Gray.Length - 1
            Assert.That(Math.Abs(matrixGray.Gray(i) - matrixGray2.Gray(i)), [Is].LessThanOrEqualTo(1)) 'Внешний редактор может рассчитывать переход RGB -> Gray немного иначе
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub UnsafeBitmapProbeGrayS4Test()
        Dim bmpGray = ResourceLoader.ResgrayProbeS4
        Dim probe = UnsafeFunctions.SKBitmapProbeGray(bmpGray, 4)
        For Each p In probe
            Assert.That(CInt(p), [Is].EqualTo(1)) 'Графический редактор при конвертировании в grayscale черный цвет преобразовал в "1"
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub UnsafeBitmapProbeRgbS4Test()
        Dim bmpRgb = ResourceLoader.ResrgbProbeS4
        Dim probe = UnsafeFunctions.SKBitmapProbeColorRgb(bmpRgb, 4)
        For Each p In probe
            Assert.That(CInt(p), [Is].EqualTo(0)) 'А в оригинале черный цвет нормальный
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub UnsafeBitmapHashGrayS4Test()
        Dim bmpRgb = ResourceLoader.ResgrayProbeS4
        ' Convert to gray8
        Dim bmpGray = New SKBitmap(New SKImageInfo(bmpRgb.Width, bmpRgb.Height, SKColorType.Gray8, SKAlphaType.Opaque))
        bmpRgb.ScalePixels(bmpGray.PeekPixels(), SKSamplingOptions.Default)
        Dim hash = UnsafeFunctions.SKBitmapHash(bmpGray, 4)
        Assert.That(CInt(hash), [Is].EqualTo(338)) 'Графический редактор при конвертировании в grayscale черный цвет преобразовал в "1"
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub UnsafeBitmapHashRgbS4Test()
        Dim bmpRgb = ResourceLoader.ResrgbProbeS4
        Dim hash = UnsafeFunctions.SKBitmapHash(bmpRgb, 4)
        Assert.That(CInt(hash), [Is].EqualTo(0)) 'А в оригинале черный цвет нормальный
    End Sub

    <Test>
    <Parallelizable(ParallelScope.None)>
    Public Sub UnsafeBitmapHashRgbAccessViolationTestGray()
        Dim stp, w, h As Integer
        Try
            For stp = 1 To 100
                For w = 8 To 100
                    For h = 8 To 100
                        Using bmp = New SKBitmap(w, h, SKColorType.Gray8, SKAlphaType.Opaque)
                            Dim hash = UnsafeFunctions.SKBitmapHash(bmp, stp)
                        End Using
                    Next
                Next
            Next
        Catch ex As Exception
            Throw New Exception(String.Format("UnsafeBitmapHashRgbAccessViolationTestGray: step:{0}, w:{1}, h:{2}", stp, w, h))
        End Try
    End Sub

    <Test>
    <Parallelizable(ParallelScope.None)>
    Public Sub UnsafeBitmapHashRgbAccessViolationTest24bpp()
        Dim stp, w, h As Integer
        Try
            For stp = 1 To 100
                For w = 8 To 100
                    For h = 8 To 100
                        Using bmp = New SKBitmap(w, h, SKColorType.Bgra8888, SKAlphaType.Premul)
                            Dim hash = UnsafeFunctions.SKBitmapHash(bmp, stp)
                        End Using
                    Next
                Next
            Next
        Catch ex As Exception
            Throw New Exception(String.Format("UnsafeBitmapHashRgbAccessViolationTest24bpp: step:{0}, w:{1}, h:{2}", stp, w, h))
        End Try
    End Sub

    <Test>
    <Parallelizable(ParallelScope.None)>
    Public Sub UnsafeBitmapHashRgbAccessViolationTest32bpp()
        Dim stp, w, h As Integer
        Try
            For stp = 1 To 100
                For w = 8 To 100
                    For h = 8 To 100
                        Using bmp = New SKBitmap(w, h)
                            Dim hash = UnsafeFunctions.SKBitmapHash(bmp, stp)
                        End Using
                    Next
                Next
            Next
        Catch ex As Exception
            Throw New Exception(String.Format("UnsafeBitmapHashRgbAccessViolationTest32bpp: step:{0}, w:{1}, h:{2}", stp, w, h))
        End Try
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapToArrayTestGray24()
        BitmapToArrayTest(ResourceLoader.Resgray24)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapToArrayTestGray25()
        BitmapToArrayTest(ResourceLoader.Resgray25)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapToArrayTestRgb24()
        BitmapToArrayTest(ResourceLoader.Resrgbw24)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapToArrayTestRgb25()
        BitmapToArrayTest(ResourceLoader.Resrgbw25)
    End Sub

    Private Sub BitmapToArrayTest(bmp1 As SKBitmap)
        For Each headerFirst In {False, True}
            Dim array1 = UnsafeFunctions.SKBitmapToArray(bmp1, headerFirst)
            Dim bmp2 = UnsafeFunctions.ArrayToSKBitmap(array1, headerFirst)
            Dim array2 = UnsafeFunctions.SKBitmapToArray(bmp2, headerFirst)
            Dim bmp3 = UnsafeFunctions.ArrayToSKBitmap(array2, headerFirst)
            Assert.That(array1.SequenceEqual(array2), [Is].True)
            CompareBitmaps(bmp1, bmp2)
            CompareBitmaps(bmp1, bmp3)
            CompareBitmaps(bmp2, bmp3)
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapToRawFrameTesGray24()
        BitmapToRawFrameTest(ResourceLoader.Resgray24)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapToRawFrameTestRgb24()
        BitmapToRawFrameTest(ResourceLoader.Resrgbw24)
    End Sub

    Private Sub BitmapToRawFrameTest(bmp1 As SKBitmap)
        For Each headerFirst In {False, True}
            Dim array1 = UnsafeFunctions.SKBitmapToArray(bmp1, headerFirst)
            Dim rawFrame1 = New RawFrame(array1, headerFirst)
            Dim rawFrame1Bytes = rawFrame1.Serialize(headerFirst)
            Dim bmp2 = UnsafeFunctions.ArrayToSKBitmap(rawFrame1Bytes, headerFirst)
            Dim array2 = UnsafeFunctions.SKBitmapToArray(bmp2, headerFirst)
            Assert.That(array1.SequenceEqual(array2), [Is].True)
            CompareBitmaps(bmp1, bmp2)
        Next
    End Sub

    Private Sub CompareBitmaps(bmp1 As SKBitmap, bmp2 As SKBitmap)
        For x = 0 To bmp1.Width - 1
            For y = 0 To bmp1.Height - 1
                Assert.That(bmp1.GetPixel(x, y), [Is].EqualTo(bmp2.GetPixel(x, y)))
            Next
        Next
    End Sub
End Class
