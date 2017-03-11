Public Class DisplayBitmapControl
    Private _display As DisplayBitmap

    Public Property Bitmap As Bitmap
        Set(value As Bitmap)
            _display = New DisplayBitmap(value)
        End Set
        Get
            Return _display.Bitmap
        End Get
    End Property

    Public Property DisplayBitmap As DisplayBitmap
        Set(value As DisplayBitmap)
            _display = value
        End Set
        Get
            Return _display
        End Get
    End Property

    Public Sub New()
        InitializeComponent()
        DisplayBitmap = New DisplayBitmap(Me.Width, Me.Height)
    End Sub

    Public Overrides Sub Refresh()
        If Object.Equals(_pictureBox.Image, _display.Bitmap) = False Then
            _pictureBox.Image = _display.Bitmap
        End If
        _pictureBox.Refresh()
    End Sub

    Private Sub OverlayDisplay_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If _display Is Nothing Then
            _display = New DisplayBitmap(Me.Width, Me.Height)
        End If
        Me.DoubleBuffered = False
    End Sub

    Private Sub _pictureBox_Resize(sender As Object, e As EventArgs) Handles _pictureBox.Resize
        Try
            If _display IsNot Nothing Then
                _display.Resize(Me.Width, Me.Height)
            End If
        Catch
        End Try
    End Sub
End Class
