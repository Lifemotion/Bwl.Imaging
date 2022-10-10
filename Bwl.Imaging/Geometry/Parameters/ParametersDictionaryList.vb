Public Class ParametersDictionaryList
    Inherits List(Of Parameter)

    Public Overloads Function Add(key As String, value As String, Optional visible As Boolean = False) As Parameter
        SyncLock Me
            If key Is Nothing OrElse key.Length = 0 Then Throw New Exception("ParametersDictionary::Add(): key Is Nothing OrElse key.Length = 0")
            For Each param In Me
                If param.Key.ToLower = key.ToLower Then
                    Throw New Exception("Parameter with this key exists")
                End If
            Next
            Dim newParam As New Parameter(key, value, visible)
            Me.Add(newParam)

            Return newParam
        End SyncLock
    End Function

    Public Overloads Sub Add(addingParameter As Parameter)
        SyncLock Me
            If addingParameter Is Nothing Then Throw New Exception("addingParameter nothing")
            If addingParameter.Key Is Nothing OrElse addingParameter.Key.Length = 0 Then Throw New Exception("ParametersDictionary::Add(): addingParameter.Key Is Nothing")
            For Each param In Me
                If param.Key.ToLower = addingParameter.Key.ToLower Then
                    Throw New Exception("Parameter with this key exists")
                End If
            Next
            MyBase.Add(addingParameter)
        End SyncLock
    End Sub

    Public Overloads Sub Remove(key As String)
        SyncLock Me
            Dim items2Remove = New LinkedList(Of Parameter)
            For Each param In Me
                If param.Key.ToLower = key.ToLower Then
                    items2Remove.AddLast(param)
                End If
            Next

            For Each item2Remove In items2Remove
                MyBase.Remove(item2Remove)
            Next
        End SyncLock
    End Sub

    Public ReadOnly Property ContainsKey(key As String) As Boolean
        Get
            SyncLock Me
                For Each param In Me
                    If param.Key.ToLower = key.ToLower Then
                        Return True
                    End If
                Next

                Return False
            End SyncLock
        End Get
    End Property

    Default Public Overloads Property Item(key As String) As Parameter
        Get
            SyncLock Me
                For Each param In Me
                    If param.Key.ToLower = key.ToLower Then
                        Return param
                    End If
                Next
                Throw New Exception("ParametersDictionary::Add(): key not found")
            End SyncLock
        End Get
        Set(value As Parameter)
            SyncLock Me
                If value Is Nothing Then Throw New Exception("ParametersDictionary::Add(): value Is Nothing")
                For Each param In Me
                    If param.Key.ToLower = key.ToLower Then
                        param.Value = value.Value
                        param.Caption = value.Caption
                        param.Unit = value.Unit
                        param.AdditionalSettings = value.AdditionalSettings
                        param.Visible = value.Visible
                    End If
                Next
                Add(value)
            End SyncLock
        End Set
    End Property

    Public Property ItemValue(key As String) As String
        Get
            SyncLock Me
                For Each param In Me
                    If param.Key.ToLower = key.ToLower Then
                        Return param.Value
                    End If
                Next
                Throw New Exception("ParametersDictionary::ItemValue(): key not found")
            End SyncLock
        End Get
        Set(value As String)
            SyncLock Me
                If value Is Nothing Then Throw New Exception("ParametersDictionary::ItemValue(): value is Nothing")
                For Each param In Me
                    If param.Key.ToLower = key.ToLower Then
                        param.Value = value
                    End If
                Next
                Add(key, value)
            End SyncLock
        End Set
    End Property
End Class
