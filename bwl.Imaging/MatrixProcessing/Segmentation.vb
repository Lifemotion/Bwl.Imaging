Imports Bwl.Imaging

Public Class Segment
    Public Property Left As Integer
    Public Property Top As Integer
    Public Property Width As Integer
    Public Property Height As Integer
    Public Property ID As Integer
    Public Property Tag As Integer
    Public Property Debug As String

    Public Property Right As Integer
        Get
            Return Left + Width
        End Get
        Set(value As Integer)
            Width = value - Left
        End Set
    End Property
    Public Property Bottom As Integer
        Get
            Return Top + Height
        End Get
        Set(value As Integer)
            Height = value - Top
        End Set
    End Property
    Public Overrides Function ToString() As String
        Return "L:" + Left.ToString + " :T" + Top.ToString + " :W" + Width.ToString + " :H" + Height.ToString
    End Function

    Public ReadOnly Property CenterX As Integer
        Get
            Return Left + Width / 2
        End Get
    End Property

    Public ReadOnly Property WHRatio As Single
        Get
            If Height = 0 Then Return 0
            Return Width / Height
        End Get
    End Property

    Public ReadOnly Property CenterY As Integer
        Get
            Return Top + Height / 2
        End Get
    End Property

    Public Function IsPointInside(x As Integer, y As Integer) As Boolean
        Return x >= Left And x <= Left + Width And y >= Top And y <= Top + Height
    End Function


End Class

Public Class Segmentation

    Public Shared Function CreateSegmentsMap(matrix As GrayMatrix, Optional binarizeThreshold As Integer = 120, Optional invert As Boolean = False) As GrayMatrix
        Dim segments As GrayMatrix
        If invert Then
            segments = FirstSegmentationInvert(matrix, binarizeThreshold)
        Else
            segments = FirstSegmentation(matrix, binarizeThreshold)
        End If
        FillSegments(segments.Gray, matrix.Width, matrix.Height)
        FillSegmentsRev(segments.Gray, matrix.Width, matrix.Height)
        Return segments
    End Function

    Private Shared Function FirstSegmentationInvert(matrix As GrayMatrix, binarizeThreshold As Integer) As GrayMatrix
        Dim segments As New GrayMatrix(matrix.Width, matrix.Height)
        Dim segmIndex As Integer = 1
        For y = 0 To matrix.Height - 1
            Dim rowstart = y * matrix.Width
            Dim last As Boolean = 0
            For x = 0 To matrix.Width - 1
                Dim pix = matrix.Gray(x + rowstart)
                If pix < binarizeThreshold Then
                    If last = False Then segmIndex += 1
                    segments.Gray(x + rowstart) = segmIndex
                    last = True
                Else
                    last = False
                End If
            Next
        Next
        Return segments
    End Function

    Private Shared Function FirstSegmentation(matrix As GrayMatrix, binarizeThreshold As Integer) As GrayMatrix
        Dim segments As New GrayMatrix(matrix.Width, matrix.Height)
        Dim segmIndex As Integer = 1
        For y = 0 To matrix.Height - 1
            Dim rowstart = y * matrix.Width
            Dim last As Boolean = 0
            For x = 0 To matrix.Width - 1
                Dim pix = matrix.Gray(x + rowstart)
                If pix > binarizeThreshold Then
                    If last = False Then segmIndex += 1
                    segments.Gray(x + rowstart) = segmIndex
                    last = True
                Else
                    last = False
                End If
            Next
        Next
        Return segments
    End Function

    Public Shared Function CreateSegmentsList(segmentsMap As GrayMatrix) As Segment()
        Dim segments As New List(Of Segment)
        For y = 0 To segmentsMap.Height - 1
            Dim rowstart = y * segmentsMap.Width
            Dim last As Integer = 0
            For x = 0 To segmentsMap.Width - 1
                Dim segm = segmentsMap.Gray(x + rowstart)
                If segm > 0 Then
                    Dim found As Boolean = False
                    For i = segments.Count - 1 To 0 Step -1
                        With segments(i)
                            If .ID = segm Then
                                found = True
                                If .Left > x Then .Left = x
                                If .Top > y Then .Top = y
                                If .Right < x Then .Right = x
                                If .Bottom < y Then .Bottom = y
                                Exit For
                            End If
                        End With
                    Next
                    If Not found Then
                        Dim seg As New Segment
                        seg.ID = segm
                        seg.Left = x
                        seg.Top = y
                        seg.Width = 1
                        seg.Height = 1
                        segments.Add(seg)
                    End If
                End If
            Next
        Next
        Return segments.ToArray
    End Function

    Public Shared Function FilterSegmentsList(segments As IEnumerable(Of Segment), minWidth As Integer, minHeight As Integer, maxWidth As Integer, maxHeight As Integer) As Segment()
        Dim results As New List(Of Segment)
        For Each segm In segments
            If segm.Width > minWidth AndAlso
               segm.Height > minHeight AndAlso
               segm.Width < maxWidth AndAlso
               segm.Height < maxHeight Then
                results.Add(segm)
            End If
        Next
        Return results.ToArray
    End Function

    Public Shared Function FilterSegmentsList(segments As IEnumerable(Of Segment), minWHratio As Single, maxWHRatio As Single) As Segment()
        Dim results As New List(Of Segment)
        For Each segm In segments
            If segm.WHRatio > 0 AndAlso segm.WHRatio >= minWHratio AndAlso segm.WHRatio <= maxWHRatio Then
                results.Add(segm)
            End If
        Next
        Return results.ToArray
    End Function

    Private Shared Sub FillSegments(segments() As Integer, width As Integer, height As Integer)
        For y = 0 To height - 2
            Dim rowstart = y * width
            Dim nextRowstart = rowstart + width
            For x = 0 To width - 1
                Dim segm = segments(rowstart + x)
                If segm > 0 Then
                    If segments(nextRowstart + x) > 0 AndAlso segm < segments(nextRowstart + x) Then
                        For i = x To width - 1
                            If segments(nextRowstart + i) = 0 Then Exit For
                            segments(nextRowstart + i) = segm
                        Next
                        For i = x To 0 Step -1
                            If segments(nextRowstart + i) = 0 Then Exit For
                            segments(nextRowstart + i) = segm
                        Next
                    End If
                End If
            Next
        Next
    End Sub

    Private Shared Sub FillSegmentsRev(segments() As Integer, width As Integer, height As Integer)
        For y = height - 1 To 1 Step -1
            Dim rowstart = y * width
            Dim nextRowstart = rowstart - width
            For x = 0 To width - 1
                Dim segm = segments(rowstart + x)
                If segm > 0 Then
                    If segments(nextRowstart + x) > 0 AndAlso segm < segments(nextRowstart + x) Then
                        For i = x To width - 1
                            If segments(nextRowstart + i) = 0 Then Exit For
                            segments(nextRowstart + i) = segm
                        Next
                        For i = x To 0 Step -1
                            If segments(nextRowstart + i) = 0 Then Exit For
                            segments(nextRowstart + i) = segm
                        Next
                    End If
                End If
            Next
        Next
    End Sub

End Class
