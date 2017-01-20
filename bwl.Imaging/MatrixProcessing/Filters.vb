Imports System.Threading.Tasks

Public Module Filters

    ''' <summary>
    ''' Фильтр повышения резкости полутонового изображения разреженной маской 5*5
    ''' </summary>
    ''' <param name="img">Исходное изображение.</param>
    Public Function Sharpen5Gray(img As GrayMatrix) As GrayMatrix
        Dim msize = 5
        Dim res As New GrayMatrix(img.Width, img.Height)
        Dim imgGray = img.Gray
        Dim resGray = res.Gray
        Parallel.For(0, img.Height - msize, Sub(row As Integer)
                                                Dim m0RowOffset = row * img.Width
                                                Dim m2RowOffset = (row + 2) * img.Width
                                                Dim m4RowOffset = (row + 4) * img.Width
                                                For col = 0 To (img.Width - msize) - 1
                                                    Dim value =
                                                    -0.1 * imgGray(m0RowOffset + col) + -0.1 * imgGray(m0RowOffset + col + 2) + -0.1 * imgGray(m0RowOffset + col + 4) +
                                                    -0.1 * imgGray(m2RowOffset + col) + +1.8 * imgGray(m2RowOffset + col + 2) + -0.1 * imgGray(m2RowOffset + col + 4) +
                                                    -0.1 * imgGray(m4RowOffset + col) + -0.1 * imgGray(m4RowOffset + col + 2) + -0.1 * imgGray(m4RowOffset + col + 4)

                                                    value = If(value < 0, 0, value)
                                                    value = If(value > 255, 255, value)
                                                    resGray(m2RowOffset + col + 2) = value
                                                Next
                                            End Sub)
        Return res
    End Function

    ''' <summary>
    ''' 2D-медианный фильтр
    ''' </summary>    
    ''' <param name="img">Исходные данные.</param>
    ''' <param name="N">Размер фильтра (нечетное значение).</param>    
    Public Function MedianFilter2D(img As GrayMatrix, N As Integer) As GrayMatrix
        N = If(N Mod 2 = 0, N + 1, N) 'Нечетный размер окна фильтра
        Dim NR = (N - 1) \ 2 'Радиус фильтра
        Dim M = ((N * N) - 1) \ 2 'Координаты медианы
        Dim res = New GrayMatrix(img.Width, img.Height)
        Dim imgGray = img.Gray
        Dim resGray = res.Gray
        Parallel.For(NR, res.Height - NR, Sub(y As Integer)
                                              Dim median = New Byte((N * N) - 1) {}
                                              Dim offset = y * img.Width
                                              For x = NR To res.Width - NR - 1
                                                  Dim k = 0
                                                  For y2 = y - NR To y + NR
                                                      For x2 = x - NR To x + NR
                                                          median(k) = img.GrayPixel(x2, y2)
                                                          k += 1
                                                      Next
                                                  Next
                                                  Array.Sort(median)
                                                  resGray(x + offset) = median(M)
                                              Next
                                          End Sub)
        Return res
    End Function

    Public Sub LinearContrast(img As GrayMatrix, Optional ymin As Integer = 0, Optional ymax As Integer = 255)
        Dim stats = GetBrightnessStats(img)
        Dim width As Integer = img.Width
        Dim height As Integer = img.Height
        Dim xmin As Integer = stats.BrMin
        Dim xmax As Integer = stats.BrMax

        If xmax > xmin Then
            Dim imgGray = img.Gray
            For k = 0 To img.Width * img.Height - 1
                imgGray(k) = (imgGray(k) - xmin) / (xmax - xmin) * (ymax - ymin) + ymin
            Next
        End If
    End Sub
End Module
