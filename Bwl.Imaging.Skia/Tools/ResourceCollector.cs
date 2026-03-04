using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bwl.Imaging.Skia
{

    public interface ICollectedResource : IComparable<ICollectedResource>
    {
        DateTime CollectTimeUtc { get; }
        bool TryCollect();
    }

    public sealed class ResourceCollector<T>
    {
        private static readonly LinkedList<ICollectedResource> _resources = new LinkedList<ICollectedResource>();
        private static readonly ConcurrentQueue<ICollectedResource> _incoming = new ConcurrentQueue<ICollectedResource>();
        private static long _count, _procTimeMs;
        public static long Count
        {
            get
            {
                return Interlocked.Read(ref _count);
            }
        }
        public static long ProcTimeMs
        {
            get
            {
                return Interlocked.Read(ref _procTimeMs);
            }
        }
        public static double PeriodMs = 250.0d;
        static ResourceCollector()
        {

            Task.Run(() => ResourceCollectorTask().ConfigureAwait(false));
        }
        public static void Add(ICollectedResource item)
        {
            _incoming.Enqueue(item);
        }
        private static async Task ResourceCollectorTask()
        {
            var sw = new Stopwatch();
            LinkedListNode<ICollectedResource> node = null;
            ICollectedResource item = null;
            while (true)
            {
                sw.Restart();
                var nowUtc = DateTime.UtcNow;
                while (_incoming.Any())
                {
                    _incoming.TryDequeue(out item);
                    node = _resources.Last;
                    while (node is not null && node.Value.CompareTo(item) > 0)
                        node = node.Previous;
                    if (node is not null)
                    {
                        _resources.AddAfter(node, item);
                    }
                    else
                    {
                        _resources.AddFirst(item);
                    }
                    Interlocked.Increment(ref _count);
                }
                node = _resources.First;
                while (node is not null)
                {
                    var current = node;
                    node = node.Next;
                    if (current.Value.CollectTimeUtc > nowUtc)
                        break;
                    bool collected;

                    try
                    {
                        collected = current.Value.TryCollect();
                    }
                    catch
                    {
                        collected = false;
                    }
                    if (collected)
                    {
                        _resources.Remove(current);
                        Interlocked.Decrement(ref _count);
                    }
                }
                sw.Stop();
                _procTimeMs = sw.ElapsedMilliseconds;
                await Task.Delay(TimeSpan.FromMilliseconds(Math.Max(PeriodMs - sw.Elapsed.TotalMilliseconds, 0d))).ConfigureAwait(false);
            }
        }
    }
}