Public Class TestForm2
    Private Sub _drawBkg64Button_Click(sender As Object, e As EventArgs) Handles _drawBkg64Button.Click
        Dim b = New Bitmap(64, 64)
        Dim g = Graphics.FromImage(b)
        Dim clr = Color.FromArgb(128, 128, 128)
        g.Clear(clr)

        OverlayDisplay1.Bitmap = b
        OverlayDisplay1.Refresh()
    End Sub

    Private Sub _drawObjectsButton_Click(sender As Object, e As EventArgs) Handles _drawObjectsButton.Click
        Dim b = New Bitmap(64, 64)
        Dim g = Graphics.FromImage(b)
        Dim clr = Color.FromArgb(128, 128, 128)
        g.Clear(clr)

        DisplayControl1.BackgroundBitmap = b
        DisplayControl1.DisplayObjects.Clear()
        DisplayControl1.DisplayObjects.Add(New DisplayObject("vector1", Color.Violet, New Vector(0.3, 0.3, 0.5, 0.5)))
        DisplayControl1.DisplayObjects.Add(New DisplayObject("rectRed", Color.Red, New RectangleF(0.1, 0.1, 0.1, 0.1)))
        DisplayControl1.DisplayObjects.Add(New DisplayObject("rectGreen", Color.Green, New RectangleF(0.45, 0.45, 0.1, 0.1)))
        DisplayControl1.DisplayObjects.Add(New DisplayObject("rectBlue", Color.Blue, New RectangleF(0.8, 0.1, 0.1, 0.1)))
        DisplayControl1.DisplayObjects.Add(New DisplayObject("redBlock", Color.Black, New BitmapObject(My.Resources.RedBlock, 0.1, 0.45, 0.2, 0.55)))
        DisplayControl1.DisplayObjects.Add(New DisplayObject("greenBlock", Color.Black, New BitmapObject(My.Resources.GreenBlock, 0.45, 0.1, 0.55, 0.2)))
        DisplayControl1.DisplayObjects.Add(New DisplayObject("blueBlock", Color.Black, New BitmapObject(My.Resources.BlueBlock, 0.8, 0.45, 0.9, 0.55)))

        For Each obj In DisplayControl1.DisplayObjects
            obj.IsMoveable = True
        Next
        DisplayControl1.Refresh()
    End Sub

    Private Sub DisplayControl1_DisplayObjectMoved(sender As DisplayObjectsControl, displayObject As DisplayObject) Handles DisplayControl1.DisplayObjectMoved
        DisplayControl2.BackgroundColor = Color.CadetBlue
        DisplayControl2.BackgroundBitmap = Nothing
        DisplayControl2.DisplayObjects.Clear()
        For Each obj In DisplayControl1.DisplayObjects
            obj.IsMoveable = True
            DisplayControl2.DisplayObjects.Add(obj)
        Next
        DisplayControl2.Refresh()

        DisplayControl3.BackgroundColor = Color.Lavender
        DisplayControl3.DisplayObjects.Clear()
        For Each obj In DisplayControl2.DisplayObjects
            obj.IsMoveable = True
            DisplayControl3.DisplayObjects.Add(obj)
        Next
        DisplayControl3.Refresh()
    End Sub

    Private Sub DisplayControl1_MouseClickF(sender As Object, e As Windows.Forms.MouseEventArgs, clickPointF As PointF) Handles DisplayControl1.MouseClickF
        _mouseClickFLabel.Text = String.Format("MouseClickF: {0}; {1}", clickPointF.X.ToString("F2"), clickPointF.Y.ToString("F2"))
        DisplayControl1_DisplayObjectMoved(Nothing, Nothing)
    End Sub

    Private Sub DisplayControl1_MouseClickOnBackgroundF(sender As Object, e As Windows.Forms.MouseEventArgs, clickPointF As PointF) Handles DisplayControl1.MouseClickOnBackgroundF
        _mouseClickOnBackgroundFLabel.Text = String.Format("MouseClickOnBackgroundF: {0}; {1}", clickPointF.X.ToString("F2"), clickPointF.Y.ToString("F2"))
        DisplayControl1_DisplayObjectMoved(Nothing, Nothing)
    End Sub

    Private Sub _refreshButton_Click(sender As Object, e As EventArgs) Handles _refreshButton.Click
        DisplayControl1_DisplayObjectMoved(Nothing, Nothing)
    End Sub
End Class
