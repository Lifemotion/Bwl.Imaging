Public Class DrawBitmapControl
    Private _overlay As DrawBitmap

    Private Sub OverlayDisplay_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If DrawBitmap Is Nothing Then DrawBitmap = New DrawBitmap(Me.Width, Me.Height)
    End Sub

    Public Property DrawBitmap As DrawBitmap
        Set(value As DrawBitmap)
            _overlay = value
        End Set
        Get
            Return _overlay
        End Get
    End Property

    Public Sub CreateBitmap()
        DrawBitmap = New DrawBitmap(_pictureBox.Width, _pictureBox.Height)
    End Sub

    Public Overrides Sub Refresh()
        MyBase.Refresh()
        _pictureBox.Image = _overlay.Bitmap
        _pictureBox.Refresh()
    End Sub

    Private Sub _pictureBox_Resize(sender As Object, e As EventArgs) Handles _pictureBox.Resize
        Try
            CreateBitmap()
        Catch ex As Exception
        End Try
    End Sub
End Class
