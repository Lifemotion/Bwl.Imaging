Imports System.Threading.Tasks

Public Module MatrixTools

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

    Public Function RectangleToRectangleF(rect As Rectangle, size As Size) As RectangleF
        Dim X = rect.X / CSng(size.Width)
        Dim Y = rect.Y / CSng(size.Height)
        Dim W = rect.Width / CSng(size.Width)
        Dim H = rect.Height / CSng(size.Height)
        Return New RectangleF(X, Y, W, H)
    End Function

    Public Function RectangleFToRectangle(rectF As RectangleF, size As Size) As Rectangle
        Dim X = CInt(Math.Floor(rectF.X * size.Width))
        Dim Y = CInt(Math.Floor(rectF.Y * size.Height))
        Dim W = CInt(Math.Floor(rectF.Width * size.Width))
        Dim H = CInt(Math.Floor(rectF.Height * size.Height))
        Return New Rectangle(X, Y, W, H)
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
