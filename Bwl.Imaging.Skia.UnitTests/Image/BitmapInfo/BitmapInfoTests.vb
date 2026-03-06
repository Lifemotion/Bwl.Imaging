Imports System.IO
Imports System.Threading
Imports NUnit.Framework
Imports Bwl.Imaging.Skia
Imports SkiaSharp

<TestFixture>
<Parallelizable(ParallelScope.Self)>
Public Class BitmapInfoTests
    Inherits BitmapInfoTestBase

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoAccessTest1()
        Dim src = GetTestBmp()
        Dim bi = New SKBitmapInfo(src)
        Dim exceptionDetected = False
        Try
            Dim bmp = bi.Bmp
        Catch ex As Exception
            exceptionDetected = True
        End Try
        Assert.That(exceptionDetected, [Is].True)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoAccessTest2()
        Dim src = GetTestBmp()
        Dim bi = New SKBitmapInfo(src)
        Dim exceptionDetected = False
        bi.BmpLock()
        Try
            Dim bmp = bi.Bmp
        Catch ex As Exception
            exceptionDetected = True
        Finally
            bi.BmpUnlock()
        End Try
        Assert.That(exceptionDetected, [Is].False)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoSetBmpTest1()
        Dim src1 = GetTestBmp()
        Dim src2 = GetTestBmp()
        For i = 0 To Math.Min(src2.Width, src2.Height) - 1
            src2.SetPixel(i, i, New SKColor(255, 255, 255, 255))
        Next
        Dim bi = New SKBitmapInfo(src1)
        Dim exceptionDetected = False
        Try
            bi.SetBmp(src2)
            bi.BmpLock()
            For i = 0 To Math.Min(src2.Width, src2.Height) - 1
                Assert.That(src2.GetPixel(i, i).Red, [Is].EqualTo(bi.Bmp.GetPixel(i, i).Red))
            Next
            bi.BmpUnlock()
        Catch ex As Exception
            exceptionDetected = True
        End Try
        Assert.That(exceptionDetected, [Is].False)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoSetBmpTest2()
        Dim src1 = GetTestBmp()
        Dim src2 = GetTestBmp()
        Dim bi = New SKBitmapInfo(src1) With {.SKBitmapKeepTimeS = 2} 'Исходный битмап загружен...
        bi.Compress() '...теперь он сжат в JPEG, а Bitmap элиминирован - ОЖИДАЕМОЕ ЭЛИМИНИРОВАНИЕ, №1
        Dim bmpJpg = bi.GetClonedBmp() '...была декомпрессия из JPEG в Bmp (затем клонирование внутреннего _bmp) и запустился отложенный Dispose для Bitmap-а ОЖИДАЕМОЕ ЭЛИМИНИРОВАНИЕ, №2
        bi.SetBmp(src2) '...и тут мы ставим второй Bmp, в то же время НЕОЖИДАЕМОЕ ЭЛИМИНИРОВАНИЕ, №2 готовится сработать
        Thread.Sleep(3000) 'за 3 секунды должен отработать/не отработать отложенный Dispose() - ОЖИДАЕМОЕ ЭЛИМИНИРОВАНИЕ, №2
        Assert.That(bi.CompressedCount, [Is].EqualTo(1))
        Assert.That(bi.DecompressedCount, [Is].EqualTo(1))
        Assert.That(bi.DisposeCount, [Is].EqualTo(2))
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoGetClonedBmpTest()
        For w = 1 To 49
            For h = 1 To 100
                Dim src1 = GetTestBmp(New SKSizeI(w, h), SKColorType.Bgra8888, SKAlphaType.Premul)
                Dim bi = New SKBitmapInfo(src1)
                Dim src2 = bi.GetClonedBmp()
                If Object.ReferenceEquals(src1, src2) Then
                    Throw New Exception("Object.ReferenceEquals(src1, src2)")
                End If
                For i = 0 To Math.Min(src1.Width, src1.Height) - 1
                    For j = 0 To Math.Min(src1.Width, src1.Height) - 1
                        Dim src1Px = src1.GetPixel(i, j)
                        Dim src2Px = src2.GetPixel(i, j)
                        Assert.That(src1Px.Red, [Is].EqualTo(src2Px.Red))
                        Assert.That(src1Px.Green, [Is].EqualTo(src2Px.Green))
                        Assert.That(src1Px.Blue, [Is].EqualTo(src2Px.Blue))
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
                Dim src = GetTestBmp(New SKSizeI(w, h), SKColorType.Bgra8888, SKAlphaType.Premul)
                Dim bi = New SKBitmapInfo(src)
                Dim exceptionDetected = False
                Try
                    bi.GetRGBMatrix()
                Catch ex As Exception
                    exceptionDetected = True
                End Try
                Assert.That(exceptionDetected, [Is].False)
                Dim mtx = bi.GetRGBMatrix()
                For i = 0 To Math.Min(src.Width, src.Height) - 1
                    For j = 0 To Math.Min(src.Width, src.Height) - 1
                        Dim srcPx = src.GetPixel(i, j)
                        Dim mtxPx = mtx.GetColorPixel(i, j)
                        Assert.That(srcPx.Red, [Is].EqualTo(mtxPx.Red))
                        Assert.That(srcPx.Green, [Is].EqualTo(mtxPx.Green))
                        Assert.That(srcPx.Blue, [Is].EqualTo(mtxPx.Blue))
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
                Dim src = GetTestBmp(New SKSizeI(w, h), SKColorType.Bgra8888, SKAlphaType.Premul)
                Dim bi = New SKBitmapInfo(src)
                Dim exceptionDetected = False
                Try
                    bi.GetRGBMatrix()
                Catch ex As Exception
                    exceptionDetected = True
                End Try
                Assert.That(exceptionDetected, [Is].False)
                Dim mtx = bi.GetRGBMatrix()
                For i = 0 To Math.Min(src.Width, src.Height) - 1
                    For j = 0 To Math.Min(src.Width, src.Height) - 1
                        Dim srcPx = src.GetPixel(i, j)
                        Dim mtxPx = mtx.GetColorPixel(i, j)
                        Assert.That(srcPx.Red, [Is].EqualTo(mtxPx.Red))
                        Assert.That(srcPx.Green, [Is].EqualTo(mtxPx.Green))
                        Assert.That(srcPx.Blue, [Is].EqualTo(mtxPx.Blue))
                    Next
                Next
            Next
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoGetRgbMatrixTest3()
        Dim src = GetTestBmp(SKColorType.Gray8, SKAlphaType.Opaque)
        Dim bi = New SKBitmapInfo(src)
        Dim exceptionDetected = False
        Try
            bi.GetRGBMatrix()
        Catch ex As Exception
            exceptionDetected = True
        End Try
        Assert.That(exceptionDetected, [Is].False)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoGetGrayMatrixTest1()
        Dim src = GetTestBmp(SKColorType.Bgra8888, SKAlphaType.Premul)
        Dim bi = New SKBitmapInfo(src)
        Dim exceptionDetected = False
        Try
            bi.GetGrayMatrix()
            Assert.That(exceptionDetected, [Is].EqualTo(False))
        Catch ex As Exception
            exceptionDetected = True
        End Try
        Assert.That(exceptionDetected, [Is].False)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoGetGrayMatrixTest2()
        Dim src = GetTestBmp(SKColorType.Bgra8888, SKAlphaType.Premul)
        Dim bi = New SKBitmapInfo(src)
        Dim exceptionDetected = False
        Try
            bi.GetGrayMatrix()
        Catch ex As Exception
            exceptionDetected = True
        End Try
        Assert.That(exceptionDetected, [Is].False)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoGetGrayMatrixTest3()
        Dim src = GetTestBmp(SKColorType.Gray8, SKAlphaType.Opaque)
        Dim bi = New SKBitmapInfo(src)
        Dim exceptionDetected = False
        Try
            bi.GetGrayMatrix()
        Catch ex As Exception
            exceptionDetected = True
        End Try
        Assert.That(exceptionDetected, [Is].False)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoJpegTest0()
        Dim jpg = ResourceLoader.LoadResourceData("636080817147998076.jpg")
        Dim sw = New Stopwatch()
        sw.Start()
        Dim bi = New SKBitmapInfo(jpg)
        sw.Stop()
        Dim jpegParseTimeMs = sw.ElapsedMilliseconds
        Assert.That(jpegParseTimeMs, [Is].LessThan(10))
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoJpegTest1()
        Dim jpg = ResourceLoader.LoadResourceData("638067000726558653.jpg")
        Dim sw = New Stopwatch()
        Dim biJpg = New SKBitmapInfo(jpg)
        Dim bmp = New SKBitmap()
        Using stream = New MemoryStream(jpg)
            bmp = SKBitmap.Decode(stream)
        End Using
        'Проверка изображений на соответствие
        Dim bmpBnfo = biJpg.GetClonedBmp()
        Dim mtrxBnfo = bmpBnfo.ToRgbMatrix() 'JPG
        Dim mtrxBmp = bmp.ToRgbMatrix() 'BMP
        Dim mtrxDiffPerc = GetAvgMatrixesDiff(mtrxBnfo, mtrxBmp) * 100
        Assert.That(mtrxDiffPerc, [Is].LessThan(0.5))
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoJpegTest2()
        Dim jpg = ResourceLoader.LoadResourceData("jpeg8bpp.jpg")
        Dim bi = New SKBitmapInfo(jpg)
        Dim bmpSize = bi.BmpSize
        Dim bmpPixelFormat = bi.ColorType
        Assert.That(bmpSize.Width, [Is].EqualTo(48))
        Assert.That(bmpSize.Height, [Is].EqualTo(96))
        Assert.That(bmpPixelFormat, [Is].EqualTo(SKColorType.Gray8))
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoJpegTest3()
        Dim jpg = ResourceLoader.LoadResourceData("jpeg24bpp.jpg")
        Dim bi = New SKBitmapInfo(jpg)
        Dim bmpSize = bi.BmpSize
        Dim bmpPixelFormat = bi.ColorType
        Assert.That(bmpSize.Width, [Is].EqualTo(48))
        Assert.That(bmpSize.Height, [Is].EqualTo(96))
        Assert.That(bmpPixelFormat, [Is].EqualTo(SKColorType.Bgra8888))
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoJpegTest4()
        Dim src = GetTestBmp(New SKSizeI(48, 96), SKColorType.Bgra8888, SKAlphaType.Premul)
        Dim jpg = src.Encode(SKEncodedImageFormat.Jpeg, 95).ToArray()
        Dim bi = New SKBitmapInfo(jpg)
        Dim bmpJpg = bi.GetClonedBmp()
        Dim mtrxSrc = src.ToRgbMatrix()
        Dim mtrxJpeg = bmpJpg.ToRgbMatrix()
        Dim mtrxDiffPerc = GetAvgMatrixesDiff(mtrxSrc, mtrxJpeg) * 100
        Assert.That(mtrxDiffPerc, [Is].LessThan(3))
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoJpegTest5()
        Dim src = GetTestBmp(New SKSizeI(48, 96), SKColorType.Bgra8888, SKAlphaType.Premul)
        Dim jpg = src.Encode(SKEncodedImageFormat.Jpeg, 95).ToArray()

        Dim bi = New SKBitmapInfo(UnsafeFunctions.SKBitmapClone(src))
        Assert.That(bi.BmpIsNothing, [Is].False)
        bi.SetJpg(jpg)
        Assert.That(bi.BmpIsNothing, [Is].True)

        Dim biJpg = New SKBitmapInfo(jpg)
        Assert.That(bi.BmpIsNothing, [Is].True)
        bi.SetBmp(UnsafeFunctions.SKBitmapClone(src))
        Assert.That(bi.BmpIsNothing, [Is].False)
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoJpegTest6()
        Dim src = GetTestBmp(New SKSizeI(48, 96), SKColorType.Bgra8888, SKAlphaType.Premul)
        Dim jpg = src.Encode(SKEncodedImageFormat.Jpeg, 95).ToArray()
        Dim bi = New SKBitmapInfo(jpg) With {.SKBitmapKeepTimeS = 1} 'Устанавливаем JPEG
        Assert.That(bi.BmpIsNothing, [Is].True) 'После установки JPEG битмап еще не развернут...

        For i = 1 To 4
            Dim bmpJpg1 = bi.GetClonedBmp() '...получаем его, активируется автоматическое элиминирование битмапа
            Assert.That(bi.BmpIsNothing, [Is].False) '...но битмап пока доступен...
            Assert.That(bmpJpg1, [Is].Not.Null) '...но битмап пока доступен.
            Thread.Sleep(2000) 'Ждем достаточное время для автоэлиминирования...
            Assert.That(bi.BmpIsNothing, [Is].True) '...теперь битмап должен быть пуст...
            Dim bmpJpg2 = bi.GetClonedBmp() '...но повторное обращение...
            Assert.That(bi.BmpIsNothing, [Is].False) '...указывает что битмап не пуст и по флагу...
            Assert.That(bmpJpg2, [Is].Not.Null) '...и по ссылке
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoJpegTest7()
        Dim src = GetTestBmp(New SKSizeI(48, 96), SKColorType.Bgra8888, SKAlphaType.Premul)
        Dim jpg = src.Encode(SKEncodedImageFormat.Jpeg, 95).ToArray()
        Dim bi = New SKBitmapInfo(jpg) With {.SKBitmapKeepTimeS = 1} 'Устанавливаем JPEG
        Assert.That(bi.BmpIsNothing, [Is].True) 'После установки JPEG битмап еще не развернут...

        For i = 1 To 4
            Dim bmpJpg1 = bi.GetClonedBmp() '...получаем его, активируется автоматическое элиминирование битмапа
            Assert.That(bi.BmpIsNothing, [Is].False) '...но битмап пока доступен...
            Assert.That(bmpJpg1, [Is].Not.Null) '...но битмап пока доступен.
            Thread.Sleep(2000) 'Ждем достаточное время для автоэлиминирования...
            Assert.That(bi.BmpIsNothing, [Is].True) '...теперь битмап должен быть пуст...

            bi.BmpLock()
            Try
                Dim bmpJpg2 = bi.Bmp() '...но повторное обращение...
                Assert.That(bi.BmpIsNothing, [Is].False) '...указывает что битмап не пуст и по флагу...
                Assert.That(bmpJpg2, [Is].Not.Null) '...и по ссылке
            Finally
                bi.BmpUnlock()
            End Try
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoJpegTest_Formats()
        For Each file In {"CMYK.jpg", "Gray.jpg", "RGB.jpg", "YCbCr.jpg", "YCbCrK.jpg"}
            Dim jpg = ResourceLoader.LoadResourceData(file) 'Читаем файл
            Dim bi = New SKBitmapInfo(jpg) 'Устанавливаем JPEG
            Assert.That(bi.BmpIsNothing, [Is].True) 'После установки JPEG битмап еще не развернут...
            Dim bmpJpg = bi.GetClonedBmp() '...получаем Bitmap
            Assert.That(bmpJpg.Size(), [Is].EqualTo(bi.BmpSize)) 'Проверка
            Assert.That(bmpJpg.ColorType, [Is].EqualTo(bi.ColorType)) 'Проверка
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoJpegTest_CamFormats()
        For Each file In {"26FWD_IZHS.jpg", "No_JFIF.jpg"}
            Dim jpg = ResourceLoader.LoadResourceData(file) 'Читаем файл
            Dim bi = New SKBitmapInfo(jpg) 'Устанавливаем JPEG
            Assert.That(bi.BmpIsNothing, [Is].True) 'После установки JPEG битмап еще не развернут...
            Dim bmpJpg = bi.GetClonedBmp() '...получаем Bitmap
            Assert.That(bmpJpg.Size(), [Is].EqualTo(bi.BmpSize)) 'Проверка
            Assert.That(bmpJpg.ColorType, [Is].EqualTo(bi.ColorType)) 'Проверка
        Next
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoCompressTest1()
        Dim src = GetTestBmp(New SKSizeI(48, 96), SKColorType.Bgra8888, SKAlphaType.Premul)
        Dim bi = New SKBitmapInfo(UnsafeFunctions.SKBitmapClone(src)) With {.SKBitmapKeepTimeS = 1} 'Устанавливаем BMP
        Assert.That(bi.BmpIsNothing, [Is].False) 'Bitmap уже есть...
        Assert.That(bi.JpgIsNothing, [Is].True) '...а JPEG не развернут
        Assert.That(bi.ColorType, [Is].EqualTo(SKColorType.Bgra8888)) 'Исходный Bitmap имеет 32 бита на пиксель
        bi.Compress()
        Assert.That(bi.BmpIsNothing, [Is].True) 'Bitmap элиминирован при установке JPEG...
        Assert.That(bi.JpgIsNothing, [Is].False) '...а JPEG развернут
        Assert.That(bi.ColorType, [Is].EqualTo(SKColorType.Bgra8888)) 'При восстановлении из JPEG пиксель 24 бита
        'Проверка изображений на соответствие
        Dim bmpJpg = bi.GetClonedBmp() 'Восстановленный из JPEG битмап
        Dim mtrxSrc = src.ToRgbMatrix()
        Dim mtrxJpeg = bmpJpg.ToRgbMatrix()
        Dim mtrxDiffPerc = GetAvgMatrixesDiff(mtrxSrc, mtrxJpeg) * 100
        Assert.That(mtrxDiffPerc, [Is].LessThan(3))
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoCompressTest2()
        Dim src = GetTestBmp(New SKSizeI(48, 96), SKColorType.Bgra8888, SKAlphaType.Premul)
        Dim bi = New SKBitmapInfo(src.Encode(SKEncodedImageFormat.Jpeg, 95).ToArray()) With {.SKBitmapKeepTimeS = 1} 'Устанавливаем BMP
        Assert.That(bi.BmpIsNothing, [Is].True) 'Bitmap-а изначально нет
        Assert.That(bi.JpgIsNothing, [Is].False) '...а JPEG развернут
        Assert.That(bi.ColorType, [Is].EqualTo(SKColorType.Bgra8888)) 'При сжатии JPEG пиксель 24 бита
        Dim bmpJpg = bi.GetClonedBmp() 'Заставляем декомпрессировать Bmp
        Assert.That(bi.BmpIsNothing, [Is].False) 'Bitmap есть...
        Assert.That(bi.JpgIsNothing, [Is].False) '...а также JPEG
        Assert.That(bi.ColorType, [Is].EqualTo(SKColorType.Bgra8888)) 'При сжатии JPEG пиксель 24 бита
        bi.Compress()
        Assert.That(bi.BmpIsNothing, [Is].True) 'Bitmap элиминирован при установке JPEG...
        Assert.That(bi.JpgIsNothing, [Is].False) '...а JPEG развернут
        Assert.That(bi.ColorType, [Is].EqualTo(SKColorType.Bgra8888)) 'При восстановлении из JPEG пиксель 24 бита        
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoCloneTest11()
        Dim src = GetTestBmp(New SKSizeI(48, 96), SKColorType.Bgra8888, SKAlphaType.Premul)
        Dim bi1 = New SKBitmapInfo(UnsafeFunctions.SKBitmapClone(src)) With {.SKBitmapKeepTimeS = 1} 'Устанавливаем BMP
        Dim bi2 = bi1.GetClonedCopy()
        'Проверка изображений на соответствие
        Dim bmp1 = bi1.GetClonedBmp()
        Dim bmp2 = bi2.GetClonedBmp()
        Dim mtrx1 = bmp1.ToRgbMatrix()
        Dim mtrx2 = bmp2.ToRgbMatrix()
        Dim mtrxDiffPerc = GetAvgMatrixesDiff(mtrx1, mtrx2) * 100
        Assert.That(mtrxDiffPerc, [Is].LessThan(0.5))
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoCloneTest12()
        Dim src = GetTestBmp(New SKSizeI(48, 96), SKColorType.Bgra8888, SKAlphaType.Premul)
        Dim bi1 = New SKBitmapInfo(UnsafeFunctions.SKBitmapClone(src)) With {.SKBitmapKeepTimeS = 1} 'Устанавливаем BMP
        Dim bi2 As SKBitmapInfo = bi1.Clone()
        'Проверка изображений на соответствие
        Dim bmp1 = bi1.GetClonedBmp()
        Dim bmp2 = bi2.GetClonedBmp()
        Dim mtrx1 = bmp1.ToRgbMatrix()
        Dim mtrx2 = bmp2.ToRgbMatrix()
        Dim mtrxDiffPerc = GetAvgMatrixesDiff(mtrx1, mtrx2) * 100
        Assert.That(mtrxDiffPerc, [Is].LessThan(0.5))
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoCloneTest21()
        Dim src = GetTestBmp(New SKSizeI(48, 96), SKColorType.Bgra8888, SKAlphaType.Premul)
        Dim bi1 = New SKBitmapInfo(src.Encode(SKEncodedImageFormat.Jpeg, 95).ToArray()) With {.SKBitmapKeepTimeS = 1} 'Устанавливаем BMP
        Dim bi2 = bi1.GetClonedCopy()
        'Проверка изображений на соответствие
        Dim bmp1 = bi1.GetClonedBmp()
        Dim bmp2 = bi2.GetClonedBmp()
        Dim mtrx1 = bmp1.ToRgbMatrix()
        Dim mtrx2 = bmp2.ToRgbMatrix()
        Dim mtrxDiffPerc = GetAvgMatrixesDiff(mtrx1, mtrx2) * 100
        Assert.That(mtrxDiffPerc, [Is].LessThan(0.5))
    End Sub

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub BitmapInfoCloneTest22()
        Dim src = GetTestBmp(New SKSizeI(48, 96), SKColorType.Bgra8888, SKAlphaType.Premul)
        Dim bi1 = New SKBitmapInfo(src.Encode(SKEncodedImageFormat.Jpeg, 95).ToArray()) With {.SKBitmapKeepTimeS = 1} 'Устанавливаем BMP
        Dim bi2 As SKBitmapInfo = bi1.Clone()
        'Проверка изображений на соответствие
        Dim bmp1 = bi1.GetClonedBmp()
        Dim bmp2 = bi2.GetClonedBmp()
        Dim mtrx1 = bmp1.ToRgbMatrix()
        Dim mtrx2 = bmp2.ToRgbMatrix()
        Dim mtrxDiffPerc = GetAvgMatrixesDiff(mtrx1, mtrx2) * 100
        Assert.That(mtrxDiffPerc, [Is].LessThan(0.5))
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
        Dim src = GetTestBmp(New SKSizeI(48, 96), SKColorType.Bgra8888, SKAlphaType.Premul)
        Dim bi = New SKBitmapInfo(src) With {.SKBitmapKeepTimeS = 1}

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

        Assert.That(Interlocked.Read(CLng(lockedCount)), [Is].EqualTo(0))
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
