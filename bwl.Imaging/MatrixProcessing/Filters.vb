Public Class Filters

    Public Function GetBrightnessStats(bmp As GrayMatrix) As BrightnessStats
        Dim tmpSum As Long
        Dim stats As New BrightnessStats With {.BrMax = 0, .BrMin = 255}
        Dim tmpHist(255) As Long, tmpMax As Long

        For k = 0 To bmp.Width * bmp.Height - 1
            For y = 0 To bmp.Height - 1
                If bmp.Gray(k) < stats.BrMin Then stats.BrMin = bmp.Gray(k)
                If bmp.Gray(k) > stats.BrMax Then stats.BrMax = bmp.Gray(k)
                tmpSum += bmp.Gray(k)
                tmpHist(bmp.Gray(k)) += 1
            Next
        Next
        tmpSum /= bmp.Width * bmp.Height
        stats.BrAvg = tmpSum

        For i = 0 To 255
            tmpMax += tmpHist(i) / 255
        Next
        For i = 0 To 255
            stats.Histogram(i) = tmpHist(i) \ (tmpMax \ 128 + 1)
        Next
        Return stats
  
    End Function

    Public Sub LinearContrast(bmp As GrayMatrix, Optional ymin As Integer = 0, Optional ymax As Integer = 255)
        Dim stats = GetBrightnessStats(bmp)
        Dim width As Integer = bmp.Width
        Dim height As Integer = bmp.Height
        Dim xmin As Integer = stats.BrMin
        Dim xmax As Integer = stats.BrMax

        If xmax > xmin Then
            For k = 0 To bmp.Width * bmp.Height - 1
                bmp.Gray(k) = (bmp.Gray(k) - xmin) / (xmax - xmin) * (ymax - ymin) + ymin
            Next
        End If
    End Sub
End Class
