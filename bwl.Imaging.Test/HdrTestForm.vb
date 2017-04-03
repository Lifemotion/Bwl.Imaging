Imports bwl.Framework
Imports bwl.Imaging
Imports bwl.Imaging.Unsafe

Public Class HdrTestForm
    Inherits FormAppBase
    Private _frame As RawIntFrame

    Private Sub TbBitOffset_Scroll(sender As Object, e As EventArgs) Handles TbBitOffset.Scroll
        Dim mtr = _frame.ConvertTo8Bit(TbBitOffset.Value)
        PbFrame.Image = mtr.ToBitmap
        PbFrame.Refresh()
    End Sub

    Private Sub IDS12BitTestForm_DragOver(sender As Object, e As DragEventArgs) Handles Me.DragOver
        e.Effect = DragDropEffects.Copy
    End Sub

    Private Sub IDS12BitTestForm_DragDrop(sender As Object, e As DragEventArgs) Handles Me.DragDrop
        Dim fname = e.Data.GetData("FileNameW")(0)
        Try
            If IO.Path.GetExtension(fname.tolower) = ".raw" Then
                _frame = RawIntFrame.FromLegacyFile(fname)
            Else
                _frame = RawIntFrame.FromFile(fname)
            End If
            TbBitOffset_Scroll(Nothing, Nothing)
            _logger.AddMessage("Frame Loaded: " + fname)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub bCombine1_Click(sender As Object, e As EventArgs) Handles BtnHdrCombine1.Click
        Dim time = Now
        Dim mtr = _frame.ConvertHDR1(TbBitOffset.Value)
        Dim bmp = mtr.ToBitmap
        _logger.AddMessage("HDR1: " + (Now - time).TotalMilliseconds.ToString("0.00") + " ms")
        PbFrame.Image = bmp
        PbFrame.Refresh()
    End Sub

    Private Sub BtnNoHdr_Click(sender As Object, e As EventArgs) Handles BtnNoHdr.Click
        TbBitOffset_Scroll(Nothing, Nothing)
    End Sub

    Private Sub BtnHdrCombine2_Click(sender As Object, e As EventArgs) Handles BtnHdrCombine2.Click
        Dim mtr = _frame.ConvertHDR2(TbBitOffset.Value)
        PbFrame.Image = mtr.ToBitmap
        PbFrame.Refresh()
    End Sub

    Private Sub BtnHdrCombine3_Click(sender As Object, e As EventArgs) Handles BtnHdrCombine3.Click
        Dim mtr = _frame.ConvertHDR3(TbBitOffset.Value)
        PbFrame.Image = mtr.ToBitmap
        PbFrame.Refresh()
    End Sub

    Private Sub BtnHdrCombine1Unsafe_Click(sender As Object, e As EventArgs) Handles BtnHdrCombine1Unsafe.Click
        Dim time = Now
        Dim bmp = RawFrameFunctions.ConvertRawToHDRBitmap1(_frame.Data, _frame.Width, _frame.Height, TbBitOffset.Value)
        _logger.AddMessage("HDR1 Unsafe: " + (Now - time).TotalMilliseconds.ToString("0.00") + " ms")
        PbFrame.Image = bmp
        PbFrame.Refresh()
    End Sub

    Private Sub bSave_Click(sender As Object, e As EventArgs) Handles bSave.Click
        _frame.Save(IO.Path.Combine(AppBase.DataFolder, Now.Ticks.ToString + ".fraw"))
    End Sub

    Private Sub HdrTestForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub bJpegSave_Click(sender As Object, e As EventArgs) Handles bJpegSave.Click
        Dim fname = IO.Path.Combine(AppBase.DataFolder, Now.Ticks.ToString)
        _frame.SaveToJpegPair(fname)
        _frame = RawIntFrame.FromJpegPair(fname)
        TbBitOffset_Scroll(Nothing, Nothing)
    End Sub
End Class