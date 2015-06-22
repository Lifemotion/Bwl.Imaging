Public Class BitmapObject

    Public Property RectangleF As RectangleF

    Public Property Bitmap As Bitmap

    Public Sub New()

    End Sub

    Public Sub New(bitmap As Bitmap, rectangle As RectangleF)
        Me.RectangleF = rectangle
        Me.Bitmap = bitmap
    End Sub
    Public Sub New(bitmap As Bitmap, x1 As Single, y1 As Single, x2 As Single, y2 As Single)
        Me.RectangleF = RectangleF.FromLTRB(x1, y1, x2, y2).ToPositiveSized
        Me.Bitmap = bitmap
    End Sub
End Class
