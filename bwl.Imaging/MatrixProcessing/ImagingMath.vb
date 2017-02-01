Public Module ImagingMath

    ''' <summary>
    '''  Статистика по яркости
    ''' </summary>    
    Public Function GetBrightnessStats(img As GrayMatrix) As BrightnessStats
        Dim tmpSum As Long
        Dim stats As New BrightnessStats With {.BrMax = 0, .BrMin = 255}
        Dim tmpHist(255) As Long, tmpMax As Long

        Dim imgGray = img.Gray
        For k = 0 To img.Width * img.Height - 1
            If imgGray(k) < stats.BrMin Then stats.BrMin = imgGray(k)
            If imgGray(k) > stats.BrMax Then stats.BrMax = imgGray(k)
            tmpSum += imgGray(k)
            tmpHist(imgGray(k)) += 1
        Next
        tmpSum /= img.Width * img.Height
        stats.BrAvg = tmpSum

        For i = 0 To 255
            tmpMax += tmpHist(i) / 255
        Next
        For i = 0 To 255
            stats.Histogram(i) = tmpHist(i) \ (tmpMax \ 128 + 1)
        Next
        Return stats
    End Function

    ''' <summary>
    ''' Билинейная интерполяция между двумя значениями
    ''' </summary>
    ''' <param name="value1">Значение 1.</param>
    ''' <param name="value2">Значение 2.</param>
    ''' <param name="weight1">Вес для значения 1.</param>
    ''' <param name="weight2">Вес для значения 2.</param>
    Public Function Bilinear(value1 As Double, value2 As Double, weight1 As Double, weight2 As Double) As Double
        Dim ws = weight1 + weight2
        weight1 = weight1 / ws
        weight2 = weight2 / ws
        Return value1 * weight1 + value2 * weight2
    End Function

    ''' <summary>
    ''' Билинейная интерполяция между двумя точками
    ''' </summary>
    ''' <param name="point1">Точка 1.</param>
    ''' <param name="point2">Точка 2.</param>
    ''' <param name="weight1">Вес для точки 1.</param>
    ''' <param name="weight2">Вес для точки 2.</param>
    Public Function Bilinear(point1 As PointF, point2 As PointF, weight1 As Double, weight2 As Double) As PointF
        Dim result As PointF
        result.X = Bilinear(point1.X, point2.X, weight1, weight2)
        result.Y = Bilinear(point1.Y, point2.Y, weight1, weight2)
        Return result
    End Function

    ''' <summary>
    ''' Поиск минимального/максимального значения в полутоновой матрице
    ''' </summary>
    ''' <param name="img">Исходная матрица.</param>
    ''' <param name="min">Найденный минимум.</param>
    ''' <param name="max">Найденный максимум.</param>
    Public Sub MinMax2D(img As GrayMatrix, ByRef min As Byte, ByRef max As Byte)
        min = img.GrayPixel(0, 0)
        max = img.GrayPixel(0, 0)
        Dim imgGray = img.Gray
        Dim offset = 0
        For y = 0 To img.Height - 1
            For x = 0 To img.Width - 1
                Dim val = imgGray(x + offset)
                min = If(min < val, min, val)
                max = If(max > val, max, val)
            Next
            offset += img.Width
        Next
    End Sub
End Module
