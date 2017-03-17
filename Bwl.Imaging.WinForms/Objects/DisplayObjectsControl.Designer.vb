<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DisplayObjectsControl
    Inherits Bwl.Imaging.DisplayBitmapControl

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DisplayObjectsControl))
        Me._selectedObjectID = New System.Windows.Forms.Label()
        Me._clickPointFLabel = New System.Windows.Forms.Label()
        CType(Me._pictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        '_pictureBox
        '
        Me._pictureBox.Size = New System.Drawing.Size(348, 293)
        '
        '_selectedObjectID
        '
        Me._selectedObjectID.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me._selectedObjectID.AutoSize = True
        Me._selectedObjectID.Location = New System.Drawing.Point(1, 298)
        Me._selectedObjectID.Name = "_selectedObjectID"
        Me._selectedObjectID.Size = New System.Drawing.Size(10, 13)
        Me._selectedObjectID.TabIndex = 1
        Me._selectedObjectID.Text = "-"
        '
        '_clickPointFLabel
        '
        Me._clickPointFLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me._clickPointFLabel.AutoSize = True
        Me._clickPointFLabel.Location = New System.Drawing.Point(335, 298)
        Me._clickPointFLabel.Name = "_clickPointFLabel"
        Me._clickPointFLabel.Size = New System.Drawing.Size(10, 13)
        Me._clickPointFLabel.TabIndex = 2
        Me._clickPointFLabel.Text = "-"
        Me._clickPointFLabel.Visible = False
        '
        'DisplayObjectsControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Bitmap = CType(resources.GetObject("$this.Bitmap"), System.Drawing.Bitmap)
        Me.Controls.Add(Me._clickPointFLabel)
        Me.Controls.Add(Me._selectedObjectID)
        Me.Name = "DisplayObjectsControl"
        Me.Size = New System.Drawing.Size(348, 319)
        Me.Controls.SetChildIndex(Me._selectedObjectID, 0)
        Me.Controls.SetChildIndex(Me._pictureBox, 0)
        Me.Controls.SetChildIndex(Me._clickPointFLabel, 0)
        CType(Me._pictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents _selectedObjectID As System.Windows.Forms.Label
    Friend WithEvents _clickPointFLabel As Label
End Class
