Imports System.IO
Imports System.Text
Imports System.Runtime.Serialization.Json

Public Class Serializer
    Public Shared Function SaveObjectToJsonString(data As Object) As String
        Dim ds = New DataContractJsonSerializer(data.GetType())
        Dim ms = New MemoryStream()
        ds.WriteObject(ms, data)
        ms.Close()
        Return Encoding.ASCII.GetChars(ms.ToArray())
    End Function

    Public Shared Function LoadObjectFromJsonString(Of T)(jsonString As String) As T
        Dim data = Encoding.ASCII.GetBytes(jsonString)
        Dim ds = New DataContractJsonSerializer(GetType(T))
        Return ds.ReadObject(New MemoryStream(data))
    End Function
End Class
