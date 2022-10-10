Imports System.Drawing

Public Class RegionWithVector
    Inherits Region
    Implements ICloneable

    Public Shared ReadOnly Property VectorMarker As String = "Vector"

    Public ReadOnly Property VectorCaption As String
        Get
            Return Caption + VectorMarker
        End Get
    End Property

    Public ReadOnly Property VectorID As String
        Get
            Return ID + VectorMarker
        End Get
    End Property

    Public Property Vector As New PointF()

    Public Property ImagingVector As Vector
        Get
            Dim point1 = New PointF(Points.Average(Function(item) item.X), Points.Average(Function(item) item.Y))
            Dim point2X = point1.X + Vector.X : point2X = If(point2X < 0, 0, point2X) : point2X = If(point2X > 1, 1, point2X)
            Dim point2Y = point1.Y + Vector.Y : point2Y = If(point2Y < 0, 0, point2Y) : point2Y = If(point2Y > 1, 1, point2Y)
            Dim point2 = New PointF(point2X, point2Y)
            Return New Vector(point1, point2)
        End Get
        Set(value As Vector)
            Vector = New PointF(value.Points(1).X - value.Points(0).X, value.Points(1).Y - value.Points(0).Y)
        End Set
    End Property

    Public Sub New()
        MyBase.New(Guid.NewGuid.ToString(), {New PointF(), New PointF(), New PointF(), New PointF()}) 'Как Tetragon
    End Sub

    Public Sub New(points As PointF())
        MyBase.New(Guid.NewGuid.ToString(), points)
    End Sub

    Public Sub New(id As String)
        MyBase.New(id, {New PointF(), New PointF(), New PointF(), New PointF()}) 'Как Tetragon
    End Sub

    Public Sub New(id As String, points As PointF())
        MyBase.New(id, points) 'Как Tetragon
    End Sub

    Public Overrides Function ToDisplayObjects(Optional fullDisplay As Boolean = True, Optional textSizeF As Single = 0.013,
                                               Optional channelIdxKey As String = "{ChannelIdxKey}") As DisplayObject()
        Dim displayObjects = New List(Of DisplayObject)(MyBase.ToDisplayObjects(fullDisplay, textSizeF, channelIdxKey))
        'Vector
        If Vector.X <> 0 Or Vector.Y <> 0 Then
            Dim displayObjectVector = New DisplayObject(VectorID, Color, ImagingVector) With
            {
                .Caption = Caption + VectorMarker,
                .IsMoveable = True,
                .IsSelectable = True
            }
            displayObjects.Insert(1, displayObjectVector)
        End If
        Return displayObjects.ToArray()
    End Function

    Public Overrides Function Clone() As Object Implements ICloneable.Clone
        Return CloneWithNewPoints(Me.Points)
    End Function

    Public Overloads Function CloneWithNewPoints(points As PointF()) As RegionWithVector
        Dim obj = New RegionWithVector(points) With
        {
            .Caption = Me.Caption,
            .ID = Me.ID,
            .Color = Me.Color,
            .Vector = Me.Vector,
            .Description = Me.Description
        }
        For Each kvp In Me.Parameters.ToArray()
            obj.Parameters.Add(kvp.Key, kvp.Value)
        Next
        Return obj
    End Function

    Public Shared Function GetVectorLines(vector As Vector) As Line()
        Dim lines As New List(Of Line)
        Dim mulX = 1.0
        Dim mulY = 1.0
        Dim x1 = vector.Point1.X
        Dim y1 = vector.Point1.Y
        Dim x2 = vector.Point2.X
        Dim y2 = vector.Point2.Y
        Dim dx = x2 - x1
        Dim dy = y2 - y1
        Dim length = Math.Sqrt(dx ^ 2 + dy ^ 2)
        If length > 0 Then
            Dim angle As Single = CSng(Math.Atan2(dy, dx))
            Dim sz = 0.005
            Dim line1 = New Line(CSng(x1 * mulX + CSng(Math.Cos(angle - Math.PI / 2.0) * sz)), CSng(y1 * mulY + CSng(Math.Sin(angle - Math.PI / 2.0) * sz)),
                                 CSng(x2 * mulX), CSng(y2 * mulY))
            Dim line2 = New Line(CSng(x1 * mulX + CSng(Math.Cos(angle + Math.PI / 2.0) * sz)), CSng(y1 * mulY + CSng(Math.Sin(angle + Math.PI / 2.0) * sz)),
                                 CSng(x2 * mulX), CSng(y2 * mulY))
            Dim line3 = New Line(CSng(x1 * mulX + CSng(Math.Cos(angle - Math.PI / 2.0) * sz)), CSng(y1 * mulY + CSng(Math.Sin(angle - Math.PI / 2.0) * sz)),
                                 CSng(x1 * mulX + CSng(Math.Cos(angle + Math.PI / 2.0) * sz)), CSng(y1 * mulY + CSng(Math.Sin(angle + Math.PI / 2.0) * sz)))
            lines.AddRange({line1, line2, line3})
        End If
        Return lines.ToArray()
    End Function
End Class
