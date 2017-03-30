Imports System.Windows.Forms

Public Class UnsafeTestForm
    Private _bmpSrc As Bitmap

    Private Sub _btnLoad_Click(sender As Object, e As EventArgs) Handles _btnLoad.Click
        Using dialog As New OpenFileDialog
            If dialog.ShowDialog = DialogResult.OK Then
                _bmpSrc = New Bitmap(dialog.FileName)
                PictureBox1.Image = _bmpSrc
                PictureBox1.Invalidate()
            End If
        End Using
    End Sub
End Class