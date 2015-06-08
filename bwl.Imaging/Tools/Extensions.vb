Imports System.Runtime.CompilerServices

Public Module Extensions
    <Extension()>
    Public Function ToDrawingPoint(point As PointF) As System.Drawing.Point
        Return New System.Drawing.Point(point.X, point.Y)
    End Function

    <Extension()>
    Public Function Dist(this As PointF, arg As PointF) As Single
        Return Math.Sqrt((this.X - arg.X) ^ 2 + (this.Y - arg.Y) ^ 2)
    End Function

    <Extension()>
    Public Function ToString(this As PointF) As String
        Return "X: " + this.X.ToString() + " Y: " + this.Y.ToString
    End Function
End Module
