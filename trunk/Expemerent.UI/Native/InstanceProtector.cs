using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Expemerent.UI.Native
{
    internal static class InstanceProtector
    {
        /// <summary>
        /// Synchronization object
        /// </summary>
        private static readonly object _syncRoot = new object();

        /// <summary>
        /// Collection of allocated handles
        /// </summary>
        private static Dictionary<IntPtr, GCHandle> _instances = new Dictionary<IntPtr, GCHandle>();

        /// <summary>
        /// Protectes object instance from garbage collection
        /// </summary>
        public static IntPtr Protect(object instance)
        {
            lock (_syncRoot)
            {
                GCHandle handle = new GCHandle();
                if (FindHandle(instance, out handle))
                    ClearUnusedHandles();

                if (handle.IsAllocated)
                    return (IntPtr)handle;

                handle = GCHandle.Alloc(instance, GCHandleType.Weak);
                var key = (IntPtr) handle;
                _instances[key] = handle;

                return key;
            }
        }

        /// <summary>
        /// Returns instance of the "protected" object
        /// </summary>
        public static object GetInstance(IntPtr cookie)
        {
            lock (_syncRoot)
            {

                var handle = _instances[cookie];
                var obj = handle.Target;

                if (obj != null)
                    return obj;

                ClearUnusedHandles();
            }

            return null;
        }

        /// <summary>
        /// Locates object handle by specified instance
        /// </summary>
        private static bool FindHandle(object instance, out GCHandle handle)
        {
            bool haveUnused = false;

            handle = new GCHandle();
            foreach (var item in _instances)
            {
                object obj = item.Value.Target;
                haveUnused |= obj == null;

                if (ReferenceEquals(instance, obj))
                {
                    handle = item.Value;
                    break;
                }
            }

            return haveUnused;
        }

        /// <summary>
        /// Removing unused handles from the list
        /// </summary>
        private static void ClearUnusedHandles()
        {
            var cookies = new List<IntPtr>(_instances.Keys);
            for (var i = 0; i < cookies.Count; ++i)
            {
                var key = cookies[i];
                if (_instances[key].Target == null)
                    _instances.Remove(key);
            }
        }
    }
}
