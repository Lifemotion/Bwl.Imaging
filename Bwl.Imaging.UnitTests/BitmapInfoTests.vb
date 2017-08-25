Imports System.Drawing
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass>
Public Class BitmapInfoTests

    <TestMethod>
    Public Sub BitmapInfoAccessTest1()
        Dim src = GetTestBmp()
        Dim bi = New BitmapInfo(src)
        Dim exceptionDetected = False
        Try
            Dim bmp = bi.Bmp
        Catch ex As Exception
            exceptionDetected = True
        End Try
        Assert.AreEqual(True, exceptionDetected)
    End Sub

    <TestMethod>
    Public Sub BitmapInfoAccessTest2()
        Dim src = GetTestBmp()
        Dim bi = New BitmapInfo(src)
        Dim exceptionDetected = False
        Try
            bi.BmpLock()
            Dim bmp = bi.Bmp
            bi.BmpUnlock()
        Catch ex As Exception
            exceptionDetected = True
        End Try
        Assert.AreEqual(False, exceptionDetected)
    End Sub

    <TestMethod>
    Public Sub BitmapInfoSetBmpTest()
        Dim src1 = GetTestBmp()
        Dim src2 = GetTestBmp()
        For i = 0 To Math.Min(src2.Width, src2.Height) - 1
            src2.SetPixel(i, i, Color.FromArgb(255, 255, 255, 255))
        Next
        Dim bi = New BitmapInfo(src1)
        Dim exceptionDetected = False
        Try
            bi.SetBmp(src2)
            bi.BmpLock()
            For i = 0 To Math.Min(src2.Width, src2.Height) - 1
                Assert.AreEqual(src2.GetPixel(i, i).R, bi.Bmp.GetPixel(i, i).R)
            Next
            bi.BmpUnlock()
        Catch ex As Exception
            exceptionDetected = True
        End Try
        Assert.AreEqual(False, exceptionDetected)
    End Sub

    <TestMethod>
    Public Sub BitmapInfoGetClonedBmpTest()
        For w = 1 To 49
            For h = 1 To 100
                Dim src1 = GetTestBmp(New Size(w, h), Drawing.Imaging.PixelFormat.Format24bppRgb)
                Dim bi = New BitmapInfo(src1)
                Dim src2 = bi.GetClonedBmp()
                If Object.ReferenceEquals(src1, src2) Then
                    Throw New Exception("Object.ReferenceEquals(src1, src2)")
                End If
                For i = 0 To Math.Min(src1.Width, src1.Height) - 1
                    For j = 0 To Math.Min(src1.Width, src1.Height) - 1
                        Dim src1Px = src1.GetPixel(i, j)
                        Dim src2Px = src2.GetPixel(i, j)
                        Assert.AreEqual(src1Px.R, src2Px.R)
                        Assert.AreEqual(src1Px.G, src2Px.G)
                        Assert.AreEqual(src1Px.B, src2Px.B)
                    Next
                Next
            Next
        Next
    End Sub

    <TestMethod>
    Public Sub BitmapInfoGetRgbMatrixTest1()
        For w = 1 To 49
            For h = 1 To 100
                Dim src = GetTestBmp(New Size(w, h), Drawing.Imaging.PixelFormat.Format24bppRgb)
                Dim bi = New BitmapInfo(src)
                Dim exceptionDetected = False
                Try
                    bi.GetRGBMatrix()
                Catch ex As Exception
                    exceptionDetected = True
                End Try
                Assert.AreEqual(False, exceptionDetected)
                Dim mtx = bi.GetRGBMatrix()
                For i = 0 To Math.Min(src.Width, src.Height) - 1
                    For j = 0 To Math.Min(src.Width, src.Height) - 1
                        Dim srcPx = src.GetPixel(i, j)
                        Dim mtxPx = mtx.ColorPixel(i, j)
                        Assert.AreEqual(srcPx.R, mtxPx.R)
                        Assert.AreEqual(srcPx.G, mtxPx.G)
                        Assert.AreEqual(srcPx.B, mtxPx.B)
                    Next
                Next
            Next
        Next
    End Sub

    <TestMethod>
    Public Sub BitmapInfoGetRgbMatrixTest2()
        For w = 1 To 49
            For h = 1 To 100
                Dim src = GetTestBmp(New Size(w, h), Drawing.Imaging.PixelFormat.Format32bppArgb)
                Dim bi = New BitmapInfo(src)
                Dim exceptionDetected = False
                Try
                    bi.GetRGBMatrix()
                Catch ex As Exception
                    exceptionDetected = True
                End Try
                Assert.AreEqual(False, exceptionDetected)
                Dim mtx = bi.GetRGBMatrix()
                For i = 0 To Math.Min(src.Width, src.Height) - 1
                    For j = 0 To Math.Min(src.Width, src.Height) - 1
                        Dim srcPx = src.GetPixel(i, j)
                        Dim mtxPx = mtx.ColorPixel(i, j)
                        Assert.AreEqual(srcPx.R, mtxPx.R)
                        Assert.AreEqual(srcPx.G, mtxPx.G)
                        Assert.AreEqual(srcPx.B, mtxPx.B)
                    Next
                Next
            Next
        Next
    End Sub

    <TestMethod>
    Public Sub BitmapInfoGetRgbMatrixTest3()
        Dim src = GetTestBmp(Drawing.Imaging.PixelFormat.Format8bppIndexed)
        Dim bi = New BitmapInfo(src)
        Dim exceptionDetected = False
        Try
            bi.GetRGBMatrix()
        Catch ex As Exception
            exceptionDetected = True
        End Try
        Assert.AreEqual(False, exceptionDetected)
    End Sub

    <TestMethod>
    Public Sub BitmapInfoGetGrayMatrixTest1()
        Dim src = GetTestBmp(Drawing.Imaging.PixelFormat.Format24bppRgb)
        Dim bi = New BitmapInfo(src)
        Dim exceptionDetected = False
        Try
            bi.GetGrayMatrix()
            Assert.AreEqual(False, exceptionDetected)
        Catch ex As Exception
            exceptionDetected = True
        End Try
        Assert.AreEqual(False, exceptionDetected)
    End Sub

    <TestMethod>
    Public Sub BitmapInfoGetGrayMatrixTest2()
        Dim src = GetTestBmp(Drawing.Imaging.PixelFormat.Format32bppArgb)
        Dim bi = New BitmapInfo(src)
        Dim exceptionDetected = False
        Try
            bi.GetGrayMatrix()
        Catch ex As Exception
            exceptionDetected = True
        End Try
        Assert.AreEqual(False, exceptionDetected)
    End Sub

    <TestMethod>
    Public Sub BitmapInfoGetGrayMatrixTest3()
        Dim src = GetTestBmp(Drawing.Imaging.PixelFormat.Format8bppIndexed)
        Dim bi = New BitmapInfo(src)
        Dim exceptionDetected = False
        Try
            bi.GetGrayMatrix()
        Catch ex As Exception
            exceptionDetected = True
        End Try
        Assert.AreEqual(False, exceptionDetected)
    End Sub

    Private Function GetTestBmp() As Bitmap
        Return GetTestBmp(New Size(49, 100), Drawing.Imaging.PixelFormat.Format32bppArgb)
    End Function

    Private Function GetTestBmp(pixelFormat As Drawing.Imaging.PixelFormat) As Bitmap
        Return GetTestBmp(New Size(49, 100), pixelFormat)
    End Function

    Private Function GetTestBmp(size As Size, pixelFormat As Drawing.Imaging.PixelFormat) As Bitmap
        Dim bmp = New Bitmap(size.Width, size.Height, pixelFormat)
        If Math.Min(bmp.Width, bmp.Height) > 50 Then
            Throw New Exception("Math.Min(bmp.Width, bmp.Height) > 50")
        End If
        If pixelFormat = Drawing.Imaging.PixelFormat.Format24bppRgb OrElse pixelFormat = Drawing.Imaging.PixelFormat.Format32bppArgb Then
            For i = 0 To Math.Min(bmp.Width, bmp.Height) - 1
                bmp.SetPixel(i, i, Color.FromArgb(i + 200, i + 50, i + 100, i + 150))
            Next
        End If
        Return bmp
    End Function
End Class
