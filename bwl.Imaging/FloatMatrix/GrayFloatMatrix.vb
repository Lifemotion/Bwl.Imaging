Public Class GrayFloatMatrix
    Inherits CommonFloatMatrix

    Sub New(matrix(,) As Single)
        MyBase.New({matrix})
    End Sub

    Sub New(matrix(,) As Byte, multiplier As Single)
        MyBase.New({matrix}, multiplier)
    End Sub

    Sub New(width As Integer, height As Integer)
        MyBase.New(1, width, height)
    End Sub

    Public ReadOnly Property Gray As Single(,)
        Get
            Return _matrices(0)
        End Get
    End Property

    Public Shadows Function Clone() As GrayFloatMatrix
        Return New GrayFloatMatrix(CloneMatrix(Gray))
    End Function

    Public Function ToRGBFloatMatrix() As RGBFloatMatrix
        Return New RGBFloatMatrix(Gray, Gray, Gray)
    End Function

End Class
