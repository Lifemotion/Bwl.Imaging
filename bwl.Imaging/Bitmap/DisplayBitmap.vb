Public Class DisplayBitmap
    Inherits DisplayGraphics
    Protected _bitmap As Bitmap

    Public ReadOnly Property Bitmap As Bitmap
        Get
            Return _bitmap
        End Get
    End Property

    Public Sub New(bitmap As Bitmap)
        MyBase.New(Graphics.FromImage(bitmap), bitmap.Width, bitmap.Height)
        _bitmap = bitmap
    End Sub

    Public Sub New(width As Integer, height As Integer)
        Me.New(New Bitmap(width, height))
        If width < 1 Then Throw New ArgumentException("width must be >0")
        If height < 1 Then Throw New ArgumentException("height must be >0")
    End Sub

    Public Sub Resize(width As Integer, height As Integer)
        SyncLock _syncRoot
            _bitmap = New Bitmap(width, height)
            MyBase.SetGraphics(Graphics.FromImage(_bitmap), width, height)
        End SyncLock
    End Sub
End Class
