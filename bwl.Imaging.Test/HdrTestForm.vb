Imports bwl.Framework
Imports bwl.Imaging
Imports bwl.Imaging.Unsafe

Public Class HdrTestForm
    Inherits FormAppBase
    Private _frame As RawIntFrame

    Private Sub ComparePerfomance(slowAction As Action, fastAction As Action, title As String)
        Dim sw1 = New System.Diagnostics.Stopwatch()
        Dim sw2 = New System.Diagnostics.Stopwatch()
        sw1.Start() : slowAction() : sw1.Stop()
        sw2.Start() : fastAction() : sw2.Stop()
        Dim fasterX = sw1.ElapsedTicks / sw2.ElapsedTicks
        _logger.AddMessage(String.Format("{0}: {1} ms, {2} ms, {3} X",
                                         title,
                                         sw1.ElapsedMilliseconds.ToString("F2"),
                                         sw2.ElapsedMilliseconds.ToString("F2"),
                                         fasterX.ToString("F2")))
    End Sub

    Private Sub CompareResults(m1 As RGBMatrix, m2 As RGBMatrix)
        Parallel.For(0, 3, Sub(channel As Integer)
                               Dim matrix1 = m1.Matrix(channel)
                               Dim matrix2 = m2.Matrix(channel)
                               For i = 0 To matrix1.Length - 1
                                   If matrix1(i) <> matrix2(i) Then
                                       Throw New Exception("m1 <> m2")
                                   End If
                               Next
                           End Sub)
    End Sub

    Private Sub IDS12BitTestForm_DragOver(sender As Object, e As DragEventArgs) Handles Me.DragOver
        e.Effect = DragDropEffects.Copy
    End Sub

    Private Sub IDS12BitTestForm_DragDrop(sender As Object, e As DragEventArgs) Handles Me.DragDrop
        Dim fname = e.Data.GetData("FileNameW")(0)
        Dim loaded = False

        Try
            _frame = RawIntFrame.FromLegacyFile(fname)
            loaded = True
        Catch
        End Try

        If Not loaded Then
            Try
                _frame = RawIntFrame.FromFile(fname)
                loaded = True
            Catch
            End Try
        End If

        If loaded Then
            TbBitOffset_Scroll(Nothing, Nothing)
            _logger.AddMessage("Frame Loaded: " + fname)
        Else
            _logger.AddMessage("Frame was not loaded: " + fname)
        End If
    End Sub

    Private Sub TbBitOffset_Scroll(sender As Object, e As EventArgs) Handles TbBitOffset.Scroll
        Dim mtr1 As RGBMatrix = Nothing
        Dim mtr2 As RGBMatrix = Nothing
        ComparePerfomance(Sub() mtr1 = _frame.ConvertTo8Bit(TbBitOffset.Value),
                          Sub() mtr2 = _frame.ConvertTo8BitFast(TbBitOffset.Value),
                "ConvertTo8Bit")
        CompareResults(mtr1, mtr2)
        PbFrame.Image = mtr2.ToBitmap()
        PbFrame.Refresh()
    End Sub

    Private Sub BtnNoHdr_Click(sender As Object, e As EventArgs) Handles BtnNoHdr.Click
        TbBitOffset_Scroll(Nothing, Nothing)
    End Sub

    Private Sub bCombine1_Click(sender As Object, e As EventArgs) Handles BtnHdrCombine1.Click
        Dim mtr1 As RGBMatrix = Nothing
        Dim mtr2 As RGBMatrix = Nothing
        ComparePerfomance(Sub() mtr1 = _frame.ConvertHDR1(TbBitOffset.Value),
                          Sub() mtr2 = _frame.ConvertHDR1Fast(TbBitOffset.Value),
                "HDR1")
        'CompareResults(mtr1, mtr2)
        PbFrame.Image = mtr2.ToBitmap()
        PbFrame.Refresh()
    End Sub

    Private Sub BtnHdrCombine2_Click(sender As Object, e As EventArgs) Handles BtnHdrCombine2.Click
        Dim mtr = _frame.ConvertHDR2(TbBitOffset.Value)
        PbFrame.Image = mtr.ToBitmap
        PbFrame.Refresh()
    End Sub

    Private Sub BtnHdrCombine3_Click(sender As Object, e As EventArgs) Handles BtnHdrCombine3.Click
        If _chbUnsafeCombine3.Checked Then
            Dim mtr1 As RGBMatrix = Nothing
            Dim bmp2 As Bitmap = Nothing
            ComparePerfomance(Sub() mtr1 = _frame.ConvertHDR3(),
                              Sub() bmp2 = RawFrameFunctions.ConvertRawToHDRBitmap3Fast(_frame.Data, _frame.Width, _frame.Height),
                              "HDR3Unsafe")
            Dim mtr2 = BitmapConverter.BitmapToRGBMatrix(bmp2)
            CompareResults(mtr1, mtr2)
            PbFrame.Image = mtr2.ToBitmap()
            PbFrame.Refresh()
        Else
            Dim mtr1 As RGBMatrix = Nothing
            Dim mtr2 As RGBMatrix = Nothing
            ComparePerfomance(Sub() mtr1 = _frame.ConvertHDR3(),
                              Sub() mtr2 = _frame.ConvertHDR3Fast(),
                    "HDR3")
            CompareResults(mtr1, mtr2)
            PbFrame.Image = mtr2.ToBitmap()
            PbFrame.Refresh()
        End If
    End Sub

    Private Sub BtnHdrCombine1Unsafe_Click(sender As Object, e As EventArgs) Handles BtnHdrCombine1Unsafe.Click
        Dim bmp1 As Bitmap = Nothing
        Dim bmp2 As Bitmap = Nothing
        ComparePerfomance(Sub() bmp1 = RawFrameFunctions.ConvertRawToHDRBitmap1(_frame.Data, _frame.Width, _frame.Height, TbBitOffset.Value),
                          Sub() bmp2 = RawFrameFunctions.ConvertRawToHDRBitmap1Fast(_frame.Data, _frame.Width, _frame.Height, TbBitOffset.Value),
                "HDR1 Unsafe")
        PbFrame.Image = bmp2
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