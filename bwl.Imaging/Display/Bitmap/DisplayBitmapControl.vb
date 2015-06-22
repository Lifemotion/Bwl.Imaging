Public Class DisplayBitmapControl
    Private _bitmap As DisplayBitmap
    Private _graphics As Graphics

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub OverlayDisplay_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If DisplayBitmap Is Nothing Then
            DisplayBitmap = New DisplayBitmap(Me.Width, Me.Height)
        End If
        Me.DoubleBuffered = False
        '_graphics = Graphics.FromHwnd(_pictureBox.Handle)
    End Sub

    Public Property DisplayBitmap As DisplayBitmap
        Set(value As DisplayBitmap)
            _bitmap = value
        End Set
        Get
            Return _bitmap
        End Get
    End Property

    Public Overrides Sub Refresh()
        'MyBase.Refresh()
        '_graphics.DrawImage(_bitmap.Bitmap, 0, 0)
        '_graphics.InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
        If Object.Equals(_pictureBox.Image, _bitmap.Bitmap) = False Then _pictureBox.Image = _bitmap.Bitmap
        _pictureBox.Refresh()
    End Sub

    Private Sub _pictureBox_Paint() Handles _pictureBox.Paint
        '_graphics.DrawImage(_bitmap.Bitmap, 0, 0)
        '_pictureBox.Refresh()
    End Sub

    Private Sub _pictureBox_Resize(sender As Object, e As EventArgs) Handles _pictureBox.Resize
        Try
            _bitmap.Resize(Me.Width, Me.Height)
        Catch ex As Exception
        End Try
    End Sub
End Class
