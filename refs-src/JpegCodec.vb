'   Copyright 2023 Artem Drobanov (artem.drobanov@gmail.com)

'   Licensed under the Apache License, Version 2.0 (the "License");
'   you may Not use this file except In compliance With the License.
'   You may obtain a copy Of the License at

'     http://www.apache.org/licenses/LICENSE-2.0

'   Unless required by applicable law Or agreed To In writing, software
'   distributed under the License Is distributed On an "AS IS" BASIS,
'   WITHOUT WARRANTIES Or CONDITIONS Of ANY KIND, either express Or implied.
'   See the License For the specific language governing permissions And
'   limitations under the License.

Imports System.IO
Imports System.Drawing
Imports System.Drawing.Imaging

Public Module JpegCodec
    Public Function Encode(bmp As Bitmap, Optional frameQuality As Integer = 60) As MemoryStream
        Dim jpegStream As New MemoryStream()
        If bmp IsNot Nothing Then
            SyncLock bmp
                bmp.Save(jpegStream, GetCodecInfo(ImageFormat.Jpeg), GetEncoderParameters(frameQuality))
                jpegStream.Seek(0, SeekOrigin.Begin)
                Return jpegStream
            End SyncLock
        End If
        Return jpegStream
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
        Return ImageCodecInfo.GetImageDecoders().FirstOrDefault(Function(codec) codec.FormatID = format.Guid)
    End Function

    Private Function GetEncoderParameters(frameQuality As Integer) As EncoderParameters
        Dim jpegEncoderParameters As New EncoderParameters(1)
        jpegEncoderParameters.Param(0) = New EncoderParameter(Encoder.Quality, frameQuality)
        Return jpegEncoderParameters
    End Function
End Module
