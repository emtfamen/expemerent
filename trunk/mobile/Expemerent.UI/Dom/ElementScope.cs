using System;
using System.Collections.Generic;
using System.Text;
using Expemerent.UI.Dom;
using System.Diagnostics;
using System.Threading;

namespace Expemerent.UI.Native
{
    /// <summary>
    /// Defines a logical scope which will protect <see cref="Element"/> handles from became invalid
    /// </summary>
    /// <remarks>
    /// Each <see cref="Element"/> should be created within enclosed scope. 
    /// When element goes out of scope it becames invalid. <see cref="ElementRef"/> can be used to protect elements between calls
    /// </remarks>
    public sealed class ElementScope
    {
        /// <summary>
        /// Thread data slot
        /// </summary>
        private static LocalDataStoreSlot _currentSlot = Thread.AllocateDataSlot();

        /// <summary>
        /// Gets the current protection scope
        /// </summary>
        internal static IHandleProtector Current
        {
            get 
            {
                var protectorObj = (IHandleProtector)Thread.GetData(_currentSlot);
                if (protectorObj == null)
                    Thread.SetData(_currentSlot, protectorObj = new NullProtector());

                return protectorObj;
            }
            private set { Thread.SetData(_currentSlot, value); }
        }
        
        /// <summary>
        /// Opens a new scope
        /// </summary>
        public static IDisposable Create()
        {
            var protector = new HandleProtector(Current);
            Current = protector;

            return protector;
        }

        /// <summary>
        /// Defines a base interface for [helement] to [element] mapping
        /// </summary>
        internal interface IHandleProtector
        {
            /// <summary>
            /// Creates a wrapper for the helement handle
            /// </summary>
            Element GetElement(IntPtr helement);
        }

        private class NullProtector : IHandleProtector
        {
            /// <summary>
            /// Does nothing
            /// </summary>
            public Element GetElement(IntPtr helement)
            {
                if (Object.ReferenceEquals(Current, this))
                    throw new InvalidOperationException("All instances of the Element class should be created within ElementScope");

                return null;
            }
        }

        /// <summary>
        /// Protectes HELEMENT handles from being disposed 
        /// </summary>
        private class HandleProtector : IHandleProtector, IDisposable
        {
            /// <summary>
            /// Storage for fast element access for several first items
            /// </summary>
            private struct Pair
            {
                public IntPtr Key;
                public Element Value;
            }

            /// <summary>
            /// Previous protector instance
            /// </summary>
            private readonly IHandleProtector _previous;

            /// <summary>
            /// Collection of the protected handles
            /// </summary>
            private Dictionary<IntPtr, Element> _elementsInUse;

            /// <summary>
            /// Fast access to the first elements
            /// </summary>
            private Pair _eax, _ebx, _ecx, _edx;

            /// <summary>
            /// Creates a new instance of the <see cref="HandleProtector"/> class
            /// </summary>
            public HandleProtector(IHandleProtector previous)
            {
                _previous = previous;
            }

            /// <summary>
            /// Releases all allocated resources
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Finalizer
            /// </summary>
            ~HandleProtector()
            {
                Dispose(false);
            }

            /// <summary>
            /// Releases all allocated resources
            /// </summary>
            protected virtual void Dispose(bool disposing)
            {
                Current = _previous;
                if (disposing)
                {
                    ReleaseElementsFast();
                    if (_elementsInUse != null)
                    {
                        var elements = Interlocked.Exchange(ref _elementsInUse, null);
                        foreach (var item in elements)
                        {
                            item.Value.Drop();
                            SciterHostApi.SciterDomApi.UnuseElement(item.Key);
                        }
                    }
                }
                else
                    Debug.Fail("HandleProtector can be used only within 'using' scope");
            }

            #region Fast get/set routines
            /// <summary>
            /// Releases inline elements
            /// </summary>
            private void ReleaseElementsFast()
            {
                if (_eax.Key != IntPtr.Zero)
                {
                    _eax.Value.Drop();
                    SciterHostApi.SciterDomApi.UnuseElement(_eax.Key);
                    
                    _eax.Key = IntPtr.Zero;
                    _eax.Value = null;
                }

                if (_ebx.Key != IntPtr.Zero)
                {
                    _ebx.Value.Drop();
                    SciterHostApi.SciterDomApi.UnuseElement(_ebx.Key);
                    _ebx.Key = IntPtr.Zero;
                    _ebx.Value = null;
                }

                if (_ecx.Key != IntPtr.Zero)
                {
                    _ecx.Value.Drop();
                    SciterHostApi.SciterDomApi.UnuseElement(_ecx.Key);
                    _ecx.Key = IntPtr.Zero;
                    _ecx.Value = null;
                }

                if (_edx.Key != IntPtr.Zero)
                {
                    _edx.Value.Drop();
                    SciterHostApi.SciterDomApi.UnuseElement(_edx.Key);
                    _edx.Key = IntPtr.Zero;
                    _edx.Value = null;
                }
            }

            /// <summary>
            /// Returns element from the inline storage
            /// </summary>
            private Element GetElementFast(IntPtr helement)
            {
                if (_eax.Key == helement)
                    return _eax.Value;

                if (_ebx.Key == helement)
                    return _ebx.Value;

                if (_ecx.Key == helement)
                    return _ecx.Value;

                if (_edx.Key == helement)
                    return _edx.Value;

                return null;
            }

            /// <summary>
            /// Stores element reference in the inline storage
            /// </summary>
            /// <returns>true if element was successfully stored</returns>
            private bool SetElementFast(IntPtr helement, Element element)
            {
                if (_eax.Key == IntPtr.Zero)
                {
                    _eax.Key = helement;
                    _eax.Value = element;

                    return true;
                }

                if (_ebx.Key == IntPtr.Zero)
                {
                    _ebx.Key = helement;
                    _ebx.Value = element;

                    return true;
                }

                if (_ecx.Key == IntPtr.Zero)
                {
                    _ecx.Key = helement;
                    _ecx.Value = element;

                    return true;
                }

                if (_edx.Key == IntPtr.Zero)
                {
                    _edx.Key = helement;
                    _edx.Value = element;

                    return true;
                }

                return false;
            } 
            #endregion

            /// <summary>
            /// Creates a wrapper for the helement handle
            /// </summary>            
            public Element GetElement(IntPtr helement)
            {
                var element = GetElementFast(helement);
                if (element == null)
                {
                if (_elementsInUse != null && _elementsInUse.TryGetValue(helement, out element))
                    return element;

                if (_previous != null)
                    element = _previous.GetElement(helement);

                if (element == null && Object.ReferenceEquals(Current, this))
                {
                    SciterHostApi.SciterDomApi.UseElement(helement);
                    element = Element.CreateInternal(helement);

                        if (_elementsInUse == null && !SetElementFast(helement, element))
                        {
                    _elementsInUse = _elementsInUse ?? new Dictionary<IntPtr, Element>();
                    _elementsInUse.Add(helement, element);                    
                        }
                    }
                }

                return element;
            }
        }
    }
}
