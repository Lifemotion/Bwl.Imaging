Imports System.Drawing
Imports System.IO
Imports System.Threading
Imports Bwl.Imaging.Unsafe
Imports NUnit.Framework

<TestFixture>
<Parallelizable(ParallelScope.Self)>
Public Class BitmapInfoTests
    Inherits BitmapInfoTestBase

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoAccessTest1()
        Dim src = GetTestBmp()
        Dim bi = New BitmapInfo(src)
        Dim exceptionDetected = False
        Try
            Dim bmp = bi.Bmp
        Catch ex As Exception
            exceptionDetected = True
        End Try
        Legacy.ClassicAssert.AreEqual(True, exceptionDetected)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
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
        Legacy.ClassicAssert.AreEqual(False, exceptionDetected)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
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
                Legacy.ClassicAssert.AreEqual(src2.GetPixel(i, i).R, bi.Bmp.GetPixel(i, i).R)
            Next
            bi.BmpUnlock()
        Catch ex As Exception
            exceptionDetected = True
        End Try
        Legacy.ClassicAssert.AreEqual(False, exceptionDetected)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoSetBmpTest2()
        Dim src1 = GetTestBmp()
        Dim src2 = GetTestBmp()
        Dim bi = New BitmapInfo(src1) With {.BitmapKeepTimeS = 2} 'Исходный битмап загружен...
        bi.Compress() '...теперь он сжат в JPEG, а Bitmap элиминирован - ОЖИДАЕМОЕ ЭЛИМИНИРОВАНИЕ, №1
        Dim bmpJpg = bi.GetClonedBmp() '...была декомпрессия из JPEG в Bmp (затем клонирование внутреннего _bmp) и запустился отложенный Dispose для Bitmap-а ОЖИДАЕМОЕ ЭЛИМИНИРОВАНИЕ, №2
        bi.SetBmp(src2) '...и тут мы ставим второй Bmp, в то же время НЕОЖИДАЕМОЕ ЭЛИМИНИРОВАНИЕ, №2 готовится сработать
        Thread.Sleep(3000) 'за 3 секунды должен отработать/не отработать отложенный Dispose() - ОЖИДАЕМОЕ ЭЛИМИНИРОВАНИЕ, №2
        Legacy.ClassicAssert.IsTrue(bi.CompressedCount = 1)
        Legacy.ClassicAssert.IsTrue(bi.DecompressedCount = 1)
        Legacy.ClassicAssert.IsTrue(bi.DisposeCount = 2)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
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
                        Legacy.ClassicAssert.AreEqual(src1Px.R, src2Px.R)
                        Legacy.ClassicAssert.AreEqual(src1Px.G, src2Px.G)
                        Legacy.ClassicAssert.AreEqual(src1Px.B, src2Px.B)
                    Next
                Next
            Next
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
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
                Legacy.ClassicAssert.AreEqual(False, exceptionDetected)
                Dim mtx = bi.GetRGBMatrix()
                For i = 0 To Math.Min(src.Width, src.Height) - 1
                    For j = 0 To Math.Min(src.Width, src.Height) - 1
                        Dim srcPx = src.GetPixel(i, j)
                        Dim mtxPx = mtx.GetColorPixel(i, j)
                        Legacy.ClassicAssert.AreEqual(srcPx.R, mtxPx.R)
                        Legacy.ClassicAssert.AreEqual(srcPx.G, mtxPx.G)
                        Legacy.ClassicAssert.AreEqual(srcPx.B, mtxPx.B)
                    Next
                Next
            Next
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
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
                Legacy.ClassicAssert.AreEqual(False, exceptionDetected)
                Dim mtx = bi.GetRGBMatrix()
                For i = 0 To Math.Min(src.Width, src.Height) - 1
                    For j = 0 To Math.Min(src.Width, src.Height) - 1
                        Dim srcPx = src.GetPixel(i, j)
                        Dim mtxPx = mtx.GetColorPixel(i, j)
                        Legacy.ClassicAssert.AreEqual(srcPx.R, mtxPx.R)
                        Legacy.ClassicAssert.AreEqual(srcPx.G, mtxPx.G)
                        Legacy.ClassicAssert.AreEqual(srcPx.B, mtxPx.B)
                    Next
                Next
            Next
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoGetRgbMatrixTest3()
        Dim src = GetTestBmp(Drawing.Imaging.PixelFormat.Format8bppIndexed)
        Dim bi = New BitmapInfo(src)
        Dim exceptionDetected = False
        Try
            bi.GetRGBMatrix()
        Catch ex As Exception
            exceptionDetected = True
        End Try
        Legacy.ClassicAssert.AreEqual(False, exceptionDetected)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoGetGrayMatrixTest1()
        Dim src = GetTestBmp(Drawing.Imaging.PixelFormat.Format24bppRgb)
        Dim bi = New BitmapInfo(src)
        Dim exceptionDetected = False
        Try
            bi.GetGrayMatrix()
            Legacy.ClassicAssert.AreEqual(False, exceptionDetected)
        Catch ex As Exception
            exceptionDetected = True
        End Try
        Legacy.ClassicAssert.AreEqual(False, exceptionDetected)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoGetGrayMatrixTest2()
        Dim src = GetTestBmp(Drawing.Imaging.PixelFormat.Format32bppArgb)
        Dim bi = New BitmapInfo(src)
        Dim exceptionDetected = False
        Try
            bi.GetGrayMatrix()
        Catch ex As Exception
            exceptionDetected = True
        End Try
        Legacy.ClassicAssert.AreEqual(False, exceptionDetected)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoGetGrayMatrixTest3()
        Dim src = GetTestBmp(Drawing.Imaging.PixelFormat.Format8bppIndexed)
        Dim bi = New BitmapInfo(src)
        Dim exceptionDetected = False
        Try
            bi.GetGrayMatrix()
        Catch ex As Exception
            exceptionDetected = True
        End Try
        Legacy.ClassicAssert.AreEqual(False, exceptionDetected)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoJpegTest0()
        Dim jpg = GetResourceFileData("636080817147998076.jpg")
        Dim sw = New Stopwatch()
        sw.Start()
        Dim bi = New BitmapInfo(jpg)
        sw.Stop()
        Dim jpegParseTimeMs = sw.ElapsedMilliseconds
        Legacy.ClassicAssert.IsTrue(jpegParseTimeMs < 10)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoJpegTest1()
        Dim jpg = GetResourceFileData("638067000726558653.jpg")
        Dim sw = New Stopwatch()
        Dim biJpg = New BitmapInfo(jpg)
        Dim bmp = New Bitmap(New MemoryStream(jpg))
        'Проверка изображений на соответствие
        Dim bmpBnfo = biJpg.GetClonedBmp()
        Dim mtrxBnfo = bmpBnfo.BitmapToRgbMatrix() 'JPG
        Dim mtrxBmp = bmp.BitmapToRgbMatrix() 'BMP
        Dim mtrxDiffPerc = GetAvgMatrixesDiff(mtrxBnfo, mtrxBmp) * 100
        Legacy.ClassicAssert.IsTrue(mtrxDiffPerc < 0.5)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoJpegTest2()
        Dim jpg = GetResourceFileData("jpeg8bpp.jpg")
        Dim bi = New BitmapInfo(jpg)
        Dim bmpSize = bi.BmpSize
        Dim bmpPixelFormat = bi.BmpPixelFormat
        Legacy.ClassicAssert.IsTrue(bmpSize.Width = 48)
        Legacy.ClassicAssert.IsTrue(bmpSize.Height = 96)
        Legacy.ClassicAssert.IsTrue(bmpPixelFormat = Drawing.Imaging.PixelFormat.Format8bppIndexed)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoJpegTest3()
        Dim jpg = GetResourceFileData("jpeg24bpp.jpg")
        Dim bi = New BitmapInfo(jpg)
        Dim bmpSize = bi.BmpSize
        Dim bmpPixelFormat = bi.BmpPixelFormat
        Legacy.ClassicAssert.IsTrue(bmpSize.Width = 48)
        Legacy.ClassicAssert.IsTrue(bmpSize.Height = 96)
        Legacy.ClassicAssert.IsTrue(bmpPixelFormat = Drawing.Imaging.PixelFormat.Format24bppRgb)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoJpegTest4()
        Dim src = GetTestBmp(New Size(48, 96), Drawing.Imaging.PixelFormat.Format24bppRgb)
        Dim jpg = JpegCodec.Encode(src).ToArray()
        Dim bi = New BitmapInfo(jpg)
        Dim bmpJpg = bi.GetClonedBmp()
        Dim mtrxSrc = src.BitmapToRgbMatrix()
        Dim mtrxJpeg = bmpJpg.BitmapToRgbMatrix()
        Dim mtrxDiffPerc = GetAvgMatrixesDiff(mtrxSrc, mtrxJpeg) * 100
        Legacy.ClassicAssert.IsTrue(mtrxDiffPerc < 3)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoJpegTest5()
        Dim src = GetTestBmp(New Size(48, 96), Drawing.Imaging.PixelFormat.Format24bppRgb)
        Dim jpg = JpegCodec.Encode(src).ToArray()

        Dim bi = New BitmapInfo(UnsafeFunctions.BitmapClone(src))
        Legacy.ClassicAssert.IsFalse(bi.BmpIsNothing)
        bi.SetJpg(jpg)
        Legacy.ClassicAssert.IsTrue(bi.BmpIsNothing)

        Dim biJpg = New BitmapInfo(jpg)
        Legacy.ClassicAssert.IsTrue(bi.BmpIsNothing)
        bi.SetBmp(UnsafeFunctions.BitmapClone(src))
        Legacy.ClassicAssert.IsFalse(bi.BmpIsNothing)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoJpegTest6()
        Dim src = GetTestBmp(New Size(48, 96), Drawing.Imaging.PixelFormat.Format24bppRgb)
        Dim jpg = JpegCodec.Encode(src).ToArray()
        Dim bi = New BitmapInfo(jpg) With {.BitmapKeepTimeS = 1} 'Устанавливаем JPEG
        Legacy.ClassicAssert.IsTrue(bi.BmpIsNothing) 'После установки JPEG битмап еще не развернут...

        For i = 1 To 4
            Dim bmpJpg1 = bi.GetClonedBmp() '...получаем его, активируется автоматическое элиминирование битмапа
            Legacy.ClassicAssert.IsFalse(bi.BmpIsNothing) '...но битмап пока доступен...
            Legacy.ClassicAssert.IsNotNull(bmpJpg1) '...но битмап пока доступен.
            Thread.Sleep(2000) 'Ждем достаточное время для автоэлиминирования...
            Legacy.ClassicAssert.IsTrue(bi.BmpIsNothing) '...теперь битмап должен быть пуст...
            Dim bmpJpg2 = bi.GetClonedBmp() '...но повторное обращение...
            Legacy.ClassicAssert.IsFalse(bi.BmpIsNothing) '...указывает что битмап не пуст и по флагу...
            Legacy.ClassicAssert.IsNotNull(bmpJpg2) '...и по ссылке
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoJpegTest7()
        Dim src = GetTestBmp(New Size(48, 96), Drawing.Imaging.PixelFormat.Format24bppRgb)
        Dim jpg = JpegCodec.Encode(src).ToArray()
        Dim bi = New BitmapInfo(jpg) With {.BitmapKeepTimeS = 1} 'Устанавливаем JPEG
        Legacy.ClassicAssert.IsTrue(bi.BmpIsNothing) 'После установки JPEG битмап еще не развернут...

        For i = 1 To 4
            Dim bmpJpg1 = bi.GetClonedBmp() '...получаем его, активируется автоматическое элиминирование битмапа
            Legacy.ClassicAssert.IsFalse(bi.BmpIsNothing) '...но битмап пока доступен...
            Legacy.ClassicAssert.IsNotNull(bmpJpg1) '...но битмап пока доступен.
            Thread.Sleep(2000) 'Ждем достаточное время для автоэлиминирования...
            Legacy.ClassicAssert.IsTrue(bi.BmpIsNothing) '...теперь битмап должен быть пуст...

            bi.BmpLock()
            Try
                Dim bmpJpg2 = bi.Bmp() '...но повторное обращение...
                Legacy.ClassicAssert.IsFalse(bi.BmpIsNothing) '...указывает что битмап не пуст и по флагу...
                Legacy.ClassicAssert.IsNotNull(bmpJpg2) '...и по ссылке
            Finally
                bi.BmpUnlock()
            End Try
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoJpegTest_Formats()
        For Each file In {"CMYK.jpg", "Gray.jpg", "RGB.jpg", "YCbCr.jpg", "YCbCrK.jpg"}
            Dim jpg = GetResourceFileData(file) 'Читаем файл
            Dim bi = New BitmapInfo(jpg) 'Устанавливаем JPEG
            Legacy.ClassicAssert.IsTrue(bi.BmpIsNothing) 'После установки JPEG битмап еще не развернут...
            Dim bmpJpg = bi.GetClonedBmp() '...получаем Bitmap
            Legacy.ClassicAssert.IsTrue(bmpJpg.Size = bi.BmpSize) 'Проверка
            Legacy.ClassicAssert.IsTrue(bmpJpg.PixelFormat = bi.BmpPixelFormat) 'Проверка
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoJpegTest_CamFormats()
        For Each file In {"26FWD_IZHS.jpg", "No_JFIF.jpg"}
            Dim jpg = GetResourceFileData(file) 'Читаем файл
            Dim bi = New BitmapInfo(jpg) 'Устанавливаем JPEG
            Legacy.ClassicAssert.IsTrue(bi.BmpIsNothing) 'После установки JPEG битмап еще не развернут...
            Dim bmpJpg = bi.GetClonedBmp() '...получаем Bitmap
            Legacy.ClassicAssert.IsTrue(bmpJpg.Size = bi.BmpSize) 'Проверка
            Legacy.ClassicAssert.IsTrue(bmpJpg.PixelFormat = bi.BmpPixelFormat) 'Проверка
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoCompressTest1()
        Dim src = GetTestBmp(New Size(48, 96), Drawing.Imaging.PixelFormat.Format32bppArgb)
        Dim bi = New BitmapInfo(UnsafeFunctions.BitmapClone(src)) With {.BitmapKeepTimeS = 1} 'Устанавливаем BMP
        Legacy.ClassicAssert.IsFalse(bi.BmpIsNothing) 'Bitmap уже есть...
        Legacy.ClassicAssert.IsTrue(bi.JpgIsNothing) '...а JPEG не развернут
        Legacy.ClassicAssert.IsTrue(bi.BmpPixelFormat = Drawing.Imaging.PixelFormat.Format32bppArgb) 'Исходный Bitmap имеет 32 бита на пиксель
        bi.Compress()
        Legacy.ClassicAssert.IsTrue(bi.BmpIsNothing) 'Bitmap элиминирован при установке JPEG...
        Legacy.ClassicAssert.IsFalse(bi.JpgIsNothing) '...а JPEG развернут
        Legacy.ClassicAssert.IsTrue(bi.BmpPixelFormat = Drawing.Imaging.PixelFormat.Format24bppRgb) 'При восстановлении из JPEG пиксель 24 бита
        'Проверка изображений на соответствие
        Dim bmpJpg = bi.GetClonedBmp() 'Восстановленный из JPEG битмап
        Dim mtrxSrc = src.BitmapToRgbMatrix()
        Dim mtrxJpeg = bmpJpg.BitmapToRgbMatrix()
        Dim mtrxDiffPerc = GetAvgMatrixesDiff(mtrxSrc, mtrxJpeg) * 100
        Legacy.ClassicAssert.IsTrue(mtrxDiffPerc < 3)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoCompressTest2()
        Dim src = GetTestBmp(New Size(48, 96), Drawing.Imaging.PixelFormat.Format32bppArgb)
        Dim bi = New BitmapInfo(JpegCodec.Encode(src).ToArray()) With {.BitmapKeepTimeS = 1} 'Устанавливаем BMP
        Legacy.ClassicAssert.IsTrue(bi.BmpIsNothing) 'Bitmap-а изначально нет
        Legacy.ClassicAssert.IsFalse(bi.JpgIsNothing) '...а JPEG развернут
        Legacy.ClassicAssert.IsTrue(bi.BmpPixelFormat = Drawing.Imaging.PixelFormat.Format24bppRgb) 'При сжатии JPEG пиксель 24 бита
        Dim bmpJpg = bi.GetClonedBmp() 'Заставляем декомпрессировать Bmp
        Legacy.ClassicAssert.IsFalse(bi.BmpIsNothing) 'Bitmap есть...
        Legacy.ClassicAssert.IsFalse(bi.JpgIsNothing) '...а также JPEG
        Legacy.ClassicAssert.IsTrue(bi.BmpPixelFormat = Drawing.Imaging.PixelFormat.Format24bppRgb) 'При сжатии JPEG пиксель 24 бита
        bi.Compress()
        Legacy.ClassicAssert.IsTrue(bi.BmpIsNothing) 'Bitmap элиминирован при установке JPEG...
        Legacy.ClassicAssert.IsFalse(bi.JpgIsNothing) '...а JPEG развернут
        Legacy.ClassicAssert.IsTrue(bi.BmpPixelFormat = Drawing.Imaging.PixelFormat.Format24bppRgb) 'При восстановлении из JPEG пиксель 24 бита        
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
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
        Legacy.ClassicAssert.IsTrue(mtrxDiffPerc < 0.5)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
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
        Legacy.ClassicAssert.IsTrue(mtrxDiffPerc < 0.5)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
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
        Legacy.ClassicAssert.IsTrue(mtrxDiffPerc < 0.5)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
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
        Legacy.ClassicAssert.IsTrue(mtrxDiffPerc < 0.5)
    End Sub

    ''' <summary>
    ''' В этом тесте два потока пытаются получить одновременный доступ к BitmapInfo.
    ''' Для одного из потоков BmpLock() успешен, а для второго - нет (в ходе ожидания,
    ''' по истечении таймаута) выбрасывается исключение. Обычно в блоке Finally
    ''' разблокируем bi при исключении, но разблокировать должен тот поток,
    ''' который выполнял блокировку. То есть BmpLock() нельзя вносить в TryCatch.
    ''' </summary>
    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoLockTest()
        Dim src = GetTestBmp(New Size(48, 96), Drawing.Imaging.PixelFormat.Format32bppArgb)
        Dim bi = New BitmapInfo(src) With {.BitmapKeepTimeS = 1}

        Dim lockedCount = 0

        'Это нормальный поток - он блокирует BitmapInfo, а потом разблокирует
        Task.Run(Async Function()
                     bi.BmpLock()
                     Try
                         Interlocked.Increment(lockedCount)
                         Await Task.Delay(5000).ConfigureAwait(False)
                     Catch ex As Exception
                     Finally
                         bi.BmpUnlock()
                         Interlocked.Decrement(lockedCount)
                     End Try
                 End Function)

        Thread.Sleep(1000)

        'Это поток-нарушитель, он пытается заблокировать BitmapInfo, а когда не получается - выполняет разблокировку
        'РАНЬШЕ, чем нормальный поток. Это приводит к ошибке на уровне нормального потока.
        Task.Run(Sub()
                     bi.BmpLock(1000)
                     Try
                         Interlocked.Increment(lockedCount)
                     Catch ex As Exception
                     Finally
                         bi.BmpUnlock()
                         Interlocked.Decrement(lockedCount)
                     End Try
                 End Sub)

        Thread.Sleep(10000)

        Legacy.ClassicAssert.IsTrue(Interlocked.Read(CLng(lockedCount)) = 0)
    End Sub

    Private Function GetAvgMatrixesDiff(m1 As RGBMatrix, m2 As RGBMatrix) As Double
        Dim avgDiffF As Double = 0
        If m1.Width <> m2.Width OrElse m1.Height <> m2.Height Then
            Throw New Exception("m1.Width <> m2.Width OrElse m1.Height <> m2.Height")
        End If
        For x = 0 To m1.Width - 1
            For y = 0 To m2.Height - 1
                Dim diff = Math.Abs(m1.GetRedPixel(x, y) - m2.GetRedPixel(x, y))
                avgDiffF += diff / Byte.MaxValue
            Next
        Next
        avgDiffF /= m1.Width * m1.Height
        Return avgDiffF
    End Function
End Class
