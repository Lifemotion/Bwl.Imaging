Imports System.Runtime.Serialization

<DataContractAttribute()>
Public Class Polygon
    <DataMemberAttribute()>
    Protected _points() As PointF
    <DataMemberAttribute()>
    Protected _isClosed As Boolean

    Public ReadOnly Property Points As PointF()
        Get
            Return _points
        End Get
    End Property

    Public ReadOnly Property IsClosed As Boolean
        Get
            Return _isClosed
        End Get
    End Property

    Public Sub New(closed As Boolean, ParamArray points() As PointF)
        If points Is Nothing OrElse points.Length < 2 Then Throw New ArgumentException("points() must have at least 2 elements")
        _isClosed = closed
        _points = points
    End Sub

    Public ReadOnly Property Left
        Get
            Dim min = Single.MaxValue
            For Each pnt In _points
                If pnt.X < min Then min = pnt.X
            Next
            Return min
        End Get
    End Property

    Public ReadOnly Property Top
        Get
            Dim min = Single.MaxValue
            For Each pnt In _points
                If pnt.Y < min Then min = pnt.Y
            Next
            Return min
        End Get
    End Property

    Public ReadOnly Property Right
        Get
            Dim max = Single.MinValue
            For Each pnt In _points
                If pnt.X > max Then max = pnt.X
            Next
            Return max
        End Get
    End Property

    Public ReadOnly Property Bottom
        Get
            Dim max = Single.MinValue
            For Each pnt In _points
                If pnt.Y > max Then max = pnt.Y
            Next
            Return max
        End Get
    End Property

    Public ReadOnly Property Width
        Get
            Return Right - Left
        End Get
    End Property

    Public ReadOnly Property Height
        Get
            Return Bottom - Top
        End Get
    End Property

    Public Function GetBoundRectangleF() As RectangleF
        Return New RectangleF(Left, Top, Width, Height)
    End Function
End Class
