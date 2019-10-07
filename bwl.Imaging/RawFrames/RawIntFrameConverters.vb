Imports System.Runtime.CompilerServices
Imports System.Threading.Tasks
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
                mtr1.RedPixel(x, y) = Math.Min(r, 255)
                mtr1.GreenPixel(x, y) = Math.Min(g, 255)
                mtr1.BluePixel(x, y) = Math.Min(b, 255)
            Next
        Next
        Return mtr1
    End Function

    <Extension()>
    Public Function ConvertTo8BitFast(frame As RawIntFrame, gainByBitOffset As Integer) As RGBMatrix
        Dim mtr1 As New RGBMatrix(frame.Width, frame.Height)
        Dim frameData = frame.Data
        If gainByBitOffset <= 4 Then
            Parallel.For(0, 3, Sub(channel As Integer)
                                   Dim matrix = mtr1.Matrix(2 - channel)
                                   For i = 0 To matrix.Length - 1
                                       Dim px = frameData(i * 3 + channel) >> (4 - gainByBitOffset)
                                       matrix(i) = Math.Min(px, 255)
                                   Next
                               End Sub)
        Else
            Parallel.For(0, 3, Sub(channel As Integer)
                                   Dim matrix = mtr1.Matrix(2 - channel)
                                   For i = 0 To matrix.Length - 1
                                       Dim px = frameData(i * 3 + channel) << (gainByBitOffset - 4)
                                       matrix(i) = Math.Min(px, 255)
                                   Next
                               End Sub)
        End If
        Return mtr1
    End Function

    Public Function ConvertTo8BitPair(frame As RawIntFrame, Optional totalBits As Integer = 12) As RGBMatrix()
        Dim mtr1 As New RGBMatrix(frame.Width, frame.Height)
        Dim mtr2 As New RGBMatrix(frame.Width, frame.Height)

        For y = 0 To frame.Height - 1
            For x = 0 To frame.Width - 1
                Dim b = frame.Data((x + y * frame.Width) * 3 + 0)
                Dim g = frame.Data((x + y * frame.Width) * 3 + 1)
                Dim r = frame.Data((x + y * frame.Width) * 3 + 2)

                If totalBits = 12 Then
                    Dim rh = (r >> 4)
                    Dim rl = (r)

                    Dim bh = (b >> 4)
                    Dim bl = (b)

                    Dim gh = (g >> 4)
                    Dim gl = (g)

                    If rl > 63 Then rl = 63
                    If gl > 63 Then gl = 63
                    If bl > 63 Then bl = 63

                    'If rl > 31 Then rl = rl And 15 Or 16
                    'If gl > 31 Then gl = gl And 15 Or 16
                    'If bl > 31 Then bl = bl And 15 Or 16

                    mtr1.RedPixel(x, y) = rh
                    mtr1.GreenPixel(x, y) = gh
                    mtr1.BluePixel(x, y) = bh

                    mtr2.RedPixel(x, y) = rl
                    mtr2.GreenPixel(x, y) = gl
                    mtr2.BluePixel(x, y) = bl
                End If
            Next
        Next
        Return {mtr1, mtr2}
    End Function

    Public Function ConvertFrom8BitPair(pair As RGBMatrix(), Optional totalBits As Integer = 12) As RawIntFrame
        Dim mtr1 = pair(0)
        Dim mtr2 = pair(1)
        Dim arr(mtr1.Width * mtr1.Height * 3 - 1) As Integer
        Dim frame As New RawIntFrame(mtr1.Width, mtr2.Height, arr)

        For y = 0 To mtr1.Height - 1
            For x = 0 To mtr1.Width - 1

                Dim rh = mtr1.RedPixel(x, y)
                Dim gh = mtr1.GreenPixel(x, y)
                Dim bh = mtr1.BluePixel(x, y)

                Dim rl = mtr2.RedPixel(x, y)
                Dim gl = mtr2.GreenPixel(x, y)
                Dim bl = mtr2.BluePixel(x, y)

                If totalBits = 12 Then
                    Dim r = ((rh And &HFF) << 4) Or ((rl And &HF))
                    Dim g = ((gh And &HFF) << 4) Or ((gl And &HF))
                    Dim b = ((bh And &HFF) << 4) Or ((bl And &HF))

                    frame.Data((x + y * frame.Width) * 3 + 0) = b
                    frame.Data((x + y * frame.Width) * 3 + 1) = g
                    frame.Data((x + y * frame.Width) * 3 + 2) = r
                End If
            Next
        Next
        Return frame
    End Function

    <Extension()>
    Public Function ConvertHDR1(frame As RawIntFrame, baseGain As Integer) As RGBMatrix
        Dim mtr1 As New RGBMatrix(frame.Width, frame.Height)
        For i = 0 To frame.Width * frame.Height - 1
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
    Public Function ConvertHDR1Fast(frame As RawIntFrame, baseGain As Integer) As RGBMatrix
        Dim mtr1 As New RGBMatrix(frame.Width, frame.Height)
        Dim frameData = frame.Data
        Dim p = 255
        Dim k = 14
        If baseGain <= 4 Then
            Dim bitShift = 4 - baseGain
            Parallel.For(0, frame.Width * frame.Height, Sub(i As Integer)
                                                            Dim b = frameData(i * 3 + 0) >> bitShift
                                                            Dim g = frameData(i * 3 + 1) >> bitShift
                                                            Dim r = frameData(i * 3 + 2) >> bitShift
                                                            While (r > p Or g > p Or b > p)
                                                                r = (r * k) >> 4
                                                                g = (g * k) >> 4
                                                                b = (b * k) >> 4
                                                            End While
                                                            mtr1.Red(i) = r
                                                            mtr1.Green(i) = g
                                                            mtr1.Blue(i) = b
                                                        End Sub)
        Else
            Dim bitShift = baseGain - 4
            Parallel.For(0, frame.Width * frame.Height, Sub(i As Integer)
                                                            Dim b = frameData(i * 3 + 0) << bitShift
                                                            Dim g = frameData(i * 3 + 1) << bitShift
                                                            Dim r = frameData(i * 3 + 2) << bitShift
                                                            While (r > p Or g > p Or b > p)
                                                                r = (r * k) >> 4
                                                                g = (g * k) >> 4
                                                                b = (b * k) >> 4
                                                            End While
                                                            mtr1.Red(i) = r
                                                            mtr1.Green(i) = g
                                                            mtr1.Blue(i) = b
                                                        End Sub)
        End If
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

    <Extension()>
    Public Function ConvertHDR3(frame As RawIntFrame) As RGBMatrix
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
    Public Function ConvertHDR3Fast(frame As RawIntFrame) As RGBMatrix
        Static powTable As Integer()
        If powTable Is Nothing Then
            powTable = New Integer((1 << 12) - 1) {}
            Dim v = 0.4
            Dim k = 12
            For powArg = 0 To powTable.Length - 1
                powTable(powArg) = Math.Min(Math.Pow(powArg, v) * k, 255)
            Next
        End If
        Dim mtr1 As New RGBMatrix(frame.Width, frame.Height)
        Dim frameData = frame.Data
        Parallel.For(0, 3, Sub(channel As Integer)
                               Dim matrix = mtr1.Matrix(2 - channel)
                               For i = 0 To matrix.Length - 1
                                   Dim px = frameData(i * 3 + channel)
                                   matrix(i) = powTable(px)
                               Next
                           End Sub)
        Return mtr1
    End Function
End Module
