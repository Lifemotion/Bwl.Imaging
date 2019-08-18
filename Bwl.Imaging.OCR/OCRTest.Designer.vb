<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class OCRTest
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
        Me.pbMain = New System.Windows.Forms.PictureBox()
        Me.pbAux1 = New System.Windows.Forms.PictureBox()
        Me.bRecog = New System.Windows.Forms.Button()
        Me.tbFile = New System.Windows.Forms.TextBox()
        Me.pbAux2 = New System.Windows.Forms.PictureBox()
        Me.bRecogSymb = New System.Windows.Forms.Button()
        Me.tbRecogSymb = New System.Windows.Forms.TextBox()
        Me.tbRecogSegment = New System.Windows.Forms.NumericUpDown()
        Me.lbSymbResults = New System.Windows.Forms.ListBox()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.pbResult = New System.Windows.Forms.PictureBox()
        Me.NumericUpDown1 = New System.Windows.Forms.NumericUpDown()
        Me.ComboBox1 = New System.Windows.Forms.ComboBox()
        Me.Button1 = New System.Windows.Forms.Button()
        CType(Me.pbMain, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pbAux1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pbAux2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbRecogSegment, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.pbResult, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumericUpDown1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pbMain
        '
        Me.pbMain.BackColor = System.Drawing.Color.White
        Me.pbMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pbMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pbMain.Location = New System.Drawing.Point(0, 0)
        Me.pbMain.Name = "pbMain"
        Me.pbMain.Size = New System.Drawing.Size(555, 772)
        Me.pbMain.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.pbMain.TabIndex = 0
        Me.pbMain.TabStop = False
        '
        'pbAux1
        '
        Me.pbAux1.BackColor = System.Drawing.Color.Gray
        Me.pbAux1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pbAux1.Location = New System.Drawing.Point(12, 106)
        Me.pbAux1.Name = "pbAux1"
        Me.pbAux1.Size = New System.Drawing.Size(217, 210)
        Me.pbAux1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.pbAux1.TabIndex = 1
        Me.pbAux1.TabStop = False
        '
        'bRecog
        '
        Me.bRecog.Location = New System.Drawing.Point(12, 12)
        Me.bRecog.Name = "bRecog"
        Me.bRecog.Size = New System.Drawing.Size(75, 23)
        Me.bRecog.TabIndex = 2
        Me.bRecog.Text = "Recognize"
        Me.bRecog.UseVisualStyleBackColor = True
        '
        'tbFile
        '
        Me.tbFile.Location = New System.Drawing.Point(12, 80)
        Me.tbFile.Name = "tbFile"
        Me.tbFile.Size = New System.Drawing.Size(217, 20)
        Me.tbFile.TabIndex = 3
        Me.tbFile.Text = "test.jpg"
        '
        'pbAux2
        '
        Me.pbAux2.BackColor = System.Drawing.Color.Gray
        Me.pbAux2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pbAux2.Location = New System.Drawing.Point(12, 322)
        Me.pbAux2.Name = "pbAux2"
        Me.pbAux2.Size = New System.Drawing.Size(217, 210)
        Me.pbAux2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.pbAux2.TabIndex = 4
        Me.pbAux2.TabStop = False
        '
        'bRecogSymb
        '
        Me.bRecogSymb.Location = New System.Drawing.Point(143, 541)
        Me.bRecogSymb.Name = "bRecogSymb"
        Me.bRecogSymb.Size = New System.Drawing.Size(86, 20)
        Me.bRecogSymb.TabIndex = 5
        Me.bRecogSymb.Text = "Recog symb"
        Me.bRecogSymb.UseVisualStyleBackColor = True
        '
        'tbRecogSymb
        '
        Me.tbRecogSymb.Location = New System.Drawing.Point(12, 541)
        Me.tbRecogSymb.Name = "tbRecogSymb"
        Me.tbRecogSymb.Size = New System.Drawing.Size(35, 20)
        Me.tbRecogSymb.TabIndex = 6
        Me.tbRecogSymb.Text = "A"
        '
        'tbRecogSegment
        '
        Me.tbRecogSegment.Location = New System.Drawing.Point(53, 542)
        Me.tbRecogSegment.Name = "tbRecogSegment"
        Me.tbRecogSegment.Size = New System.Drawing.Size(84, 20)
        Me.tbRecogSegment.TabIndex = 7
        '
        'lbSymbResults
        '
        Me.lbSymbResults.FormattingEnabled = True
        Me.lbSymbResults.Location = New System.Drawing.Point(12, 568)
        Me.lbSymbResults.Name = "lbSymbResults"
        Me.lbSymbResults.Size = New System.Drawing.Size(217, 95)
        Me.lbSymbResults.TabIndex = 8
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SplitContainer1.Location = New System.Drawing.Point(235, 12)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.pbMain)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.pbResult)
        Me.SplitContainer1.Size = New System.Drawing.Size(1103, 772)
        Me.SplitContainer1.SplitterDistance = 555
        Me.SplitContainer1.TabIndex = 9
        '
        'pbResult
        '
        Me.pbResult.BackColor = System.Drawing.Color.White
        Me.pbResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pbResult.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pbResult.Location = New System.Drawing.Point(0, 0)
        Me.pbResult.Name = "pbResult"
        Me.pbResult.Size = New System.Drawing.Size(544, 772)
        Me.pbResult.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.pbResult.TabIndex = 1
        Me.pbResult.TabStop = False
        '
        'NumericUpDown1
        '
        Me.NumericUpDown1.Increment = New Decimal(New Integer() {10, 0, 0, 0})
        Me.NumericUpDown1.Location = New System.Drawing.Point(12, 54)
        Me.NumericUpDown1.Maximum = New Decimal(New Integer() {250, 0, 0, 0})
        Me.NumericUpDown1.Minimum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.NumericUpDown1.Name = "NumericUpDown1"
        Me.NumericUpDown1.Size = New System.Drawing.Size(84, 20)
        Me.NumericUpDown1.TabIndex = 10
        Me.NumericUpDown1.Value = New Decimal(New Integer() {200, 0, 0, 0})
        '
        'ComboBox1
        '
        Me.ComboBox1.FormattingEnabled = True
        Me.ComboBox1.Items.AddRange(New Object() {"Arial", "Times New Roman"})
        Me.ComboBox1.Location = New System.Drawing.Point(102, 53)
        Me.ComboBox1.Name = "ComboBox1"
        Me.ComboBox1.Size = New System.Drawing.Size(121, 21)
        Me.ComboBox1.TabIndex = 11
        Me.ComboBox1.Text = "Arial"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(93, 12)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 12
        Me.Button1.Text = "Clipboard"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'OCRTest
        '
        Me.AllowDrop = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1340, 782)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.ComboBox1)
        Me.Controls.Add(Me.NumericUpDown1)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.lbSymbResults)
        Me.Controls.Add(Me.tbRecogSegment)
        Me.Controls.Add(Me.tbRecogSymb)
        Me.Controls.Add(Me.bRecogSymb)
        Me.Controls.Add(Me.pbAux2)
        Me.Controls.Add(Me.tbFile)
        Me.Controls.Add(Me.bRecog)
        Me.Controls.Add(Me.pbAux1)
        Me.Name = "OCRTest"
        Me.Text = "OCR Test"
        CType(Me.pbMain, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pbAux1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pbAux2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbRecogSegment, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        CType(Me.pbResult, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumericUpDown1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents pbMain As PictureBox
    Friend WithEvents pbAux1 As PictureBox
    Friend WithEvents bRecog As Button
    Friend WithEvents tbFile As TextBox
    Friend WithEvents pbAux2 As PictureBox
    Friend WithEvents bRecogSymb As Button
    Friend WithEvents tbRecogSymb As TextBox
    Friend WithEvents tbRecogSegment As NumericUpDown
    Friend WithEvents lbSymbResults As ListBox
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents pbResult As PictureBox
    Friend WithEvents NumericUpDown1 As NumericUpDown
    Friend WithEvents ComboBox1 As ComboBox
    Friend WithEvents Button1 As Button
End Class
