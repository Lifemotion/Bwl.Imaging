Public Class GrayMatrix
    Inherits CommonMatrix

    Sub New(width As Integer, height As Integer)
        MyBase.New(1, width, height)
    End Sub

    Sub New(matrix() As Integer, width As Integer, height As Integer)
        MyBase.New({matrix}, width, height)
    End Sub

    Sub New(matrix() As Double, width As Integer, height As Integer)
        MyBase.New({matrix}, width, height)
    End Sub

    Sub New(matrix() As Integer, width As Integer, height As Integer, multiplier As Double)
        MyBase.New({matrix}, width, height, multiplier)
    End Sub

    Sub New(matrix() As Double, width As Integer, height As Integer, multiplier As Double)
        MyBase.New({matrix}, width, height, multiplier)
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
        Return New RGBMatrix(CloneMatrix(Gray), CloneMatrix(Gray), CloneMatrix(Gray), Width, Height)
    End Function

    Public Overloads Function ResizeTwo() As GrayMatrix
        Dim resized = MyBase.ResizeTwo()
        Return New GrayMatrix(resized.Matrix(0), resized.Width, resized.Height)
    End Function

    Public Overloads Function ResizeHalf() As GrayMatrix
        Dim resized = MyBase.ResizeHalf()
        Return New GrayMatrix(resized.Matrix(0), resized.Width, resized.Height)
    End Function
End Class
