<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class HdrTestForm

    'Форма переопределяет dispose для очистки списка компонентов.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.PbFrame = New System.Windows.Forms.PictureBox()
        Me.TbBitOffset = New System.Windows.Forms.TrackBar()
        Me.BtnHdrCombine1 = New System.Windows.Forms.Button()
        Me.BtnHdrCombine2 = New System.Windows.Forms.Button()
        Me.BtnNoHdr = New System.Windows.Forms.Button()
        Me.BtnHdrCombine3 = New System.Windows.Forms.Button()
        Me.BtnHdrCombine1Unsafe = New System.Windows.Forms.Button()
        CType(Me.PbFrame, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TbBitOffset, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'logWriter
        '
        Me.logWriter.Location = New System.Drawing.Point(0, 526)
        Me.logWriter.Size = New System.Drawing.Size(1078, 179)
        '
        'PbFrame
        '
        Me.PbFrame.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PbFrame.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PbFrame.Location = New System.Drawing.Point(12, 68)
        Me.PbFrame.Name = "PbFrame"
        Me.PbFrame.Size = New System.Drawing.Size(1054, 455)
        Me.PbFrame.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PbFrame.TabIndex = 4
        Me.PbFrame.TabStop = False
        '
        'TbBitOffset
        '
        Me.TbBitOffset.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TbBitOffset.Location = New System.Drawing.Point(893, 28)
        Me.TbBitOffset.Maximum = 6
        Me.TbBitOffset.Name = "TbBitOffset"
        Me.TbBitOffset.Size = New System.Drawing.Size(173, 45)
        Me.TbBitOffset.TabIndex = 5
        Me.TbBitOffset.Value = 4
        '
        'BtnHdrCombine1
        '
        Me.BtnHdrCombine1.Location = New System.Drawing.Point(133, 26)
        Me.BtnHdrCombine1.Name = "BtnHdrCombine1"
        Me.BtnHdrCombine1.Size = New System.Drawing.Size(115, 23)
        Me.BtnHdrCombine1.TabIndex = 13
        Me.BtnHdrCombine1.Text = "Combine 1"
        Me.BtnHdrCombine1.UseVisualStyleBackColor = True
        '
        'BtnHdrCombine2
        '
        Me.BtnHdrCombine2.Location = New System.Drawing.Point(254, 26)
        Me.BtnHdrCombine2.Name = "BtnHdrCombine2"
        Me.BtnHdrCombine2.Size = New System.Drawing.Size(115, 23)
        Me.BtnHdrCombine2.TabIndex = 14
        Me.BtnHdrCombine2.Text = "Combine 2"
        Me.BtnHdrCombine2.UseVisualStyleBackColor = True
        '
        'BtnNoHdr
        '
        Me.BtnNoHdr.Location = New System.Drawing.Point(12, 26)
        Me.BtnNoHdr.Name = "BtnNoHdr"
        Me.BtnNoHdr.Size = New System.Drawing.Size(115, 23)
        Me.BtnNoHdr.TabIndex = 15
        Me.BtnNoHdr.Text = "No HDR"
        Me.BtnNoHdr.UseVisualStyleBackColor = True
        '
        'BtnHdrCombine3
        '
        Me.BtnHdrCombine3.Location = New System.Drawing.Point(375, 26)
        Me.BtnHdrCombine3.Name = "BtnHdrCombine3"
        Me.BtnHdrCombine3.Size = New System.Drawing.Size(115, 23)
        Me.BtnHdrCombine3.TabIndex = 16
        Me.BtnHdrCombine3.Text = "Combine 3"
        Me.BtnHdrCombine3.UseVisualStyleBackColor = True
        '
        'BtnHdrCombine1Unsafe
        '
        Me.BtnHdrCombine1Unsafe.Location = New System.Drawing.Point(496, 26)
        Me.BtnHdrCombine1Unsafe.Name = "BtnHdrCombine1Unsafe"
        Me.BtnHdrCombine1Unsafe.Size = New System.Drawing.Size(115, 23)
        Me.BtnHdrCombine1Unsafe.TabIndex = 17
        Me.BtnHdrCombine1Unsafe.Text = "Combine 1 Unsafe"
        Me.BtnHdrCombine1Unsafe.UseVisualStyleBackColor = True
        '
        'HdrTestForm
        '
        Me.AllowDrop = True
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1078, 702)
        Me.Controls.Add(Me.BtnHdrCombine1Unsafe)
        Me.Controls.Add(Me.BtnHdrCombine3)
        Me.Controls.Add(Me.BtnNoHdr)
        Me.Controls.Add(Me.BtnHdrCombine2)
        Me.Controls.Add(Me.BtnHdrCombine1)
        Me.Controls.Add(Me.PbFrame)
        Me.Controls.Add(Me.TbBitOffset)
        Me.Name = "HdrTestForm"
        Me.Text = "HdrTestForm"
        Me.Controls.SetChildIndex(Me.logWriter, 0)
        Me.Controls.SetChildIndex(Me.TbBitOffset, 0)
        Me.Controls.SetChildIndex(Me.PbFrame, 0)
        Me.Controls.SetChildIndex(Me.BtnHdrCombine1, 0)
        Me.Controls.SetChildIndex(Me.BtnHdrCombine2, 0)
        Me.Controls.SetChildIndex(Me.BtnNoHdr, 0)
        Me.Controls.SetChildIndex(Me.BtnHdrCombine3, 0)
        Me.Controls.SetChildIndex(Me.BtnHdrCombine1Unsafe, 0)
        CType(Me.PbFrame, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TbBitOffset, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents PbFrame As PictureBox
    Friend WithEvents TbBitOffset As TrackBar
    Friend WithEvents BtnHdrCombine1 As Button
    Friend WithEvents BtnHdrCombine2 As Button
    Friend WithEvents BtnNoHdr As Button
    Friend WithEvents BtnHdrCombine3 As Button
    Friend WithEvents BtnHdrCombine1Unsafe As Button
End Class
