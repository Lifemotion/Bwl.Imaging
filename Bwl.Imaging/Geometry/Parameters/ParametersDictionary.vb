Public Class ParametersDictionary
    Implements IEnumerable(Of Parameter)

    Private _data As New Dictionary(Of String, Parameter)
    Private _syncRoot As New Object

    Public Sub New()
    End Sub

    Public Sub New(values As IEnumerable(Of Parameter))
        Me.AddRange(values)
    End Sub

    Public Overridable Function GetEnumerator() As IEnumerator(Of Parameter) Implements IEnumerable(Of Parameter).GetEnumerator
        Dim values As IEnumerable(Of Parameter) = Nothing
        SyncLock _syncRoot
            values = _data.Values.ToArray()
        End SyncLock
        Return values.GetEnumerator()
    End Function

    Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Throw New NotImplementedException()
    End Function

    Default Public Overloads Property Item(key As String) As Parameter
        Get
            SyncLock _syncRoot
                Return _data(key)
            End SyncLock
        End Get
        Set(value As Parameter)
            SyncLock _syncRoot
                _data(key) = value
            End SyncLock
        End Set
    End Property

    Public Function ItemValue(key As String) As String
        SyncLock _syncRoot
            Return _data(key).Value
        End SyncLock
    End Function

    Public Overloads Sub Add(key As String, value As String, Optional visible As Boolean = False)
        SyncLock _syncRoot
            _data.Add(key, New Parameter(key, value, visible))
        End SyncLock
    End Sub

    Public Overloads Sub Add(value As Parameter)
        SyncLock _syncRoot
            _data.Add(value.Key, value)
        End SyncLock
    End Sub

    Public Sub AddRange(values As IEnumerable(Of Parameter))
        SyncLock _syncRoot
            For Each value In values
                _data.Add(value.Key, value)
            Next
        End SyncLock
    End Sub

    Public Sub Remove(key As String)
        SyncLock _syncRoot
            _data.Remove(key)
        End SyncLock
    End Sub

    Public Function ContainsKey(key As String) As Boolean
        SyncLock _syncRoot
            Return _data.ContainsKey(key)
        End SyncLock
    End Function

    Public Overloads Sub Clear()
        SyncLock _syncRoot
            _data.Clear()
        End SyncLock
    End Sub

    Public Function ToArray() As Parameter()
        SyncLock _syncRoot
            Return _data.Values.ToArray()
        End SyncLock
    End Function
End Class
