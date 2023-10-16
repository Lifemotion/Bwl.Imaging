Imports System.Drawing

<TestClass()> Public Class RawFrameTests
    <TestMethod()> Public Sub RawFrameTest()
        For bytesPerPixel = 1 To 3
            For width = 1 To 1024 Step 127
                For height = 1 To 1024 Step 127
                    Dim pixelDataOrig = GetRandomBytes(width * height * bytesPerPixel)
                    Dim rawFrame1 = New RawFrame(width, height, bytesPerPixel, pixelDataOrig)
                    Dim rawFrame1Bytes = rawFrame1.Serialize()
                    Dim rawFrame2 = New RawFrame(rawFrame1Bytes)
                    Dim rawFrame2Bytes = rawFrame2.Serialize()
                    Assert.AreEqual(rawFrame1.Width, rawFrame2.Width)
                    Assert.AreEqual(rawFrame1.Height, rawFrame2.Height)
                    Assert.AreEqual(rawFrame1.BytesPerPixel, rawFrame2.BytesPerPixel)
                    CompareData(pixelDataOrig, rawFrame1.PixelData)
                    CompareData(pixelDataOrig, rawFrame2.PixelData)
                    CompareData(rawFrame1Bytes, rawFrame2Bytes)
                Next
            Next
        Next
    End Sub

    Private Sub CompareData(data1 As Byte(), data2 As Byte())
        For i = 0 To data1.Length - 1
            Assert.AreEqual(data1(i), data2(i))
        Next
    End Sub

    Private Function GetRandomBytes(size As Integer) As Byte()
        Static rnd As New Random(Now.Ticks Mod Integer.MaxValue)
        Dim result = New Byte(size - 1) {}
        rnd.NextBytes(result)
        Return result
    End Function
End Class
