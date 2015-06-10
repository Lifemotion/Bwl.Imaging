Public Class TestForm
    Dim image As Bitmap

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        OverlayDisplay1.DrawBitmap.Clear()
        With OverlayDisplay1.DrawBitmap
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
        OverlayDisplay1.DrawBitmap.Clear()
        With OverlayDisplay1.DrawBitmap
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

            '  .DrawBitmap(image, 0.1, 0.1)
        End With
        OverlayDisplay1.Refresh()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        With DisplayControl1
            Dim rnd As New Random
            .Add(New DisplayObject("img1", Color.Black, New BitmapObject(image, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble)))
            .Add(New DisplayObject("line1", Color.Red, New Line(rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble)))
            .Add(New DisplayObject("line2", Color.Blue, New Line(rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble)))
            .Add(New DisplayObject("rect1", Color.Green, New RectangleF(rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble)))
            .Add(New DisplayObject("point1", Color.Blue, New PointF(rnd.NextDouble, rnd.NextDouble)))
            .Add(New DisplayObject("point2", Color.Red, New PointC(rnd.NextDouble, rnd.NextDouble)))
            .Add(New DisplayObject("test1", Color.Green, New TextObject(rnd.NextDouble, rnd.NextDouble, "test", 0.05)))
            .Add(New DisplayObject("tetra1", Color.BlueViolet, New Tetragon(rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble)))

            ' Dim t = Serializer.SaveObjectToJsonString(DisplayControl1.DisplayObjects)
            ' .Clear()
            ' Dim objects = Serializer.LoadObjectFromJsonString(Of List(Of DisplayObject))(t)
            '.AddRange(objects)
        End With

    End Sub
End Class
