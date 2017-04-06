Public Class ColorMatrix(Of T As Structure)
    Protected _matrix() As T
    Protected _width As Integer
    Protected _height As Integer

    Public Sub New(width As Integer, height As Integer)
        If width < 1 Then Throw New ArgumentException("width must be >0")
        If height < 1 Then Throw New ArgumentException("height must be >0")
        _width = width
        _height = height
        Dim matrix(width * height - 1) As T
        _matrix = Matrix
    End Sub

    Public Sub New(matrix As T(), width As Integer, height As Integer)
        If width < 1 Then Throw New ArgumentException("width must be >0")
        If height < 1 Then Throw New ArgumentException("height must be >0")
        If height * width <> matrix.Length Then Throw New ArgumentException("matrix length <> width*height")
        _width = width
        _height = height
        _matrix = matrix
    End Sub

    Public Property Pixel(x As Integer, y As Integer) As T
        Get
            Return _matrix(x + y * Width)
        End Get
        Set(value As T)
            _matrix(x + y * Width) = value
        End Set
    End Property

    Public ReadOnly Property Matrix() As T()
        Get
            Return _matrix
        End Get
    End Property

    Public Function FitX(x As Integer) As Integer
        If x < 0 Then x = 0
        If x >= Width Then x = Width - 1
        Return x
    End Function

    Public Function FitY(y As Integer) As Integer
        If y < 0 Then y = 0
        If y >= Height Then y = Height - 1
        Return y
    End Function

    Public Function HalfWidth() As Integer
        Return Width \ 2
    End Function

    Public Function HalfHeight() As Integer
        Return Height \ 2
    End Function

    Public ReadOnly Property Width As Integer
        Get
            Return _width
        End Get
    End Property

    Public ReadOnly Property Height As Integer
        Get
            Return _height
        End Get
    End Property

    Public Function CloneMatrix(matrix As T()) As T()
        Dim arr(matrix.Length - 1) As T
        Array.Copy(matrix, arr, matrix.Length)
        Return arr
    End Function

    Public Function ResizeMatrixHalf(matrix As T()) As T()
        Throw New NotImplementedException
        Dim result((_width \ 2) * (_height \ 2) - 1) As T
        For y = 0 To _height \ 2 - 1
            Dim lineOffset1 = y * 2 * _width
            Dim lineOffset2 = (y * 2 + 1) * _width
            Dim resOffset = y * (_width \ 2)
            For x = 0 To _width \ 2 - 1
                'Dim point As Integer = 0
                'point += matrix(x * 2 + lineOffset1)
                'Point += matrix(x * 2 + 1 + lineOffset1)
                'Point += matrix(x * 2 + lineOffset2)
                'Point += matrix(x * 2 + 1 + lineOffset2)
                'Point \= 4
                'result(x + resOffset) = point
            Next
        Next
        Return result
    End Function

    Public Function ResizeMatrixTwo(matrix As T()) As T()
        Throw New NotImplementedException
        Dim result((_width * 2) * (_height * 2) - 1) As T
        For y = 0 To Height - 1
            Dim lineOffset1 = (y * 2) * _width * 2
            Dim lineOffset2 = (y * 2 + 1) * _width * 2
            Dim offset = y * _width
            For x = 0 To _width - 1
                Dim elem = matrix(x + offset)
                'result(x * 2 + lineOffset1) = elem
                'result(x * 2 + 1 + lineOffset1) = elem
                'result(x * 2 + lineOffset2) = elem
                'result(x * 2 + 1 + lineOffset2) = elem
            Next
        Next
        Return result
    End Function

    Public Overridable Function Clone() As ColorMatrix(Of T)
        Return New ColorMatrix(Of T)(CloneMatrix(_matrix), Width, Height)
    End Function

    Public Overridable Function ResizeTwo() As ColorMatrix(Of T)
        Throw New NotImplementedException
        '  Dim list As New List(Of Integer())
        ' For Each mtr In _matrices
        '     list.Add(ResizeMatrixTwo(mtr))
        ' Next
        ' Return New CommonMatrix(list, Width * 2, Height * 2)
    End Function

    Public Overridable Function ResizeHalf() As ColorMatrix(Of T)
        Throw New NotImplementedException
        '  Dim list As New List(Of Integer())
        '  For Each mtr In _matrices
        '      list.Add(ResizeMatrixHalf(mtr))
        '  Next
        '  Return New CommonMatrix(list, Width \ 2, Height \ 2)
    End Function
End Class
