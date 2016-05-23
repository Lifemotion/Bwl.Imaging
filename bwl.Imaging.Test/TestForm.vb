Imports System.Windows.Forms
Imports bwl.Imaging

Public Class TestForm
    Dim image As Bitmap

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        OverlayDisplay1.DisplayBitmap.Clear()
        With OverlayDisplay1.DisplayBitmap
            Dim rnd As New Random
            .DrawVector(Color.Pink, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble)
            .DrawLine(Color.Red, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble)
            .DrawLine(Color.Red, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, 2)
            .DrawRectangle(Color.Blue, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble)
            .DrawRectangle(Color.Blue, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, 2)
            .DrawPoint(Color.Green, rnd.NextDouble, rnd.NextDouble)
            .DrawCircle(Color.Gray, rnd.NextDouble, rnd.NextDouble, 0.04)
            .DrawText(Color.Black, rnd.NextDouble, rnd.NextDouble, 0.02, "text")
            .DrawBitmap(image, 0.1, 0.1, 0.2, 0.2)
        End With
        OverlayDisplay1.Refresh()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        image = new Bitmap(100,100)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        OverlayDisplay1.DisplayBitmap.Clear()
        With OverlayDisplay1.DisplayBitmap
            Dim rnd As New Random
            .DrawObject(Color.Violet, New Vector(rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble))
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
            .Add(New DisplayObject("vector1", Color.Violet, New Vector(rnd.NextDouble, rnd.NextDouble, rnd.NextDouble, rnd.NextDouble)))
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
            .Refresh()
        End With
        For Each obj In DisplayControl1.DisplayObjects
            obj.IsMoveable = True
        Next

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

    Private Sub DisplayControl1_ObjectSelect(sender As Object, selected As DisplayObject, e As MouseEventArgs) Handles DisplayControl1.ObjectSelect

    End Sub

    Private Sub DisplayControl1_DisplayObjectMoved(sender As DisplayObjectsControl, displayObject As DisplayObject) Handles DisplayControl1.DisplayObjectMoved

    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        OverlayDisplay1.DisplayBitmap.Clear()
        With OverlayDisplay1.DisplayBitmap
            .DrawBitmap(image, 0.1, 0.1, 0.2, 0.8)
            .DrawBitmap(image.BitmapToGrayMatrix.ToBitmap, 0.3, 0.1, 0.4, 0.8)
            .DrawBitmap(image.BitmapToRgbMatrix.ToBitmap, 0.5, 0.1, 0.6, 0.8)

            Dim byteRgb = image.BitmapToRgbMatrix
            Dim floatRgb = New RGBFloatMatrix(byteRgb.Red, byteRgb.Green, byteRgb.Blue, 1 / 256)
            Dim newByteRgb = New RGBMatrix(floatRgb.Red, floatRgb.Green, floatRgb.Blue, 512)

            Dim byteGray = image.BitmapToGrayMatrix
            Dim floatGray = New GrayFloatMatrix(byteGray.Gray, 1 / 256)
            Dim newByteGray = New GrayMatrix(floatGray.Gray, 512)

            .DrawBitmap(newByteRgb.ToBitmap, 0.7, 0.1, 0.8, 0.8)
            .DrawBitmap(newByteGray.ToBitmap, 0.9, 0.1, 0.99, 0.8)

            Dim redmat = New RGBMatrix(4, 1)
            Dim greenmat = New RGBMatrix(4, 1)
            Dim bluemat = New RGBMatrix(4, 1)
            redmat.Red(0, 0) = 255
            greenmat.Green(0, 0) = 255
            bluemat.Blue(0, 0) = 255
            .DrawBitmap(redmat.ToBitmap, 0.1, 0.9, 0.2, 0.95)
            .DrawBitmap(greenmat.ToBitmap, 0.2, 0.9, 0.3, 0.95)
            .DrawBitmap(bluemat.ToBitmap, 0.3, 0.9, 0.4, 0.95)
            .DrawBitmap(redmat.ToGrayMatrix.ToBitmap, 0.4, 0.9, 0.5, 0.95)
            .DrawBitmap(greenmat.ToGrayMatrix.ToRGBMatrix.ToBitmap, 0.5, 0.9, 0.6, 0.95)

            Dim rows = 20
            Dim cols = 20
            Dim rgbResult As New RGBMatrix(cols, rows)
            '-------------------------------------------------------------------------
            For i = 0 To rows - 1
                For j = 0 To cols - 1
                    rgbResult.Red(j, i) = 255.0
                    rgbResult.Green(j, i) = 0
                    rgbResult.Blue(j, i) = 0
                Next
            Next
            .DrawBitmap(rgbResult.ToBitmap, 0.6, 0.9, 0.7, 0.95)

        End With
        OverlayDisplay1.Refresh()
    End Sub
End Class
