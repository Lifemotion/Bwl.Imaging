Imports System.Runtime.Serialization

Public Class Vector
    Inherits Line

    <IgnoreDataMember>
    Public Property PointFrom As PointF
        Set(value As PointF)
            _points(0) = value
        End Set
        Get
            Return _points(0)
        End Get
    End Property

    <IgnoreDataMember>
    Public Property PointTo As PointF
        Set(value As PointF)
            _points(1) = value
        End Set
        Get
            Return _points(1)
        End Get
    End Property

    <IgnoreDataMember>
    Public Property Vector As PointF
        Set(value As PointF)
            _points(1).X = _points(0).X + value.X
            _points(1).Y = _points(0).Y + value.Y
        End Set
        Get
            Return New PointF(_points(1).X - _points(0).X, _points(1).Y - _points(0).Y)
        End Get
    End Property

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(x1 As Single, y1 As Single, x2 As Single, y2 As Single)
        MyBase.New(x1, y1, x2, y2)
    End Sub

    Public Sub New(p1 As PointF, p2 As PointF)
        MyBase.New(p1, p2)
    End Sub

    Public Overrides Function ToString() As String
        Return "Vector: " + Point1.ToString + " -> " + Point2.ToString
    End Function
End Class
