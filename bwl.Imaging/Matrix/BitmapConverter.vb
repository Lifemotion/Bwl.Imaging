Imports System.Runtime.CompilerServices
Imports System.Threading.Tasks

Public Class BitmapConverter
    Private Class BitmapOperations
        Private _rawBytes As Byte()
        Private _width As Integer, _height As Integer
        Private _channels As Integer

        Public Property RawBytes() As Byte()
            Set(value As Byte())
                _rawBytes = value
            End Set
            Get
                Return _rawBytes
            End Get
        End Property

        Public ReadOnly Property Channels As Integer
            Get
                Return _channels
            End Get
        End Property

        Public Sub LoadBitmap(bitmap As Bitmap)
            _channels = If(bitmap.PixelFormat = PixelFormat.Format8bppIndexed, 1, 3)
            _width = bitmap.Width
            _height = bitmap.Height
            Dim tmpBD As BitmapData
            Dim tmpRect As Rectangle
            tmpRect = Rectangle.FromLTRB(0, 0, bitmap.Width, bitmap.Height)
            Dim size As Integer = bitmap.Width * bitmap.Height
            ReDim _rawBytes(size * _channels)
            Dim tmpChannels = If(bitmap.PixelFormat = PixelFormat.Format32bppArgb, 4, _channels)
            If tmpChannels = 4 Then
                tmpBD = bitmap.LockBits(tmpRect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
                Dim tmpBytes = New Byte(size * tmpChannels - 1) {}
                Runtime.InteropServices.Marshal.Copy(tmpBD.Scan0, tmpBytes, 0, size * tmpChannels)
                For i = 0 To tmpBytes.Length \ 4 - 1
                    _rawBytes(i * 3 + 0) = tmpBytes(i * 4 + 0)
                    _rawBytes(i * 3 + 1) = tmpBytes(i * 4 + 1)
                    _rawBytes(i * 3 + 2) = tmpBytes(i * 4 + 2)
                Next
            Else
                tmpBD = bitmap.LockBits(tmpRect, ImageLockMode.ReadOnly, If(_channels = 1, PixelFormat.Format8bppIndexed, PixelFormat.Format24bppRgb))
                Runtime.InteropServices.Marshal.Copy(tmpBD.Scan0, _rawBytes, 0, size * _channels)
            End If
            bitmap.UnlockBits(tmpBD)
        End Sub

        Public Function GetGrayMatrix() As GrayMatrix
            Dim result As GrayMatrix = Nothing
            Dim bytesGray2D(_width * _height - 1) As Integer
            Select Case _channels
                Case 1
                    For i = 0 To _width * _height - 1
                        bytesGray2D(i) = _rawBytes(i)
                    Next
                    result = New GrayMatrix(bytesGray2D, _width, _height)
                Case 3
                    For i = 0 To _width * _height - 1
                        bytesGray2D(i) = _rawBytes(i * 3) * 0.222 + _rawBytes(i * 3 + 1) * 0.707 + _rawBytes(i * 3 + 2) * 0.071
                    Next
                    result = New GrayMatrix(bytesGray2D, _width, _height)
            End Select
            Return result
        End Function

        Public Function GetRGBMatrix() As RGBMatrix
            Dim result As RGBMatrix = Nothing
            Dim bytesRed2D(_width * _height - 1) As Integer
            Dim bytesGreen2D(_width * _height - 1) As Integer
            Dim bytesBlue2D(_width * _height - 1) As Integer
            Select Case _channels
                Case 1
                    For i = 0 To _width * _height - 1
                        Dim rawByte = _rawBytes(i)
                        bytesRed2D(i) = rawByte
                        bytesGreen2D(i) = rawByte
                        bytesBlue2D(i) = rawByte
                    Next
                    result = New RGBMatrix(bytesRed2D, bytesGreen2D, bytesBlue2D, _width, _height)
                Case 3
                    For i = 0 To _width * _height - 1
                        bytesRed2D(i) = _rawBytes(i * 3 + 2)
                        bytesGreen2D(i) = _rawBytes(i * 3 + 1)
                        bytesBlue2D(i) = _rawBytes(i * 3)
                    Next
                    result = New RGBMatrix(bytesRed2D, bytesGreen2D, bytesBlue2D, _width, _height)
            End Select
            Return result
        End Function

        Public Sub LoadGrayMatrixWithLimiter(matrix As GrayMatrix)
            _channels = 1
            _width = matrix.Width
            _height = matrix.Height
            ReDim _rawBytes(_width * _height - 1)
            Dim matrixGray = matrix.Gray
            For i = 0 To _width * _height - 1
                Dim pixel = matrixGray(i)
                If pixel < 0 Then pixel = 0
                If pixel > 255 Then pixel = 255
                _rawBytes(i) = pixel
            Next
        End Sub

        Public Sub LoadRGBMatrixWithLimiter(matrix As RGBMatrix)
            _channels = 3
            _width = matrix.Width
            _height = matrix.Height
            ReDim _rawBytes(_width * _height * 3 - 1)
            Dim matrixRed = matrix.Red
            Dim matrixGreen = matrix.Green
            Dim matrixBlue = matrix.Blue
            Dim i, x, y As Integer
            For y = 0 To _height - 1
                Dim offset = _width * y
                For x = 0 To _width - 1
                    i = x + offset
                    Dim pixelR = matrixRed(i)
                    Dim pixelG = matrixGreen(i)
                    Dim pixelB = matrixBlue(i)
                    If pixelR < 0 Then pixelR = 0
                    If pixelR > 255 Then pixelR = 255
                    If pixelG < 0 Then pixelG = 0
                    If pixelG > 255 Then pixelG = 255
                    If pixelB < 0 Then pixelB = 0
                    If pixelB > 255 Then pixelB = 255
                    _rawBytes(i * 3 + 2) = pixelR
                    _rawBytes(i * 3 + 1) = pixelG
                    _rawBytes(i * 3) = pixelB
                Next
            Next
        End Sub

        Public Function GetBitmap() As Bitmap
            If _width Mod 4 <> 0 Then Throw New Exception("GetBitmap() - image width must be multiplicity of 4 to create bitmaps correctly")
            Dim tmpBitmap As New Bitmap(_width, _height, If(_channels = 1, PixelFormat.Format8bppIndexed, PixelFormat.Format24bppRgb))
            If _channels = 1 Then
                tmpBitmap.Palette = GetGrayScalePalette()
            End If
            Dim tmpBD As BitmapData
            Dim tmpRect As Rectangle
            tmpRect = Rectangle.FromLTRB(0, 0, _width, _height)
            tmpBD = tmpBitmap.LockBits(tmpRect, ImageLockMode.ReadWrite, If(_channels = 1, PixelFormat.Format8bppIndexed, PixelFormat.Format24bppRgb))
            System.Runtime.InteropServices.Marshal.Copy(_rawBytes, 0, tmpBD.Scan0, _rawBytes.Length)
            tmpBitmap.UnlockBits(tmpBD)
            Return tmpBitmap
        End Function
    End Class

    Public Shared Function BitmapToGrayMatrix(bitmap As Bitmap) As GrayMatrix
        Dim processor As New BitmapOperations
        processor.LoadBitmap(bitmap)
        Return processor.GetGrayMatrix
    End Function

    Public Shared Function BitmapToRGBMatrix(bitmap As Bitmap) As RGBMatrix
        Dim processor As New BitmapOperations
        processor.LoadBitmap(bitmap)
        Return processor.GetRGBMatrix
    End Function

    Public Shared Function GrayMatrixToBitmap(matrix As GrayMatrix) As Bitmap
        Dim processor As New BitmapOperations
        processor.LoadGrayMatrixWithLimiter(matrix)
        Return processor.GetBitmap
    End Function

    Public Shared Function RGBMatrixToBitmap(matrix As RGBMatrix, Optional useLimiter As Boolean = False) As Bitmap
        Dim processor As New BitmapOperations
        processor.LoadRGBMatrixWithLimiter(matrix)
        Return processor.GetBitmap
    End Function

    Private Shared Function GetGrayScalePalette() As ColorPalette
        Dim bmp As Bitmap = New Bitmap(1, 1, PixelFormat.Format8bppIndexed)
        Dim monoPalette As ColorPalette = bmp.Palette
        Dim entries() As Color = monoPalette.Entries
        For i = 0 To 255
            entries(i) = Color.FromArgb(i, i, i)
        Next
        Return monoPalette
    End Function
End Class

Public Module BitmapConverterExtensions
    <Extension()>
    Public Function BitmapToGrayMatrix(bitmap As Bitmap) As GrayMatrix
        Return BitmapConverter.BitmapToGrayMatrix(bitmap)
    End Function

    <Extension()>
    Public Function BitmapToRgbMatrix(bitmap As Bitmap) As RGBMatrix
        Return BitmapConverter.BitmapToRGBMatrix(bitmap)
    End Function

    <Extension()>
    Public Function ToBitmap(matrix As GrayMatrix) As Bitmap
        Return BitmapConverter.GrayMatrixToBitmap(matrix)
    End Function

    <Extension()>
    Public Function ToBitmap(matrix As RGBMatrix) As Bitmap
        Return BitmapConverter.RGBMatrixToBitmap(matrix)
    End Function
End Module
