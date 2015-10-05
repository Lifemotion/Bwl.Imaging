Imports System.Runtime.CompilerServices

Public Class BitmapConverter
    Private Class BitmapOperations
        Private _rawBytes As Byte()
        Private _width As Integer, _height As Integer

        Public Property RawBytes() As Byte()
            Set(value As Byte())
                _rawBytes = value
            End Set
            Get
                Return _rawBytes
            End Get
        End Property

        Public Sub LoadBitmap(bitmap As Bitmap)
            _width = bitmap.Width
            _height = bitmap.Height
            Dim tmpBD As BitmapData
            Dim tmpRect As Rectangle
            tmpRect = Rectangle.FromLTRB(0, 0, bitmap.Width, bitmap.Height)
            tmpBD = bitmap.LockBits(tmpRect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb)
            Dim size As Integer = bitmap.Width * bitmap.Height
            ReDim _rawBytes(size * 3)
            Runtime.InteropServices.Marshal.Copy(tmpBD.Scan0, _rawBytes, 0, size * 3)
            bitmap.UnlockBits(tmpBD)
        End Sub

        Public Function GetGrayMatrix() As GrayMatrix
            Dim bytesGray2D(_width - 1, _height - 1) As Byte
            Dim i, x, y As Integer
            For i = 0 To _width * _height - 1
                bytesGray2D(x, y) = _rawBytes(i * 3) * 0.222 + _rawBytes(i * 3 + 1) * 0.707 + _rawBytes(i * 3 + 2) * 0.071
                x += 1
                If x = _width Then
                    x = 0
                    y += 1
                End If
            Next
            Return New GrayMatrix(bytesGray2D)
        End Function

        Public Function GetRGBMatrix() As RGBMatrix
            Dim bytesRed2D(_width - 1, _height - 1) As Byte
            Dim bytesGreen2D(_width - 1, _height - 1) As Byte
            Dim bytesBlue2D(_width - 1, _height - 1) As Byte
            Dim i, x, y As Integer
            For i = 0 To _width * _height - 1
                bytesRed2D(x, y) = _rawBytes(i * 3 + 2)
                bytesGreen2D(x, y) = _rawBytes(i * 3 + 1)
                bytesBlue2D(x, y) = _rawBytes(i * 3 )
                x += 1
                If x = _width Then
                    x = 0
                    y += 1
                End If
            Next
            Return New RGBMatrix(bytesRed2D, bytesGreen2D, bytesBlue2D)
        End Function

        Public Sub LoadGrayMatrix(matrix As GrayMatrix)
            _width = matrix.Width
            _height = matrix.Height
            ReDim _rawBytes(_width * _height * 3 - 1)
            Dim i, x, y As Integer
            For x = 0 To _width - 1
                For y = 0 To _height - 1
                    i = _width * y + x
                    _rawBytes(i * 3) = matrix.Gray(x, y)
                    _rawBytes(i * 3 + 1) = matrix.Gray(x, y)
                    _rawBytes(i * 3 + 2) = matrix.Gray(x, y)
                Next
            Next
        End Sub

        Public Sub LoadRGBMatrix(matrix As RGBMatrix)
            _width = matrix.Width
            _height = matrix.Height
            ReDim _rawBytes(_width * _height * 3 - 1)
            Dim i, x, y As Integer
            For x = 0 To _width - 1
                For y = 0 To _height - 1
                    i = _width * y + x
                    _rawBytes(i * 3 + 2) = matrix.Red(x, y)
                    _rawBytes(i * 3 + 1) = matrix.Green(x, y)
                    _rawBytes(i * 3) = matrix.Blue(x, y)
                Next
            Next
        End Sub

        Public Function GetBitmap() As Bitmap
            If _width Mod 4 <> 0 Then Throw New Exception("GetBitmap() - image width must be multiplicity of 4 to create bitmaps correctly")
            Dim tmpBitmap As New Bitmap(_width, _height, PixelFormat.Format24bppRgb)
            Dim tmpBD As BitmapData
            Dim tmpRect As Rectangle
            tmpRect = Rectangle.FromLTRB(0, 0, _width, _height)
            tmpBD = tmpBitmap.LockBits(tmpRect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb)
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
        processor.LoadGrayMatrix(matrix)
        Return processor.GetBitmap
    End Function

    Public Shared Function RGBMatrixToBitmap(matrix As RGBMatrix) As Bitmap
        Dim processor As New BitmapOperations
        processor.LoadRGBMatrix(matrix)
        Return processor.GetBitmap
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