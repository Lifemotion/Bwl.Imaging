Imports Bwl.Imaging

Public Class RawIntFrame
    Public Data As Integer()
    Public ReadOnly Property Width As Integer
    Public ReadOnly Property Height As Integer

    Public Sub New(width As Integer, height As Integer)
        _Width = width
        _Height = height
    End Sub

    Public Sub Save(filename As String)
        Dim fs As New IO.FileStream(filename, IO.FileMode.OpenOrCreate, IO.FileAccess.Write)
        Dim sw As New IO.StreamWriter(fs)
        sw.WriteLine(Width)
        sw.WriteLine(Height)
        For i = 0 To Data.Length - 1
            sw.WriteLine(Data(i))
        Next
        sw.Flush()
        fs.Close()
        fs.Dispose()
    End Sub

    Public Shared Function FromFile(filename As String) As RawIntFrame
        Dim fs As New IO.FileStream(filename, IO.FileMode.Open, IO.FileAccess.Read)
        Dim sw As New IO.StreamReader(fs)
        Dim frame As New RawIntFrame(sw.ReadLine, sw.ReadLine)
        Dim arr(frame.Width * frame.Height * 3) As Integer
        frame.Data = arr
        For i = 0 To frame.Data.Length - 1
            frame.Data(i) = sw.ReadLine()
        Next
        fs.Close()
        Return frame
    End Function


End Class
