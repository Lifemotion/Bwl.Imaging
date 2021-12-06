
Imports System.Drawing
''' <summary>
''' Простой детектор движения (изменений в кадре). Для каждого видеоисточника должен быть свой детектор.
''' </summary>
Public Class MoveDetector
    ''' <summary>
    ''' Порог разности яркости, при котором точка считается изменившейся. Для фильтрации шумов. Можно не менять.
    ''' </summary>
    ''' <returns></returns>
    Public Property PointDiffThreshSetting As Integer = 5
    ''' <summary>
    ''' Порог изменений в кадре, после которых обнаруживается движение. Доля изменившихся точек относительно общего количества пикселей кадра.
    ''' </summary>
    ''' <returns></returns>
    Public Property MoveThresholdSetting As Double = 1.0
    ''' <summary>
    ''' В течение какого количества кадров БЕЗ движения после кадра С движением будет выдаваться результат наличия движения.
    ''' </summary>
    ''' <returns></returns>
    Public Property AfterMoveSetting As Integer = 1
    Public Event Logger(type As String, msg As String)

    Private _afterMoveCounter As Integer

    Public Function Process(image As Bitmap) As Boolean
        Dim bmp As New Bitmap(image, 64, 32)
        Dim mtr As GrayMatrix = BitmapConverter.BitmapToGrayMatrix(bmp)
        Return Process(mtr)
    End Function

    Public Function Process(matrix As GrayMatrix) As Boolean
        SyncLock Me
            Static lastMatrix As GrayMatrix
            If lastMatrix IsNot Nothing AndAlso lastMatrix.Width = matrix.Width AndAlso lastMatrix.Height = matrix.Height Then
                Dim diff As Long
                Dim cnt As Integer
                For y = 0 To lastMatrix.Height - 1
                    For x = 0 To lastMatrix.Width - 1
                        Dim pd = Math.Abs(CInt(matrix.GrayPixel(x, y)) - CInt(lastMatrix.GrayPixel(x, y)))
                        If pd > PointDiffThreshSetting Then diff += pd
                        cnt += 1
                    Next
                Next
                Dim val = diff / cnt
                lastMatrix = matrix.Clone
                Dim result = val > MoveThresholdSetting
                If result = True Then
                    _afterMoveCounter = AfterMoveSetting
                    RaiseEvent Logger("DBG", "Move TRUE, Diff: " + val.ToString("0.0") + ", Thresh: " + MoveThresholdSetting.ToString("0.0"))
                Else
                    If _afterMoveCounter > 0 Then
                        result = True
                        _afterMoveCounter -= 1
                        RaiseEvent Logger("DBG", "Move TRUE AFTERMOVE (" + _afterMoveCounter.ToString + "), Diff: " + val.ToString("0.0") + ", Thresh: " + MoveThresholdSetting.ToString("0.0"))
                    Else
                        RaiseEvent Logger("DBG", "Move FALSE, Diff: " + val.ToString("0.0") + ", Thresh: " + MoveThresholdSetting.ToString("0.0"))
                    End If
                End If
                Return result
            Else
                RaiseEvent Logger("DBG", "No last frame or last frame has different size")
                lastMatrix = matrix.Clone
                Return False
            End If
        End SyncLock
    End Function
End Class
