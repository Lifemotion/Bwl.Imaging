Imports System.Drawing
Imports Bwl.Imaging.Unsafe

<TestClass()> Public Class UnsafeTests

    <TestMethod()> Public Sub UnsafeCropGray24()
        UnsafeCropGray(My.Resources.gray24)
    End Sub

    <TestMethod()> Public Sub UnsafeCropGray25()
        UnsafeCropGray(My.Resources.gray25)
    End Sub

    Private Sub UnsafeCropGray(bmpGray As Bitmap)
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

        Dim redFromRow2 = UnsafeFunctions.CropGray(bmpGray, rectRedRow1)
        Dim greenFromRow2 = UnsafeFunctions.CropGray(bmpGray, rectGreenRow1)
        Dim blueFromRow2 = UnsafeFunctions.CropGray(bmpGray, rectBlueRow1)
        Dim whiteFromRow2 = UnsafeFunctions.CropGray(bmpGray, rectWhiteRow1)

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
                Assert.IsTrue(Math.Abs(redMatrixFromRow1.GrayPixel(row, col) - 0.299 * 255) <= 2)
                Assert.IsTrue(Math.Abs(greenMatrixFromRow1.GrayPixel(row, col) - 0.587 * 255) <= 2)
                Assert.IsTrue(Math.Abs(blueMatrixFromRow1.GrayPixel(row, col) - 0.114 * 255) <= 2)
                Assert.IsTrue(Math.Abs(redMatrixFromRow2.GrayPixel(row, col) - 0.299 * 255) <= 2)
                Assert.IsTrue(Math.Abs(greenMatrixFromRow2.GrayPixel(row, col) - 0.587 * 255) <= 2)
                Assert.IsTrue(Math.Abs(blueMatrixFromRow2.GrayPixel(row, col) - 0.114 * 255) <= 2)
            Next
        Next
    End Sub

    <TestMethod()> Public Sub UnsafeCropRgb24()
        UnsafeCropRgb(My.Resources.rgbw24)
    End Sub

    <TestMethod()> Public Sub UnsafeCropRgb25()
        UnsafeCropRgb(My.Resources.rgbw25)
    End Sub

    Private Sub UnsafeCropRgb(bmpRgb As Bitmap)
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

        Dim redFromRow2 = UnsafeFunctions.CropRgb(bmpRgb, rectRedRow1)
        Dim greenFromRow2 = UnsafeFunctions.CropRgb(bmpRgb, rectGreenRow1)
        Dim blueFromRow2 = UnsafeFunctions.CropRgb(bmpRgb, rectBlueRow1)
        Dim whiteFromRow2 = UnsafeFunctions.CropRgb(bmpRgb, rectWhiteRow1)

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

    <TestMethod()> Public Sub UnsafeRgbToGray24()
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
End Class
