Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading.Tasks

Public Class BitmapOperations
    <DllImport("kernel32.dll", EntryPoint:="CopyMemory", SetLastError:=False)>
    Public Shared Sub CopyMemory(ByVal dest As IntPtr, ByVal src As IntPtr, ByVal count As UInteger)
    End Sub

    Private _rawBytes As Byte()
    Private _width As Integer, _height As Integer
    Private _channels As Integer
    Private _bytesGray2D As Integer()

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
        Dim srcBD As BitmapData
        Dim srcStride As Integer
        Dim srcRect = Rectangle.FromLTRB(0, 0, bitmap.Width, bitmap.Height)
        Dim size As Integer = bitmap.Width * bitmap.Height
        If _rawBytes Is Nothing OrElse _rawBytes.Length <> (size * _channels) Then
            ReDim _rawBytes(size * _channels - 1)
        End If
        Dim tmpBytes As Byte()
        Dim tmpChannels = If(bitmap.PixelFormat = PixelFormat.Format32bppArgb, 4, _channels)

        If tmpChannels = 4 Then
            srcBD = bitmap.LockBits(srcRect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
            srcStride = srcBD.Stride
            tmpBytes = New Byte(size * tmpChannels - 1) {}
        Else
            srcBD = bitmap.LockBits(srcRect, ImageLockMode.ReadOnly, If(_channels = 1, PixelFormat.Format8bppIndexed, PixelFormat.Format24bppRgb))
            srcStride = srcBD.Stride
            tmpBytes = _rawBytes
        End If

        'Исходный битмап, некратный по ширине 4, имеет выравнивание, и загружать из него данные, как было реализовано ранее,
        'то есть одним блоком - нельзя! Требуетя загрузка построчно, с игнорированием выравнивания.
        If srcStride = bitmap.Width * tmpChannels Then
            Using ap As New AutoPinner(tmpBytes)
                Dim srcBytes = srcBD.Scan0
                Dim rawBytes = ap.GetIntPtr()
                CopyMemory(rawBytes, srcBytes, tmpBytes.Length) 'fast!
            End Using
        Else
            Using ap As New AutoPinner(tmpBytes)
                Dim srcBytes = srcBD.Scan0
                Dim rawBytes = ap.GetIntPtr()
                Dim rawStride = _width * _channels
                For row = 0 To _height - 1
                    CopyMemory(rawBytes, srcBytes, rawStride) 'exact!
                    srcBytes += srcStride
                    rawBytes += rawStride
                Next
            End Using
        End If

        'Выбрасываем альфа-канал
        If tmpChannels = 4 Then
            For i = 0 To tmpBytes.Length \ 4 - 1
                _rawBytes(i * 3 + 0) = tmpBytes(i * 4 + 0)
                _rawBytes(i * 3 + 1) = tmpBytes(i * 4 + 1)
                _rawBytes(i * 3 + 2) = tmpBytes(i * 4 + 2)
            Next
        Else
            _rawBytes = tmpBytes
        End If

        bitmap.UnlockBits(srcBD)
    End Sub

    Public Function GetGrayMatrix() As GrayMatrix
        Dim result As GrayMatrix = Nothing
        If _bytesGray2D Is Nothing OrElse _bytesGray2D.Length <> (_width * _height) Then
            ReDim _bytesGray2D(_width * _height - 1)
        End If
        Select Case _channels
            Case 1
                For i = 0 To _width * _height - 1
                    _bytesGray2D(i) = _rawBytes(i)
                Next
                result = New GrayMatrix(_bytesGray2D, _width, _height)
            Case 3
                For i = 0 To _width * _height - 1
                    'Y = 0.299 R + 0.587 G + 0.114 B - CCIR-601 (http://inst.eecs.berkeley.edu/~cs150/Documents/ITU601.PDF)
                    _bytesGray2D(i) = _rawBytes(i * 3) * 0.114 + _rawBytes(i * 3 + 1) * 0.587 + _rawBytes(i * 3 + 2) * 0.299
                Next
                result = New GrayMatrix(_bytesGray2D, _width, _height)
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
        Dim result As New Bitmap(_width, _height, If(_channels = 1, PixelFormat.Format8bppIndexed, PixelFormat.Format24bppRgb))
        If _channels = 1 Then
            result.Palette = GetGrayScalePalette()
        End If
        Dim resRect = Rectangle.FromLTRB(0, 0, _width, _height)
        Dim resultBD = result.LockBits(resRect, ImageLockMode.ReadWrite, If(_channels = 1, PixelFormat.Format8bppIndexed, PixelFormat.Format24bppRgb))
        Dim resStride = resultBD.Stride
        If resStride = result.Width * _channels Then
            Using ap As New AutoPinner(_rawBytes)
                Dim rawBytes = ap.GetIntPtr()
                Dim resBytes = resultBD.Scan0
                CopyMemory(resBytes, rawBytes, _rawBytes.Length) 'fast!
            End Using
        Else
            Using ap As New AutoPinner(_rawBytes)
                Dim rawBytes = ap.GetIntPtr()
                Dim resBytes = resultBD.Scan0
                Dim rawStride = _width * _channels
                For row = 0 To _height - 1
                    CopyMemory(resBytes, rawBytes, rawStride) 'exact!
                    rawBytes += rawStride
                    resBytes += resStride
                Next
            End Using
        End If

        result.UnlockBits(resultBD)
        Return result
    End Function

    Public Shared Function GetGrayScalePalette() As ColorPalette
        Dim bmp As Bitmap = New Bitmap(1, 1, PixelFormat.Format8bppIndexed)
        Dim monoPalette As ColorPalette = bmp.Palette
        Dim entries() As Color = monoPalette.Entries
        For i = 0 To 255
            entries(i) = Color.FromArgb(i, i, i)
        Next
        Return monoPalette
    End Function
End Class