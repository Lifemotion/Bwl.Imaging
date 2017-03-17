<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class TestForm2
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TestForm2))
        Me._drawBkg64Button = New System.Windows.Forms.Button()
        Me._drawObjectsButton = New System.Windows.Forms.Button()
        Me._mouseClickFLabel = New System.Windows.Forms.Label()
        Me._mouseClickOnBackgroundFLabel = New System.Windows.Forms.Label()
        Me.DisplayControl3 = New bwl.Imaging.DisplayObjectsControl()
        Me.DisplayControl2 = New bwl.Imaging.DisplayObjectsControl()
        Me.DisplayControl1 = New bwl.Imaging.DisplayObjectsControl()
        Me.OverlayDisplay1 = New bwl.Imaging.DisplayBitmapControl()
        Me._refreshButton = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        '_drawBkg64Button
        '
        Me._drawBkg64Button.Location = New System.Drawing.Point(12, 430)
        Me._drawBkg64Button.Name = "_drawBkg64Button"
        Me._drawBkg64Button.Size = New System.Drawing.Size(84, 52)
        Me._drawBkg64Button.TabIndex = 2
        Me._drawBkg64Button.Text = "Draw BKG 64"
        Me._drawBkg64Button.UseVisualStyleBackColor = True
        '
        '_drawObjectsButton
        '
        Me._drawObjectsButton.Location = New System.Drawing.Point(102, 430)
        Me._drawObjectsButton.Name = "_drawObjectsButton"
        Me._drawObjectsButton.Size = New System.Drawing.Size(84, 52)
        Me._drawObjectsButton.TabIndex = 5
        Me._drawObjectsButton.Text = "Draw Objects"
        Me._drawObjectsButton.UseVisualStyleBackColor = True
        '
        '_mouseClickFLabel
        '
        Me._mouseClickFLabel.AutoSize = True
        Me._mouseClickFLabel.Location = New System.Drawing.Point(192, 430)
        Me._mouseClickFLabel.Name = "_mouseClickFLabel"
        Me._mouseClickFLabel.Size = New System.Drawing.Size(71, 13)
        Me._mouseClickFLabel.TabIndex = 8
        Me._mouseClickFLabel.Text = "MouseClickF:"
        '
        '_mouseClickOnBackgroundFLabel
        '
        Me._mouseClickOnBackgroundFLabel.AutoSize = True
        Me._mouseClickOnBackgroundFLabel.Location = New System.Drawing.Point(192, 450)
        Me._mouseClickOnBackgroundFLabel.Name = "_mouseClickOnBackgroundFLabel"
        Me._mouseClickOnBackgroundFLabel.Size = New System.Drawing.Size(143, 13)
        Me._mouseClickOnBackgroundFLabel.TabIndex = 9
        Me._mouseClickOnBackgroundFLabel.Text = "MouseClickOnBackgroundF:"
        '
        'DisplayControl3
        '
        Me.DisplayControl3.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DisplayControl3.BackgroundBitmap = Nothing
        Me.DisplayControl3.BackgroundColor = System.Drawing.Color.White
        Me.DisplayControl3.Bitmap = CType(resources.GetObject("DisplayControl3.Bitmap"), System.Drawing.Bitmap)
        Me.DisplayControl3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.DisplayControl3.KeepBackgroundAspectRatio = True
        Me.DisplayControl3.Location = New System.Drawing.Point(12, 488)
        Me.DisplayControl3.MoveMode = False
        Me.DisplayControl3.MoveModePointColor = System.Drawing.Color.Red
        Me.DisplayControl3.MovePoints = CType(resources.GetObject("DisplayControl3.MovePoints"), System.Collections.Generic.List(Of System.Drawing.PointF))
        Me.DisplayControl3.Name = "DisplayControl3"
        Me.DisplayControl3.RedrawObjectsWhenCollectionChanged = True
        Me.DisplayControl3.SelectedObject = Nothing
        Me.DisplayControl3.ShowClickPoint = False
        Me.DisplayControl3.ShowStatusBar = True
        Me.DisplayControl3.Size = New System.Drawing.Size(1153, 114)
        Me.DisplayControl3.TabIndex = 7
        '
        'DisplayControl2
        '
        Me.DisplayControl2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DisplayControl2.BackgroundBitmap = Nothing
        Me.DisplayControl2.BackgroundColor = System.Drawing.Color.White
        Me.DisplayControl2.Bitmap = CType(resources.GetObject("DisplayControl2.Bitmap"), System.Drawing.Bitmap)
        Me.DisplayControl2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.DisplayControl2.KeepBackgroundAspectRatio = True
        Me.DisplayControl2.Location = New System.Drawing.Point(753, 12)
        Me.DisplayControl2.MoveMode = False
        Me.DisplayControl2.MoveModePointColor = System.Drawing.Color.Red
        Me.DisplayControl2.MovePoints = CType(resources.GetObject("DisplayControl2.MovePoints"), System.Collections.Generic.List(Of System.Drawing.PointF))
        Me.DisplayControl2.Name = "DisplayControl2"
        Me.DisplayControl2.RedrawObjectsWhenCollectionChanged = True
        Me.DisplayControl2.SelectedObject = Nothing
        Me.DisplayControl2.ShowClickPoint = False
        Me.DisplayControl2.ShowStatusBar = True
        Me.DisplayControl2.Size = New System.Drawing.Size(412, 412)
        Me.DisplayControl2.TabIndex = 6
        '
        'DisplayControl1
        '
        Me.DisplayControl1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DisplayControl1.BackgroundBitmap = Nothing
        Me.DisplayControl1.BackgroundColor = System.Drawing.Color.LimeGreen
        Me.DisplayControl1.Bitmap = CType(resources.GetObject("DisplayControl1.Bitmap"), System.Drawing.Bitmap)
        Me.DisplayControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.DisplayControl1.KeepBackgroundAspectRatio = True
        Me.DisplayControl1.Location = New System.Drawing.Point(12, 323)
        Me.DisplayControl1.MoveMode = False
        Me.DisplayControl1.MoveModePointColor = System.Drawing.Color.Red
        Me.DisplayControl1.MovePoints = CType(resources.GetObject("DisplayControl1.MovePoints"), System.Collections.Generic.List(Of System.Drawing.PointF))
        Me.DisplayControl1.Name = "DisplayControl1"
        Me.DisplayControl1.RedrawObjectsWhenCollectionChanged = True
        Me.DisplayControl1.SelectedObject = Nothing
        Me.DisplayControl1.ShowClickPoint = True
        Me.DisplayControl1.ShowStatusBar = True
        Me.DisplayControl1.Size = New System.Drawing.Size(735, 101)
        Me.DisplayControl1.TabIndex = 4
        '
        'OverlayDisplay1
        '
        Me.OverlayDisplay1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.OverlayDisplay1.Bitmap = CType(resources.GetObject("OverlayDisplay1.Bitmap"), System.Drawing.Bitmap)
        Me.OverlayDisplay1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.OverlayDisplay1.Location = New System.Drawing.Point(12, 12)
        Me.OverlayDisplay1.Name = "OverlayDisplay1"
        Me.OverlayDisplay1.Size = New System.Drawing.Size(735, 305)
        Me.OverlayDisplay1.TabIndex = 1
        '
        '_refreshButton
        '
        Me._refreshButton.Location = New System.Drawing.Point(663, 430)
        Me._refreshButton.Name = "_refreshButton"
        Me._refreshButton.Size = New System.Drawing.Size(84, 52)
        Me._refreshButton.TabIndex = 10
        Me._refreshButton.Text = "Refresh"
        Me._refreshButton.UseVisualStyleBackColor = True
        '
        'TestForm2
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1177, 637)
        Me.Controls.Add(Me._refreshButton)
        Me.Controls.Add(Me._mouseClickOnBackgroundFLabel)
        Me.Controls.Add(Me._mouseClickFLabel)
        Me.Controls.Add(Me.DisplayControl3)
        Me.Controls.Add(Me.DisplayControl2)
        Me.Controls.Add(Me._drawObjectsButton)
        Me.Controls.Add(Me.DisplayControl1)
        Me.Controls.Add(Me._drawBkg64Button)
        Me.Controls.Add(Me.OverlayDisplay1)
        Me.MaximizeBox = False
        Me.Name = "TestForm2"
        Me.Text = "TestForm2"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents OverlayDisplay1 As DisplayBitmapControl
    Friend WithEvents _drawBkg64Button As Windows.Forms.Button
    Friend WithEvents DisplayControl1 As DisplayObjectsControl
    Friend WithEvents _drawObjectsButton As Windows.Forms.Button
    Friend WithEvents DisplayControl2 As DisplayObjectsControl
    Friend WithEvents DisplayControl3 As DisplayObjectsControl
    Friend WithEvents _mouseClickFLabel As Windows.Forms.Label
    Friend WithEvents _mouseClickOnBackgroundFLabel As Windows.Forms.Label
    Friend WithEvents _refreshButton As Windows.Forms.Button
End Class
