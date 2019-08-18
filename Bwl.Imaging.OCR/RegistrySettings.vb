'   Copyright 2016 Igor Koshelev (igor@lifemotion.ru)

'   Licensed under the Apache License, Version 2.0 (the "License");
'   you may Not use this file except In compliance With the License.
'   You may obtain a copy Of the License at

'     http://www.apache.org/licenses/LICENSE-2.0

'   Unless required by applicable law Or agreed To In writing, software
'   distributed under the License Is distributed On an "AS IS" BASIS,
'   WITHOUT WARRANTIES Or CONDITIONS Of ANY KIND, either express Or implied.
'   See the License For the specific language governing permissions And
'   limitations under the License.

Imports System.Windows.Forms

Public Class RegistryStorage
    Public Class Setting
        Dim _storage As RegistryStorage
        Friend Sub New(storage As RegistryStorage, name As String)
            _storage = storage
            _Name = name
        End Sub
        Public ReadOnly Property Name As String
        Public Property DefaultValue As String
        Public Property Value As String
            Get
                Return CType(_storage.Key.GetValue(Name, DefaultValue), String)
            End Get
            Set(value As String)
                _storage.Key.SetValue(Name, value)
            End Set
        End Property
    End Class

    Private _key As Microsoft.Win32.RegistryKey

    Public Sub New()
        _key = GetRegistryAppKey()
    End Sub

    Public Sub New(keyName As String)
        _key = GetRegistryKey(keyName)
    End Sub

    Public ReadOnly Property Key As Microsoft.Win32.RegistryKey
        Get
            Return _key
        End Get
    End Property

    Public Function CreateSetting(name As String, defaultValue As String) As Setting
        Dim setting = New Setting(Me, name)
        setting.DefaultValue = defaultValue
        Return setting
    End Function

    Public Shared Function GetRegistryKey(name As String) As Microsoft.Win32.RegistryKey
        Dim key = My.Computer.Registry.CurrentUser.OpenSubKey(name, True)
        If key Is Nothing Then
            key = My.Computer.Registry.CurrentUser.CreateSubKey(name)
        End If
        Return key
    End Function

    Public Shared Function GetRegistryAppKey() As Microsoft.Win32.RegistryKey
        Return GetRegistryKey("Bwl " + Application.ProductName)
    End Function
End Class
