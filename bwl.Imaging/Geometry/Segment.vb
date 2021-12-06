Imports System.Drawing

Public Class Segment
    Public Property Left As Integer
    Public Property Top As Integer
    Public Property Width As Integer
    Public Property Height As Integer
    Public Property ID As Integer
    Public Property Tag As Integer
    Public Property Debug As String

    Public Property Right As Integer
        Get
            Return Left + Width
        End Get
        Set(value As Integer)
            Width = value - Left
        End Set
    End Property
    Public Property Bottom As Integer
        Get
            Return Top + Height
        End Get
        Set(value As Integer)
            Height = value - Top
        End Set
    End Property
    Public Overrides Function ToString() As String
        Return "L:" + Left.ToString + " :T" + Top.ToString + " :W" + Width.ToString + " :H" + Height.ToString
    End Function

    Public ReadOnly Property CenterX As Integer
        Get
            Return CInt(Left + Width / 2)
        End Get
    End Property

    Public ReadOnly Property WHRatio As Single
        Get
            If Height = 0 Then Return 0
            Return CInt(Width / Height)
        End Get
    End Property

    Public ReadOnly Property CenterY As Integer
        Get
            Return CInt(Top + Height / 2)
        End Get
    End Property

    Public Function IsPointInside(x As Integer, y As Integer) As Boolean
        Return x >= Left And x <= Left + Width And y >= Top And y <= Top + Height
    End Function

    Public Function ToRectangle() As Rectangle
        Return New Rectangle(Left, Top, Width, Height)
    End Function

    Public Function ToRectangleF() As RectangleF
        Return New RectangleF(Left, Top, Width, Height)
    End Function

    Public Sub New()

    End Sub

    Public Sub New(x As Integer, y As Integer, width As Integer, height As Integer)
        Me.Left = x
        Me.Top = y
        Me.Width = width
        Me.Height = height
    End Sub

End Class