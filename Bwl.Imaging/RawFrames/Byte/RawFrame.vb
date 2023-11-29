''' <summary>
''' Универсальный формат сырого кадра.
''' </summary>
Public Class RawFrame
    Implements ICloneable

    Public ReadOnly Property Width As Integer
    Public ReadOnly Property Height As Integer
    Public ReadOnly Property Channels As Integer
    Public ReadOnly Property PixelData As Byte()

    Public Sub New()
    End Sub

    Public Sub New(serialized As Byte(), Optional headerFirst As Boolean = False)
        Deserialize(serialized, headerFirst)
    End Sub

    Public Sub New(width As Integer, height As Integer, channels As Integer, pixelData As Byte(),
                   Optional clone As Boolean = True)
        Import(width, height, channels, pixelData, clone)
    End Sub

    Public Sub New(pixelData As Byte(,,))
        Import(pixelData)
    End Sub

    Public Overrides Function Equals(obj As Object) As Boolean
        If (obj Is Nothing) OrElse Not Me.GetType().Equals(obj.GetType()) Then
            Return False
        Else
            Dim rawFrame = DirectCast(obj, RawFrame)
            Return rawFrame.Width = Me.Width AndAlso
                   rawFrame.Height = Me.Height AndAlso
                   rawFrame.Channels = Me.Channels AndAlso
                   rawFrame.PixelData.SequenceEqual(Me.PixelData)
        End If
    End Function

    Public Sub Import(width As Integer, height As Integer, channels As Integer, pixelDataSrc As Byte(),
                      Optional clone As Boolean = True)
        If width * height * channels > pixelDataSrc.Length Then
            Throw New Exception($"{Me.GetType().Name}.Import(): width * height * channels > pixelDataSrc.Length")
        End If
        Dim pixelData As Byte() = Nothing
        If clone Then
            pixelData = New Byte(width * height * channels - 1) {}
            Array.Copy(pixelDataSrc, pixelData, pixelData.Length)
        Else
            pixelData = pixelDataSrc
        End If
        _Width = width
        _Height = height
        _Channels = channels
        _PixelData = pixelData
    End Sub

    Public Sub Import(pixelDataSrc As Byte(,,))
        Dim width = pixelDataSrc.GetLength(1)
        Dim height = pixelDataSrc.GetLength(0)
        Dim channels = pixelDataSrc.GetLength(2)
        Dim pixelData = New Byte(width * height * channels - 1) {}
        If channels = 1 Then
            ImportChannel(pixelDataSrc, pixelData, 0)
        Else
            Parallel.For(0, channels, Sub(channel As Integer)
                                          ImportChannel(pixelDataSrc, pixelData, channel)
                                      End Sub)
        End If
        _Width = width
        _Height = height
        _Channels = channels
        _PixelData = pixelData
    End Sub

    Public Function Export() As Byte(,,)
        Dim pixelDataTgt = New Byte(Height - 1, Width - 1, Channels - 1) {}
        If Channels = 1 Then
            ExportChannel(PixelData, pixelDataTgt, 0)
        Else
            Parallel.For(0, Channels, Sub(channel As Integer)
                                          ExportChannel(PixelData, pixelDataTgt, channel)
                                      End Sub)
        End If
        Return pixelDataTgt
    End Function

    Public Function Serialize(Optional headerFirst As Boolean = False) As Byte()
        Dim pixelDataLength = PixelData.Length
        Dim headerOffset = If(headerFirst, 0, pixelDataLength)
        Dim dataOffset = If(headerFirst, 5, 0)
        Dim serialized = New Byte(pixelDataLength + 5 - 1) {}
        serialized(0 + headerOffset) = CByte(Channels And &HFF)
        serialized(1 + headerOffset) = CByte((Width >> 0) And &HFF)
        serialized(2 + headerOffset) = CByte((Width >> 8) And &HFF)
        serialized(3 + headerOffset) = CByte((Height >> 0) And &HFF)
        serialized(4 + headerOffset) = CByte((Height >> 8) And &HFF)
        Array.Copy(PixelData, 0, serialized, dataOffset, pixelDataLength)
        Return serialized
    End Function

    Public Sub Deserialize(serialized As Byte(), Optional headerFirst As Boolean = False)
        Dim pixelDataLength = serialized.Length - 5
        Dim headerOffset = If(headerFirst, 0, pixelDataLength)
        Dim dataOffset = If(headerFirst, 5, 0)
        Dim channels = serialized(0 + headerOffset)
        Dim width = serialized(1 + headerOffset) Or (CInt(serialized(2 + headerOffset)) << 8)
        Dim height = serialized(3 + headerOffset) Or (CInt(serialized(4 + headerOffset)) << 8)
        If width * height * channels <> pixelDataLength Then
            Throw New Exception($"{Me.GetType().Name}.Deserialize(): width * height * channels <> pixelDataLength")
        End If
        Dim pixelData = New Byte(pixelDataLength - 1) {}
        Array.Copy(serialized, dataOffset, pixelData, 0, pixelDataLength)
        _Width = width
        _Height = height
        _Channels = channels
        _PixelData = pixelData
    End Sub

    Public Function Copy() As RawFrame
        Return New RawFrame(Width, Height, Channels, PixelData, True)
    End Function

    Public Function Clone() As Object Implements ICloneable.Clone
        Return Copy()
    End Function

    Private Sub ImportChannel(src As Byte(,,), tgt As Byte(), channel As Integer)
        Dim width = src.GetLength(1)
        Dim height = src.GetLength(0)
        Dim channels = src.GetLength(2)
        Dim offset = channel
        For y = 0 To height - 1
            For x = 0 To width - 1
                tgt(offset) = src(y, x, channel)
                offset += channels
            Next
        Next
    End Sub

    Private Sub ExportChannel(src As Byte(), tgt As Byte(,,), channel As Integer)
        Dim width = tgt.GetLength(1)
        Dim height = tgt.GetLength(0)
        Dim channels = tgt.GetLength(2)
        Dim offset = channel
        For y = 0 To height - 1
            For x = 0 To width - 1
                tgt(y, x, channel) = src(offset)
                offset += channels
            Next
        Next
    End Sub
End Class
