<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UnsafeTestForm
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
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.PictureBox2 = New System.Windows.Forms.PictureBox()
        Me._btnLoad = New System.Windows.Forms.Button()
        Me._btnSharpen5RGB = New System.Windows.Forms.Button()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PictureBox1
        '
        Me.PictureBox1.Location = New System.Drawing.Point(12, 51)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(380, 337)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBox1.TabIndex = 0
        Me.PictureBox1.TabStop = False
        '
        'PictureBox2
        '
        Me.PictureBox2.Location = New System.Drawing.Point(429, 51)
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.Size = New System.Drawing.Size(380, 337)
        Me.PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBox2.TabIndex = 1
        Me.PictureBox2.TabStop = False
        '
        '_btnLoad
        '
        Me._btnLoad.Location = New System.Drawing.Point(13, 13)
        Me._btnLoad.Name = "_btnLoad"
        Me._btnLoad.Size = New System.Drawing.Size(75, 23)
        Me._btnLoad.TabIndex = 2
        Me._btnLoad.Text = "Load"
        Me._btnLoad.UseVisualStyleBackColor = True
        '
        '_btnSharpen5RGB
        '
        Me._btnSharpen5RGB.Location = New System.Drawing.Point(815, 51)
        Me._btnSharpen5RGB.Name = "_btnSharpen5RGB"
        Me._btnSharpen5RGB.Size = New System.Drawing.Size(84, 23)
        Me._btnSharpen5RGB.TabIndex = 3
        Me._btnSharpen5RGB.Text = "Sharpen5RGB"
        Me._btnSharpen5RGB.UseVisualStyleBackColor = True
        '
        'UnsafeTestForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(906, 400)
        Me.Controls.Add(Me._btnSharpen5RGB)
        Me.Controls.Add(Me._btnLoad)
        Me.Controls.Add(Me.PictureBox2)
        Me.Controls.Add(Me.PictureBox1)
        Me.Name = "UnsafeTestForm"
        Me.Text = "UnsafeTestForm"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents PictureBox1 As Windows.Forms.PictureBox
    Friend WithEvents PictureBox2 As Windows.Forms.PictureBox
    Friend WithEvents _btnLoad As Windows.Forms.Button
    Friend WithEvents _btnSharpen5RGB As Windows.Forms.Button
End Class
