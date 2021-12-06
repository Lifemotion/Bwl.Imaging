Imports System.Drawing
Imports System.Runtime.Serialization

Public Class RectangleFC
    Implements ICloneable

    Public Property RectangleF As RectangleF

    Public Function Clone() As Object Implements ICloneable.Clone
        Return New RectangleFC(RectangleF)
    End Function

    Public Sub New(rect As RectangleF)
        Me.RectangleF = rect
    End Sub

    Public Sub New(rect As Rectangle)
        Me.RectangleF = rect
    End Sub

    Public Sub New(x1 As Single, y1 As Single, x2 As Single, y2 As Single)
        Me.RectangleF = Drawing.RectangleF.FromLTRB(x1, y1, x2, y2)
    End Sub

    Public Sub New(location As PointF, size As SizeF)
        Me.RectangleF = New RectangleF(location, size)
    End Sub

    Public Sub New()

    End Sub

    Public Overrides Function ToString() As String
        Return "RectangleC: " + RectangleF.ToString
    End Function

End Class
