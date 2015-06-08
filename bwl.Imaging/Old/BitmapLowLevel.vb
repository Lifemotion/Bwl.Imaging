Public Class BitmapLowLevel
    Enum ReturnBitmapColor
        Gray = 0
        Color = 1
        Gray2D = 5
        Color3D = 6
    End Enum

    Structure StatsStructure
        Dim BrMin As Byte
        Dim BrMax As Byte
        Dim BrAvg As Byte
    End Structure

    Private bmpWidth, bmpHeight As Integer
    Private bmpHistogram(255) As Integer
    Private bmpStats As StatsStructure
    Private bmpLoaded As Boolean
    Public tag As Object
    Public bytesGray() As Integer, bytesGrayEnabled As Boolean = True
    Public bytesGray2D(,) As Integer, bytesGray2DEnabled As Boolean = True
    Public bytesColor(2)() As Integer, bytesColorEnabled As Boolean = True
    Public bytesColor3D(,,) As Integer, bytesColor3DEnabled As Boolean = True

    Public ReadOnly Property PixelLastIndex() As Integer
        Get
            Return bmpWidth * bmpHeight - 1
        End Get
    End Property

    Public Sub LoadFromBitmap(ByRef SrcBitmap As Bitmap)
        Dim tmpBD As BitmapData
        Dim tmpRect As Rectangle
        tmpRect = Rectangle.FromLTRB(0, 0, SrcBitmap.Width, SrcBitmap.Height)
        tmpBD = SrcBitmap.LockBits(tmpRect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb)
        Dim size As Integer = SrcBitmap.Width * SrcBitmap.Height
        Dim tmpBytes As Byte()
        ReDim tmpBytes(size * 3)
        ReDim bytesGray(size)
        ReDim bytesColor(0)(size)
        ReDim bytesColor(1)(size)
        ReDim bytesColor(2)(size)
        ReDim bytesGray2D(SrcBitmap.Width - 1, SrcBitmap.Height - 1)
        ReDim bytesColor3D(2, SrcBitmap.Width - 1, SrcBitmap.Height - 1)
        Runtime.InteropServices.Marshal.Copy(tmpBD.Scan0, tmpBytes, 0, size * 3)
        bmpWidth = SrcBitmap.Width
        bmpHeight = SrcBitmap.Height
        Dim i, x, y As Integer
        For i = 0 To size - 1
            bytesGray(i) = tmpBytes(i * 3) * 0.222 + tmpBytes(i * 3 + 1) * 0.707 + tmpBytes(i * 3 + 2) * 0.071
            bytesGray2D(x, y) = bytesGray(i)
            bytesColor(0)(i) = tmpBytes(i * 3)
            bytesColor(1)(i) = tmpBytes(i * 3 + 1)
            bytesColor(2)(i) = tmpBytes(i * 3 + 2)
            bytesColor3D(0, x, y) = tmpBytes(i * 3)
            bytesColor3D(1, x, y) = tmpBytes(i * 3 + 1)
            bytesColor3D(2, x, y) = tmpBytes(i * 3 + 2)
            x += 1
            If x = bmpWidth Then
                x = 0
                y += 1
            End If
        Next
        SrcBitmap.UnlockBits(tmpBD)
        bmpLoaded = True
    End Sub

    Public Sub LoadFromFile(ByVal Filename As String)
        bmpLoaded = False
        Try
            Dim tmpBitmap As Bitmap
            tmpBitmap = Bitmap.FromFile(Filename)
            LoadFromBitmap(tmpBitmap)
        Catch ex As Exception
            'Throw ex
        End Try
    End Sub

    Public Function GetBitmap(Optional ByVal returnMode As ReturnBitmapColor = ReturnBitmapColor.Gray) As Bitmap
        If IsLoaded Then
            Dim tmpBitmap As New Bitmap(bmpWidth, bmpHeight, PixelFormat.Format24bppRgb)
            Dim tmpBD As BitmapData
            Dim tmpRect As Rectangle
            tmpRect = Rectangle.FromLTRB(0, 0, bmpWidth, bmpHeight)
            tmpBD = tmpBitmap.LockBits(tmpRect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb)
            Dim size As Integer = bmpWidth * bmpHeight
            Dim tmpBytes As Byte()
            ReDim tmpBytes(size * 3)
            Dim i, x, y As Integer
            If returnMode = ReturnBitmapColor.Gray Then
                If Not bytesGrayEnabled Then Throw New Exception("Mode not enabled!")
                For i = 0 To size - 1
                    tmpBytes(i * 3) = bytesGray(i)
                    tmpBytes(i * 3 + 1) = bytesGray(i)
                    tmpBytes(i * 3 + 2) = bytesGray(i)
                Next
            End If
            If returnMode = ReturnBitmapColor.Gray2D Then
                If Not bytesGray2DEnabled Then Throw New Exception("Mode not enabled!")
                For i = 0 To size - 1
                    tmpBytes(i * 3) = bytesGray2D(x, y)
                    tmpBytes(i * 3 + 1) = bytesGray2D(x, y)
                    tmpBytes(i * 3 + 2) = bytesGray2D(x, y)
                    x += 1
                    If x = bmpWidth Then
                        x = 0
                        y += 1
                    End If
                Next
            End If
            If returnMode = ReturnBitmapColor.Color Then
                If Not bytesColorEnabled Then Throw New Exception("Mode not enabled!")
                For i = 0 To size - 1
                    tmpBytes(i * 3) = bytesColor(0)(i)
                    tmpBytes(i * 3 + 1) = bytesColor(1)(i)
                    tmpBytes(i * 3 + 2) = bytesColor(2)(i)
                Next
            End If
            If returnMode = ReturnBitmapColor.Color3D Then
                If Not bytesColor3DEnabled Then Throw New Exception("Mode not enabled!")
                For i = 0 To size - 1
                    tmpBytes(i * 3) = bytesColor3D(0, x, y)
                    tmpBytes(i * 3 + 1) = bytesColor3D(1, x, y)
                    tmpBytes(i * 3 + 2) = bytesColor3D(2, x, y)
                    x += 1
                    If x = bmpWidth Then
                        x = 0
                        y += 1
                    End If
                Next
            End If

            System.Runtime.InteropServices.Marshal.Copy(tmpBytes, 0, tmpBD.Scan0, size * 3)
            tmpBitmap.UnlockBits(tmpBD)
            bmpLoaded = True
            Return tmpBitmap
        Else
            Return Nothing
        End If
    End Function

    Public Sub DrawToPictureBox(ByRef pictureBox As PictureBox, Optional ByVal returnMode As ReturnBitmapColor = ReturnBitmapColor.Gray)
        If IsLoaded And bmpWidth > 0 And bmpHeight > 0 Then
            Dim bmp As Bitmap = GetBitmap(returnMode)
            pictureBox.Image = bmp
        End If
    End Sub

    ReadOnly Property Width() As Integer
        Get
            Return bmpWidth
        End Get
    End Property

    ReadOnly Property Height() As Integer
        Get
            Return bmpHeight
        End Get
    End Property

    Public Function Histogram() As Integer()

        Dim i As Integer, tmpHist(255) As Long, tmpMax As Long
        For i = 0 To bmpWidth * bmpHeight - 1
            tmpHist(bytesGray(i)) += 1
        Next

        For i = 0 To 255
            tmpMax += tmpHist(i) / 255
        Next

        For i = 0 To 255
            bmpHistogram(i) = tmpHist(i) \ (tmpMax \ 128 + 1)
        Next

        Return bmpHistogram

    End Function

    Public Function GetStats() As StatsStructure
        Dim i As Integer, tmpSum As Long
        bmpStats.BrMin = 255
        bmpStats.BrMax = 0
        For i = 0 To bmpWidth * bmpHeight - 1
            If bytesGray(i) < bmpStats.BrMin Then bmpStats.BrMin = bytesGray(i)
            If bytesGray(i) > bmpStats.BrMax Then bmpStats.BrMax = bytesGray(i)
            tmpSum += bytesGray(i)
        Next
        Return bmpStats
    End Function
   
    Sub CopyFromBitmapLowLevel(ByVal Src As BitmapLowLevel)
        If Src.IsLoaded Then
            bmpWidth = Src.Width
            bmpHeight = Src.Height
            Dim size As Integer = bmpWidth * bmpHeight
            ReDim bytesGray(size)
            ReDim bytesColor(0)(size)
            ReDim bytesColor(1)(size)
            ReDim bytesColor(2)(size)
            Array.Copy(Src.bytesGray, bytesGray, size)
            Array.Copy(Src.bytesColor(0), bytesColor(0), size)
            Array.Copy(Src.bytesColor(1), bytesColor(1), size)
            Array.Copy(Src.bytesColor(2), bytesColor(2), size)
            bmpLoaded = Src.bmpLoaded
        End If
    End Sub

    Public ReadOnly Property IsLoaded() As Boolean
        Get
            Return (bmpLoaded)
        End Get
    End Property

    Public Sub SetSizeNoSave(ByVal newWidth As Integer, ByVal newHeight As Integer, Optional ByVal backColor As Long = 255)
        bmpHeight = newHeight
        bmpWidth = newWidth
        ReDim bytesGray(newWidth * newHeight)
        ReDim bytesColor(0)(newWidth * newHeight)
        ReDim bytesColor(1)(newWidth * newHeight)
        ReDim bytesColor(2)(newWidth * newHeight)
        bmpLoaded = True
    End Sub

    Sub New()
        CreateNew(0, 0)
    End Sub

    Public Sub CreateNew(ByVal newWidth As Integer, ByVal newHeight As Integer)
        bmpHeight = newHeight
        bmpWidth = newWidth
        '    ReDim bytesGray(newWidth * newHeight)
        '    ReDim bytesColor(0)(newWidth * newHeight)
        '    ReDim bytesColor(1)(newWidth * newHeight)
        '   ReDim bytesColor(2)(newWidth * newHeight)
        ReDim bytesGray2D(newWidth - 1, newHeight - 1)
        '   ReDim bytesColor3D(2, newWidth - 1, newHeight - 1)
        bmpLoaded = True
    End Sub

    Public Sub CopyColorToGray()
        Dim i As Integer
        For i = 0 To bmpHeight * bmpWidth - 1
            bytesGray(i) = bytesColor(0)(i) * 0.222 + bytesColor(1)(i) * 0.707 + bytesColor(2)(i) * 0.071
        Next
    End Sub
End Class
