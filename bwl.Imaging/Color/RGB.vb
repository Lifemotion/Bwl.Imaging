Public Structure RGB
    'Новых конструкторов добавлять не надо!
    Implements IRGBConvertable

    Public A As Integer
    Public R As Integer
    Public G As Integer
    Public B As Integer

    Public Sub New(r As Integer, g As Integer, b As Integer)
        Me.R = r
        Me.G = g
        Me.B = b
        Me.A = 255
    End Sub

    Public Sub New(r As Integer, g As Integer, b As Integer, a As Integer)
        Me.R = r
        Me.G = g
        Me.B = b
        Me.A = a
    End Sub

    Public Sub New(rgb As Color)
        R = rgb.R
        G = rgb.G
        B = rgb.B
        A = rgb.A
    End Sub

    Public Shared Function FromHsv(hsv As HSV) As RGB
        Return FromHsv(hsv.H, hsv.S, hsv.V, 255)
    End Function

    Public Shared Function FromHsv(h As Integer, s As Integer, v As Integer) As RGB
        Return FromHsv(h, s, v, 255)
    End Function

    Public Shared Function FromHsv(h As Integer, s As Integer, v As Integer, alpha As Integer) As RGB
        Dim valuemax = 255
        Dim rgb As RGB
        Dim hi = Math.Floor(h / 60.0) Mod 6
        Dim vmin = (valuemax - s) * v / valuemax
        Dim a = (v - vmin) * ((h Mod 60) / 60.0)
        Dim vinc = vmin + a
        Dim vdec = v - a
        Select Case hi
            Case 0 : rgb.R = v : rgb.G = vinc : rgb.B = vmin
            Case 1 : rgb.R = vdec : rgb.G = v : rgb.B = vmin
            Case 2 : rgb.R = vmin : rgb.G = v : rgb.B = vinc
            Case 3 : rgb.R = vmin : rgb.G = vdec : rgb.B = v
            Case 4 : rgb.R = vinc : rgb.G = vmin : rgb.B = v
            Case 5 : rgb.R = v : rgb.G = vmin : rgb.B = vdec
        End Select
        rgb.A = alpha
        Return rgb
    End Function

    Public Function ToHSV() As HSV
        Return HSV.FromRgb(Me)
    End Function

    Public Function ToColor() As Color
        Return Color.FromArgb(Limit(A), Limit(R), Limit(G), Limit(B))
    End Function

    Public Shared Function Limit(data As Integer) As Byte
        If data < 0 Then data = 0
        If data > 255 Then data = 255
        Return data
    End Function

    Public Function ToRGB() As RGB Implements IRGBConvertable.ToRGB
        Return Me
    End Function
End Structure
