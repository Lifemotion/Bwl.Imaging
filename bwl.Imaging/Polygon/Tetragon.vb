Imports System.Drawing
Imports System.Text

Public Class Tetragon
    Inherits Polygon

    Public Property Point1 As PointF
        Set(value As PointF)
            _points(0) = value
        End Set
        Get
            Return _points(0)
        End Get
    End Property
    Public Property Point2 As PointF
        Set(value As PointF)
            _points(1) = value
        End Set
        Get
            Return _points(1)
        End Get
    End Property

    Public Property Point3 As PointF
        Set(value As PointF)
            _points(2) = value
        End Set
        Get
            Return _points(2)
        End Get
    End Property

    Public Property Point4 As PointF
        Set(value As PointF)
            _points(3) = value
        End Set
        Get
            Return _points(3)
        End Get
    End Property

    Public Overrides Function ToString() As String
        Dim sb = New StringBuilder()
        sb.Append(String.Format("Point1:{0}; ", Point1))
        sb.Append(String.Format("Point2:{0}; ", Point2))
        sb.Append(String.Format("Point3:{0}; ", Point3))
        sb.Append(String.Format("Point4:{0}; ", Point4))
        Return sb.ToString()
    End Function

    Public Sub SetRectangle(left As Single, top As Single, right As Single, bottom As Single)
        Point1 = New PointF(left, top)
        Point2 = New PointF(right, top)
        Point3 = New PointF(right, bottom)
        Point4 = New PointF(left, bottom)
    End Sub

    Public Sub New()
        MyBase.New(True, {New PointF, New PointF, New PointF, New PointF})
    End Sub

    Public Sub New(x1 As Single, y1 As Single, x2 As Single, y2 As Single, x3 As Single, y3 As Single, x4 As Single, y4 As Single)
        MyBase.New(True, New PointF(x1, y1), New PointF(x2, y2), New PointF(x3, y3), New PointF(x4, y4))
    End Sub

    Public Sub New(p1 As PointF, p2 As PointF, p3 As PointF, p4 As PointF)
        MyBase.New(False, p1, p2, p3, p4)
    End Sub

    Public Sub New(x1 As Single, y1 As Single, x2 As Single, y2 As Single)
        Me.New()
        SetRectangle(x1, y1, x2, y2)
    End Sub

End Class
