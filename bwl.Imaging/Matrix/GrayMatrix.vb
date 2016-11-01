Public Class GrayMatrix
    Inherits CommonMatrix

    Sub New(matrix(,) As Byte)
        MyBase.New({matrix})
    End Sub

    Sub New(matrix(,) As Single, multiplier As Single)
        MyBase.New({matrix}, multiplier)
    End Sub

    Sub New(width As Integer, height As Integer)
        MyBase.New(1, width, height)
    End Sub

    Public ReadOnly Property Gray As Byte(,)
        Get
            Return _matrices(0)
        End Get
    End Property

    Public Shadows Function Clone() As GrayMatrix
        Return New GrayMatrix(CloneMatrix(Gray))
    End Function

    Public Function ToRGBMatrix() As RGBMatrix
        Return New RGBMatrix(Gray, Gray, Gray)
    End Function
End Class
