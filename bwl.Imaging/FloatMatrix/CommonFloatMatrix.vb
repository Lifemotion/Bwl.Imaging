Public Class CommonFloatMatrix
    Protected _matrices As List(Of Single(,))
    Protected _width As Integer
    Protected _height As Integer

    Public Sub New(channels As Integer, width As Integer, height As Integer)
        If channels < 1 Then Throw New ArgumentException("channels must be >0")
        If channels > 1024 Then Throw New ArgumentException("channels must be <=1024 due to memory overflow protection")
        If width < 1 Then Throw New ArgumentException("width must be >0")
        If height < 1 Then Throw New ArgumentException("height must be >0")
        _width = width
        _height = height
        _matrices = New List(Of Single(,))
        For i = 1 To channels
            Dim channel(width - 1, height - 1) As Single
            _matrices.Add(channel)
        Next
    End Sub

    Public ReadOnly Property Matrix(channel As Integer) As Single(,)
        Get
            Return _matrices(channel)
        End Get
    End Property

    Protected Function GetWidth(matrix As Single(,)) As Integer
        Return matrix.GetLength(0)
    End Function

    Protected Function GetHeight(matrix As Single(,)) As Integer
        Return matrix.GetLength(1)
    End Function

    Public Sub New(matrices As IEnumerable(Of Byte(,)), multiplier As Single)
        If matrices Is Nothing Then Throw New ArgumentException("matrices must not be null")
        If matrices.Count = 0 Then Throw New ArgumentException("matrices must contain at least one matrix")
        _width = matrices(0).GetLength(0)
        _height = matrices(0).GetLength(1)
        _matrices = New List(Of Single(,))

        For Each mtr In matrices
            If mtr.GetLength(0) <> _width Then Throw New Exception("all matrices must be one size")
            If mtr.GetLength(1) <> _height Then Throw New Exception("all matrices must be one size")
            Dim channel(Width - 1, Height - 1) As Single
            For x = 0 To _width - 1
                For y = 0 To _height - 1
                    channel(x, y) = CSng(mtr(x, y)) * multiplier
                Next
            Next
            _matrices.Add(channel)
        Next
    End Sub

    Public Sub New(matrices As IEnumerable(Of Single(,)))
        If matrices Is Nothing Then Throw New ArgumentException("matrices must not be null")
        If matrices.Count = 0 Then Throw New ArgumentException("matrices must contain at least one matrix")
        _width = GetWidth(matrices(0))
        _height = GetHeight(matrices(0))
        _matrices = New List(Of Single(,))

        For Each mtr In matrices
            If GetWidth(mtr) <> _width Then Throw New Exception("all matrices must be one size")
            If GetHeight(mtr) <> _height Then Throw New Exception("all matrices must be one size")
            _matrices.Add(mtr)
        Next
    End Sub

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

    Public Function CloneMatrix(matrix As Single(,)) As Single(,)
        Dim arr(Width - 1, Height - 1) As Single
        For x = 0 To Width - 1
            For y = 0 To Height - 1
                arr(x, y) = matrix(x, y)
            Next
        Next
        Return arr
    End Function

    Public Function ResizeMatrixHalf(matrix As Single(,)) As Single(,)
        Dim result(_width \ 2 - 1, _height \ 2 - 1) As Single

        For x = 0 To _width \ 2 - 1
            For y = 0 To _height \ 2 - 1
                Dim point As Integer = 0
                point += matrix(x * 2, y * 2)
                point += matrix(x * 2 + 1, y * 2)
                point += matrix(x * 2, y * 2 + 1)
                point += matrix(x * 2 + 1, y * 2 + 1)
                point \= 4
                result(x, y) = point
            Next
        Next
        Return result
    End Function

    Public Function ResizeMatrixTwo(matrix As Single(,)) As Single(,)
        Dim result(_width * 2 - 1, _height * 2 - 1) As Single
        For x = 0 To Width - 1
            For y = 0 To Height - 1
                result(x * 2, y * 2) = matrix(x, y)
                result(x * 2 + 1, y * 2) = matrix(x, y)
                result(x * 2, y * 2 + 1) = matrix(x, y)
                result(x * 2 + 1, y * 2 + 1) = matrix(x, y)
            Next
        Next
        Return result
    End Function

    Public Overridable Function Clone() As CommonFloatMatrix
        Dim list As New List(Of Single(,))
        For Each mtr In _matrices
            list.Add(CloneMatrix(mtr))
        Next
        Return New CommonFloatMatrix(list)
    End Function

    Public Overridable Function ResizeTwo() As CommonFloatMatrix
        Dim list As New List(Of Single(,))
        For Each mtr In _matrices
            list.Add(ResizeMatrixTwo(mtr))
        Next
        Return New CommonFloatMatrix(list)
    End Function

    Public Overridable Function ResizeHalf() As CommonFloatMatrix
        Dim list As New List(Of Single(,))
        For Each mtr In _matrices
            list.Add(ResizeMatrixHalf(mtr))
        Next
        Return New CommonFloatMatrix(list)
    End Function

End Class
