Imports System.Text

Public Class CharInfo
    Public Property Value As Char
    Public Property Left As Integer
    Public Property Right As Integer
    Public Property Top As Integer
    Public Property Bottom As Integer

    Public Overrides Function ToString() As String
        Return Value.ToString
    End Function

    Public Function ToRectangle() As Rectangle
        Return New Rectangle(Left, Top, Right - Left, Bottom - Top)
    End Function

    Public ReadOnly Property Width As Integer
        Get
            Return Right - Left
        End Get
    End Property

    Public ReadOnly Property Height As Integer
        Get
            Return Bottom - Top
        End Get
    End Property

    Public ReadOnly Property WHRatio As Single
        Get
            If Height = 0 Then Return 0
            Return Width / Height
        End Get
    End Property

End Class

Public Class CharCollection
    Public ReadOnly Property Font As Font
    Public ReadOnly Property Chars As CharInfo()
    Public ReadOnly Property Matrix As GrayMatrix

    Private Sub New(f As Font, cs As CharInfo(), mtr As GrayMatrix)
        Me.Font = f
        Me.Chars = cs
        Me.Matrix = mtr
    End Sub

    Public Shared Function Create(fontName As String, Optional fontSizePixels As Integer = 40, Optional fontStyle As FontStyle = FontStyle.Regular) As CharCollection
        Dim font = New Font(fontName, fontSizePixels, fontStyle, GraphicsUnit.Pixel)
        Return Create(font)
    End Function

    Public Function FindCharInfo(chr As Char) As CharInfo
        For Each ci In Chars
            If ci.Value = chr Then Return ci
        Next
        Return Nothing
    End Function

    Public Function FindCharInfo(str As String) As CharInfo
        Return FindCharInfo(str(0))
    End Function

    Public Shared Function Create(font As Font) As CharCollection
        Dim str = GetCharsString(CharTypeEnum.RussianLower Or CharTypeEnum.Digit)
        Dim testBmp As New Bitmap(100, 100)
        Dim testGraphics = Graphics.FromImage(testBmp)
        Dim x = 5
        Dim y = 5
        Dim chars As New List(Of CharInfo)
        Dim maxHeight As Integer = 0
        For Each c In str
            Dim ci As New CharInfo
            ci.Value = c
            Dim size = testGraphics.MeasureString(c, font)
            ci.Left = x
            ci.Top = y
            ci.Right = x + size.Width
            ci.Bottom = y + size.Height
            x += size.Width
            If size.Height > maxHeight Then maxHeight = size.Height
            chars.Add(ci)
        Next
        testGraphics.Dispose()
        testBmp.Dispose()

        Dim bmp As New Bitmap(x, maxHeight + 10)
        Dim graph = Graphics.FromImage(bmp)
        graph.TextRenderingHint = Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit
        graph.Clear(Color.White)
        For Each ci In chars
            graph.DrawString(ci.Value, font, Brushes.Black, ci.Left, ci.Top)
            ' graph.DrawRectangle(Pens.Yellow, ci.Rect.ToRectangle)
        Next
        ' graph.Dispose()
        Dim mtr = bmp.BitmapToGrayMatrix
        For Each ci In chars
            RefineRect(mtr, ci)
        Next

        For Each ci In chars
            graph.DrawRectangle(Pens.Red, ci.ToRectangle)
        Next
        bmp.Save("test.bmp")
        bmp.Dispose()

        Dim cc As New CharCollection(font, chars.ToArray, mtr)
        Return cc
    End Function

    Private Shared Sub RefineRect(mtr As GrayMatrix, ci As CharInfo)
        Dim val = 250
        For y = ci.Top To ci.Bottom
            Dim cleanLine As Boolean = True
            For x = ci.Left To ci.Right
                If mtr.GrayPixel(x, y) < val Then cleanLine = False
            Next
            If Not cleanLine Then
                ci.Top = y - 1
                Exit For
            End If
        Next

        For y = ci.Bottom - 1 To ci.Top Step -1
            Dim cleanLine As Boolean = True
            For x = ci.Left To ci.Right
                If mtr.GrayPixel(x, y) < val Then cleanLine = False
            Next
            If Not cleanLine Then
                ci.Bottom = y + 1
                Exit For
            End If
        Next

        For x = ci.Left To ci.Right
            Dim cleanLine As Boolean = True
            For y = ci.Top To ci.Bottom
                If mtr.GrayPixel(x, y) < val Then cleanLine = False
            Next
            If Not cleanLine Then
                ci.Left = x - 1
                Exit For
            End If
        Next

        For x = ci.Right To ci.Left Step -1
            Dim cleanLine As Boolean = True
            For y = ci.Top To ci.Bottom
                If mtr.GrayPixel(x, y) < val Then cleanLine = False
            Next
            If Not cleanLine Then
                ci.Right = x + 1
                Exit For
            End If
        Next
    End Sub

    Public Shared Function GetCharsString(include As CharTypeEnum) As String
        Dim english = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        Dim russian = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ"
        Dim digits = "0123456789"
        Dim symbolsBase = ".,!?:;-""()«»+/\"
        Dim symbolsExt = "''#%&*@{}•[]"

        Dim sb As New StringBuilder
        If include And CharTypeEnum.RussianUpper Then sb.Append(russian)
        If include And CharTypeEnum.RussianLower Then sb.Append(russian.ToLower)
        If include And CharTypeEnum.EnglishUpper Then sb.Append(english)
        If include And CharTypeEnum.EnglishLower Then sb.Append(english.ToLower)
        If include And CharTypeEnum.Digit Then sb.Append(digits)
        If include And CharTypeEnum.SymbolBase Then sb.Append(symbolsBase)
        If include And CharTypeEnum.SymbolExt Then sb.Append(symbolsExt)
        Dim result = sb.ToString
        Return result
    End Function


    Public Enum CharTypeEnum
        EnglishLower = 1
        EnglishUpper = 2
        English = 3
        RussianLower = 4
        RussianUpper = 8
        Russian = 12
        Digit = 16
        SymbolBase = 32
        SymbolExt = 64
        Symbol = 96
    End Enum

End Class

