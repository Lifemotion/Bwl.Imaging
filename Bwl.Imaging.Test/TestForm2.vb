Public Class TestForm2
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim b = New Bitmap(64, 64)
        Dim g = Graphics.FromImage(b)
        Dim clr = Color.FromArgb(128, 128, 128)
        g.Clear(clr)

        OverlayDisplay1.Bitmap = b
        OverlayDisplay1.Refresh()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim b = New Bitmap(64, 64)
        Dim g = Graphics.FromImage(b)
        Dim clr = Color.FromArgb(128, 128, 128)
        g.Clear(clr)

        DisplayControl1.BackgroundBitmap = b
        DisplayControl1.DisplayObjects.Clear()
        'DisplayControl1.DisplayObjects.Add(New DisplayObject("vector1", Color.Violet, New Vector(0.1, 0.1, 0.9, 0.9)))
        DisplayControl1.DisplayObjects.Add(New DisplayObject("vector2", Color.Violet, New Vector(0.1, 0.1, 0.9, 0.9)))
        For Each obj In DisplayControl1.DisplayObjects
            obj.IsMoveable = True
        Next
        DisplayControl1.Refresh()
    End Sub

    Private Sub DisplayControl1_DisplayObjectMoved(sender As DisplayObjectsControl, displayObject As DisplayObject) Handles DisplayControl1.DisplayObjectMoved
        DisplayControl2.BackgroundBitmap = Nothing
        DisplayControl2.DisplayObjects.Clear()
        For Each obj In DisplayControl1.DisplayObjects
            obj.IsMoveable = True
            DisplayControl2.DisplayObjects.Add(obj)
        Next
        DisplayControl2.Refresh()
    End Sub
End Class