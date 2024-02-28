Public Module SimpleLSB
    Private ReadOnly _bitMask As Byte() = {1, 2, 4, 8, 16, 32, 64, 128}

    Public Function GetMaxPayloadSize(hostSize As Integer) As Integer
        Return (hostSize \ 8) - 32
    End Function

    Public Sub Append(host As Byte(), hostOffset As Integer, payload As Byte())
        CheckMaxPayloadSize(host.Length - hostOffset, payload.Length)
        Dim payloadSizeBytes = BitConverter.GetBytes(CType(payload.Length, Int32))
        SetBits(host, hostOffset, payloadSizeBytes)
        SetBits(host, hostOffset + payloadSizeBytes.Length * 8, payload)
    End Sub

    Public Function Extract(host As Byte(), hostOffset As Integer) As Byte()
        Dim payloadSizeBytes = GetBits(host, hostOffset, 4)
        Dim payloadSize = BitConverter.ToInt32(payloadSizeBytes, 0)
        CheckMaxPayloadSize(host.Length - hostOffset, payloadSize)
        Return GetBits(host, hostOffset + payloadSizeBytes.Length * 8, payloadSize)
    End Function

    Private Function GetBits(host As Byte(), hostOffset As Integer, payloadCount As Integer) As Byte()
        Dim lsbMask As Byte = &H1
        Dim payload = New Byte(payloadCount - 1) {}
        Dim payloadIdx = 0
        Dim offset = hostOffset
        For i = 0 To payloadCount - 1
            Dim bt = 0
            bt = bt Or ((host(offset + 0) And lsbMask) << 0)
            bt = bt Or ((host(offset + 1) And lsbMask) << 1)
            bt = bt Or ((host(offset + 2) And lsbMask) << 2)
            bt = bt Or ((host(offset + 3) And lsbMask) << 3)
            bt = bt Or ((host(offset + 4) And lsbMask) << 4)
            bt = bt Or ((host(offset + 5) And lsbMask) << 5)
            bt = bt Or ((host(offset + 6) And lsbMask) << 6)
            bt = bt Or ((host(offset + 7) And lsbMask) << 7)
            offset += 8
            payload(payloadIdx) = CByte(bt) : payloadIdx += 1
        Next
        Return payload
    End Function

    Private Sub SetBits(host As Byte(), hostOffset As Integer, payload As Byte())
        Dim topBitsMask As Byte = &HFE
        Dim offset = hostOffset
        For i = 0 To payload.Length - 1
            Dim bt = payload(i)
            host(offset + 0) = (host(offset + 0) And topBitsMask) Or ((bt And _bitMask(0)) >> 0)
            host(offset + 1) = (host(offset + 1) And topBitsMask) Or ((bt And _bitMask(1)) >> 1)
            host(offset + 2) = (host(offset + 2) And topBitsMask) Or ((bt And _bitMask(2)) >> 2)
            host(offset + 3) = (host(offset + 3) And topBitsMask) Or ((bt And _bitMask(3)) >> 3)
            host(offset + 4) = (host(offset + 4) And topBitsMask) Or ((bt And _bitMask(4)) >> 4)
            host(offset + 5) = (host(offset + 5) And topBitsMask) Or ((bt And _bitMask(5)) >> 5)
            host(offset + 6) = (host(offset + 6) And topBitsMask) Or ((bt And _bitMask(6)) >> 6)
            host(offset + 7) = (host(offset + 7) And topBitsMask) Or ((bt And _bitMask(7)) >> 7)
            offset += 8
        Next
    End Sub

    Private Sub CheckMaxPayloadSize(hostSize As Integer, payloadSize As Integer)
        Dim maxPayloadSize = GetMaxPayloadSize(hostSize)
        If payloadSize > maxPayloadSize Then
            Throw New Exception($"SimpleLSB.CheckMaxPayloadSize(): payloadSize({payloadSize}) > maxPayloadSize({maxPayloadSize})")
        End If
    End Sub
End Module
