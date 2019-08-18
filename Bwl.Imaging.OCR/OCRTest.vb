Imports System.Drawing.Text
Imports System.Text

Public Class OCRTest
    Private _storage As New RegistryStorage()
    Private _path As RegistryStorage.Setting = _storage.CreateSetting("Path1", ".")
    Private _charsArial As CharCollection
    Private _charsTNR As CharCollection
    Private _charsCN As CharCollection

    Private _list As Segment()
    Private _gray As GrayMatrix
    Private _dbg As Boolean

    Private Sub OCRTest_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _charsArial = CharCollection.Create("Arial", 40, FontStyle.Regular)
        _charsTNR = CharCollection.Create("Times New Roman", 40, FontStyle.Regular)
        _charsCN = CharCollection.Create("Courier New", 40, FontStyle.Regular)
        Try
            Recog()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles bRecog.Click
        _dbg = True

        Recog()
    End Sub

    Public Class RecognizedWord
        Public Property Word As String
        Public Property X As Integer
        Public Property Y As Integer
        Public Property Height As Integer
    End Class

    Public Class RecognizedLine
        Public Property Words As New List(Of RecognizedWord)
    End Class
    Private _charset As CharCollection

    Private Sub Recog()
        Dim bmp = Bitmap.FromFile(tbFile.Text)
        Recog(bmp)
    End Sub

    Private Sub Recog(bmp As Bitmap)
        Dim gray = BitmapConverter.BitmapToGrayMatrix(bmp)
        bmp.Dispose()
        Dim rgb = gray.ToRGBMatrix
        Dim threshold = NumericUpDown1.Value
        Select Case ComboBox1.Text
            Case "Arial" : _charset = _charsArial
            Case "Times New Roman" : _charset = _charsTNR
            Case "Courier New" : _charset = _charsCN
            Case Else : Throw New Exception
        End Select

        Dim map = Segmentation.CreateSegmentsMapWithBinarize(gray, threshold, True, 2)
        Dim mapc = Segmentation.ColorizeSegments(map)

        Dim list = Segmentation.CreateSegmentsList(map)
        list = Segmentation.FilterSegmentsList(list, 2, 6, gray.Width / 2, gray.Height / 2)
        _gray = gray
        _list = list

        Dim debug0 = mapc.ToBitmap
        Dim gr = Graphics.FromImage(debug0)

        Dim rmap(gray.Width, gray.Height) As RecognizedSegment
        For Each segm In list
            Dim rsegm As New RecognizedSegment
            rsegm.segment = segm
            rsegm.recogDict = RecogSymbol(_charset, gray, segm, threshold)
            Dim best = rsegm.recogDict.First
            If best.Value > 0.5 Then rsegm.recogChar = best.Key.Value

            ' segm.Top = Math.Round(segm.Top / 10) * 10

            For y = segm.Top To segm.Bottom
                rmap(segm.Left, y) = rsegm
            Next

            If rsegm.recogChar <> " "c Then
                gr.DrawString(best.Key.Value.ToString, DefaultFont, Brushes.Yellow, segm.Left, segm.Bottom)
            End If
        Next

        Dim lines = FindWordLines(gray, rmap)
        Dim resBmp As New Bitmap(gray.Width, gray.Height)
        Dim resGr = Graphics.FromImage(resBmp)
        resGr.Clear(Color.White)
        For Each line In lines
            For Each word In line.Words
                Dim font = New Font("Arial", 20, GraphicsUnit.Pixel)
                Dim brush = Brushes.Black
                If word.Word.ToUpper = "ИСПОЛНИТЕЛЬ" Then brush = Brushes.Blue
                If word.Word.ToUpper = "ЗАКАЗЧИК" Then brush = Brushes.Blue
                If word.Word.ToUpper = "АКТ" Then brush = Brushes.Green
                If word.Word.ToUpper = "ИНН" Then brush = Brushes.Red
                If word.Word.ToUpper = "КПП" Then brush = Brushes.Red
                If word.Word.ToUpper = "БИК" Then brush = Brushes.Red
                resGr.DrawString(word.Word.ToUpper, font, brush, word.X, word.Y)

            Next
        Next
        pbResult.Image = resBmp
        pbResult.Refresh()

        pbMain.Image = debug0
        pbMain.Refresh()
    End Sub

    Public Function FindWordLines(gray As GrayMatrix, rmap As RecognizedSegment(,)) As List(Of RecognizedLine)
        Dim text = ""
        Dim lines As New List(Of RecognizedLine)
        For y = 0 To gray.Height - 1 Step 4
            Dim line = ""
            Dim word = ""
            Dim wordStartSegm As RecognizedSegment = Nothing
            Dim first As RecognizedSegment = Nothing
            Dim yCorr = y
            Dim prev As RecognizedSegment = Nothing
            Dim words As New List(Of RecognizedWord)

            For x = 0 To gray.Width - 1

                If first Is Nothing Then
                    Dim psegm = rmap(x, yCorr)
                    If psegm IsNot Nothing AndAlso psegm.used = False Then
                        'first found
                        first = psegm
                        wordStartSegm = psegm
                        yCorr = psegm.segment.CenterY

                        word += psegm.recogChar
                        psegm.used = True
                        prev = psegm
                    End If
                Else
                    Dim psegm = rmap(x, yCorr)
                    If psegm IsNot Nothing AndAlso psegm.used = False Then
                        If (psegm.segment.Left - prev.segment.Right) > Math.Max(psegm.segment.Width, prev.segment.Width) * 0.5 Then
                            Dim wordObj As New RecognizedWord
                            wordObj.Word = word
                            wordObj.X = wordStartSegm.segment.Left
                            wordObj.Y = wordStartSegm.segment.Top
                            wordObj.Height = wordStartSegm.segment.Height
                            words.Add(wordObj)
                            wordStartSegm = psegm
                            line += word + " "
                            word = ""
                        End If
                        yCorr = psegm.segment.CenterY

                        word += psegm.recogChar
                        psegm.used = True
                        prev = psegm
                    End If
                                    End If
            Next
            If word > "" Then
                Dim wordObj As New RecognizedWord
                wordObj.Word = word
                wordObj.X = wordStartSegm.segment.Left
                wordObj.Y = wordStartSegm.segment.Top
                wordObj.Height = wordStartSegm.segment.Height
                words.Add(wordObj)
                line += word + " "
            End If
            If line > "" Then
                Dim lineObj As New RecognizedLine
                lineObj.Words = words
                lines.Add(lineObj)
                text += line + vbCrLf
            End If
        Next
        Return lines
    End Function

    Public Class RecognizedSegment
        Public segment As Segment
        Public recogDict As Dictionary(Of CharInfo, Double)
        Public recogChar As Char = " "c
        Public used = False
    End Class

    Protected Function RecogSymbol(charset As CharCollection, source As GrayMatrix, segment As Segment, threshold As Integer) As Dictionary(Of CharInfo, Double)
        Dim dict As New Dictionary(Of CharInfo, Double)
        For Each template In charset.Chars
            dict.Add(template, TestSymbol(template, charset.Matrix, source, segment, threshold))
        Next
        Dim sorted = From pair In dict
                     Order By pair.Value Descending
        Dim sortedDictionary = sorted.ToDictionary(Function(p) p.Key, Function(p) p.Value)
        Dim arr = sortedDictionary.ToList
        Return sortedDictionary
    End Function

    Protected Function TestSymbol(template As CharInfo, templateMatrix As GrayMatrix, source As GrayMatrix, segment As Segment, threshold As Integer) As Double
        Dim correct As Integer = 0
        Dim miss As Integer = 0
        For x = segment.Left To segment.Right
            For y = segment.Top To segment.Bottom
                Dim xi = CInt((x - segment.Left) / segment.Width * template.Width) + template.Left
                Dim yi = CInt((y - segment.Top) / segment.Height * template.Height) + template.Top
                Dim ps = source.GrayPixel(x, y)
                Dim pt = templateMatrix.GrayPixel(xi, yi)
                If (pt < threshold And ps < threshold) Or (pt > threshold And ps > threshold) Then correct += 1 Else miss += 1
                ' If (pt > 200 And ps < 200) Then miss += 1 Else correct += 1
            Next
        Next
        Dim P = correct / (correct + miss)
        P = P - (Math.Abs(template.WHRatio - segment.WHRatio) * 0.3)
        Return P
    End Function

    Private Sub StereoSystemCalibrator_DragOver(sender As Object, e As DragEventArgs) Handles Me.DragOver
        Dim d = e.Data.GetData("FileDrop")
        If d.length = 1 Then e.Effect = DragDropEffects.Copy
    End Sub

    Private Sub StereoSystemCalibrator_DragDrop(sender As Object, e As DragEventArgs) Handles Me.DragDrop
        Dim d = e.Data.GetData("FileDrop")
        If d.length = 1 Then
            Dim file As String = d(0)
            Try
                _dbg = False
                tbFile.Text = file.Replace("""", "")
                Recog()
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
        End If
    End Sub

    Private Sub bRecogSymb_Click(sender As Object, e As EventArgs) Handles bRecogSymb.Click
        If _list Is Nothing Then Return
        Dim segm = _list(tbRecogSegment.Value)
        Dim mtr = MatrixTools.GrayMatrixSubRect(_gray, segm.ToRectangle)
        For i = 0 To mtr.Gray.Length - 1
            If mtr.Gray(i) > 200 Then mtr.Gray(i) = 255 Else mtr.Gray(i) = 0
        Next
        pbAux1.ShowMatrix(mtr)
        If tbRecogSymb.Text > "" Then
            Dim ci = _charset.FindCharInfo(tbRecogSymb.Text)
            If ci IsNot Nothing Then
                Dim tmp = MatrixTools.GrayMatrixSubRect(_charset.Matrix, ci.ToRectangle)
                pbAux2.ShowMatrix(tmp)
            End If
        End If

        Dim results = RecogSymbol(_charset, _gray, segm, 200)
        lbSymbResults.Items.Clear()
        For Each res In results
            lbSymbResults.Items.Add(res.Value.ToString("0.00") + " " + res.Key.Value.ToString)
        Next
    End Sub

    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        Recog(My.Computer.Clipboard.GetImage)
    End Sub
End Class
