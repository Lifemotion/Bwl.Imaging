Imports System.Windows.Forms
Imports bwl.Imaging.Unsafe

Public Class UnsafeTestForm
    Private Sub _btnLoad_Click(sender As Object, e As EventArgs) Handles _btnLoad.Click
        Using dialog As New OpenFileDialog
            If dialog.ShowDialog = DialogResult.OK Then
                PictureBox1.Image = New Bitmap(dialog.FileName)
                PictureBox1.Refresh()
            End If
        End Using
    End Sub

    Private Sub _btnSharpen5RGB_Click(sender As Object, e As EventArgs) Handles _btnSharpen5RGB.Click
        Dim sharpened = UnsafeFunctions.Sharpen5Rgb(PictureBox1.Image)
        PictureBox2.Image = sharpened
        PictureBox2.Refresh()
    End Sub
End Class