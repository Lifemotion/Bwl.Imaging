Public Class RGBFloatMatrix
    Inherits CommonFloatMatrix

    Sub New(red(,) As Single, green(,) As Single, blue(,) As Single)
        MyBase.New({red, green, blue})
    End Sub

    Sub New(red(,) As Byte, green(,) As Byte, blue(,) As Byte, multiplier As Single)
        MyBase.New({red, green, blue}, multiplier)
    End Sub

    Sub New(width As Integer, height As Integer)
        MyBase.New(3, width, height)
    End Sub

    Public ReadOnly Property Red As Single(,)
        Get
            Return _matrices(0)
        End Get
    End Property

    Public ReadOnly Property Green As Single(,)
        Get
            Return _matrices(1)
        End Get
    End Property

    Public ReadOnly Property Blue As Single(,)
        Get
            Return _matrices(2)
        End Get
    End Property

    Public Shadows Function Clone() As RGBFloatMatrix
        Return New RGBFloatMatrix(CloneMatrix(Red), CloneMatrix(Green), CloneMatrix(Blue))
    End Function

    Public Function ToGrayFloatMatrix() As GrayFloatMatrix
        Dim gray(Width - 1, Height - 1) As Single
        For x = 0 To Width - 1
            For y = 0 To Height - 1
                gray(x, y) = Red(x, y) * 0.222 + Green(x, y) * 0.707 + Blue(x, y) * 0.071
            Next
        Next
        Return New GrayFloatMatrix(gray)
    End Function
End Class
