﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
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
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me._mouseClickFLabel = New System.Windows.Forms.Label()
        Me.DisplayControl3 = New bwl.Imaging.DisplayObjectsControl()
        Me.DisplayControl2 = New bwl.Imaging.DisplayObjectsControl()
        Me.DisplayControl1 = New bwl.Imaging.DisplayObjectsControl()
        Me.OverlayDisplay1 = New bwl.Imaging.DisplayBitmapControl()
        Me._mouseClickOnBackgroundFLabel = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(12, 430)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(84, 52)
        Me.Button1.TabIndex = 2
        Me.Button1.Text = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(102, 430)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(84, 52)
        Me.Button2.TabIndex = 5
        Me.Button2.Text = "Button2"
        Me.Button2.UseVisualStyleBackColor = True
        '
        '_mouseClickFLabel
        '
        Me._mouseClickFLabel.AutoSize = True
        Me._mouseClickFLabel.Location = New System.Drawing.Point(188, 404)
        Me._mouseClickFLabel.Name = "_mouseClickFLabel"
        Me._mouseClickFLabel.Size = New System.Drawing.Size(71, 13)
        Me._mouseClickFLabel.TabIndex = 8
        Me._mouseClickFLabel.Text = "MouseClickF:"
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
        Me.DisplayControl3.ShowStatusBar = True
        Me.DisplayControl3.Size = New System.Drawing.Size(722, 114)
        Me.DisplayControl3.TabIndex = 7
        '
        'DisplayControl2
        '
        Me.DisplayControl2.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DisplayControl2.BackgroundBitmap = Nothing
        Me.DisplayControl2.BackgroundColor = System.Drawing.Color.White
        Me.DisplayControl2.Bitmap = CType(resources.GetObject("DisplayControl2.Bitmap"), System.Drawing.Bitmap)
        Me.DisplayControl2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.DisplayControl2.KeepBackgroundAspectRatio = True
        Me.DisplayControl2.Location = New System.Drawing.Point(739, 12)
        Me.DisplayControl2.MoveMode = False
        Me.DisplayControl2.MoveModePointColor = System.Drawing.Color.Red
        Me.DisplayControl2.MovePoints = CType(resources.GetObject("DisplayControl2.MovePoints"), System.Collections.Generic.List(Of System.Drawing.PointF))
        Me.DisplayControl2.Name = "DisplayControl2"
        Me.DisplayControl2.RedrawObjectsWhenCollectionChanged = True
        Me.DisplayControl2.SelectedObject = Nothing
        Me.DisplayControl2.ShowStatusBar = True
        Me.DisplayControl2.Size = New System.Drawing.Size(148, 412)
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
        Me.DisplayControl1.ShowStatusBar = True
        Me.DisplayControl1.Size = New System.Drawing.Size(722, 101)
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
        Me.OverlayDisplay1.Size = New System.Drawing.Size(722, 305)
        Me.OverlayDisplay1.TabIndex = 1
        '
        '_mouseClickOnBackgroundFLabel
        '
        Me._mouseClickOnBackgroundFLabel.AutoSize = True
        Me._mouseClickOnBackgroundFLabel.Location = New System.Drawing.Point(356, 404)
        Me._mouseClickOnBackgroundFLabel.Name = "_mouseClickOnBackgroundFLabel"
        Me._mouseClickOnBackgroundFLabel.Size = New System.Drawing.Size(143, 13)
        Me._mouseClickOnBackgroundFLabel.TabIndex = 9
        Me._mouseClickOnBackgroundFLabel.Text = "MouseClickOnBackgroundF:"
        '
        'TestForm2
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1143, 637)
        Me.Controls.Add(Me._mouseClickOnBackgroundFLabel)
        Me.Controls.Add(Me._mouseClickFLabel)
        Me.Controls.Add(Me.DisplayControl3)
        Me.Controls.Add(Me.DisplayControl2)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.DisplayControl1)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.OverlayDisplay1)
        Me.MaximizeBox = False
        Me.Name = "TestForm2"
        Me.Text = "TestForm2"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents OverlayDisplay1 As DisplayBitmapControl
    Friend WithEvents Button1 As Windows.Forms.Button
    Friend WithEvents DisplayControl1 As DisplayObjectsControl
    Friend WithEvents Button2 As Windows.Forms.Button
    Friend WithEvents DisplayControl2 As DisplayObjectsControl
    Friend WithEvents DisplayControl3 As DisplayObjectsControl
    Friend WithEvents _mouseClickFLabel As Windows.Forms.Label
    Friend WithEvents _mouseClickOnBackgroundFLabel As Windows.Forms.Label
End Class
