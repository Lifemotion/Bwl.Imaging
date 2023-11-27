''' <summary>
''' Универсальный формат сырого кадра.
''' </summary>
Public Class RawFrame
    Implements ICloneable

    Public ReadOnly Property Width As Integer
    Public ReadOnly Property Height As Integer
    Public ReadOnly Property BytesPerPixel As Integer
    Public ReadOnly Property PixelData As Byte()

    Public Sub New(width As Integer, height As Integer, bytesPerPixel As Integer, pixelData As Byte(),
                   Optional clone As Boolean = False)
        _Width = width
        _Height = height
        _BytesPerPixel = bytesPerPixel
        _PixelData = If(clone, pixelData.ToArray(), pixelData)
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

        _BytesPerPixel = bytes(0 + headerOffset)
        _Width = bytes(1 + headerOffset) Or (CInt(bytes(2 + headerOffset)) << 8)
        _Height = bytes(3 + headerOffset) Or (CInt(bytes(4 + headerOffset)) << 8)

        If Width * Height * BytesPerPixel <> pixelDataLength Then
            Throw New Exception($"{Me.GetType().Name}.Deserialize(): Width * Height * BytesPerPixel <> pixelDataLength")
        End If

        _PixelData = New Byte(pixelDataLength - 1) {}
        Array.Copy(bytes, dataOffset, _PixelData, 0, pixelDataLength)
    End Sub

    Public Function Copy() As RawFrame
        Return New RawFrame(Width, Height, BytesPerPixel, PixelData, True)
    End Function

    Public Function Clone() As Object Implements ICloneable.Clone
        Return Copy()
    End Function
End Class
