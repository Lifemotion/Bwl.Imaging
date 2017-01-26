Imports System.Windows.Forms

Public Class MoveDetectorTest
    Private md As New MoveDetector

    Private Sub MoveDetectorTest_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AddHandler md.Logger, Sub(type As String, msg As String)
                                  lbResults.Items.Add(type + " " + msg)
                              End Sub
    End Sub

    Private Sub StereoSystemCalibrator_DragOver(sender As Object, e As DragEventArgs) Handles Me.DragOver
        Dim d = e.Data.GetData("FileDrop")
        If d.length = 1 Then e.Effect = DragDropEffects.Copy
    End Sub

    Private Sub StereoSystemCalibrator_DragDrop(sender As Object, e As DragEventArgs) Handles Me.DragDrop
        Dim d = e.Data.GetData("FileDrop")
        If d.length = 1 Then
            Dim file As String = d(0)
            Try
                ReplaceTestFrame(New Bitmap(file))
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
        End If
    End Sub

    Private Sub ReplaceTestFrame(bitmap As Bitmap)
        lbResults.Items.Clear()
        pbFrame.Image = bitmap
        pbFrame.Refresh()
        Dim result = md.Process(bitmap)
        lbResults.Items.Add("Move: " + result.ToString)
    End Sub
End Class