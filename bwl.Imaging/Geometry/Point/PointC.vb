Imports System.Drawing
Imports System.Runtime.Serialization

Public Class PointC
    Implements ICloneable

    Public Property PointF As PointF

    <IgnoreDataMember>
    Public Property X As Single
        Set(value As Single)
            _PointF.X = value
        End Set
        Get
            Return PointF.X
        End Get
    End Property
    <IgnoreDataMember>
    Public Property Y As Single
        Set(value As Single)
            _PointF.Y = value
        End Set
        Get
            Return PointF.Y
        End Get
    End Property

    Public Sub New(p As PointF)
        PointF = p
    End Sub

    Public Sub New(p As Point)
        PointF = p
    End Sub

    Public Sub New(p As PointC)
        PointF = New PointF(p.X, p.Y)
    End Sub

    Public Sub New(x As Single, y As Single)
        PointF = New PointF(x, y)
    End Sub

    Public Sub New()

    End Sub

    Public Sub CopyFrom(point As PointF)
        PointF = point
    End Sub

    Public Sub CopyFrom(point As PointC)
        PointF = New PointF(point.X, point.Y)
    End Sub

    Public Function Clone() As Object Implements ICloneable.Clone
        Return New PointF(X, Y)
    End Function

    Public Function ToPoint() As Point
        Return New Point(CInt(X), CInt(Y))
    End Function

End Class
