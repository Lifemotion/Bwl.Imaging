Imports System.Runtime.InteropServices

'Using ap As New AutoPinner(MyManagedObject)
'   UnmanagedIntPtr = ap ' Use the operator to retrieve the IntPtr
'   'do your stuff
'End Using

Friend Class AutoPinner
    Implements IDisposable

    Private _pinnedArray As GCHandle

    Public Sub New(ByVal obj As Object)
        _pinnedArray = GCHandle.Alloc(obj, GCHandleType.Pinned)
    End Sub

    Public Function GetIntPtr() As IntPtr
        Return _pinnedArray.AddrOfPinnedObject()
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        _pinnedArray.Free()
    End Sub
End Class
