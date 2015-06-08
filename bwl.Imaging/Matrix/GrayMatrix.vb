Public Class GrayMatrix
    Private _matrix(,) As Byte

    Sub New(matrix(,) As Byte)
        _matrix = matrix
    End Sub

    Sub New(width As Integer, height As Integer)
        ReDim _matrix(width - 1, height - 1)
    End Sub

    Public ReadOnly Property Width As Integer
        Get
            Return _matrix.GetLength(0)
        End Get
    End Property
    Public ReadOnly Property Height As Integer
        Get
            Return _matrix.GetLength(1)
        End Get
    End Property
    Public ReadOnly Property Gray As Byte(,)
        Get
            Return _matrix
        End Get
    End Property

    Public Function Clone() As GrayMatrix
        Dim arr(Width - 1, Height - 1) As Byte
        For x = 0 To Width - 1
            For y = 0 To Height - 1
                arr(x, y) = _matrix(x, y)
            Next
        Next
        Return New GrayMatrix(arr)
    End Function

    Public Function ResizeHalf() As GrayMatrix
        Dim matrix = Me
        Dim width = matrix.Width
        Dim height = matrix.Height
        Dim result(width \ 2 - 1, height \ 2 - 1) As Byte
        For x = 0 To width \ 2 - 1
            For y = 0 To height \ 2 - 1
                Dim point As Integer = 0
                point += matrix.Gray(x * 2, y * 2)
                point += matrix.Gray(x * 2 + 1, y * 2)
                point += matrix.Gray(x * 2, y * 2 + 1)
                point += matrix.Gray(x * 2 + 1, y * 2 + 1)
                point \= 4
                result(x, y) = point
            Next
        Next
        Return New GrayMatrix(result)
    End Function

    Public Function ResizeTwo() As GrayMatrix
        Dim matrix = Me
        Dim width = matrix.Width
        Dim height = matrix.Height
        Dim result(width * 2 - 1, height * 2 - 1) As Byte
        For x = 0 To width - 1
            For y = 0 To height - 1
                Dim point As Integer = 0
                result(x * 2, y * 2) = matrix.Gray(x, y)
                result(x * 2 + 1, y * 2) = matrix.Gray(x, y)
                result(x * 2, y * 2 + 1) = matrix.Gray(x, y)
                result(x * 2 + 1, y * 2 + 1) = matrix.Gray(x, y)
            Next
        Next
        Return New GrayMatrix(result)
    End Function
End Class
