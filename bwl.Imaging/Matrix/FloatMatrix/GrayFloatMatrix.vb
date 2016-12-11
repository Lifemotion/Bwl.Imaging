Public Class GrayFloatMatrix
    Inherits CommonFloatMatrix

    Sub New(matrix() As Double, width As Integer, height As Integer)
        MyBase.New({matrix}, width, height)
    End Sub

    Sub New(matrix() As Integer, width As Integer, height As Integer, multiplier As Double)
        MyBase.New({matrix}, width, height, multiplier)
    End Sub

    Sub New(width As Integer, height As Integer)
        MyBase.New(1, width, height)
    End Sub

    Public Property GrayPixel(x As Integer, y As Integer) As Double
        Get
            Return _matrices(0)(x + y * Width)
        End Get
        Set(value As Double)
            _matrices(0)(x + y * Width) = value
        End Set
    End Property

    Public ReadOnly Property Gray As Double()
        Get
            Return _matrices(0)
        End Get
    End Property

    Public Shadows Function Clone() As GrayFloatMatrix
        Return New GrayFloatMatrix(CloneMatrix(Gray), Width, Height)
    End Function

    Public Function ToRGBMatrix() As RGBFloatMatrix
        Return New RGBFloatMatrix(Gray, Gray, Gray, Width, Height)
    End Function

End Class
