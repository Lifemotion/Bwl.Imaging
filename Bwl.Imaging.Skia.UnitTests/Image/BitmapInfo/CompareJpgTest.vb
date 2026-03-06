Imports NUnit.Framework
Imports Bwl.Imaging.Skia
Imports SkiaSharp

<TestFixture>
<Parallelizable(ParallelScope.Self)>
Public Class CompareJpgTest
    Inherits BitmapInfoTestBase

    ' TODO: probably should be removed, stream comparison byte by byte doesn't make much sense in .NET 5+ and gives no advantage over SequenceEqual anymore. 
    ' Also it randomly might complete or not complete in time, which makes it unreliable for testing.
    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub CompareJpgTest()
        Dim multX = 1 'Множитель базового размера...
        Dim imgSize = New SKSizeI(1920 * multX, 1080 * multX) '...1920 * 1080

        'Два идентичных Bitmap-а
        Dim bmp1 = GetTestBmp(imgSize, SKColorType.Bgra8888, SKAlphaType.Premul)
        Dim bmp2 = GetTestBmp(imgSize, SKColorType.Bgra8888, SKAlphaType.Premul)
        'Bitmap с отличием от исходного...
        Dim bmp3 = GetTestBmp(imgSize, SKColorType.Bgra8888, SKAlphaType.Premul)
        bmp3.SetPixel(0, 0, New SKColor(255, 255, 255)) '...на один пиксель

        'Формирование тестируемых структур данных
        Dim bi1 = New SKBitmapInfo(bmp1) : bi1.Compress()
        Dim bi2 = New SKBitmapInfo(bmp2) : bi2.Compress()
        Dim bi3 = New SKBitmapInfo(bmp3) : bi3.Compress()

        'Тест
        Dim compareTest = Sub(b1 As SKBitmapInfo, b2 As SKBitmapInfo, result As Boolean)
                              Dim sw = New Stopwatch()
                              'Оптимизированное сравнение потоков jpg
                              sw.Restart()
                              Dim cmp1 = b1.CompareJpgFast(b2)
                              sw.Stop()
                              Dim cmp1_time = sw.Elapsed.TotalMilliseconds

                              'Сравнение одинаковых потоков jpg "стандартными средствами" (успешное сравнение)
                              sw.Restart()
                              Dim cmp2 = b1.GetJpgFast().SequenceEqual(b2.GetJpgFast())
                              sw.Stop()
                              Dim cmp2_time = sw.Elapsed.TotalMilliseconds

                              'Анализ результатов сравнения
                              Assert.That(cmp1, [Is].EqualTo(cmp2))

                              'Анализ производительности сравнения
                              Assert.That(cmp1_time, [Is].LessThan(cmp2_time))

                              'Сравнение полученных потоков Jpg "в-лоб"
                              Dim jp1 = b1.GetJpgFast()
                              Dim jp2 = b2.GetJpgFast()
                              If result Then
                                  Assert.That(jp1.Length, [Is].EqualTo(jp2.Length))
                                  Assert.That(jp1.SequenceEqual(jp2), [Is].True)
                              Else
                                  Assert.That(jp1.Length, [Is].Not.EqualTo(jp2.Length))
                                  Assert.That(jp1.SequenceEqual(jp2), [Is].False)
                              End If
                          End Sub
        compareTest(bi1, bi2, True) 'JPEG-структуры должны быть равны
        compareTest(bi1, bi3, False) 'JPEG-структуры должны отличаться
    End Sub
End Class
