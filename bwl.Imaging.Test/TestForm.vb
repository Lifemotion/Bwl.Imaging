Public Class TestForm
    Dim image As Bitmap

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        OverlayDisplay1.DisplayBitmap.Clear()
        With OverlayDisplay1.DisplayBitmap
            Dim rnd As New Random
            .DrawLine(Color.Red, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble)
            .DrawLine(Color.Red, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, 2)
            .DrawRectangle(Color.Blue, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble)
            .DrawRectangle(Color.Blue, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, 2)
            .DrawPoint(Color.Green, rnd.NextDouble, rnd.NextDouble)
            .DrawCircle(Color.Gray, rnd.NextDouble, rnd.NextDouble, 0.04)
            .DrawText(Color.Black, rnd.NextDouble, rnd.NextDouble, 0.02, "text")
            .DrawBitmap(image, 0.1, 0.1, 0.9, 0.9)
        End With
        OverlayDisplay1.Refresh()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        image = Bitmap.FromFile("mOABkQ1wC1Q.jpg")
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        OverlayDisplay1.DisplayBitmap.Clear()
        With OverlayDisplay1.DisplayBitmap
            Dim rnd As New Random
            .DrawObject(Color.Red, New Line(rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble))
            .DrawObject(Color.Red, New Line(rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble), 2)
            .DrawObject(Color.Blue, New RectangleF(rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble))
            .DrawObject(Color.Blue, New Rectangle(rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble), 2)
            .DrawObject(Color.Red, New PointF(rnd.NextDouble, rnd.NextDouble), 2)
            .DrawObject(Color.Green, New Point(rnd.NextDouble, rnd.NextDouble))
            .DrawObject(Color.Green, New PointF(rnd.NextDouble, rnd.NextDouble))
            .DrawObject(Color.Green, New PointF(rnd.NextDouble, rnd.NextDouble))
            .DrawObject(Color.Green, New TextObject(rnd.NextDouble, rnd.NextDouble, "test", 0.05))
            .DrawObject(Color.BlueViolet, New Tetragon(rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble))
            .DrawObject(Color.BlueViolet, New Tetragon(rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble))
        End With
        OverlayDisplay1.Refresh()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        With DisplayControl1
            Dim rnd As New Random
            .RedrawObjectsWhenCollectionChanged = False
            .Add(New DisplayObject("img1", Color.Black, New BitmapObject(image, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble)))
            .Add(New DisplayObject("line1", Color.Red, New Line(rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble)))
            .Add(New DisplayObject("line2", Color.Blue, New Line(rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble)))
            .Add(New DisplayObject("rect1", Color.Green, New RectangleF(rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble)))
            .Add(New DisplayObject("point1", Color.Blue, New PointF(rnd.NextDouble, rnd.NextDouble)))
            .Add(New DisplayObject("point2", Color.Red, New PointC(rnd.NextDouble, rnd.NextDouble)))
            .Add(New DisplayObject("test1", Color.Green, New TextObject(rnd.NextDouble, rnd.NextDouble, "test", 0.05)))
            .Add(New DisplayObject("tetra1", Color.BlueViolet, New Tetragon(rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble)))
            Dim objs1 = .Find("", "")
            Dim objs2 = .Find("2", "")
            Dim objs3 = .Find("", "back")
            .Find("2", "").All(Function(obj As DisplayObject)
                                   obj.IsMoveable = True
                                   Return False
                               End Function)
            .Refresh()
        End With

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim imgs(20) As Bitmap
        For i = 1 To 20
            '     Dim b As New Bitmap(DisplayControl1._pictureBox.Width, DisplayControl1._pictureBox.Height)
            Dim b As New Bitmap(10, 20)

            Dim g = Graphics.FromImage(b)
            Dim clr = Color.FromArgb(i * 1, i * 1.5, i * 2)
            g.Clear(clr)
            imgs(i) = b
        Next
        Dim t = Now
        For j = 1 To 20
            '    DisplayControl1._pictureBox.Image = imgs(j)
            '   DisplayControl1._pictureBox.Refresh()
            DisplayControl1.BackgroundBitmap = imgs(j)
            DisplayControl1.Refresh()

        Next
        Dim s = (Now - t).TotalMilliseconds.ToString("0.0")
        MsgBox(s)
    End Sub
End Class
