Imports System.Collections.Concurrent
Imports System.Threading

Public Interface ICollectedResource
    Inherits IComparable(Of ICollectedResource)
    ReadOnly Property CollectTimeUtc As Date
    Function TryCollect() As Boolean
End Interface

Public NotInheritable Class ResourceCollector(Of T)
    Private Shared ReadOnly _resources As LinkedList(Of ICollectedResource) = New LinkedList(Of ICollectedResource)()
    Private Shared ReadOnly _incoming As ConcurrentQueue(Of ICollectedResource) = New ConcurrentQueue(Of ICollectedResource)()
    Private Shared _count, _procTimeMs As Long
    Public Shared ReadOnly Property Count As Long
        Get
            Return Interlocked.Read(_count)
        End Get
    End Property
    Public Shared ReadOnly Property ProcTimeMs As Long
        Get
            Return Interlocked.Read(_procTimeMs)
        End Get
    End Property
    Public Shared PeriodMs As Double = 250.0
    Shared Sub New()

        Call Task.Run(Function() ResourceCollectorTask().ConfigureAwait(False))
    End Sub
    Public Shared Sub Add(item As ICollectedResource)
        _incoming.Enqueue(item)
    End Sub
    Private Shared Async Function ResourceCollectorTask() As Task
        Dim sw = New Stopwatch()
        Dim node As LinkedListNode(Of ICollectedResource) = Nothing
        Dim item As ICollectedResource = Nothing
        While True
            sw.Restart()
            Dim nowUtc = Date.UtcNow
            While _incoming.Any()
                _incoming.TryDequeue(item)
                node = _resources.Last
                While node IsNot Nothing AndAlso node.Value.CompareTo(item) > 0
                    node = node.Previous
                End While
                If node IsNot Nothing Then
                    _resources.AddAfter(node, item)
                Else
                    _resources.AddFirst(item)
                End If
                Interlocked.Increment(_count)
            End While
            node = _resources.First
            While node IsNot Nothing
                Dim current = node
                node = node.Next
                If current.Value.CollectTimeUtc > nowUtc Then Exit While
                Dim collected As Boolean

                Try
                    collected = current.Value.TryCollect()
                Catch
                    collected = False
                End Try
                If collected Then
                    _resources.Remove(current)
                    Interlocked.Decrement(_count)
                End If
            End While
            sw.Stop()
            _procTimeMs = sw.ElapsedMilliseconds
            Await Task.Delay(TimeSpan.FromMilliseconds(Math.Max(PeriodMs - sw.Elapsed.TotalMilliseconds, 0))).ConfigureAwait(False)
        End While
    End Function
End Class
