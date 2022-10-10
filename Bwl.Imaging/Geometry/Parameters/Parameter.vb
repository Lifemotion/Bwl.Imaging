Public Class Parameter
    Public Property Key As String = String.Empty
    Public Property Value As String = String.Empty
    Public Property Unit As String = String.Empty
    Public Property Visible As Boolean = True
    Public Property Caption As String = String.Empty
    Public Property AdditionalSettings As String = String.Empty

    Public Sub New()
    End Sub

    Public Sub New(key As String, value As String)
        Me.Key = key
        Me.Value = value
    End Sub

    Public Sub New(key As String, value As String, visible As Boolean)
        Me.Key = key
        Me.Value = value
        Me.Visible = visible
    End Sub

    Public Sub New(key As String, value As String, unit As String, visible As Boolean, caption As String, additionalSettings As String)
        Me.Key = key
        Me.Value = value
        Me.Unit = unit
        Me.Visible = visible
        Me.Caption = caption
        Me.AdditionalSettings = additionalSettings
    End Sub
End Class
