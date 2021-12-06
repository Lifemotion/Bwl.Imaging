Imports System.Drawing
Imports System.Runtime.Serialization

<DataContractAttribute()>
Public Class Line
    Inherits Polygon

    <IgnoreDataMember>
    Public Property Point1 As PointF
        Set(value As PointF)
            _points(0) = value
        End Set
        Get
            Return _points(0)
        End Get
    End Property

    <IgnoreDataMember>
    Public Property Point2 As PointF
        Set(value As PointF)
            _points(1) = value
        End Set
        Get
            Return _points(1)
        End Get
    End Property

    Public Sub New()
        MyBase.New(False, New PointF, New PointF)
    End Sub

    Public Sub New(x1 As Single, y1 As Single, x2 As Single, y2 As Single)
        MyBase.New(False, New PointF(x1, y1), New PointF(x2, y2))
    End Sub

    Public Sub New(p1 As PointF, p2 As PointF)
        MyBase.New(False, p1, p2)
    End Sub

    Public Overrides Function ToString() As String
        Return "Line: " + Point1.ToString + " - " + Point2.ToString
    End Function
End Class
