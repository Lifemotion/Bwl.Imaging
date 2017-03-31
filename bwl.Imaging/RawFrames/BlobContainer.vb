Public Class IntegerBlob
    Public ID As String
    Public Data As Integer()

    Public Sub WriteDataToStream(fs As IO.FileStream)
        Dim bytes As Byte()
        For i = 0 To Data.Length - 1
            bytes = BitConverter.GetBytes(Data(i))
            fs.Write(bytes, 0, bytes.Length)
        Next
    End Sub

    Public Sub ReadDataFromStream(fs As IO.FileStream, datalength As Integer)
        ReDim Data(datalength - 1)
        Dim bytes = BitConverter.GetBytes(CInt(0))
        For i = 0 To Data.Length - 1
            fs.Read(bytes, 0, bytes.Length)
            Data(i) = BitConverter.ToInt32(bytes, 0)
        Next
    End Sub
End Class

Public Class BlobContainer
    Public ReadOnly Property Attributes As New Dictionary(Of String, String)
    Public ReadOnly Property Blobs As New List(Of IntegerBlob)

    Public Sub New()

    End Sub

    Public Sub New(bc As BlobContainer)
        Blobs = bc.Blobs
        Attributes = bc.Attributes
    End Sub

    Public Sub Save(filename As String)
        Dim fs As New IO.FileStream(filename, IO.FileMode.OpenOrCreate, IO.FileAccess.ReadWrite)
        For Each attr In Attributes
            WriteLineToStream(fs, attr.Key + "=" + attr.Value + vbCrLf)
        Next
        For Each Blob In Blobs
            WriteLineToStream(fs, "{BlobStart}=" + Blob.ID + "," + Blob.Data.GetType.ToString() + "," + Blob.Data.Length.ToString + vbNullChar)
            Blob.WriteDataToStream(fs)
            WriteLineToStream(fs, "{BlobEnd}=" + Blob.ID + vbCrLf)
        Next
        WriteLineToStream(fs, "{End}={End}" + vbCrLf)
        fs.Close()
        fs.Dispose()
    End Sub

    Private Shared Sub WriteLineToStream(fs As IO.FileStream, line As String)
        Dim bytes = Text.Encoding.UTF8.GetBytes(line)
        fs.Write(bytes, 0, bytes.Length)
    End Sub

    Private Shared Function ReadLineFromStream(fs As IO.FileStream) As String
        Dim bytes As New List(Of Byte)
        Dim read = fs.ReadByte
        Do While read = 10 Or read = 13
            read = fs.ReadByte
        Loop
        Do While read <> 10 And read <> 13 And read <> &H0
            bytes.Add(read)
            read = fs.ReadByte
        Loop
        Return System.Text.Encoding.UTF8.GetString(bytes.ToArray)
    End Function

    Public Shared Function FromFile(filename As String) As BlobContainer
        Dim fs As New IO.FileStream(filename, IO.FileMode.Open, IO.FileAccess.Read)
        Dim file As New BlobContainer
        Do
            Dim keyvalue = ReadLineFromStream(fs).Split("=")
            If keyvalue.Length > 1 Then
                Dim key = keyvalue(0)
                Dim value = keyvalue(1)
                Select Case key
                    Case "{BlobStart}"
                        Dim params = value.Split(",")
                        If params.Length <> 3 Then Throw New Exception("Bad file format")
                        Dim id = params(0)
                        Dim typename = params(1)
                        Dim length = CInt(params(2))
                        Select Case typename
                            Case "System.Int32[]"
                                Dim blob As New IntegerBlob
                                blob.ReadDataFromStream(fs, length)
                                blob.ID = id
                                file.Blobs.Add(blob)
                                Dim endline = ReadLineFromStream(fs).Split("=")
                                If endline(0) <> "{BlobEnd}" Then Throw New Exception("Bad BlobContainer file: BLOB " + id + " not finished with {BlobEnd}")
                            Case Else
                                Throw New Exception("Bad BlobContainer file: unsupported BLOB type:" + typename)
                        End Select
                    Case "{End}"
                        Exit Do
                    Case Else
                        file.Attributes.Add(key, value)
                End Select
            End If
        Loop
        Return file
    End Function

End Class
