Public Structure HSV
    'Новых конструкторов и ToRgb, ToColor добавлять не надо!

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
        Return FromRgb(rgb.R, rgb.G, rgb.B, rgb.A)
    End Function

    Public Shared Function FromRgb(rgb As Color) As HSV
        Return FromRgb(rgb.R, rgb.G, rgb.B, rgb.A)
    End Function

    Public Shared Function FromRgb(R As Integer, G As Integer, B As Integer) As HSV
        Return FromRgb(R, G, B, 255)
    End Function

    Public Shared Function FromRgb(A As Integer, R As Integer, G As Integer, B As Integer) As HSV
        Dim result As HSV
        Dim rf = R / 255.0
        Dim gf = G / 255.0
        Dim bf = B / 255.0
        Dim cmax = Math.Max(Math.Max(rf, gf), bf)
        Dim cmin = Math.Min(Math.Min(rf, gf), bf)
        'H
        If cmax = cmin Then
            result.H = 0
        ElseIf cmax = rf And gf >= bf Then
            result.H = (60 * (gf - bf) / (cmax - cmin)) / 360
        ElseIf cmax = rf And gf < bf Then
            result.H = (60 * (gf - bf) / (cmax - cmin) + 360) / 360
        ElseIf cmax = gf Then
            result.H = (60 * (bf - rf) / (cmax - cmin) + 120) / 360
        ElseIf cmax = bf Then
            result.H = (60 * (rf - gf) / (cmax - cmin) + 240) / 360
        End If
        'S
        If cmax = 0 Then
            result.S = 0
        Else
            result.S = 1 - (cmin / cmax)
        End If
        'V
        result.V = cmax
        result.A = A
        Return result
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
