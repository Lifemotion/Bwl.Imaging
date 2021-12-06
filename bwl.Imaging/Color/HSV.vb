Imports System.Drawing

Public Structure HSV
    'Новых конструкторов добавлять не надо!
    Implements IRGBConvertable
    Public A As Integer
    Public H As Integer
    Public S As Integer
    Public V As Integer

    Public Sub New(H As Integer, S As Integer, V As Integer)
        Me.H = H
        Me.S = S
        Me.V = V
        Me.A = 255
    End Sub

    Public Sub New(H As Integer, S As Integer, V As Integer, A As Integer)
        Me.H = H
        Me.S = S
        Me.V = V
        Me.A = A
    End Sub

    Public Shared Function FromRgb(rgb As RGB) As HSV
        Return FromRgb(rgb.A, rgb.R, rgb.G, rgb.B)
    End Function

    Public Shared Function FromRgb(rgb As Color) As HSV
        Return FromRgb(rgb.A, rgb.R, rgb.G, rgb.B)
    End Function

    Public Shared Function FromRgb(R As Integer, G As Integer, B As Integer) As HSV
        Return FromRgb(255, R, G, B)
    End Function

    Public Shared Function FromRgb(A As Integer, R As Integer, G As Integer, B As Integer) As HSV
        Dim result As HSV
        Dim cmax = Math.Max(Math.Max(R, G), B)
        Dim cmin = Math.Min(Math.Min(R, G), B)
        'H
        If cmax = cmin Then
            result.H = 0
        ElseIf cmax = R And G >= B Then
            result.H = Limit((60 * (G - B) / (cmax - cmin)))
        ElseIf cmax = R And G < B Then
            result.H = Limit((60 * (G - B) / (cmax - cmin) + 360))
        ElseIf cmax = G Then
            result.H = Limit((60 * (B - R) / (cmax - cmin) + 120))
        ElseIf cmax = B Then
            result.H = Limit((60 * (R - G) / (cmax - cmin) + 240))
        End If
        'S
        If cmax = 0 Then
            result.S = 0
        Else
            result.S = Limit((1 - (cmin / cmax)) * 255)
        End If
        'V
        result.V = cmax
        result.A = A
        Return result
    End Function

    Public Function ToRgb() As RGB Implements IRGBConvertable.ToRGB
        Return RGB.FromHsv(Me)
    End Function

    Public Function ToColor() As Color
        Return RGB.FromHsv(Me).ToColor
    End Function

    ''' <summary>
    ''' Вычисление кратчайшего расстояния между двумя тонами
    ''' </summary>
    ''' <remarks>Наибольшее возможное расстояние в пространстве Hue - это 0.5 [0..1],
    ''' которое сооотв. противоположному цвету. Если получили больше 0.5, нужно
    ''' нормализовывать результат.</remarks>
    Public Shared Function HueDistance(hue1 As Double, hue2 As Double) As Double
        Dim dist = Math.Abs(hue1 - hue2)
        Return If(dist > 0.5, 1 - dist, dist)
    End Function
End Structure
