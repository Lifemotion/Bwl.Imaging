Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Threading.Tasks

Public Module MatrixTools

    ''' <summary>
    ''' Получение полутоновой матрицы на основе BitmapInfo
    ''' </summary>
    <Extension()>
    Public Function GetGrayMatrix(bi As BitmapInfo) As GrayMatrix
        Return BitmapConverter.BitmapToGrayMatrix(bi.GetClonedBmp())
    End Function

    ''' <summary>
    ''' Получение полноцветной матрицы на основе BitmapInfo
    ''' </summary>
    <Extension()>
    Public Function GetRGBMatrix(bi As BitmapInfo) As RGBMatrix
        Return BitmapConverter.BitmapToRGBMatrix(bi.GetClonedBmp())
    End Function

    ''' <summary>
    ''' Выравнивание полутоновой матрицы до ширины кратной 4
    ''' </summary>
    Public Function GrayMatrixAlign4(img As GrayMatrix) As GrayMatrix
        Return GrayMatrixAlign(img, 4)
    End Function

    ''' <summary>
    ''' Выравнивание полутоновой матрицы до ширины кратной align
    ''' </summary>
    Public Function GrayMatrixAlign(img As GrayMatrix, align As Integer) As GrayMatrix
        If img.Width Mod align <> 0 Then
            Dim padding = align - img.Width Mod align
            Dim paddingL = padding \ 2
            Dim result = New GrayMatrix(img.Width + padding, img.Height)
            Dim imgGray = img.Gray
            Dim resultGray = result.Gray
            Dim offsetImg = 0
            Dim offsetRes = paddingL
            For y = 0 To img.Height - 1
                For x = 0 To img.Width - 1
                    resultGray(x + offsetRes) = imgGray(x + offsetImg)
                Next
                offsetImg += img.Width
                offsetRes += result.Width
            Next
            Return result
        Else
            Return img
        End If
    End Function

    ''' <summary>
    ''' Выравнивание цветной матрицы до ширины кратной 4
    ''' </summary>
    Public Function RGBMatrixAlign4(img As RGBMatrix) As RGBMatrix
        Return RGBMatrixAlign(img, 4)
    End Function

    ''' <summary>
    ''' Выравнивание цветной матрицы до ширины кратной align
    ''' </summary>
    Public Function RGBMatrixAlign(img As RGBMatrix, align As Integer) As RGBMatrix
        If img.Width Mod align <> 0 Then
            Dim padding = align - img.Width Mod align
            Dim paddingL = padding \ 2
            Dim result = New RGBMatrix(img.Width + padding, img.Height)
            Parallel.For(0, 3, Sub(channel As Integer)
                                   Dim imgMatrix = img.Matrix(channel)
                                   Dim resultMatrix = result.Matrix(channel)
                                   Dim offsetImg = 0
                                   Dim offsetRes = paddingL
                                   For y = 0 To img.Height - 1
                                       For x = 0 To img.Width - 1
                                           resultMatrix(x + offsetRes) = imgMatrix(x + offsetImg)
                                       Next
                                       offsetImg += img.Width
                                       offsetRes += result.Width
                                   Next
                               End Sub)
            Return result
        Else
            Return img
        End If
    End Function

    ''' <summary>
    ''' Установка прямогольника по "выровненным" позициям (каждая позиция становится кратной align)
    ''' </summary>
    Public Function RectangleAlign(rect As Rectangle, Optional align As Integer = 4) As Rectangle
        Dim rX1 = rect.X
        Dim rY1 = rect.Y
        Dim rX2 = rX1 + rect.Width
        Dim rY2 = rY1 + rect.Height
        rX1 += align - (rX1 Mod align)
        rY1 += align - (rY1 Mod align)
        rX2 -= (rX2 Mod align)
        rY2 -= (rY2 Mod align)
        If rX2 < align Then rX2 = align
        If rY2 < align Then rY2 = align
        Return New Rectangle(rX1, rY1, rX2 - rX1, rY2 - rY1)
    End Function

    ''' <summary>
    ''' Получение полутоной подматрицы
    ''' </summary>
    Public Function GrayMatrixSubRect(img As GrayMatrix, rect As Rectangle) As GrayMatrix
        Dim result = New GrayMatrix(rect.Width, rect.Height)
        Dim imgGray = img.Gray
        Dim resultGray = result.Gray
        Dim offsetImg = rect.X + rect.Y * img.Width
        Dim offsetRes = 0
        For y = 0 To rect.Height - 1
            For x = 0 To rect.Width - 1
                resultGray(x + offsetRes) = imgGray(x + offsetImg)
            Next
            offsetImg += img.Width
            offsetRes += result.Width
        Next
        Return result
    End Function

    ''' <summary>
    ''' Получение цветной подматрицы
    ''' </summary>
    Public Function RGBMatrixSubRect(img As RGBMatrix, rect As Rectangle) As RGBMatrix
        Dim result = New RGBMatrix(rect.Width, rect.Height)
        Parallel.For(0, 3, Sub(channel As Integer)
                               Dim imgMatrix = img.Matrix(channel)
                               Dim resultMatrix = result.Matrix(channel)
                               Dim offsetImg = rect.X + rect.Y * img.Width
                               Dim offsetRes = 0
                               For y = 0 To rect.Height - 1
                                   For x = 0 To rect.Width - 1
                                       resultMatrix(x + offsetRes) = imgMatrix(x + offsetImg)
                                   Next
                                   offsetImg += img.Width
                                   offsetRes += result.Width
                               Next
                           End Sub)
        Return result
    End Function

    ''' <summary>
    ''' Инверсия полутонового изображения
    ''' </summary>    
    Public Sub InverseGray(img As GrayMatrix)
        Dim imgGray = img.Gray
        Dim offset = 0
        For y = 0 To img.Height - 1
            For x = 0 To img.Width - 1
                imgGray(x + offset) = Byte.MaxValue - imgGray(x + offset)
            Next
            offset += img.Width
        Next
    End Sub

    ''' <summary>
    ''' Инверсия цветного изображения
    ''' </summary>
    Public Sub InverseRGB(img As RGBMatrix)
        Parallel.For(0, 3, Sub(channel As Integer)
                               Dim imgMatrix = img.Matrix(channel)
                               Dim offset = 0
                               For y = 0 To img.Height - 1
                                   For x = 0 To img.Width - 1
                                       imgMatrix(x + offset) = Byte.MaxValue - imgMatrix(x + offset)
                                   Next
                                   offset += img.Width
                               Next
                           End Sub)
    End Sub
End Module
