Imports System.Drawing
Imports NUnit.Framework

<TestFixture>
<Parallelizable(ParallelScope.Self)>
Public Class CompareJpgTest
    Inherits BitmapInfoTestBase

    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub CompareJpgTest()
        Dim multX = 1 'Множитель базового размера...
        Dim imgSize = New Size(1920 * multX, 1080 * multX) '...1920 * 1080

        'Два идентичных Bitmap-а
        Dim bmp1 = GetTestBmp(imgSize, Drawing.Imaging.PixelFormat.Format24bppRgb)
        Dim bmp2 = GetTestBmp(imgSize, Drawing.Imaging.PixelFormat.Format24bppRgb)
        'Bitmap с отличием от исходного...
        Dim bmp3 = GetTestBmp(imgSize, Drawing.Imaging.PixelFormat.Format24bppRgb)
        bmp3.SetPixel(0, 0, Color.FromArgb(255, 255, 255)) '...на один пиксель

        'Формирование тестируемых структур данных
        Dim bi1 = New BitmapInfo(bmp1) : bi1.Compress()
        Dim bi2 = New BitmapInfo(bmp2) : bi2.Compress()
        Dim bi3 = New BitmapInfo(bmp3) : bi3.Compress()

        'Тест
        Dim compareTest = Sub(b1 As BitmapInfo, b2 As BitmapInfo, result As Boolean)
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
                              Legacy.ClassicAssert.IsTrue(cmp1 = cmp2)

                              'Анализ производительности сравнения
                              Legacy.ClassicAssert.IsTrue(cmp1_time < cmp2_time / 2)

                              'Сравнение полученных потоков Jpg "в-лоб"
                              Dim jp1 = b1.GetJpgFast()
                              Dim jp2 = b2.GetJpgFast()
                              If result Then
                                  Legacy.ClassicAssert.IsTrue(jp1.Length = jp2.Length)
                                  Legacy.ClassicAssert.IsTrue(jp1.SequenceEqual(jp2))
                              Else
                                  Legacy.ClassicAssert.IsFalse(jp1.Length = jp2.Length)
                                  Legacy.ClassicAssert.IsFalse(jp1.SequenceEqual(jp2))
                              End If
                          End Sub
        compareTest(bi1, bi2, True) 'JPEG-структуры должны быть равны
        compareTest(bi1, bi3, False) 'JPEG-структуры должны отличаться
    End Sub
End Class
