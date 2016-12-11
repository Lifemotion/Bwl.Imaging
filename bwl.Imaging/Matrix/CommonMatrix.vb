﻿Public Class CommonMatrix
    Protected _matrices As List(Of Integer())
    Protected _width As Integer
    Protected _height As Integer

    Public Sub New(channels As Integer, width As Integer, height As Integer)
        If channels < 1 Then Throw New ArgumentException("channels must be >0")
        If channels > 1024 Then Throw New ArgumentException("channels must be <=1024 due to memory overflow protection")
        If width < 1 Then Throw New ArgumentException("width must be >0")
        If height < 1 Then Throw New ArgumentException("height must be >0")
        _width = width
        _height = height
        _matrices = New List(Of Integer())
        For i = 1 To channels
            Dim channel(width * height - 1) As Integer
            _matrices.Add(channel)
        Next
    End Sub

    Public Property MatrixPixel(channel As Integer, x As Integer, y As Integer) As Integer
        Get
            Return _matrices(channel)(x + y * Width)
        End Get
        Set(value As Integer)
            _matrices(channel)(x + y * Width) = value
        End Set
    End Property

    Public ReadOnly Property Matrix(channel As Integer) As Integer()
        Get
            Return _matrices(channel)
        End Get
    End Property

    Public Sub New(matrices As IEnumerable(Of Integer()), width As Integer, height As Integer)
        If matrices Is Nothing Then Throw New ArgumentException("matrices must not be null")
        If matrices.Count = 0 Then Throw New ArgumentException("matrices must contain at least one matrix")
        _width = width
        _height = height
        _height = matrices(0).Length / width
        _matrices = New List(Of Integer())

        For Each mtr In matrices
            If mtr.Length <> width * height Then Throw New Exception("all matrices must have width*height elements")
            _matrices.Add(mtr)
        Next
    End Sub

    Public Sub New(matrices As IEnumerable(Of Double()), width As Integer, height As Integer, multiplier As Double)
        If matrices Is Nothing Then Throw New ArgumentException("matrices must not be null")
        If matrices.Count = 0 Then Throw New ArgumentException("matrices must contain at least one matrix")
        _width = width
        _height = height
        _matrices = New List(Of Integer())

        For Each mtr In matrices
            If mtr.Length <> width * height Then Throw New Exception("all matrices must have width*height elements")
            Dim channel(mtr.Length - 1) As Integer

            For i = 0 To mtr.Length - 1
                Dim pixel As Double = mtr(i) * multiplier
                If pixel < 0 Then pixel = 0
                If pixel > 255 Then pixel = 255
                channel(i) = pixel
            Next
            _matrices.Add(channel)
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

    Public Function CloneMatrix(matrix As Integer()) As Integer()
        Dim arr(matrix.Length - 1) As Integer
        Array.Copy(matrix, arr, matrix.Length)
        Return arr
    End Function

    Public Function ResizeMatrixHalf(matrix As Integer()) As Integer()
        Dim result(_width * _height \ 4 - 1) As Integer

        For y = 0 To _height \ 2 - 1
            Dim lineOffset1 = y * 2 * _width
            Dim lineOffset2 = (y * 2 + 1) * _width
            For x = 0 To _width \ 2 - 1
                Dim point As Integer = 0
                point += matrix(x * 2 + lineOffset1)
                point += matrix(x * 2 + 1 + lineOffset1)
                point += matrix(x * 2 + lineOffset2)
                point += matrix(x * 2 + 1 + lineOffset2)
                point \= 4
                result(x + y * _width \ 2) = point
            Next
        Next
        Return result
    End Function

    Public Function ResizeMatrixTwo(matrix As Integer()) As Integer()
        Dim result(_width * _height * 4 - 1) As Integer
        For x = 0 To Width - 1
            For y = 0 To Height - 1
                result(x * 2 + (y * 2) * _width * 2) = matrix(x + y * Width)
                result(x * 2 + 1 + (y * 2) * _width * 2) = matrix(x + y * Width)
                result(x * 2 + (y * 2 + 1) * _width * 2) = matrix(x + y * Width)
                result(x * 2 + 1 + (y * 2 + 1) * _width * 2) = matrix(x + y * Width)
            Next
        Next
        Return result
    End Function

    Public Overridable Function Clone() As CommonMatrix
        Dim list As New List(Of Integer())
        For Each mtr In _matrices
            list.Add(CloneMatrix(mtr))
        Next
        Return New CommonMatrix(list, Width, Height)
    End Function

    Public Overridable Function ResizeTwo() As CommonMatrix
        Dim list As New List(Of Integer())
        For Each mtr In _matrices
            list.Add(ResizeMatrixTwo(mtr))
        Next
        Return New CommonMatrix(list, Width * 2, Height * 2)
    End Function

    Public Overridable Function ResizeHalf() As CommonMatrix
        Dim list As New List(Of Integer())
        For Each mtr In _matrices
            list.Add(ResizeMatrixHalf(mtr))
        Next
        Return New CommonMatrix(list, Width / 2, Height * 2)
    End Function

End Class
