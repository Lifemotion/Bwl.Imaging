Imports Bwl.Imaging

Public Class RawIntFrame
    Inherits BlobContainer

    Public Data As Integer()
    Public ReadOnly Property Width As Integer
    Public ReadOnly Property Height As Integer

    Public Sub New(width As Integer, height As Integer, data As Integer())
        _Width = width
        _Height = height
        Me.Data = data
        Attributes.Add("Width", width.ToString)
        Attributes.Add("Height", height.ToString)
        Blobs.Add(New IntegerBlob With {.ID = "Scan0", .Data = data})
    End Sub

    Public Sub New(bc As BlobContainer)
        MyBase.New(bc)
        Width = CInt(Attributes("Width"))
        Height = CInt(Attributes("Height"))
        Data = Blobs(0).Data
    End Sub

    Public Shared Function FromLegacyFile(filename As String) As RawIntFrame
        Dim fs As New IO.FileStream(filename, IO.FileMode.Open, IO.FileAccess.Read)
        Dim sw As New IO.StreamReader(fs)
        Dim width = sw.ReadLine
        Dim height = sw.ReadLine
        Dim arr(width * height * 3) As Integer
        For i = 0 To arr.Length - 1
            arr(i) = sw.ReadLine()
        Next
        fs.Close()
        Dim frame As New RawIntFrame(width, height, arr)
        frame.Data = arr
        Return frame
    End Function

    Public Overloads Shared Function FromFile(filename As String) As RawIntFrame
        Dim file As New RawIntFrame(BlobContainer.FromFile(filename))
        Return file
    End Function

    Private Function GetCodecInfo(ByVal format As ImageFormat) As ImageCodecInfo
        Dim codecs As ImageCodecInfo() = ImageCodecInfo.GetImageDecoders()
        Dim codec As ImageCodecInfo
        For Each codec In codecs
            If codec.FormatID = format.Guid Then
                Return codec
            End If
        Next codec
        Return Nothing
    End Function

    Public Sub SaveToJpegPair(filenameWithoutExthension As String, Optional quality As Integer = 95)
        Dim _encoderParameters As New EncoderParameters(1)
        Dim _codecInfo As ImageCodecInfo = GetCodecInfo(ImageFormat.Jpeg)
        _encoderParameters.Param(0) = New EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality)
        Dim mtrs = ConvertTo8BitPair(Me)
        Dim bmph = mtrs(0).ToBitmap
        Dim bmpl = mtrs(1).ToBitmap
        Dim fnameh = filenameWithoutExthension + "_h.jpeg"
        Dim fnamel = filenameWithoutExthension + "_l.jpeg"
        bmph.Save(fnameh, _codecInfo, _encoderParameters)
        bmpl.Save(fnamel, _codecInfo, _encoderParameters)
    End Sub

    Public Shared Function FromJpegPair(filenameWithoutExthension As String) As RawIntFrame
        Dim fnameh = filenameWithoutExthension + "_h.jpeg"
        Dim fnamel = filenameWithoutExthension + "_l.jpeg"
        Dim bmph = New Bitmap(fnameh)
        Dim bmpl = New Bitmap(fnamel)
        Dim mtrs = {bmph.BitmapToRgbMatrix, bmpl.BitmapToRgbMatrix}
        Dim frame = ConvertFrom8BitPair(mtrs)
        Return frame
    End Function
End Class
