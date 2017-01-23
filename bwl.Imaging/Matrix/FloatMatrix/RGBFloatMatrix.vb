Public Class RGBFloatMatrix
    Inherits CommonFloatMatrix

    Sub New(width As Integer, height As Integer)
        MyBase.New(3, width, height)
    End Sub

    Sub New(red() As Double, green() As Double, blue() As Double, width As Integer, height As Integer)
        MyBase.New({red, green, blue}, width, height)
    End Sub

    Sub New(red() As Integer, green() As Integer, blue() As Integer, width As Integer, height As Integer)
        MyBase.New({red, green, blue}, width, height)
    End Sub

    Sub New(red() As Double, green() As Double, blue() As Double, width As Integer, height As Integer, multiplier As Double)
        MyBase.New({red, green, blue}, width, height, multiplier)
    End Sub

    Sub New(red() As Integer, green() As Integer, blue() As Integer, width As Integer, height As Integer, multiplier As Double)
        MyBase.New({red, green, blue}, width, height, multiplier)
    End Sub

    Public Property RedPixel(x As Integer, y As Integer) As Double
        Get
            Return _matrices(0)(x + y * Width)
        End Get
        Set(value As Double)
            _matrices(0)(x + y * Width) = value
        End Set
    End Property

    Public Property GreenPixel(x As Integer, y As Integer) As Double
        Get
            Return _matrices(1)(x + y * Width)
        End Get
        Set(value As Double)
            _matrices(1)(x + y * Width) = value
        End Set
    End Property

    Public Property BluePixel(x As Integer, y As Integer) As Double
        Get
            Return _matrices(2)(x + y * Width)
        End Get
        Set(value As Double)
            _matrices(2)(x + y * Width) = value
        End Set
    End Property

    Public ReadOnly Property Red As Double()
        Get
            Return _matrices(0)
        End Get
    End Property

    Public ReadOnly Property Green As Double()
        Get
            Return _matrices(1)
        End Get
    End Property

    Public ReadOnly Property Blue As Double()
        Get
            Return _matrices(2)
        End Get
    End Property

    Public Shadows Function Clone() As RGBFloatMatrix
        Return New RGBFloatMatrix(CloneMatrix(Red), CloneMatrix(Green), CloneMatrix(Blue), Width, Height)
    End Function

    Public Function ToGrayMatrix() As GrayFloatMatrix
        Dim gray(Width * Height - 1) As Double
        For i = 0 To Width * Height - 1
            gray(i) = Red(i) * 0.222 + Green(i) * 0.707 + Blue(i) * 0.071
        Next
        Return New GrayFloatMatrix(gray, Width, Height)
    End Function

    Public Overloads Function ResizeTwo() As RGBFloatMatrix
        Dim resized = MyBase.ResizeTwo()
        Return New RGBFloatMatrix(resized.Matrix(0), resized.Matrix(1), resized.Matrix(2), resized.Width, resized.Height)
    End Function

    Public Overloads Function ResizeHalf() As RGBFloatMatrix
        Dim resized = MyBase.ResizeHalf()
        Return New RGBFloatMatrix(resized.Matrix(0), resized.Matrix(1), resized.Matrix(2), resized.Width, resized.Height)
    End Function
End Class
