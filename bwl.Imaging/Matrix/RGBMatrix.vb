Public Class RGBMatrix
    Inherits CommonMatrix

    Sub New(width As Integer, height As Integer)
        MyBase.New(3, width, height)
    End Sub

    Sub New(red() As Integer, green() As Integer, blue() As Integer, width As Integer, height As Integer)
        MyBase.New({red, green, blue}, width, height)
    End Sub

    Sub New(red() As Double, green() As Double, blue() As Double, width As Integer, height As Integer)
        MyBase.New({red, green, blue}, width, height)
    End Sub

    Sub New(red() As Integer, green() As Integer, blue() As Integer, width As Integer, height As Integer, multiplier As Double)
        MyBase.New({red, green, blue}, width, height, multiplier)
    End Sub

    Sub New(red() As Double, green() As Double, blue() As Double, width As Integer, height As Integer, multiplier As Double)
        MyBase.New({red, green, blue}, width, height, multiplier)
    End Sub

    Public Property RGBPixel(x As Integer, y As Integer) As RGB
        Get
            Dim r = _matrices(0)(x + y * Width)
            Dim g = _matrices(1)(x + y * Width)
            Dim b = _matrices(2)(x + y * Width)
            Return New RGB(r, g, b)
        End Get
        Set(value As RGB)
            _matrices(0)(x + y * Width) = value.R
            _matrices(1)(x + y * Width) = value.G
            _matrices(2)(x + y * Width) = value.B
        End Set
    End Property

    'high cpu overhead!
    Public Property HSVPixel(x As Integer, y As Integer) As HSV
        Get
            Dim r = _matrices(0)(x + y * Width)
            Dim g = _matrices(1)(x + y * Width)
            Dim b = _matrices(2)(x + y * Width)
            Return (New RGB(r, g, b)).ToHSV
        End Get
        Set(value As HSV)
            Dim rgb = value.ToRgb
            _matrices(0)(x + y * Width) = rgb.R
            _matrices(1)(x + y * Width) = rgb.G
            _matrices(2)(x + y * Width) = rgb.B
        End Set
    End Property

    Public Property ColorPixel(x As Integer, y As Integer) As Color
        Get
            Dim r = _matrices(0)(x + y * Width)
            Dim g = _matrices(1)(x + y * Width)
            Dim b = _matrices(2)(x + y * Width)
            Return Color.FromArgb(r, g, b)
        End Get
        Set(value As Color)
            _matrices(0)(x + y * Width) = value.R
            _matrices(1)(x + y * Width) = value.G
            _matrices(2)(x + y * Width) = value.B
        End Set
    End Property

    Public Property RedPixel(x As Integer, y As Integer) As Integer
        Get
            Return _matrices(0)(x + y * Width)
        End Get
        Set(value As Integer)
            _matrices(0)(x + y * Width) = value
        End Set
    End Property

    Public Property GreenPixel(x As Integer, y As Integer) As Integer
        Get
            Return _matrices(1)(x + y * Width)
        End Get
        Set(value As Integer)
            _matrices(1)(x + y * Width) = value
        End Set
    End Property

    Public Property BluePixel(x As Integer, y As Integer) As Integer
        Get
            Return _matrices(2)(x + y * Width)
        End Get
        Set(value As Integer)
            _matrices(2)(x + y * Width) = value
        End Set
    End Property

    Public ReadOnly Property Red As Integer()
        Get
            Return _matrices(0)
        End Get
    End Property

    Public ReadOnly Property Green As Integer()
        Get
            Return _matrices(1)
        End Get
    End Property

    Public ReadOnly Property Blue As Integer()
        Get
            Return _matrices(2)
        End Get
    End Property

    Public Shadows Function Clone() As RGBMatrix
        Return New RGBMatrix(CloneMatrix(Red), CloneMatrix(Green), CloneMatrix(Blue), Width, Height)
    End Function

    Public Function ToGrayMatrix() As GrayMatrix
        Dim gray(Width * Height - 1) As Integer
        For i = 0 To Width * Height - 1
            'Y = 0.299 R + 0.587 G + 0.114 B - CCIR-601 (http://inst.eecs.berkeley.edu/~cs150/Documents/ITU601.PDF)
            gray(i) = Red(i) * 0.299 + Green(i) * 0.587 + Blue(i) * 0.114
        Next
        Return New GrayMatrix(gray, Width, Height)
    End Function

    Public Overloads Function ResizeTwo() As RGBMatrix
        Dim resized = MyBase.ResizeTwo()
        Return New RGBMatrix(resized.Matrix(0), resized.Matrix(1), resized.Matrix(2), resized.Width, resized.Height)
    End Function

    Public Overloads Function ResizeHalf() As RGBMatrix
        Dim resized = MyBase.ResizeHalf()
        Return New RGBMatrix(resized.Matrix(0), resized.Matrix(1), resized.Matrix(2), resized.Width, resized.Height)
    End Function
End Class
