using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

public interface ICollectedResource : IComparable<ICollectedResource>
{
    DateTime CollectTimeUtc { get; }
    bool TryCollect();
}

public static class ResourceCollector<T>
{
    private static readonly LinkedList<ICollectedResource> _resources = new LinkedList<ICollectedResource>();
    private static readonly ConcurrentQueue<ICollectedResource> _incoming = new ConcurrentQueue<ICollectedResource>();
    private static long _count, _procTimeMs;
    public static long Count => Interlocked.Read(ref _count);
    public static long ProcTimeMs => Interlocked.Read(ref _procTimeMs);
    public static double PeriodMs = 250.0;
    static ResourceCollector() => new Thread(ResourceCollectorThread) { IsBackground = true }.Start();
    public static void Add(ICollectedResource item) { _incoming.Enqueue(item); }
    private static void ResourceCollectorThread()
    {
        var sw = new Stopwatch();
        LinkedListNode<ICollectedResource> node = null;
        while (true)
        {
            sw.Restart();
            var nowUtc = DateTime.UtcNow;
            while (_incoming.Any())
            {
                _incoming.TryDequeue(out ICollectedResource item);
                node = _resources.Last;
                while ((node != null) && (node.Value.CompareTo(item) > 0)) node = node.Previous;
                if (node != null) _resources.AddAfter(node, item); else _resources.AddFirst(item);
                Interlocked.Increment(ref _count);
            }
            node = _resources.First;
            while (node != null)
            {
                var current = node; node = node.Next;
                if (current.Value.CollectTimeUtc > nowUtc) break;
                bool collected; try { collected = current.Value.TryCollect(); } catch { collected = false; }
                if (collected) { _resources.Remove(current); Interlocked.Decrement(ref _count); }
            }
            sw.Stop(); _procTimeMs = sw.ElapsedMilliseconds;
            Thread.Sleep(TimeSpan.FromMilliseconds(Math.Max(PeriodMs - sw.Elapsed.TotalMilliseconds, 0)));
        }
    }
}
