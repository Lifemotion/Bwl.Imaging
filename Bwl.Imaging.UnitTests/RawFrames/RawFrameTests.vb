Imports System.IO
Imports NUnit.Framework

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
                    Legacy.ClassicAssert.IsTrue(rawFrameDeserial1.Equals(rawFrameDeserial2))
                    Legacy.ClassicAssert.IsTrue(rawFrameOrig.Equals(rawFrameDeserial1))
                    Legacy.ClassicAssert.IsTrue(rawFrameOrig.Equals(rawFrameDeserial2))
                    '...в том числе по массивам сериализации
                    Dim rawFrameDeserialBytes1 = rawFrameDeserial1.Serialize()
                    Dim rawFrameDeserialBytes2 = rawFrameDeserial2.Serialize()
                    Legacy.ClassicAssert.IsTrue(rawFrameDeserialBytes1.SequenceEqual(rawFrameDeserialBytes2))
                    Legacy.ClassicAssert.IsTrue(rawFrameOrigBytes.SequenceEqual(rawFrameDeserialBytes1))
                    Legacy.ClassicAssert.IsTrue(rawFrameOrigBytes.SequenceEqual(rawFrameDeserialBytes2))

                    'Тест экспорта в массив (размерность 1)
                    Dim rawFrameOrig2 = New RawFrame(rawFrameOrig.Width, rawFrameOrig.Height, rawFrameOrig.Channels, rawFrameOrig.PixelData)
                    Legacy.ClassicAssert.IsTrue(rawFrameOrig.Equals(rawFrameOrig))

                    'Тест экспорта в массив (размерность 3)
                    Dim rawFrameArray3 = rawFrameOrig.Export()
                    Dim rawFrameFromExport = New RawFrame(rawFrameArray3)
                    Legacy.ClassicAssert.IsTrue(rawFrameOrig.Equals(rawFrameFromExport))

                    'Тест копии
                    Dim rawFrameOrigCopy = rawFrameOrig.Copy()
                    Legacy.ClassicAssert.IsTrue(rawFrameOrig.Equals(rawFrameOrigCopy))

                    'Тест клонирования
                    Dim rawFrameOrigCloned = DirectCast(rawFrameOrig.Clone(), RawFrame)
                    Legacy.ClassicAssert.IsTrue(rawFrameOrig.Equals(rawFrameOrigCloned))
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
