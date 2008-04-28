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
        /// Protection scope
        /// </summary>
        [ThreadStatic]
        private static IHandleProtector _current = new NullProtector();

        /// <summary>
        /// Gets the current protection scope
        /// </summary>
        internal static IHandleProtector Current
        {
            [DebuggerStepThrough]
            get { return _current; }
            [DebuggerStepThrough]
            private set { _current = value; }
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
            /// Previous protector instance
            /// </summary>
            private readonly IHandleProtector _previous;

            /// <summary>
            /// Collection of the protected handles
            /// </summary>
            private Dictionary<IntPtr, Element> _elementsInUse;

            /// <summary>
            /// 
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

            /// <summary>
            /// Creates a wrapper for the helement handle
            /// </summary>            
            public Element GetElement(IntPtr helement)
            {
                var element = default(Element);
                if (_elementsInUse != null && _elementsInUse.TryGetValue(helement, out element))
                    return element;

                if (_previous != null)
                    element = _previous.GetElement(helement);

                if (element == null && Object.ReferenceEquals(Current, this))
                {
                    SciterHostApi.SciterDomApi.UseElement(helement);
                    element = Element.CreateInternal(helement);

                    _elementsInUse = _elementsInUse ?? new Dictionary<IntPtr, Element>();
                    _elementsInUse.Add(helement, element);                    
                }

                return element;
            }
        }
    }
}
