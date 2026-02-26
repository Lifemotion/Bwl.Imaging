using System;
using System.Runtime.InteropServices;

namespace Bwl.Imaging
{

    // Using ap As New AutoPinner(MyManagedObject)
    // UnmanagedIntPtr = ap ' Use the operator to retrieve the IntPtr
    // 'do your stuff
    // End Using

    internal class AutoPinner : IDisposable
    {

        private GCHandle _pinnedArray;

        public AutoPinner(object obj)
        {
            _pinnedArray = GCHandle.Alloc(obj, GCHandleType.Pinned);
        }

        public IntPtr GetIntPtr()
        {
            return _pinnedArray.AddrOfPinnedObject();
        }

        public void Dispose()
        {
            _pinnedArray.Free();
        }
    }
}