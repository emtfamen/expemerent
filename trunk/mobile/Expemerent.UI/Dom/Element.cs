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
    public sealed class Element : IStyleAccessor, IAttributeAccessor
    {
        #region Internal data
        /// <summary>
        /// The element handle
        /// </summary>
        private IntPtr _handle;
        
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
        /// Creates a new handle without adding it to the tree
        /// </summary>
        public static ElementRef CreateElement(String tag, String text)
        {
            return SciterDomApi.CreateElement(tag, text);
        }

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
        /// Gets "deep" visible value
        /// </summary>
        public bool IsVisible
        {
            [DebuggerStepThrough]
            get { return SciterDomApi.IsElementVisible(this); }
        }

        /// <summary>
        /// Gets "deep" enabled value
        /// </summary>
        public bool IsEnabled
        {
            [DebuggerStepThrough]
            get { return SciterDomApi.IsElementEnabled(this); }
        }

        /// <summary>
        /// Gets element index within parent collection
        /// </summary>
        public int ElementIndex
        {
            [DebuggerStepThrough]
            get { return SciterDomApi.GetElementIndex(this); }
        }

        /// <summary>
        /// Gets element parent
        /// </summary>
        public Element Parent
        {
            [DebuggerStepThrough]
            get { return SciterDomApi.GetParentElement(this); }
        }

        /// <summary>
        /// Gets element tag
        /// </summary>
        public string Tag
        {
            [DebuggerStepThrough]
            get { return SciterDomApi.GetElementType(this); }
        }

        /// <summary>
        /// Gets or sets element text
        /// </summary>
        public string Text
        {
            [DebuggerStepThrough]
            get { return SciterDomApi.GetElementText(this); }
            [DebuggerStepThrough]
            set { SciterDomApi.SetElementText(this, value ?? String.Empty); }
        }

        /// <summary>
        /// Gets or sets element html (utf8 encoded string)
        /// </summary>
        public string InnerHtml
        {
            [DebuggerStepThrough]
            get { return SciterDomApi.GetElementHtml(this, false); }
            [DebuggerStepThrough]
            set { SciterDomApi.SetElementHtml(this, value ?? String.Empty, SET_ELEMENT_HTML.SIH_REPLACE_CONTENT); }
        }

        /// <summary>
        /// Gets or sets element html (utf8 encoded string)
        /// </summary>
        public string OuterHtml
        {
            [DebuggerStepThrough]
            get { return SciterDomApi.GetElementHtml(this, true); }
            [DebuggerStepThrough]
            set { SciterDomApi.SetElementHtml(this, value ?? String.Empty, SET_ELEMENT_HTML.SOH_REPLACE); }
        } 

        /// <summary>
        /// Gets attributes collection
        /// </summary>
        public IAttributeAccessor Attributes
        {
            [DebuggerStepThrough]
            get { return this; }
        }

        /// <summary>
        /// Gets a style object
        /// </summary>
        public IStyleAccessor Style
        {
            [DebuggerStepThrough]
            get { return this; }
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

        /// <summary>
        /// Scrolls element to the view
        /// </summary>
        public void ScrollToView()
        {
            ScrollToView(false);
        }

        /// <summary>
        /// Scrolls element to the view
        /// </summary>
        public void ScrollToView(bool topOfView)
        {
            SciterDomApi.ScrollToView(this, topOfView);
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
        public Element FindParent(string selector)
        {
            return SciterDomApi.SelectParent(this, selector, 0);            
        }

        /// <summary>
        /// Tests element against css selector
        /// </summary>
        public bool Test(string selector)
        {
            return SciterDomApi.SelectParent(this, selector, 1) != null;
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
        /// Attaches event handler to the element. 
        /// </summary>
        /// <remarks>
        /// Element do not hold reference to the behavior instance. 
        /// Is is responsibility of calling code to protect behavior from being garbage collected
        /// </remarks>
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

        /// <summary>
        /// Calls behavior specific method
        /// </summary>
        public bool CallBehaviorMethod(BehaviorMethods methodId)
        {
            var result = default(object);
            return SciterDomApi.CallScriptingMethod(this, methodId, out result);
        }

        /// <summary>
        /// Send event by sinking/bubbling on the parent/child chain of this element
        /// </summary>
        public bool SendEvent(BehaviorEventType eventCode, Element source)
        {
            var reason = default(IntPtr);
            return SciterDomApi.SendEvent(this, eventCode, reason, source);
        }

        /// <summary>
        /// Post event by sinking/bubbling on the parent/child chain of this element
        /// </summary>
        public void PostEvent(BehaviorEventType eventCode, Element source)
        {
            var reason = default(IntPtr);
            SciterDomApi.PostEvent(this, eventCode, reason, source);
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
        public void SetState(ElementState stateToSet)
        {
            SetState(stateToSet, ElementState.None);
        }

        /// <summary>
        /// Changes element state
        /// </summary>
        public void SetState(ElementState stateToSet, ElementState stateToClear)
        {
            SetState(stateToSet, stateToClear, true);
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

        #region Object equality
        /// <summary>
        /// Determines whether the specified <see cref="Element"/> is equal to the current <see cref="Element"/>
        /// </summary>
        public override bool Equals(object obj)
        {
            var element = obj as Element;
            if (element != null)
                return Handle == element.Handle;

            return false;
        }

        /// <summary>
        /// Evaluates object hash code
        /// </summary>
        public override int GetHashCode()
        {
            return Handle.GetHashCode();
        } 
        #endregion

        #region IStyleAccessor Members
        /// <summary>
        /// Gets or sets style attribute
        /// </summary>
        string IStyleAccessor.this[string name]
        {
            [DebuggerStepThrough]
            get { return SciterDomApi.GetStyleAttribute(this, name); }
            [DebuggerStepThrough]
            set { SciterDomApi.SetStyleAttribute(this, name, value); }
        }

        #endregion

        #region IAttributeAccessor Members
        /// <summary>
        /// Gets or sets attribute value by name
        /// </summary>
        string IAttributeAccessor.this[string name]
        {
            get { return SciterDomApi.GetAttributeByName(this, name); }
            set { SciterDomApi.SetAttributeByName(this, name, value); }
        }

        /// <summary>
        /// Gets a number of element's attribues
        /// </summary>
        int IAttributeAccessor.Count
        {
            [DebuggerStepThrough]
            get { return SciterDomApi.GetAttributeCount(this); }
        }

        /// <summary>
        /// Clears attributes collection
        /// </summary>
        void IAttributeAccessor.Clear()
        {
            SciterDomApi.ClearAttributes(this);
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,string>> Members
        /// <summary>
        /// Gets attribues enumerator
        /// </summary>
        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string,string>>.GetEnumerator()
        {
            for (int i = 0; i < Attributes.Count; i++)
                yield return SciterDomApi.GetAttribute(this, i);
        }

        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Gets attribues enumerator
        /// </summary>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Attributes.GetEnumerator();
        }

        #endregion
    }
}
