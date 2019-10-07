Imports System.Runtime.CompilerServices

Public Class BitmapConverter
    Public Shared Function BitmapToGrayMatrix(bitmap As Bitmap) As GrayMatrix
        Dim processor As New BitmapOperations
        processor.LoadBitmap(bitmap)
        Return processor.GetGrayMatrix()
    End Function

    Public Shared Function BitmapToRGBMatrix(bitmap As Bitmap) As RGBMatrix
        Dim processor As New BitmapOperations
        processor.LoadBitmap(bitmap)
        Return processor.GetRGBMatrix()
    End Function

    Public Shared Function GrayMatrixToBitmap(matrix As GrayMatrix) As Bitmap
        Dim processor As New BitmapOperations
        processor.LoadGrayMatrixWithLimiter(matrix)
        Return processor.GetBitmap()
    End Function

    Public Shared Function RGBMatrixToBitmap(matrix As RGBMatrix, Optional useLimiter As Boolean = False) As Bitmap
        Dim processor As New BitmapOperations
        processor.LoadRGBMatrixWithLimiter(matrix)
        Return processor.GetBitmap()
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
