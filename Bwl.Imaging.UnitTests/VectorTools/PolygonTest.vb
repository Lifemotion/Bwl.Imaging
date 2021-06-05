Imports System.Drawing

Public Class PolygonTest
    Private Enum EdgeType
        TOUCHING
        CROSSING
        INESSENTIAL
    End Enum

    Private Enum PointOverEdge
        LEFT
        RIGHT
        BETWEEN
        OUTSIDE
    End Enum

    Public Enum PointInPolygonResult
        INSIDE
        OUTSIDE
        BOUNDARY
    End Enum

    Protected _points As PointF()

    Public ReadOnly Property MinX As Single
    Public ReadOnly Property MaxX As Single
    Public ReadOnly Property MinY As Single
    Public ReadOnly Property MaxY As Single
    Public ReadOnly Property Width As Single
    Public ReadOnly Property Height As Single

    Public ReadOnly Property Points As PointF()
        Get
            Return _points
        End Get
    End Property

    Public Sub New(points As IEnumerable(Of PointF))
        If points.Count < 3 Then
            Throw New Exception("Polygon.New(): points.Count < 3")
        End If
        _points = points.ToArray()
        _MinX = _points.Min(Function(item) item.X)
        _MaxX = _points.Max(Function(item) item.X)
        _MinY = _points.Min(Function(item) item.Y)
        _MaxY = _points.Max(Function(item) item.Y)
        _Width = _MaxX - _MinX
        _Height = _MaxY - _MinY
    End Sub

    Public Sub New(points As PointF(), N As Integer)
        Me.New(points.Take(N))
    End Sub

    Public Function PointInPolygonTest(p As PointF) As PointInPolygonResult
        Dim parity = True
        For i = 0 To _points.Length - 1
            Dim v1 = _points(i)
            Dim v2 = _points((i + 1) Mod _points.Length)
            Select Case GetEdgeType(p, v1, v2)
                Case EdgeType.TOUCHING
                    Return PointInPolygonResult.BOUNDARY
                Case EdgeType.CROSSING
                    parity = Not parity
            End Select
        Next
        Return If(parity, PointInPolygonResult.OUTSIDE, PointInPolygonResult.INSIDE)
    End Function

    Private Function GetEdgeType(p As PointF, v1 As PointF, v2 As PointF) As EdgeType
        Select Case Classify(p, v1, v2)
            Case PointOverEdge.LEFT
                Return If((v1.Y < p.Y) AndAlso (p.Y <= v2.Y), EdgeType.CROSSING, EdgeType.INESSENTIAL)
            Case PointOverEdge.RIGHT
                Return If((v2.Y < p.Y) AndAlso (p.Y <= v1.Y), EdgeType.CROSSING, EdgeType.INESSENTIAL)
            Case PointOverEdge.BETWEEN
                Return EdgeType.TOUCHING
            Case Else
                Return EdgeType.INESSENTIAL
        End Select
    End Function

    Private Function Classify(p As PointF, v1 As PointF, v2 As PointF) As PointOverEdge
        'Коэффициенты уравнения прямой
        Dim a = v1.Y - v2.Y
        Dim b = v2.X - v1.X
        Dim c = v1.X * v2.Y - v2.X * v1.Y

        'Подставим точку в уравнение прямой
        Dim f = a * p.X + b * p.Y + c
        If f > 0 Then
            Return PointOverEdge.RIGHT 'Точка лежит справа от отрезка
        End If
        If f < 0 Then
            Return PointOverEdge.LEFT 'Точка лежит слева от отрезка
        End If

        Dim minX = Math.Min(v1.X, v2.X)
        Dim maxX = Math.Max(v1.X, v2.X)
        Dim minY = Math.Min(v1.Y, v2.Y)
        Dim maxY = Math.Max(v1.Y, v2.Y)
        If minX <= p.X AndAlso p.X <= maxX AndAlso minY <= p.Y AndAlso p.Y <= maxY Then
            Return PointOverEdge.BETWEEN 'Точка лежит на отрезке
        End If
        Return PointOverEdge.OUTSIDE 'Точка лежит на прямой, но не на отрезке
    End Function
End Class
