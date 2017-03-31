Imports System.Runtime.CompilerServices
Imports Bwl.Imaging

Public Module RawIntFrameConverters

    <Extension()>
    Public Function ConvertTo8Bit(frame As RawIntFrame, gainByBitOffset As Integer) As RGBMatrix
        Dim mtr1 As New RGBMatrix(frame.Width, frame.Height)
        For y = 0 To frame.Height - 1
            For x = 0 To frame.Width - 1
                Dim b = frame.Data((x + y * frame.Width) * 3 + 0)
                Dim g = frame.Data((x + y * frame.Width) * 3 + 1)
                Dim r = frame.Data((x + y * frame.Width) * 3 + 2)
                If gainByBitOffset <= 4 Then
                    r = r >> (4 - gainByBitOffset)
                    g = g >> (4 - gainByBitOffset)
                    b = b >> (4 - gainByBitOffset)
                Else
                    r = r << (gainByBitOffset - 4)
                    g = g << (gainByBitOffset - 4)
                    b = b << (gainByBitOffset - 4)
                End If
                If r > 255 Then r = 255
                If g > 255 Then g = 255
                If b > 255 Then b = 255
                mtr1.RedPixel(x, y) = Math.Min(r, 255)
                mtr1.GreenPixel(x, y) = Math.Min(g, 255)
                mtr1.BluePixel(x, y) = Math.Min(b, 255)
            Next
        Next
        Return mtr1

    End Function
    <Extension()>
    Public Function ConvertHDR3(frame As RawIntFrame, baseGain As Integer) As RGBMatrix
        Dim mtr1 As New RGBMatrix(frame.Width, frame.Height)
        For y = 0 To frame.Height - 1
            For x = 0 To frame.Width - 1
                Dim b = frame.Data((x + y * frame.Width) * 3 + 0)
                Dim g = frame.Data((x + y * frame.Width) * 3 + 1)
                Dim r = frame.Data((x + y * frame.Width) * 3 + 2)

                Dim v = 0.4
                Dim k = 12
                Dim m = 0
                If r < m Then r = m
                If g < m Then g = m
                If b < m Then b = m
                b = Math.Pow((b - m), v) * k
                g = Math.Pow((g - m), v) * k
                r = Math.Pow((r - m), v) * k

                mtr1.RedPixel(x, y) = Math.Min(r, 255)
                mtr1.GreenPixel(x, y) = Math.Min(g, 255)
                mtr1.BluePixel(x, y) = Math.Min(b, 255)
            Next
        Next
        Return mtr1
    End Function

    <Extension()>
    Public Function ConvertHDR1(frame As RawIntFrame, baseGain As Integer) As RGBMatrix
        Dim mtr1 As New RGBMatrix(frame.Width, frame.Height)
        For i = 0 To frame.Width * frame.Height - 1
            ' For y = 0 To frame.Height - 1
            '  For x = 0 To frame.Width - 1
            Dim b = frame.Data(i * 3 + 0)
            Dim g = frame.Data(i * 3 + 1)
            Dim r = frame.Data(i * 3 + 2)

            If baseGain <= 4 Then
                r = r >> (4 - baseGain)
                g = g >> (4 - baseGain)
                b = b >> (4 - baseGain)
            Else
                r = r << (baseGain - 4)
                g = g << (baseGain - 4)
                b = b << (baseGain - 4)
            End If
            Dim p = 255
            Dim t = 60
            Dim k = 0.8
            Do While (r > p Or g > p Or b > p) 'And (Math.Abs(r - b) > t Or Math.Abs(g - b) > t Or Math.Abs(g - r) > t)
                '    Do While (r > p Or g > p Or b > p) And (Math.Abs(r - b) > t Or Math.Abs(g - b) > t Or Math.Abs(g - r) > t)
                '    Do While (r > p Or g > p Or b > p) And Not (r > t And g > t And b > t)
                r = r * k
                g = g * k
                b = b * k
            Loop
            mtr1.Red(i) = Math.Min(r, 255)
            mtr1.Green(i) = Math.Min(g, 255)
            mtr1.Blue(i) = Math.Min(b, 255)
        Next
        Return mtr1
    End Function

    <Extension()>
    Public Function ConvertHDR2(frame As RawIntFrame, baseGain As Integer) As RGBMatrix
        Dim mtr1 As New RGBMatrix(frame.Width, frame.Height)
        Dim max1 As New GrayMatrix(frame.Width / 4 + 8, frame.Height / 4 + 8)
        Dim min1 As New GrayMatrix(frame.Width / 4 + 8, frame.Height / 4 + 8)

        For y = 0 To frame.Height - 1
            For x = 0 To frame.Width - 1
                Dim b = frame.Data((x + y * frame.Width) * 3 + 0)
                Dim g = frame.Data((x + y * frame.Width) * 3 + 1)
                Dim r = frame.Data((x + y * frame.Width) * 3 + 2)

                If baseGain > 4 Then baseGain = 4
                '     r = r >> (4 - baseGain)
                '    g = g >> (4 - baseGain)
                '    b = b >> (4 - baseGain)

                Dim max = Math.Max(Math.Max(r, g), b)
                Dim min = Math.Min(Math.Min(r, g), b)
                max1.GrayPixel(x / 4, y / 4) = max / 16
                min1.GrayPixel(x / 4, y / 4) = min / 16

                mtr1.RedPixel(x, y) = r
                mtr1.GreenPixel(x, y) = g
                mtr1.BluePixel(x, y) = b
            Next
        Next
        Dim bmp = max1.ToBitmap

        Dim n = 4
        Dim k = 256
        max1 = Filters.MedianFilter2D(max1, n)
        For y = 0 To frame.Height - 1
            For x = 0 To frame.Width - 1
                Dim b = frame.Data((x + y * frame.Width) * 3 + 0)
                Dim g = frame.Data((x + y * frame.Width) * 3 + 1)
                Dim r = frame.Data((x + y * frame.Width) * 3 + 2)

                Dim min = min1.GrayPixel(x / 4, y / 4) * 16
                Dim max = max1.GrayPixel(x / 4, y / 4) * 16

                If max > 255 Then
                    r = r * k / max
                    g = g * k / max
                    b = b * k / max
                End If

                mtr1.RedPixel(x, y) = Math.Min(r, 255)
                mtr1.GreenPixel(x, y) = Math.Min(g, 255)
                mtr1.BluePixel(x, y) = Math.Min(b, 255)
            Next
        Next
        Return mtr1
    End Function
End Module
