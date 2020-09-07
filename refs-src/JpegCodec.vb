Imports System.IO
Imports System.Drawing
Imports System.Drawing.Imaging

Public Module JpegCodec
    Public Function Encode(bmp As Bitmap, Optional frameQuality As Integer = 50) As MemoryStream
        If bmp IsNot Nothing Then
            SyncLock bmp
                Dim jpegStream As New MemoryStream()
                Dim jpegEncoderParameters As New EncoderParameters(1)
                Dim qualityEncoderParameter As New EncoderParameter(Encoder.Quality, frameQuality)
                Dim jpegCodecInfo As ImageCodecInfo = GetCodecInfo(ImageFormat.Jpeg)
                jpegEncoderParameters.Param(0) = qualityEncoderParameter
                bmp.Save(jpegStream, jpegCodecInfo, jpegEncoderParameters)
                jpegStream.Seek(0, SeekOrigin.Begin)
                Return jpegStream
            End SyncLock
        Else
            Return New MemoryStream()
        End If
    End Function

    Public Function Decode(jpegBytes As Byte()) As Bitmap
        Return Decode(New MemoryStream(jpegBytes))
    End Function

    Public Function Decode(jpegStream As Stream) As Bitmap
        Try
            Return New Bitmap(jpegStream)
        Catch
            Return Nothing
        End Try
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
End Module
