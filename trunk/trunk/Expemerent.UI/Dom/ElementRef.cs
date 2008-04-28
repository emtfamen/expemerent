using System;
using System.Diagnostics;
using Expemerent.UI.Native;
using System.Threading;

namespace Expemerent.UI.Dom
{
    /// <summary>
    /// Type of the element reference
    /// </summary>
    public enum ElementRefType
    {
        /// <summary>
        /// Element will be referenced by UID
        /// </summary>
        Weak,

        /// <summary>
        /// Element will be referenced by Element handle
        /// </summary>
        Normal
    }

    /// <summary>
    /// Represents a reference to the sciter HTML element. 
    /// The <see cref="ElementRef"/> it is a way to protect element handles between calls
    /// </summary>
    public sealed class ElementRef : IDisposable
    {
        /// <summary>
        /// Handle of the element window
        /// </summary>
        private readonly IntPtr _elementHwnd;

        /// <summary>
        /// See <see cref="ReferenceType"/> property
        /// </summary>
        private readonly ElementRefType _referenceType;

        /// <summary>
        /// Element handle or UID
        /// </summary>
        private IntPtr _element;

        /// <summary>
        /// Creates a new instance of the <see cref="ElementRef"/> class
        /// </summary>
        internal ElementRef(Element he, ElementRefType rt)
            : this(he, rt, false)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ElementRef"/> class
        /// </summary>
        /// <param name="he">Handle will be "attached" without calling Sciter_UseElement</param>
        internal ElementRef(IntPtr he)
            : this (Element.CreateInternal(he), ElementRefType.Normal, true)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ElementRef"/> class
        /// </summary>
        /// <param name="he">element handle</param>
        /// <param name="rt">reference text</param>
        /// <param name="attach">uses existing handle</param>
        private ElementRef(Element he, ElementRefType rt, bool attach)
        {
            Debug.Assert(rt != ElementRefType.Weak || !attach, "Weak handle cannot be attached");

            _referenceType = rt;
            switch (_referenceType)
            {
                case ElementRefType.Weak:
                    GC.SuppressFinalize(this);
                    
                    _elementHwnd = SciterDomApi.GetElementHwnd(he, true);
                    _element = new IntPtr(SciterDomApi.GetElementUID(he));
                    break;
                case ElementRefType.Normal:
                    _element = he.Handle;

                    if (!attach)
                    {
                        SciterDomApi.UseElement(he.Handle);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException("rt", rt.ToString());
            }
        }

        /// <summary>
        /// Gets instance of sciter dom api
        /// </summary>
        private static SciterDomApi SciterDomApi
        {
            [DebuggerStepThrough]
            get { return SciterHostApi.SciterDomApi; }
        }

        /// <summary>
        /// Get text of the element reference
        /// </summary>
        public ElementRefType ReferenceType
        {
            [DebuggerStepThrough]
            get { return _referenceType; }
        }

        /// <summary>
        /// Returns contained element
        /// </summary>
        public Element Element
        {
            get
            {
                switch (_referenceType)
                {
                    case ElementRefType.Weak:
                        return SciterDomApi.GetElementByUID(_elementHwnd, _element.ToInt32());
                    case ElementRefType.Normal:
                        return Element.Create(_element);
                    default:
                        throw new InvalidOperationException("Invalid enum value");
                }
            }
        }

        #region Disposable implementation

        /// <summary>
        /// Finalizer
        /// </summary>
        ~ElementRef()
        {
            Dispose();
        }

        /// <summary>
        /// Disposes element 
        /// </summary>
        void IDisposable.Dispose()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases resources
        /// </summary>
        public void Dispose()
        {
            var element = MarshalUtility.InterlockedExchange(ref _element, IntPtr.Zero);
            
            if (ReferenceType == ElementRefType.Normal && element != IntPtr.Zero)
                SciterDomApi.UnuseElement(element);            
        }

        #endregion
    }
}
