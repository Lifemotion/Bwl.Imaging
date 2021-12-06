
Imports System.Drawing

Public Class TextObject

    Public Property Point1 As PointF
    Public Property Text As String = ""
    Public Property Size As Single = 0.03

    Public Sub New()
    End Sub

    Public Sub New(point1 As PointF, text As String, size As Single)
        Me.Text = text
        Me.Point1 = point1
        Me.Size = size
    End Sub

    Public Sub New(x1 As Single, y1 As Single, text As String, size As Single)
        Me.Point1 = New PointF(x1, y1)
        Me.Text = text
        Me.Size = size
    End Sub
End Class
