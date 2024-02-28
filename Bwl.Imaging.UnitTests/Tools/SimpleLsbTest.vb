<TestClass()> Public Class SimpleLsbTest
    <TestMethod()> Public Sub SimpleLsbTest1()
        For hostSize = 1024 To 8192
            For hostOffset = 0 To 16
                Dim host = GetRandomBytes(hostSize)
                Dim maxPayloadSize = SimpleLSB.GetMaxPayloadSize(host.Length - hostOffset)
                Dim payload = GetRandomBytes(maxPayloadSize)
                SimpleLSB.Append(host, hostOffset, payload)
                Dim payloadExt = SimpleLSB.Extract(host, hostOffset)
                Assert.IsTrue(payload.SequenceEqual(payloadExt))
            Next
        Next
    End Sub

    <TestMethod()> Public Sub SimpleLsbTest2()
        Dim MB = 1024 * 1024
        Dim hostSize = 5 * 1000 * 1000 * 3
        Dim payloadSize = SimpleLSB.GetMaxPayloadSize(hostSize)
        Dim payloadSizeShort = 8 * 1024
        Dim payloadShortCoeff = payloadSizeShort / payloadSize
        Dim host = GetRandomBytes(hostSize)
        Dim payload = GetRandomBytes(payloadSize)

        Dim sw1 = Stopwatch.StartNew()
        SimpleLSB.Append(host, 0, payload)
        sw1.Stop()
        Dim msAppendFull = sw1.Elapsed.TotalMilliseconds
        Dim msAppendShort = msAppendFull * payloadShortCoeff
        IO.File.WriteAllText($"!SimpleLSB.Append({payloadSizeShort}).ms.txt", msAppendShort.ToString("F3"))
        'IO.File.WriteAllText($"!SimpleLSB.Append({payloadSizeShort}).fps.txt", (1000.0 / msAppendShort).ToString("F3"))

        Dim sw2 = Stopwatch.StartNew()
        Dim payloadExt = SimpleLSB.Extract(host, 0)
        sw2.Stop()
        Dim msExtractFull = sw2.Elapsed.TotalMilliseconds
        Dim msExtractShort = msExtractFull * payloadShortCoeff
        IO.File.WriteAllText($"!SimpleLSB.Extract({payloadSizeShort}).ms.txt", msExtractShort.ToString("F3"))
        'IO.File.WriteAllText($"!SimpleLSB.Extract({payloadSizeShort}).fps.txt", (1000.0 / msExtractShort).ToString("F3"))

        Assert.IsTrue(payload.SequenceEqual(payloadExt))
    End Sub

    <TestMethod()> Public Sub SimpleLsbTest3()
        Dim N = 1000
        Dim MB = 1024 * 1024
        Dim hostSize = 5 * 1000 * 1000 * 3
        Dim payloadSize = 8 * 1024
        Dim host = GetRandomBytes(hostSize)
        Dim payload = GetRandomBytes(payloadSize)

        Dim sw1 = Stopwatch.StartNew()
        For i = 1 To N
            SimpleLSB.Append(host, 0, payload)
        Next
        sw1.Stop()
        Dim msAppend = sw1.Elapsed.TotalMilliseconds / N
        IO.File.WriteAllText($"!SimpleLSB.AppendN({payloadSize}).ms.txt", msAppend.ToString("F3"))
        'IO.File.WriteAllText($"!SimpleLSB.AppendN({payloadSize}).fps.txt", (1000.0 / msAppend).ToString("F3"))

        Dim sw2 = Stopwatch.StartNew()
        For i = 1 To N
            Dim payloadExt = SimpleLSB.Extract(host, 0)
        Next
        sw2.Stop()
        Dim msExtract = sw2.Elapsed.TotalMilliseconds / N
        IO.File.WriteAllText($"!SimpleLSB.ExtractN({payloadSize}).ms.txt", msExtract.ToString("F3"))
        'IO.File.WriteAllText($"!SimpleLSB.ExtractN({payloadSize}).fps.txt", (1000.0 / msExtract).ToString("F3"))
    End Sub

    Private Function GetRandomBytes(size As Integer) As Byte()
        Static rnd As New Random(Now.Ticks Mod Integer.MaxValue)
        Dim result = New Byte(size - 1) {}
        rnd.NextBytes(result)
        Return result
    End Function
End Class