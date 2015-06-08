<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DrawBitmapControl
    Inherits System.Windows.Forms.UserControl

    'Пользовательский элемент управления (UserControl) переопределяет метод Dispose для очистки списка компонентов.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Является обязательной для конструктора форм Windows Forms
    Private components As System.ComponentModel.IContainer

    'Примечание: следующая процедура является обязательной для конструктора форм Windows Forms
    'Для ее изменения используйте конструктор форм Windows Form.  
    'Не изменяйте ее в редакторе исходного кода.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me._pictureBox = New System.Windows.Forms.PictureBox()
        CType(Me._pictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        '_pictureBox
        '
        Me._pictureBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me._pictureBox.BackColor = System.Drawing.Color.White
        Me._pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me._pictureBox.Location = New System.Drawing.Point(0, 0)
        Me._pictureBox.Name = "_pictureBox"
        Me._pictureBox.Size = New System.Drawing.Size(367, 311)
        Me._pictureBox.TabIndex = 0
        Me._pictureBox.TabStop = False
        '
        'DrawBitmapControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me._pictureBox)
        Me.Name = "DrawBitmapControl"
        Me.Size = New System.Drawing.Size(367, 311)
        CType(Me._pictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Protected WithEvents _pictureBox As System.Windows.Forms.PictureBox

End Class
