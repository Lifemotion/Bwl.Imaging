Imports System.Drawing
Imports System.Runtime.CompilerServices

Public Module Extensions
    <Extension()>
    Public Function ToPoint(pointF As PointF) As System.Drawing.Point
        Return New System.Drawing.Point(CInt(pointF.X), CInt(pointF.Y))
    End Function

    <Extension()>
    Public Function ToPointF(point As Point) As System.Drawing.PointF
        Return New System.Drawing.PointF(point.X, point.Y)
    End Function

    <Extension()>
    Public Function Dist(this As PointF, arg As PointF) As Single
        Return CSng(Math.Sqrt((this.X - arg.X) ^ 2 + (this.Y - arg.Y) ^ 2))
    End Function

    <Extension()>
    Public Function ToString(this As PointF) As String
        Return "X: " + this.X.ToString() + " Y: " + this.Y.ToString
    End Function

    <Extension()>
    Public Function ToPositiveSized(this As RectangleF) As RectangleF
        Dim left = Math.Min(this.Left, this.Right)
        Dim right = Math.Max(this.Left, this.Right)
        Dim top = Math.Min(this.Top, this.Bottom)
        Dim bottom = Math.Max(this.Top, this.Bottom)
        Return RectangleF.FromLTRB(left, top, right, bottom)
    End Function

    <Extension()>
    Public Function ToPositiveSized(this As Rectangle) As Rectangle
        Dim left = Math.Min(this.Left, this.Right)
        Dim right = Math.Max(this.Left, this.Right)
        Dim top = Math.Min(this.Top, this.Bottom)
        Dim bottom = Math.Max(this.Top, this.Bottom)
        Return Rectangle.FromLTRB(left, top, right, bottom)
    End Function
End Module
