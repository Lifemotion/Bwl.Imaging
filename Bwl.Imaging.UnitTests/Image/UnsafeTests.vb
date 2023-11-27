Imports System.Drawing
Imports Bwl.Imaging.Unsafe

<TestClass()> Public Class UnsafeTests

    <TestMethod()> Public Sub UnsafeCropGray24Test()
        UnsafeCropGrayTest(My.Resources.gray24)
    End Sub

    <TestMethod()> Public Sub UnsafeCropGray25Test()
        UnsafeCropGrayTest(My.Resources.gray25)
    End Sub

    Private Sub UnsafeCropGrayTest(bmpGray As Bitmap)
        Dim S = bmpGray.Height \ 2

        Dim rectRedRow1 = New Rectangle(0, 0, S, S)
        Dim rectGreenRow1 = New Rectangle(S, 0, S, S)
        Dim rectBlueRow1 = New Rectangle(2 * S, 0, S, S)
        Dim rectWhiteRow1 = New Rectangle(3 * S, 0, S, S)

        Dim rectRedRow2 = New Rectangle(3 * S, S, S, S)
        Dim rectGreenRow2 = New Rectangle(2 * S, S, S, S)
        Dim rectBlueRow2 = New Rectangle(1 * S, S, S, S)
        Dim rectWhiteRow2 = New Rectangle(0, S, S, S)

        Dim redFromRow1 = UnsafeFunctions.CropGray(bmpGray, rectRedRow1)
        Dim greenFromRow1 = UnsafeFunctions.CropGray(bmpGray, rectGreenRow1)
        Dim blueFromRow1 = UnsafeFunctions.CropGray(bmpGray, rectBlueRow1)
        Dim whiteFromRow1 = UnsafeFunctions.CropGray(bmpGray, rectWhiteRow1)

        Dim redFromRow2 = UnsafeFunctions.CropGray(bmpGray, rectRedRow2)
        Dim greenFromRow2 = UnsafeFunctions.CropGray(bmpGray, rectGreenRow2)
        Dim blueFromRow2 = UnsafeFunctions.CropGray(bmpGray, rectBlueRow2)
        Dim whiteFromRow2 = UnsafeFunctions.CropGray(bmpGray, rectWhiteRow2)

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
                Assert.AreEqual(redMatrixFromRow1.GrayPixel(row, col), 77)
                Assert.AreEqual(greenMatrixFromRow1.GrayPixel(row, col), 151)
                Assert.AreEqual(blueMatrixFromRow1.GrayPixel(row, col), 29)
                Assert.AreEqual(whiteMatrixFromRow1.GrayPixel(row, col), 255)

                Assert.AreEqual(whiteMatrixFromRow2.GrayPixel(row, col), 255)
                Assert.AreEqual(blueMatrixFromRow2.GrayPixel(row, col), 29)
                Assert.AreEqual(greenMatrixFromRow2.GrayPixel(row, col), 151)
                Assert.AreEqual(redMatrixFromRow2.GrayPixel(row, col), 77)
            Next
        Next
    End Sub

    <TestMethod()> Public Sub UnsafePatchGray24Test()
        UnsafePatchGrayTest(My.Resources.gray24)
    End Sub

    <TestMethod()> Public Sub UnsafePatchGray25Test()
        UnsafePatchGrayTest(My.Resources.gray25)
    End Sub

    Private Sub UnsafePatchGrayTest(bmpGray As Bitmap)
        Dim S = bmpGray.Height \ 2

        Dim rectRedRow1 = New Rectangle(0, 0, S, S)
        Dim rectGreenRow1 = New Rectangle(S, 0, S, S)
        Dim rectBlueRow1 = New Rectangle(2 * S, 0, S, S)
        Dim rectWhiteRow1 = New Rectangle(3 * S, 0, S, S)

        Dim rectRedRow2 = New Rectangle(3 * S, S, S, S)
        Dim rectGreenRow2 = New Rectangle(2 * S, S, S, S)
        Dim rectBlueRow2 = New Rectangle(1 * S, S, S, S)
        Dim rectWhiteRow2 = New Rectangle(0, S, S, S)

        Dim redFromRow1 = UnsafeFunctions.CropGray(bmpGray, rectRedRow1)
        Dim greenFromRow1 = UnsafeFunctions.CropGray(bmpGray, rectGreenRow1)
        Dim blueFromRow1 = UnsafeFunctions.CropGray(bmpGray, rectBlueRow1)
        Dim whiteFromRow1 = UnsafeFunctions.CropGray(bmpGray, rectWhiteRow1)

        Dim redFromRow2 = UnsafeFunctions.CropGray(bmpGray, rectRedRow2)
        Dim greenFromRow2 = UnsafeFunctions.CropGray(bmpGray, rectGreenRow2)
        Dim blueFromRow2 = UnsafeFunctions.CropGray(bmpGray, rectBlueRow2)
        Dim whiteFromRow2 = UnsafeFunctions.CropGray(bmpGray, rectWhiteRow2)

        '---

        UnsafeFunctions.PatchGray(whiteFromRow1, bmpGray, rectRedRow1)
        UnsafeFunctions.PatchGray(blueFromRow1, bmpGray, rectGreenRow1)
        UnsafeFunctions.PatchGray(greenFromRow1, bmpGray, rectBlueRow1)
        UnsafeFunctions.PatchGray(redFromRow1, bmpGray, rectWhiteRow1)

        UnsafeFunctions.PatchGray(redFromRow2, bmpGray, rectWhiteRow2)
        UnsafeFunctions.PatchGray(greenFromRow2, bmpGray, rectBlueRow2)
        UnsafeFunctions.PatchGray(blueFromRow2, bmpGray, rectGreenRow2)
        UnsafeFunctions.PatchGray(whiteFromRow2, bmpGray, rectRedRow2)

        bmpGray.RotateFlip(RotateFlipType.RotateNoneFlipY)
        UnsafeCropGrayTest(bmpGray)
    End Sub

    <TestMethod()> Public Sub UnsafeCropRgb24Test()
        UnsafeCropRgbTest(My.Resources.rgbw24)
    End Sub

    <TestMethod()> Public Sub UnsafeCropRgb25Test()
        UnsafeCropRgbTest(My.Resources.rgbw25)
    End Sub

    Private Sub UnsafeCropRgbTest(bmpRgb As Bitmap)
        Dim S = bmpRgb.Height \ 2

        Dim rectRedRow1 = New Rectangle(0, 0, S, S)
        Dim rectGreenRow1 = New Rectangle(S, 0, S, S)
        Dim rectBlueRow1 = New Rectangle(2 * S, 0, S, S)
        Dim rectWhiteRow1 = New Rectangle(3 * S, 0, S, S)

        Dim rectRedRow2 = New Rectangle(3 * S, S, S, S)
        Dim rectGreenRow2 = New Rectangle(2 * S, S, S, S)
        Dim rectBlueRow2 = New Rectangle(1 * S, S, S, S)
        Dim rectWhiteRow2 = New Rectangle(0, S, S, S)

        Dim redFromRow1 = UnsafeFunctions.CropRgb(bmpRgb, rectRedRow1)
        Dim greenFromRow1 = UnsafeFunctions.CropRgb(bmpRgb, rectGreenRow1)
        Dim blueFromRow1 = UnsafeFunctions.CropRgb(bmpRgb, rectBlueRow1)
        Dim whiteFromRow1 = UnsafeFunctions.CropRgb(bmpRgb, rectWhiteRow1)

        Dim redFromRow2 = UnsafeFunctions.CropRgb(bmpRgb, rectRedRow2)
        Dim greenFromRow2 = UnsafeFunctions.CropRgb(bmpRgb, rectGreenRow2)
        Dim blueFromRow2 = UnsafeFunctions.CropRgb(bmpRgb, rectBlueRow2)
        Dim whiteFromRow2 = UnsafeFunctions.CropRgb(bmpRgb, rectWhiteRow2)

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
                Assert.AreEqual(redMatrixFromRow1.ColorPixel(row, col).R, CByte(255))
                Assert.AreEqual(redMatrixFromRow1.ColorPixel(row, col).G, CByte(0))
                Assert.AreEqual(redMatrixFromRow1.ColorPixel(row, col).B, CByte(0))

                Assert.AreEqual(greenMatrixFromRow1.ColorPixel(row, col).R, CByte(0))
                Assert.AreEqual(greenMatrixFromRow1.ColorPixel(row, col).G, CByte(255))
                Assert.AreEqual(greenMatrixFromRow1.ColorPixel(row, col).B, CByte(0))

                Assert.AreEqual(blueMatrixFromRow1.ColorPixel(row, col).R, CByte(0))
                Assert.AreEqual(blueMatrixFromRow1.ColorPixel(row, col).G, CByte(0))
                Assert.AreEqual(blueMatrixFromRow1.ColorPixel(row, col).B, CByte(255))

                Assert.AreEqual(whiteMatrixFromRow1.ColorPixel(row, col).R, CByte(255))
                Assert.AreEqual(whiteMatrixFromRow1.ColorPixel(row, col).G, CByte(255))
                Assert.AreEqual(whiteMatrixFromRow1.ColorPixel(row, col).B, CByte(255))

                Assert.AreEqual(whiteMatrixFromRow2.ColorPixel(row, col).R, CByte(255))
                Assert.AreEqual(whiteMatrixFromRow2.ColorPixel(row, col).G, CByte(255))
                Assert.AreEqual(whiteMatrixFromRow2.ColorPixel(row, col).B, CByte(255))

                Assert.AreEqual(redMatrixFromRow2.ColorPixel(row, col).R, CByte(255))
                Assert.AreEqual(redMatrixFromRow2.ColorPixel(row, col).G, CByte(0))
                Assert.AreEqual(redMatrixFromRow2.ColorPixel(row, col).B, CByte(0))

                Assert.AreEqual(greenMatrixFromRow2.ColorPixel(row, col).R, CByte(0))
                Assert.AreEqual(greenMatrixFromRow2.ColorPixel(row, col).G, CByte(255))
                Assert.AreEqual(greenMatrixFromRow2.ColorPixel(row, col).B, CByte(0))

                Assert.AreEqual(blueMatrixFromRow2.ColorPixel(row, col).R, CByte(0))
                Assert.AreEqual(blueMatrixFromRow2.ColorPixel(row, col).G, CByte(0))
                Assert.AreEqual(blueMatrixFromRow2.ColorPixel(row, col).B, CByte(255))
            Next
        Next
    End Sub

    <TestMethod()> Public Sub UnsafePatchRgb24Test()
        UnsafePatchRgbTest(My.Resources.rgbw24)
    End Sub

    <TestMethod()> Public Sub UnsafePatchRgb25Test()
        UnsafePatchRgbTest(My.Resources.rgbw25)
    End Sub

    Private Sub UnsafePatchRgbTest(bmpRgb As Bitmap)
        Dim S = bmpRgb.Height \ 2

        Dim rectRedRow1 = New Rectangle(0, 0, S, S)
        Dim rectGreenRow1 = New Rectangle(S, 0, S, S)
        Dim rectBlueRow1 = New Rectangle(2 * S, 0, S, S)
        Dim rectWhiteRow1 = New Rectangle(3 * S, 0, S, S)

        Dim rectRedRow2 = New Rectangle(3 * S, S, S, S)
        Dim rectGreenRow2 = New Rectangle(2 * S, S, S, S)
        Dim rectBlueRow2 = New Rectangle(1 * S, S, S, S)
        Dim rectWhiteRow2 = New Rectangle(0, S, S, S)

        Dim redFromRow1 = UnsafeFunctions.CropRgb(bmpRgb, rectRedRow1)
        Dim greenFromRow1 = UnsafeFunctions.CropRgb(bmpRgb, rectGreenRow1)
        Dim blueFromRow1 = UnsafeFunctions.CropRgb(bmpRgb, rectBlueRow1)
        Dim whiteFromRow1 = UnsafeFunctions.CropRgb(bmpRgb, rectWhiteRow1)

        Dim redFromRow2 = UnsafeFunctions.CropRgb(bmpRgb, rectRedRow2)
        Dim greenFromRow2 = UnsafeFunctions.CropRgb(bmpRgb, rectGreenRow2)
        Dim blueFromRow2 = UnsafeFunctions.CropRgb(bmpRgb, rectBlueRow2)
        Dim whiteFromRow2 = UnsafeFunctions.CropRgb(bmpRgb, rectWhiteRow2)

        '---

        UnsafeFunctions.PatchRgb(whiteFromRow1, bmpRgb, rectRedRow1)
        UnsafeFunctions.PatchRgb(blueFromRow1, bmpRgb, rectGreenRow1)
        UnsafeFunctions.PatchRgb(greenFromRow1, bmpRgb, rectBlueRow1)
        UnsafeFunctions.PatchRgb(redFromRow1, bmpRgb, rectWhiteRow1)

        UnsafeFunctions.PatchRgb(redFromRow2, bmpRgb, rectWhiteRow2)
        UnsafeFunctions.PatchRgb(greenFromRow2, bmpRgb, rectBlueRow2)
        UnsafeFunctions.PatchRgb(blueFromRow2, bmpRgb, rectGreenRow2)
        UnsafeFunctions.PatchRgb(whiteFromRow2, bmpRgb, rectRedRow2)

        bmpRgb.RotateFlip(RotateFlipType.RotateNoneFlipY)
        UnsafeCropRgbTest(bmpRgb)
    End Sub

    <TestMethod()>
    Public Sub UnsafeNormalizeRGBTest()
        Dim bmpRgb = My.Resources.rgb
        Dim bmp2 = New Bitmap(bmpRgb.Width, bmpRgb.Height, bmpRgb.PixelFormat)
        UnsafeFunctions.NormalizeRgb(bmpRgb, bmp2, 0)
        'bmpRgb.Save("D:\1.bmp")
        'bmp2.Save("D:\2.bmp")
    End Sub

    <TestMethod()> Public Sub UnsafeRgbToGray24Test()
        Dim bmpRgb = My.Resources.rgbw24
        Dim matrixGray = BitmapConverter.BitmapToGrayMatrix(bmpRgb)
        Dim bmpGray2 = UnsafeFunctions.RgbToGray(bmpRgb)
        Dim matrixGray2 = BitmapConverter.BitmapToGrayMatrix(bmpGray2)
        Assert.AreEqual(matrixGray.Width, matrixGray2.Width)
        Assert.AreEqual(matrixGray.Height, matrixGray2.Height)
        For i = 0 To matrixGray.Gray.Length - 1
            Assert.IsTrue(Math.Abs(matrixGray.Gray(i) - matrixGray2.Gray(i)) <= 1) 'Внешний редактор может рассчитывать переход RGB -> Gray немного иначе
        Next
    End Sub

    <TestMethod()> Public Sub UnsafeBitmapProbeGrayS4Test()
        Dim bmpGray = My.Resources.grayProbeS4
        Dim probe = UnsafeFunctions.BitmapProbeGray(bmpGray, 4)
        For Each p In probe
            Assert.AreEqual(CInt(p), 1) 'Графический редактор при конвертировании в grayscale черный цвет преобразовал в "1"
        Next
    End Sub

    <TestMethod()> Public Sub UnsafeBitmapProbeRgbS4Test()
        Dim bmpRgb = My.Resources.rgbProbeS4
        Dim probe = UnsafeFunctions.BitmapProbeRgb(bmpRgb, 4)
        For Each p In probe
            Assert.AreEqual(CInt(p), 0) 'А в оригинале черный цвет нормальный
        Next
    End Sub

    <TestMethod()> Public Sub UnsafeBitmapHashGrayS4Test()
        Dim bmpGray = My.Resources.grayProbeS4
        Dim hash = UnsafeFunctions.BitmapHashGray(bmpGray, 4)
        Assert.AreEqual(CInt(hash), 338) 'Графический редактор при конвертировании в grayscale черный цвет преобразовал в "1"
    End Sub

    <TestMethod()> Public Sub UnsafeBitmapHashRgbS4Test()
        Dim bmpRgb = My.Resources.rgbProbeS4
        Dim hash = UnsafeFunctions.BitmapHashRgb(bmpRgb, 4)
        Assert.AreEqual(CInt(hash), 0) 'А в оригинале черный цвет нормальный
    End Sub

    <TestMethod()> Public Sub UnsafeBitmapHashRgbAccessViolationTestGray()
        Dim stp, w, h As Integer
        Try
            For stp = 1 To 100
                For w = 8 To 100
                    For h = 8 To 100
                        Dim bmp = New Bitmap(w, h, Drawing.Imaging.PixelFormat.Format8bppIndexed)
                        Dim hash = UnsafeFunctions.BitmapHashGray(bmp, stp)
                        bmp.Dispose()
                    Next
                Next
            Next
        Catch ex As Exception
            Throw New Exception(String.Format("UnsafeBitmapHashRgbAccessViolationTestGray: step:{0}, w:{1}, h:{2}", stp, w, h))
        End Try
    End Sub

    <TestMethod()> Public Sub UnsafeBitmapHashRgbAccessViolationTest24bpp()
        Dim stp, w, h As Integer
        Try
            For stp = 1 To 100
                For w = 8 To 100
                    For h = 8 To 100
                        Dim bmp = New Bitmap(w, h, Drawing.Imaging.PixelFormat.Format24bppRgb)
                        Dim hash = UnsafeFunctions.BitmapHashRgb(bmp, stp)
                        bmp.Dispose()
                    Next
                Next
            Next
        Catch ex As Exception
            Throw New Exception(String.Format("UnsafeBitmapHashRgbAccessViolationTest24bpp: step:{0}, w:{1}, h:{2}", stp, w, h))
        End Try
    End Sub

    <TestMethod()> Public Sub UnsafeBitmapHashRgbAccessViolationTest32bpp()
        Dim stp, w, h As Integer
        Try
            For stp = 1 To 100
                For w = 8 To 100
                    For h = 8 To 100
                        Dim bmp = New Bitmap(w, h)
                        Dim hash = UnsafeFunctions.BitmapHashRgb(bmp, stp)
                        bmp.Dispose()
                    Next
                Next
            Next
        Catch ex As Exception
            Throw New Exception(String.Format("UnsafeBitmapHashRgbAccessViolationTest32bpp: step:{0}, w:{1}, h:{2}", stp, w, h))
        End Try
    End Sub

    <TestMethod()> Public Sub BitmapToArrayTestGray24()
        BitmapToArrayTest(My.Resources.gray24)
    End Sub

    <TestMethod()> Public Sub BitmapToArrayTestGray25()
        BitmapToArrayTest(My.Resources.gray25)
    End Sub

    <TestMethod()> Public Sub BitmapToArrayTestRgb24()
        BitmapToArrayTest(My.Resources.rgbw24)
    End Sub

    <TestMethod()> Public Sub BitmapToArrayTestRgb25()
        BitmapToArrayTest(My.Resources.rgbw25)
    End Sub

    Private Sub BitmapToArrayTest(bmp1 As Bitmap)
        For Each headerFirst In {False, True}
            Dim array1 = UnsafeFunctions.BitmapToArray(bmp1, headerFirst)
            Dim bmp2 = UnsafeFunctions.ArrayToBitmap(array1, headerFirst)
            Dim array2 = UnsafeFunctions.BitmapToArray(bmp2, headerFirst)
            Dim bmp3 = UnsafeFunctions.ArrayToBitmap(array2, headerFirst)
            Assert.IsTrue(array1.SequenceEqual(array2))
            CompareBitmaps(bmp1, bmp2)
            CompareBitmaps(bmp1, bmp3)
            CompareBitmaps(bmp2, bmp3)
        Next
    End Sub

    <TestMethod()> Public Sub BitmapToRawFrameTesGray24()
        BitmapToRawFrameTest(My.Resources.gray24)
    End Sub

    <TestMethod()> Public Sub BitmapToRawFrameTestRgb24()
        BitmapToRawFrameTest(My.Resources.rgbw24)
    End Sub

    Private Sub BitmapToRawFrameTest(bmp1 As Bitmap)
        For Each headerFirst In {False, True}
            Dim array1 = UnsafeFunctions.BitmapToArray(bmp1, headerFirst)
            Dim rawFrame1 = New RawFrame(array1, headerFirst)
            Dim rawFrame1Bytes = rawFrame1.Serialize(headerFirst)
            Dim bmp2 = UnsafeFunctions.ArrayToBitmap(rawFrame1Bytes, headerFirst)
            Dim array2 = UnsafeFunctions.BitmapToArray(bmp2, headerFirst)
            Assert.IsTrue(array1.SequenceEqual(array2))
            CompareBitmaps(bmp1, bmp2)
        Next
    End Sub

    Private Sub CompareBitmaps(bmp1 As Bitmap, bmp2 As Bitmap)
        For x = 0 To bmp1.Width - 1
            For y = 0 To bmp1.Height - 1
                If bmp1.GetPixel(x, y) <> bmp2.GetPixel(x, y) Then
                    Throw New Exception("bmp1.GetPixel(x, y) <> bmp2.GetPixel(x, y)")
                End If
            Next
        Next
    End Sub
End Class
