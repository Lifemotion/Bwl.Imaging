Imports System.IO
Imports NUnit.Framework
Imports Bwl.Imaging.Skia

<TestFixture>
<Parallelizable(ParallelScope.Self)>
Public Class RawFrameTests
    <Test>
    <Parallelizable(ParallelScope.Self)>
    Public Sub RawFrameTest()
        For channels = 1 To 3
            For width = 1 To 1024 Step 127
                For height = 1 To 1024 Step 127
                    'Формируем кадр на основе случайных данных
                    Dim pixelDataOrig = GetRandomBytes(width * height * channels)
                    Dim rawFrameOrig = New RawFrame(width, height, channels, pixelDataOrig)

                    'Тест сериализации
                    Dim rawFrameOrigBytes = rawFrameOrig.Serialize()
                    Dim rawFrameDeserial1 = New RawFrame(rawFrameOrigBytes)
                    Dim rawFrameDeserial2 = New RawFrame(New MemoryStream(rawFrameOrigBytes))
                    Assert.That(rawFrameDeserial1.Equals(rawFrameDeserial2), [Is].True)
                    Assert.That(rawFrameOrig.Equals(rawFrameDeserial1), [Is].True)
                    Assert.That(rawFrameOrig.Equals(rawFrameDeserial2), [Is].True)
                    '...в том числе по массивам сериализации
                    Dim rawFrameDeserialBytes1 = rawFrameDeserial1.Serialize()
                    Dim rawFrameDeserialBytes2 = rawFrameDeserial2.Serialize()
                    Assert.That(rawFrameDeserialBytes1.SequenceEqual(rawFrameDeserialBytes2), [Is].True)
                    Assert.That(rawFrameOrigBytes.SequenceEqual(rawFrameDeserialBytes1), [Is].True)
                    Assert.That(rawFrameOrigBytes.SequenceEqual(rawFrameDeserialBytes2), [Is].True)

                    'Тест экспорта в массив (размерность 1)
                    Dim rawFrameOrig2 = New RawFrame(rawFrameOrig.Width, rawFrameOrig.Height, rawFrameOrig.Channels, rawFrameOrig.PixelData)
                    Assert.That(rawFrameOrig.Equals(rawFrameOrig2), [Is].True)

                    'Тест экспорта в массив (размерность 3)
                    Dim rawFrameArray3 = rawFrameOrig.Export()
                    Dim rawFrameFromExport = New RawFrame(rawFrameArray3)
                    Assert.That(rawFrameOrig.Equals(rawFrameFromExport), [Is].True)

                    'Тест копии
                    Dim rawFrameOrigCopy = rawFrameOrig.Copy()
                    Assert.That(rawFrameOrig.Equals(rawFrameOrigCopy), [Is].True)
                    'Тест клонирования
                    Dim rawFrameOrigCloned = DirectCast(rawFrameOrig.Clone(), RawFrame)
                    Assert.That(rawFrameOrig.Equals(rawFrameOrigCloned), [Is].True)
                Next
            Next
        Next
    End Sub

    Private Function GetRandomBytes(size As Integer) As Byte()
        Static rnd As New Random(Now.Ticks Mod Integer.MaxValue)
        Dim result = New Byte(size - 1) {}
        rnd.NextBytes(result)
        Return result
    End Function
End Class
