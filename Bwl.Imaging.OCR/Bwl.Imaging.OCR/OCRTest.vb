Imports System.Drawing.Text
Imports System.Text

Public Class OCRTest
    Private _storage As New RegistryStorage()
    Private _path As RegistryStorage.Setting = _storage.CreateSetting("Path1", ".")
    Private _charsA As CharCollection
    Private _charsT As CharCollection

    Private Sub OCRTest_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _charsA = CharCollection.Create("Arial", 40, FontStyle.Regular)
        _charsT = CharCollection.Create("TimesNewRoman", 40)
        Try
            Recog()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles bRecog.Click
        dbg = True

        Recog()
    End Sub

    Private _list As Segment()
    Private _gray As GrayMatrix

    Private Sub Recog()
        Dim bmp = Bitmap.FromFile(tbFile.Text)
        Dim gray = BitmapConverter.BitmapToGrayMatrix(bmp)
        bmp.Dispose()
        Dim rgb = gray.ToRGBMatrix

        Dim map = Segmentation.CreateSegmentsMapWithBinarize(gray, 200, True, 2)
        Dim mapc = Segmentation.ColorizeSegments(map)

        Dim list = Segmentation.CreateSegmentsList(map)
        list = Segmentation.FilterSegmentsList(list, 3, 9, gray.Width / 2, gray.Height / 2)
        _gray = gray
        _list = list


        Dim debug0 = mapc.ToBitmap
        Dim gr = Graphics.FromImage(debug0)
        For Each segm In list
            Dim results = RecogSymbol(_charsA, gray, segm)
            Dim best = results.First

            If best.Value > 0.6 Then
                gr.DrawString(best.Key.Value.ToString, DefaultFont, Brushes.Yellow, segm.Left, segm.Bottom)
            End If
        Next
        pbMain.Image = debug0
        pbMain.Refresh()
    End Sub
    Private dbg As Boolean

    Protected Function RecogSymbol(charset As CharCollection, mtr As GrayMatrix, segment As Segment) As Dictionary(Of CharInfo, Double)
        If dbg Then
            Dim subm = MatrixTools.GrayMatrixSubRect(mtr, segment.ToRectangle)
            subm.ToBitmap.Save("dbg.bmp")
        End If

        Dim dict As New Dictionary(Of CharInfo, Double)
        For Each template In charset.Chars

            Dim correct As Integer = 0
            Dim miss As Integer = 0
            For x = segment.Left To segment.Right
                For y = segment.Top To segment.Bottom
                    Dim xi = CInt((x - segment.Left) / segment.Width * template.Width) + template.Left
                    Dim yi = CInt((y - segment.Top) / segment.Height * template.Height) + template.Top
                    Dim ps = mtr.GrayPixel(x, y)
                    Dim pt = charset.Matrix.GrayPixel(xi, yi)
                    If (pt < 200 And ps < 200) Or (pt > 200 And ps > 200) Then correct += 1 Else miss += 1
                    ' If (pt > 200 And ps < 200) Then miss += 1 Else correct += 1
                Next
            Next
            If dbg Then
                Dim subm1 = MatrixTools.GrayMatrixSubRect(charset.Matrix, template.ToRectangle)
                subm1.ToBitmap.Save("dbg1.bmp")
                If template.Value = "о" Then
                    Dim b = 2
                End If
            End If
            Dim P = correct / (correct + miss)
            P = P - (Math.Abs(template.WHRatio - segment.WHRatio) * 0.3)
            dict.Add(template, P)
        Next
        Dim sorted = From pair In dict
                     Order By pair.Value Descending
        Dim sortedDictionary = sorted.ToDictionary(Function(p) p.Key, Function(p) p.Value)
        Dim arr = sortedDictionary.ToList
        Return sortedDictionary
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
                dbg = False
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
            Dim ci = _charsA.FindCharInfo(tbRecogSymb.Text)
            If ci IsNot Nothing Then
                Dim tmp = MatrixTools.GrayMatrixSubRect(_charsA.Matrix, ci.ToRectangle)
                pbAux2.ShowMatrix(tmp)
            End If
        End If

        Dim results = RecogSymbol(_charsA, _gray, segm)
        lbSymbResults.Items.Clear()
        For Each res In results
            lbSymbResults.Items.Add(res.Value.ToString("0.00") + " " + res.Key.Value.ToString)
        Next
    End Sub


End Class
