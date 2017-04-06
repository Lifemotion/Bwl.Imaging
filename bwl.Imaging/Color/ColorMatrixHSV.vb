Public Class ColorMatrixHSV
    Inherits ColorMatrix(Of HSV)

    Public Sub New(width As Integer, height As Integer)
        MyBase.New(width, height)
    End Sub

    Public Sub New(matrix As HSV(), width As Integer, height As Integer)
        MyBase.New(matrix, width, height)
    End Sub

    Public Sub New(rgbmatrix As RGBMatrix)
        MyBase.New(rgbmatrix.Width, rgbmatrix.Height)
        For i = 0 To _matrix.Length - 1
            _matrix(i) = (New RGB(rgbmatrix.Red(i), rgbmatrix.Green(i), rgbmatrix.Blue(i))).ToHSV
        Next
    End Sub

    Public Function ToRGBMatrix() As RGBMatrix
        Dim rgbm As New RGBMatrix(Me.Width, Me.Height)
        For i = 0 To _matrix.Length - 1
            Dim pixel = _matrix(i).ToRgb
            rgbm.Red(i) = pixel.R
            rgbm.Green(i) = pixel.G
            rgbm.Blue(i) = pixel.B
        Next
        Return rgbm
    End Function
End Class
