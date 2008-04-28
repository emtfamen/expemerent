using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Expemerent.UI.Behaviors;
using Expemerent.UI.Native;

namespace Expemerent.UI.Dom
{
    /// <summary>
    /// Represents a sciter HTML element 
    /// </summary>
    public sealed class Element
    {
        #region Internal data
        /// <summary>
        /// The element handle
        /// </summary>
        private IntPtr _handle;

        /// <summary>
        /// See <see cref="Attributes"/> property
        /// </summary>
        private AttributeCollection _attributes;

        /// <summary>
        /// See <see cref="Style"/> property
        /// </summary>
        private Style _style;
        
        /// <summary>
        /// Gets instance of sciter dom api
        /// </summary>
        private static SciterDomApi SciterDomApi
        {
            [DebuggerStepThrough]
            get { return SciterHostApi.SciterDomApi; }
        }

        /// <summary>
        /// Gets the element handle
        /// </summary>
        internal IntPtr Handle
        {
            [DebuggerStepThrough]
            get 
            {
                if (_handle == null)
                    throw new ObjectDisposedException("Element");

                return _handle; 
            }
            [DebuggerStepThrough]
            private set { _handle = value; }
        }
        #endregion

        #region Construction
        /// <summary>
        /// Creates a new instance of the <see cref="Element"/> class
        /// </summary>
        private Element(IntPtr handle)
        {
            Debug.Assert(handle != IntPtr.Zero, "element cannot be null");
            _handle = handle;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Element"/> class within the protected scope
        /// </summary>
        internal static Element Create(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                return null;

            return ElementScope.Current.GetElement(handle);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Element"/> class
        /// This instance will be "uncounted" and became invalid when handle deleted
        /// </summary>
        internal static Element CreateInternal(IntPtr handle)
        {
            return new Element(handle);
        } 
        #endregion        

        #region Element properties & attributes
        /// <summary>
        /// Gets element tag
        /// </summary>
        public string Tag
        {
            get { return SciterDomApi.GetElementType(this); }
        }

        /// <summary>
        /// Gets or sets element text
        /// </summary>
        public string Text
        {
            get { return SciterDomApi.GetElementText(this); }
            set { SciterDomApi.SetElementText(this, value ?? String.Empty); }
        }

        /// <summary>
        /// Gets or sets element html (utf8 encoded string)
        /// </summary>
        public string InnerHtml
        {
            get { return SciterDomApi.GetElementHtml(this, false); }
            set { SciterDomApi.SetElementHtml(this, value ?? String.Empty, SET_ELEMENT_HTML.SIH_REPLACE_CONTENT); }
        }

        /// <summary>
        /// Gets or sets element html (utf8 encoded string)
        /// </summary>
        public string OuterHtml
        {
            get { return SciterDomApi.GetElementHtml(this, true); }
            set { SciterDomApi.SetElementHtml(this, value ?? String.Empty, SET_ELEMENT_HTML.SOH_REPLACE); }
        } 

        /// <summary>
        /// Gets attributes collection
        /// </summary>
        public AttributeCollection Attributes
        {
            [DebuggerStepThrough]
            get { return _attributes = _attributes ?? new AttributeCollection(this); }
        }

        /// <summary>
        /// Gets a style object
        /// </summary>
        public Style Style
        {
            get { return _style = _style ?? new Style(this); }
        }
        #endregion

        #region Position & Representation related
        /// <summary>
        /// Gets element rectangle
        /// </summary>
        public Rectangle Rectangle
        {
            [DebuggerStepThrough]
            get { return GetLocation(ElementLocation.RootRelative | ElementLocation.MarginBox); }
        }

        /// <summary>
        /// Returns element location
        /// </summary>
        public Rectangle GetLocation(ElementLocation loc)
        {
            return SciterDomApi.GetElementLocation(this, loc);
        }

        /// <summary>
        /// Updates element representation
        /// </summary>
        public void Update()
        {
            Update(false);
        }

        /// <summary>
        /// Updates element representation
        /// </summary>
        public void Update(bool forceUpdate)
        {
            SciterDomApi.UpdateElement(this, forceUpdate);
        } 
        #endregion

        #region Selecting child elements
        /// <summary>
        /// Returns all descendant elements
        /// </summary>
        public ElementsCollection All
        {
            [DebuggerStepThrough]
            get { return Select("*"); }
        }

        /// <summary>
        /// Returns a collection of all direct childs
        /// </summary>
        public ElementsCollection Children
        {
            [DebuggerStepThrough]
            get { return new ChildrenCollection(this); }
        }

        /// <summary>
        /// Selects all elements by css selector
        /// </summary>
        public ElementsCollection Select(string selector)
        {
            return Select(selector, elem => true);
        }

        /// <summary>
        /// Selects all elements by css selector & optional filter
        /// </summary>
        /// <param name="selector">css selector</param>
        /// <param name="predicate">Element passed as a paramter in predicate is not "counted" and became invalid on return</param>
        public ElementsCollection Select(string selector, Predicate<Element> predicate)
        {
            // populating items array with element that matches to predicate
            var items = new List<Element>();
            SciterDomApi.SelectElements(this, selector,
                element =>
                {
                    if (predicate(element))
                        items.Add(Element.Create(element.Handle));

                    return false;
                });

            return new AllElementsCollection(items);
        }

        /// <summary>
        /// Searches for an element that matches the conditions 
        /// </summary>
        public Element Find(string selector)
        {
            return Find(selector, elem => true);
        }

        /// <summary>
        /// Searches for an element that matches the conditions
        /// </summary>
        /// <param name="selector">css selector</param>
        /// <param name="predicate">Element passed as a paramter in predicate is not "counted" and became invalid on return</param>
        public Element Find(string selector, Predicate<Element> predicate)
        {
            // locating first element that matches specified predicate
            var result = default(Element);
            SciterDomApi.SelectElements(
                this, 
                selector, 
                element => (predicate(element) ? result = Element.Create(element.Handle) : result = null) != null);

            return result;
        }        
        #endregion

        #region Scripting and behaviors
        /// <summary>
        /// Calls behavior specific method
        /// </summary>
        public object ScriptingCall(string methodName, params object[] args)
        {
            return SciterDomApi.CallScriptingMethod(this, methodName, args);
        }

        /// <summary>
        /// Attaches event handler to the element
        /// </summary>
        public void AttachBehavior(SciterBehavior behavior)
        {
            SciterDomApi.AttachEventHandler(this, behavior);
        }

        /// <summary>
        /// Detaches event handler from the element
        /// </summary>
        public void DetachBehavior(SciterBehavior behavior)
        {
            SciterDomApi.DetachEventHandler(this, behavior);
        }        
        #endregion

        #region Lifetime management
        /// <summary>
        /// Allocates reference to the element
        /// </summary>
        /// <remarks>
        /// By default weak reference will be allocated
        /// </remarks>
        public ElementRef Use()
        {
            return Use(ElementRefType.Normal);
        }

        /// <summary>
        /// Allocates reference to the element
        /// </summary>
        /// <remarks>
        /// Element references uses ref counting and allowed to use between calls
        /// </remarks>
        public ElementRef Use(ElementRefType elementRefType)
        {
            return new ElementRef(this, elementRefType);
        } 
        #endregion

        #region Popup support
        /// <summary>
        /// Shows block element (DIV) in popup window.
        /// </summary>
        public void ShowPopup(Element anchor, PopupPlacement placement)
        {
            SciterDomApi.ShowPopup(this, anchor, placement);
        }

        /// <summary>
        /// Shows block element (DIV) in popup window.
        /// </summary>
        public void ShowPopup(Point localtion, bool animate)
        {
            SciterDomApi.ShowPopup(this, localtion, animate);
        }

        /// <summary>
        /// Hides popup window
        /// </summary>
        public void HidePopup()
        {
            SciterDomApi.HidePopup(this);
        }

        #endregion

        #region Element states
        /// <summary>
        /// Gets current element state
        /// </summary>
        public ElementState State
        {
            [DebuggerStepThrough]
            get { return SciterDomApi.GetElementState(this); }
        }

        /// <summary>
        /// Changes element state
        /// </summary>
        public void SetState(ElementState stateToSet, ElementState stateToClear)
        {
            SetState(stateToSet, stateToClear, false);
        }

        /// <summary>
        /// Changes element state
        /// </summary>
        public void SetState(ElementState stateToSet, ElementState stateToClear, bool update)
        {
            SciterDomApi.SetElementState(this, stateToSet, stateToClear, update);
        } 
        #endregion

        #region Working with elements tree
        /// <summary>
        /// Deletes element from the DOM tree
        /// </summary>
        public void Delete()
        {
            SciterDomApi.DeleteElement(this);
        }

        /// <summary>
        /// Creates cloned copy of the element
        /// </summary>
        public ElementRef Clone()
        {
            return SciterDomApi.Clone(this);
        }

        /// <summary>
        /// Inserts child to the elemets tree
        /// </summary>
        public void InsertElement(Element child, int pos)
        {
            SciterDomApi.InsertElement(child, this, pos);
        } 
        #endregion

        #region Mouse capture management
        /// <summary>
        /// 
        /// </summary>
        public void SetCapture()
        {
            SciterDomApi.SetCapture(this);
        }
        #endregion

        #region Private implementation
        /// <summary>
        /// Clears helement handle
        /// </summary>
        internal void Drop()
        {
            Handle = IntPtr.Zero;
        } 
	    #endregion    
    }
}