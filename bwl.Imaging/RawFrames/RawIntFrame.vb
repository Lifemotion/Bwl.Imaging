Imports Bwl.Imaging

Public Class RawIntFrame
    Inherits BlobContainer

    Public Data As Integer()
    Public ReadOnly Property Width As Integer
    Public ReadOnly Property Height As Integer

    Public Sub New(width As Integer, height As Integer, data As Integer())
        _Width = width
        _Height = height
        Me.Data = data
        Attributes.Add("Width", width.ToString)
        Attributes.Add("Height", height.ToString)
        Blobs.Add(New IntegerBlob With {.ID = "Scan0", .Data = data})
    End Sub

    Public Sub New(bc As BlobContainer)
        MyBase.New(bc)
        Width = CInt(Attributes("Width"))
        Height = CInt(Attributes("Height"))
        Data = Blobs(0).Data
    End Sub

    Public Shared Function FromLegacyFile(filename As String) As RawIntFrame
        Dim fs As New IO.FileStream(filename, IO.FileMode.Open, IO.FileAccess.Read)
        Dim sw As New IO.StreamReader(fs)
        Dim width = sw.ReadLine
        Dim height = sw.ReadLine
        Dim arr(width * height * 3) As Integer
        For i = 0 To arr.Length - 1
            arr(i) = sw.ReadLine()
        Next
        fs.Close()
        Dim frame As New RawIntFrame(width, height, arr)
        frame.Data = arr
        Return frame
    End Function

    Public Overloads Shared Function FromFile(filename As String) As RawIntFrame
        Dim file As New RawIntFrame(BlobContainer.FromFile(filename))
        Return file
    End Function
End Class
