Public Class GrayMatrix
    Inherits CommonMatrix

    Sub New(matrix() As Integer, width As Integer, height As Integer)
        MyBase.New({matrix}, width, height)
    End Sub

    Sub New(matrix() As Double, width As Integer, height As Integer, multiplier As Double)
        MyBase.New({matrix}, width, height, multiplier)
    End Sub

    Sub New(width As Integer, height As Integer)
        MyBase.New(1, width, height)
    End Sub

    Public Property GrayPixel(x As Integer, y As Integer) As Integer
        Get
            Return _matrices(0)(x + y * Width)
        End Get
        Set(value As Integer)
            _matrices(0)(x + y * Width) = value
        End Set
    End Property

    Public ReadOnly Property Gray As Integer()
        Get
            Return _matrices(0)
        End Get
    End Property

    Public Shadows Function Clone() As GrayMatrix
        Return New GrayMatrix(CloneMatrix(Gray), Width, Height)
    End Function

    Public Function ToRGBMatrix() As RGBMatrix
        Return New RGBMatrix(Gray, Gray, Gray, Width, Height)
    End Function
End Class
