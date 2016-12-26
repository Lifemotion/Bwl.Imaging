<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MoveDetectorTest
    Inherits System.Windows.Forms.Form

    'Форма переопределяет dispose для очистки списка компонентов.
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
        Me.pbFrame = New System.Windows.Forms.PictureBox()
        Me.lbResults = New System.Windows.Forms.ListBox()
        CType(Me.pbFrame, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pbFrame
        '
        Me.pbFrame.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pbFrame.BackColor = System.Drawing.SystemColors.ControlDark
        Me.pbFrame.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pbFrame.Location = New System.Drawing.Point(12, 12)
        Me.pbFrame.Name = "pbFrame"
        Me.pbFrame.Size = New System.Drawing.Size(621, 438)
        Me.pbFrame.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.pbFrame.TabIndex = 0
        Me.pbFrame.TabStop = False
        '
        'lbResults
        '
        Me.lbResults.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lbResults.FormattingEnabled = True
        Me.lbResults.Location = New System.Drawing.Point(12, 456)
        Me.lbResults.Name = "lbResults"
        Me.lbResults.Size = New System.Drawing.Size(621, 160)
        Me.lbResults.TabIndex = 1
        '
        'MoveDetectorTest
        '
        Me.AllowDrop = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(645, 631)
        Me.Controls.Add(Me.lbResults)
        Me.Controls.Add(Me.pbFrame)
        Me.Name = "MoveDetectorTest"
        Me.Text = "MoveDetectorTest"
        CType(Me.pbFrame, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pbFrame As Windows.Forms.PictureBox
    Friend WithEvents lbResults As Windows.Forms.ListBox
End Class
