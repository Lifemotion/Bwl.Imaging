''' <summary>
''' Универсальный формат сырого кадра.
''' </summary>
Public Class RawFrame
    Implements ICloneable

    Public ReadOnly Property Width As Integer
    Public ReadOnly Property Height As Integer
    Public ReadOnly Property BytesPerPixel As Integer
    Public ReadOnly Property PixelData As Byte()

    Public Sub New(width As Integer, height As Integer, bytesPerPixel As Integer, pixelDataSrc As Byte(),
                   Optional clone As Boolean = True)
        If width * height * bytesPerPixel > pixelDataSrc.Length Then
            Throw New Exception($"{Me.GetType().Name}.New(): width * height * bytesPerPixel > pixelDataSrc.Length")
        End If
        Dim pixelData As Byte() = Nothing
        If clone Then
            pixelData = New Byte(width * height * bytesPerPixel - 1) {}
            Array.Copy(pixelDataSrc, pixelData, pixelData.Length)
        Else
            pixelData = pixelDataSrc
        End If
        _Width = width
        _Height = height
        _BytesPerPixel = bytesPerPixel
        _PixelData = pixelData
    End Sub

    Public Sub New(width As Integer, height As Integer, bytesPerPixel As Integer, pixelDataSrc As Byte(,,),
                   Optional channelIdx As Integer = 0)
        If width > pixelDataSrc.GetLength(1) OrElse height > pixelDataSrc.GetLength(0) Then
            Throw New Exception($"{Me.GetType().Name}.New(): width > pixelDataSrc.GetLength(1) OrElse height > pixelDataSrc.GetLength(0)")
        End If
        Dim pixelData = New Byte(width * height * bytesPerPixel - 1) {}
        For y = 0 To height - 1
            Dim offset = y * width
            For x = 0 To width - 1
                pixelData(offset + x) = pixelDataSrc(y, x, channelIdx)
            Next
        Next
        _Width = width
        _Height = height
        _BytesPerPixel = bytesPerPixel
        _PixelData = pixelData
    End Sub

    Public Sub New(bytes As Byte(), Optional headerFirst As Boolean = False)
        Deserialize(bytes, headerFirst)
    End Sub

    Public Function Serialize(Optional headerFirst As Boolean = False) As Byte()
        Dim pixelDataLength = PixelData.Length
        Dim headerOffset = If(headerFirst, 0, pixelDataLength)
        Dim dataOffset = If(headerFirst, 5, 0)
        Dim bytes = New Byte(pixelDataLength + 5 - 1) {}
        bytes(0 + headerOffset) = CByte(BytesPerPixel And &HFF)
        bytes(1 + headerOffset) = CByte((Width >> 0) And &HFF)
        bytes(2 + headerOffset) = CByte((Width >> 8) And &HFF)
        bytes(3 + headerOffset) = CByte((Height >> 0) And &HFF)
        bytes(4 + headerOffset) = CByte((Height >> 8) And &HFF)
        Array.Copy(PixelData, 0, bytes, dataOffset, pixelDataLength)
        Return bytes
    End Function

    Public Sub Deserialize(bytes As Byte(), Optional headerFirst As Boolean = False)
        Dim pixelDataLength = bytes.Length - 5
        Dim headerOffset = If(headerFirst, 0, pixelDataLength)
        Dim dataOffset = If(headerFirst, 5, 0)
        Dim bytesPerPixel = bytes(0 + headerOffset)
        Dim width = bytes(1 + headerOffset) Or (CInt(bytes(2 + headerOffset)) << 8)
        Dim height = bytes(3 + headerOffset) Or (CInt(bytes(4 + headerOffset)) << 8)
        If width * height * bytesPerPixel <> pixelDataLength Then
            Throw New Exception($"{Me.GetType().Name}.Deserialize(): width * height * bytesPerPixel <> pixelDataLength")
        End If
        Dim pixelData = New Byte(pixelDataLength - 1) {}
        Array.Copy(bytes, dataOffset, pixelData, 0, pixelDataLength)
        _Width = width
        _Height = height
        _BytesPerPixel = bytesPerPixel
        _PixelData = pixelData
    End Sub

    Public Function Copy() As RawFrame
        Return New RawFrame(Width, Height, BytesPerPixel, PixelData, True)
    End Function

    Public Function Clone() As Object Implements ICloneable.Clone
        Return Copy()
    End Function
End Class
