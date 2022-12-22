Imports System.Drawing
Imports System.Threading
Imports Bwl.Imaging.Unsafe
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
        bi.BmpLock()
        Try
            Dim bmp = bi.Bmp
        Catch ex As Exception
            exceptionDetected = True
        Finally
            bi.BmpUnlock()
        End Try
        Assert.AreEqual(False, exceptionDetected)
    End Sub

    <TestMethod>
    Public Sub BitmapInfoSetBmpTest1()
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
    Public Sub BitmapInfoSetBmpTest2()
        Dim src1 = GetTestBmp()
        Dim src2 = GetTestBmp()
        Dim bi = New BitmapInfo(src1) With {.BitmapKeepTimeS = 2} 'Исходный битмап загружен...
        bi.Compress() '...теперь он сжат в JPEG, а Bitmap элиминирован - ОЖИДАЕМОЕ ЭЛИМИНИРОВАНИЕ, №1
        Dim bmpJpg = bi.GetClonedBmp() '...была декомпрессия из Jpeg в Bmp и запустился отложенный Dispose для Bitmap-а НЕОЖИДАЕМОЕ ЭЛИМИНИРОВАНИЕ, №2
        bi.SetBmp(src2) '...и тут мы ставим второй Bmp, в то же время НЕОЖИДАЕМОЕ ЭЛИМИНИРОВАНИЕ, №2 готовиться сработать
        Thread.Sleep(3000) 'за 3 секунды должен отработать/не отработать отложенный Dispose() - НЕОЖИДАЕМОЕ ЭЛИМИНИРОВАНИЕ, №2, но т.к. цель сменилась - отработки нет
        Assert.IsTrue(bi.CompressedCount = 1)
        Assert.IsTrue(bi.DecompressedCount = 1)
        Assert.IsTrue(bi.BitmapEliminatedCount = 1) 'Если все прошло нормально, второй Bitmap не попадет под ненужное элиминирование
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

    <TestMethod>
    Public Sub BitmapInfoJpegTest0()
        Dim jpg = GetResourceFileData("636080817147998076.jpg")
        Dim sw = New Stopwatch()
        sw.Start()
        Dim bi = New BitmapInfo(jpg)
        sw.Stop()
        Dim jpegParseTimeMs = sw.ElapsedMilliseconds
        Assert.IsTrue(jpegParseTimeMs < 10)
    End Sub

    <TestMethod>
    Public Sub BitmapInfoJpegTest1()
        Dim jpg = GetResourceFileData("jpeg8bpp.jpg")
        Dim bi = New BitmapInfo(jpg)
        Dim bmpSize = bi.BmpSize
        Dim bmpPixelFormat = bi.BmpPixelFormat
        Assert.IsTrue(bmpSize.Width = 48)
        Assert.IsTrue(bmpSize.Height = 96)
        Assert.IsTrue(bmpPixelFormat = Drawing.Imaging.PixelFormat.Format8bppIndexed)
    End Sub

    <TestMethod>
    Public Sub BitmapInfoJpegTest2()
        Dim jpg = GetResourceFileData("jpeg24bpp.jpg")
        Dim bi = New BitmapInfo(jpg)
        Dim bmpSize = bi.BmpSize
        Dim bmpPixelFormat = bi.BmpPixelFormat
        Assert.IsTrue(bmpSize.Width = 48)
        Assert.IsTrue(bmpSize.Height = 96)
        Assert.IsTrue(bmpPixelFormat = Drawing.Imaging.PixelFormat.Format24bppRgb)
    End Sub

    <TestMethod>
    Public Sub BitmapInfoJpegTest3()
        Dim src = GetTestBmp(New Size(48, 96), Drawing.Imaging.PixelFormat.Format24bppRgb)
        Dim jpg = JpegCodec.Encode(src).ToArray()
        Dim bi = New BitmapInfo(jpg)
        Dim bmpJpg = bi.GetClonedBmp()
        Dim mtrxSrc = src.BitmapToRgbMatrix()
        Dim mtrxJpeg = bmpJpg.BitmapToRgbMatrix()
        Dim mtrxDiffPerc = GetAvgMatrixesDiff(mtrxSrc, mtrxJpeg) * 100
        Assert.IsTrue(mtrxDiffPerc < 0.5)
    End Sub

    <TestMethod>
    Public Sub BitmapInfoJpegTest4()
        Dim src = GetTestBmp(New Size(48, 96), Drawing.Imaging.PixelFormat.Format24bppRgb)
        Dim jpg = JpegCodec.Encode(src).ToArray()

        Dim bi = New BitmapInfo(UnsafeFunctions.BitmapClone(src))
        Assert.IsFalse(bi.BmpIsNothing)
        bi.SetJpg(jpg)
        Assert.IsTrue(bi.BmpIsNothing)

        Dim biJpg = New BitmapInfo(jpg)
        Assert.IsTrue(bi.BmpIsNothing)
        bi.SetBmp(UnsafeFunctions.BitmapClone(src))
        Assert.IsFalse(bi.BmpIsNothing)
    End Sub

    <TestMethod>
    Public Sub BitmapInfoJpegTest5()
        Dim src = GetTestBmp(New Size(48, 96), Drawing.Imaging.PixelFormat.Format24bppRgb)
        Dim jpg = JpegCodec.Encode(src).ToArray()
        Dim bi = New BitmapInfo(jpg) With {.BitmapKeepTimeS = 1} 'Устанавливаем JPEG
        Assert.IsTrue(bi.BmpIsNothing) 'После установки JPEG битмап еще не развернут...

        For i = 1 To 4
            Dim bmpJpg1 = bi.GetClonedBmp() '...получаем его, автоматически активируется автоматическое элиминирование битмапа
            Assert.IsFalse(bi.BmpIsNothing) '...но битмап пока доступен...
            Assert.IsNotNull(bmpJpg1) '...но битмап пока доступен.
            System.Threading.Thread.Sleep(2000) 'Ждем достаточное время для автоэлиминирования...
            Assert.IsTrue(bi.BmpIsNothing) '...теперь битмап должен быть пуст...
            Dim bmpJpg2 = bi.GetClonedBmp() '...но повторное обращение...
            Assert.IsFalse(bi.BmpIsNothing) '...указывает что битмап не пуст и по флагу...
            Assert.IsNotNull(bmpJpg2) '...и по ссылке
        Next
    End Sub

    <TestMethod>
    Public Sub BitmapInfoJpegTest6()
        Dim src = GetTestBmp(New Size(48, 96), Drawing.Imaging.PixelFormat.Format24bppRgb)
        Dim jpg = JpegCodec.Encode(src).ToArray()
        Dim bi = New BitmapInfo(jpg) With {.BitmapKeepTimeS = 1} 'Устанавливаем JPEG
        Assert.IsTrue(bi.BmpIsNothing) 'После установки JPEG битмап еще не развернут...

        For i = 1 To 4
            Dim bmpJpg1 = bi.GetClonedBmp() '...получаем его, автоматически активируется автоматическое элиминирование битмапа
            Assert.IsFalse(bi.BmpIsNothing) '...но битмап пока доступен...
            Assert.IsNotNull(bmpJpg1) '...но битмап пока доступен.
            System.Threading.Thread.Sleep(2000) 'Ждем достаточное время для автоэлиминирования...
            Assert.IsTrue(bi.BmpIsNothing) '...теперь битмап должен быть пуст...

            bi.BmpLock()
            Try
                Dim bmpJpg2 = bi.Bmp() '...но повторное обращение...
                Assert.IsFalse(bi.BmpIsNothing) '...указывает что битмап не пуст и по флагу...
                Assert.IsNotNull(bmpJpg2) '...и по ссылке
            Finally
                bi.BmpUnlock()
            End Try
        Next
    End Sub

    <TestMethod>
    Public Sub BitmapInfoJpegTest_Formats()
        For Each file In {"CMYK.jpg", "Gray.jpg", "RGB.jpg", "YCbCr.jpg", "YCbCrK.jpg"}
            Dim jpg = GetResourceFileData(file) 'Читаем файл
            Dim bi = New BitmapInfo(jpg) 'Устанавливаем JPEG
            Assert.IsTrue(bi.BmpIsNothing) 'После установки JPEG битмап еще не развернут...
            Dim bmpJpg = bi.GetClonedBmp() '...получаем Bitmap
            Assert.IsTrue(bmpJpg.Size = bi.BmpSize) 'Проверка
            Assert.IsTrue(bmpJpg.PixelFormat = bi.BmpPixelFormat) 'Проверка
        Next
    End Sub

    <TestMethod>
    Public Sub BitmapInfoJpegTest_CamFormats()
        For Each file In {"26FWD_IZHS.jpg", "No_JFIF.jpg"}
            Dim jpg = GetResourceFileData(file) 'Читаем файл
            Dim bi = New BitmapInfo(jpg) 'Устанавливаем JPEG
            Assert.IsTrue(bi.BmpIsNothing) 'После установки JPEG битмап еще не развернут...
            Dim bmpJpg = bi.GetClonedBmp() '...получаем Bitmap
            Assert.IsTrue(bmpJpg.Size = bi.BmpSize) 'Проверка
            Assert.IsTrue(bmpJpg.PixelFormat = bi.BmpPixelFormat) 'Проверка
        Next
    End Sub

    <TestMethod>
    Public Sub BitmapInfoCompressTest1()
        Dim src = GetTestBmp(New Size(48, 96), Drawing.Imaging.PixelFormat.Format32bppArgb)
        Dim bi = New BitmapInfo(UnsafeFunctions.BitmapClone(src)) With {.BitmapKeepTimeS = 1} 'Устанавливаем BMP
        Assert.IsFalse(bi.BmpIsNothing) 'Bitmap уже есть...
        Assert.IsTrue(bi.JpgIsNothing) '...а JPEG не развернут
        Assert.IsTrue(bi.BmpPixelFormat = Drawing.Imaging.PixelFormat.Format32bppArgb) 'Исходный Bitmap имеет 32 бита на пиксель
        bi.Compress()
        Assert.IsTrue(bi.BmpIsNothing) 'Bitmap элиминирован при установке JPEG...
        Assert.IsFalse(bi.JpgIsNothing) '...а JPEG развернут
        Assert.IsTrue(bi.BmpPixelFormat = Drawing.Imaging.PixelFormat.Format24bppRgb) 'При восстановлении из JPEG пиксель 24 бита
        'Проверка изображений на соответствие
        Dim bmpJpg = bi.GetClonedBmp() 'Восстановленный из JPEG битмап
        Dim mtrxSrc = src.BitmapToRgbMatrix()
        Dim mtrxJpeg = bmpJpg.BitmapToRgbMatrix()
        Dim mtrxDiffPerc = GetAvgMatrixesDiff(mtrxSrc, mtrxJpeg) * 100
        Assert.IsTrue(mtrxDiffPerc < 0.5)
    End Sub

    <TestMethod>
    Public Sub BitmapInfoCompressTest2()
        Dim src = GetTestBmp(New Size(48, 96), Drawing.Imaging.PixelFormat.Format32bppArgb)
        Dim bi = New BitmapInfo(JpegCodec.Encode(src).ToArray()) With {.BitmapKeepTimeS = 1} 'Устанавливаем BMP
        Assert.IsTrue(bi.BmpIsNothing) 'Bitmap-а изначально нет
        Assert.IsFalse(bi.JpgIsNothing) '...а JPEG развернут
        Assert.IsTrue(bi.BmpPixelFormat = Drawing.Imaging.PixelFormat.Format24bppRgb) 'При сжатии JPEG пиксель 24 бита
        Dim bmpJpg = bi.GetClonedBmp() 'Заставляем декомпрессировать Bmp
        Assert.IsFalse(bi.BmpIsNothing) 'Bitmap есть...
        Assert.IsFalse(bi.JpgIsNothing) '...а также JPEG
        Assert.IsTrue(bi.BmpPixelFormat = Drawing.Imaging.PixelFormat.Format24bppRgb) 'При сжатии JPEG пиксель 24 бита
        bi.Compress()
        Assert.IsTrue(bi.BmpIsNothing) 'Bitmap элиминирован при установке JPEG...
        Assert.IsFalse(bi.JpgIsNothing) '...а JPEG развернут
        Assert.IsTrue(bi.BmpPixelFormat = Drawing.Imaging.PixelFormat.Format24bppRgb) 'При восстановлении из JPEG пиксель 24 бита        
    End Sub

    <TestMethod>
    Public Sub BitmapInfoCloneTest11()
        Dim src = GetTestBmp(New Size(48, 96), Drawing.Imaging.PixelFormat.Format32bppArgb)
        Dim bi1 = New BitmapInfo(UnsafeFunctions.BitmapClone(src)) With {.BitmapKeepTimeS = 1} 'Устанавливаем BMP
        Dim bi2 = bi1.GetClonedCopy()
        'Проверка изображений на соответствие
        Dim bmp1 = bi1.GetClonedBmp()
        Dim bmp2 = bi2.GetClonedBmp()
        Dim mtrx1 = bmp1.BitmapToRgbMatrix()
        Dim mtrx2 = bmp2.BitmapToRgbMatrix()
        Dim mtrxDiffPerc = GetAvgMatrixesDiff(mtrx1, mtrx2) * 100
        Assert.IsTrue(mtrxDiffPerc < 0.5)
    End Sub

    <TestMethod>
    Public Sub BitmapInfoCloneTest12()
        Dim src = GetTestBmp(New Size(48, 96), Drawing.Imaging.PixelFormat.Format32bppArgb)
        Dim bi1 = New BitmapInfo(UnsafeFunctions.BitmapClone(src)) With {.BitmapKeepTimeS = 1} 'Устанавливаем BMP
        Dim bi2 As BitmapInfo = bi1.Clone()
        'Проверка изображений на соответствие
        Dim bmp1 = bi1.GetClonedBmp()
        Dim bmp2 = bi2.GetClonedBmp()
        Dim mtrx1 = bmp1.BitmapToRgbMatrix()
        Dim mtrx2 = bmp2.BitmapToRgbMatrix()
        Dim mtrxDiffPerc = GetAvgMatrixesDiff(mtrx1, mtrx2) * 100
        Assert.IsTrue(mtrxDiffPerc < 0.5)
    End Sub

    <TestMethod>
    Public Sub BitmapInfoCloneTest21()
        Dim src = GetTestBmp(New Size(48, 96), Drawing.Imaging.PixelFormat.Format32bppArgb)
        Dim bi1 = New BitmapInfo(JpegCodec.Encode(src).ToArray()) With {.BitmapKeepTimeS = 1} 'Устанавливаем BMP
        Dim bi2 = bi1.GetClonedCopy()
        'Проверка изображений на соответствие
        Dim bmp1 = bi1.GetClonedBmp()
        Dim bmp2 = bi2.GetClonedBmp()
        Dim mtrx1 = bmp1.BitmapToRgbMatrix()
        Dim mtrx2 = bmp2.BitmapToRgbMatrix()
        Dim mtrxDiffPerc = GetAvgMatrixesDiff(mtrx1, mtrx2) * 100
        Assert.IsTrue(mtrxDiffPerc < 0.5)
    End Sub

    <TestMethod>
    Public Sub BitmapInfoCloneTest22()
        Dim src = GetTestBmp(New Size(48, 96), Drawing.Imaging.PixelFormat.Format32bppArgb)
        Dim bi1 = New BitmapInfo(JpegCodec.Encode(src).ToArray()) With {.BitmapKeepTimeS = 1} 'Устанавливаем BMP
        Dim bi2 As BitmapInfo = bi1.Clone()
        'Проверка изображений на соответствие
        Dim bmp1 = bi1.GetClonedBmp()
        Dim bmp2 = bi2.GetClonedBmp()
        Dim mtrx1 = bmp1.BitmapToRgbMatrix()
        Dim mtrx2 = bmp2.BitmapToRgbMatrix()
        Dim mtrxDiffPerc = GetAvgMatrixesDiff(mtrx1, mtrx2) * 100
        Assert.IsTrue(mtrxDiffPerc < 0.5)
    End Sub

    ''' <summary>
    ''' В этом тесте два потока пытаются получить одновременный доступ к BitmapInfo.
    ''' Для одного из потоков BmpLock() успешен,
    ''' а для второго - нет (в ходе ожидания, по истечении таймаута) выбрасывается
    ''' исключение. Обычно в блоке Finally разблокируем bi при исключении, но разблокировать
    ''' должен тот поток, который выполнял блокировку. То есть BmpLock() нельзя вносить
    ''' в TryCatch.
    ''' </summary>
    <TestMethod>
    Public Sub BitmapInfoLockTest()
        Dim src = GetTestBmp(New Size(48, 96), Drawing.Imaging.PixelFormat.Format32bppArgb)
        Dim bi = New BitmapInfo(src) With {.BitmapKeepTimeS = 1}

        Dim lockedCount = 0

        'Это нормальный поток - он блокирует BitmapInfo, а потом разблокирует
        Dim thr1 = New Threading.Thread(Sub()
                                            bi.BmpLock()
                                            Try
                                                Interlocked.Increment(lockedCount)
                                                Thread.Sleep(5000)
                                            Catch ex As Exception
                                            Finally
                                                bi.BmpUnlock()
                                                Interlocked.Decrement(lockedCount)
                                            End Try
                                        End Sub) With {.IsBackground = True}
        thr1.Start()

        Thread.Sleep(1000)

        'Это поток-нарушитель, он пытается заблокировать BitmapInfo, а когда не получается - выполняет разблокировку
        'РАНЬШЕ, чем нормальный поток. Это приводит к ошибке на уровне нормального потока.
        Dim thr2 = New Threading.Thread(Sub()
                                            bi.BmpLock(1000)
                                            Try
                                                Interlocked.Increment(lockedCount)
                                            Catch ex As Exception
                                            Finally
                                                bi.BmpUnlock()
                                                Interlocked.Decrement(lockedCount)
                                            End Try
                                        End Sub) With {.IsBackground = True}
        thr2.Start()

        Thread.Sleep(10000)

        Assert.IsTrue(Interlocked.Read(lockedCount) = 0)
    End Sub

    Private Function GetAvgMatrixesDiff(m1 As RGBMatrix, m2 As RGBMatrix) As Double
        Dim avgDiffF As Double = 0
        If m1.Width <> m2.Width OrElse m1.Height <> m2.Height Then
            Throw New Exception("m1.Width <> m2.Width OrElse m1.Height <> m2.Height")
        End If
        For x = 0 To m1.Width - 1
            For y = 0 To m2.Height - 1
                Dim diff = Math.Abs(m1.RedPixel(x, y) - m2.RedPixel(x, y))
                avgDiffF += diff / Byte.MaxValue
            Next
        Next
        avgDiffF /= m1.Width * m1.Height
        Return avgDiffF
    End Function

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

    Private Function GetResourceFileData(fileName As String) As Byte()
        Dim exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
        Dim dataPath = IO.Path.Combine(exePath, "..", "..", "Resources")
        Dim data = IO.File.ReadAllBytes(IO.Path.Combine(dataPath, fileName))
        Return data
    End Function
End Class
