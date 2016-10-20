Public Module ImagingMath
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
    ''' 2D-медианный фильтр
    ''' </summary>    
    ''' <param name="img">Исходные данные.</param>
    ''' <param name="N">Размер фильтра (нечетное значение).</param>    
    Public Function MedianFilter2D(img As GrayMatrix, N As Integer) As GrayMatrix
        N = If(N Mod 2 = 0, N + 1, N)
        Dim NR = (N - 1) \ 2
        Dim M = ((N * N) - 1) \ 2
        Dim trgt = New GrayMatrix(img.Width, img.Height)
        Dim median = New Byte((N * N) - 1) {}
        For i = NR To trgt.Height - NR - 1
            For j = NR To trgt.Width - NR - 1
                Dim k = 0
                For p = i - NR To i + NR
                    For q = j - NR To j + NR
                        median(k) = img.Gray(q, p)
                        k += 1
                    Next
                Next
                Array.Sort(median)
                trgt.Gray(j, i) = median(M)
            Next
        Next
        Return trgt
    End Function

    ''' <summary>
    ''' Поиск минимального/максимального значения в полутоновой матрице
    ''' </summary>
    ''' <param name="img">Исходная матрица.</param>
    ''' <param name="min">Найденный минимум.</param>
    ''' <param name="max">Найденный максимум.</param>
    Public Sub MinMax2D(img As GrayMatrix, ByRef min As Byte, ByRef max As Byte)
        min = img.Gray(0, 0)
        max = img.Gray(0, 0)
        For i = 0 To img.Height - 1
            For j = 0 To img.Width - 1
                Dim val = img.Gray(j, i)
                min = If(min < val, min, val)
                max = If(max > val, max, val)
            Next
        Next
    End Sub
End Module
