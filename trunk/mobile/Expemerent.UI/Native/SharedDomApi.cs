using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using Expemerent.UI.Behaviors;
using Expemerent.UI.Dom;

namespace Expemerent.UI.Native
{
    internal enum REQUEST_TYPE : int
    {
        GET_ASYNC, // async GET
        POST_ASYNC, // async POST
        GET_SYNC, // synchronous GET 
        POST_SYNC // synchronous POST 
    } ;

    #region Callbacks

    /// <summary>
    /// Callback comparator function used with #SciterSortElements().
    /// Shall return -1,0,+1 values to indicate result of comparison of two elements
    /// </summary>
    internal delegate int ElementComparator(IntPtr he1, IntPtr he2, IntPtr param);

    /// <summary>
    /// Element callback function for all types of events. Similar to ProcessMessage
    /// </summary>
    /// <returns>bool</returns>
    internal delegate int ElementEventProc(IntPtr tag, IntPtr he, EVENT_GROUPS evtg, IntPtr prms);

    /// <summary>
    /// Callback function used with #SciterVisitElement()
    /// </summary>
    /// <returns>bool</returns>
    internal delegate int SciterElementCallback(IntPtr he, IntPtr param);

    #endregion

    internal partial class SciterDomApi
    {
        #region Public interface

        /// <summary>
        /// SendEvent - sends sinking/bubbling event to the child/parent chain of element element.
        /// First event will be send in SINKING mode (with SINKING flag) - from root to element element itself.
        /// Then from element element to its root on parents chain without SINKING flag (bubbling phase).
        /// </summary>
        public bool SendEvent(Element he, BehaviorEventType eventCode, IntPtr reason, Element source)
        {
            var handled = default(bool);
            CheckResult(SciterSendEvent(he.Handle, (int)eventCode, source != null ? source.Handle : he.Handle, reason, out handled));

            return handled;
        }

        /// <summary>
        /// PostEvent - post sinking/bubbling event to the child/parent chain of element element.
        /// Function will return immediately posting event into input queue of the application. 
        /// </summary>
        public void PostEvent(Element he, BehaviorEventType eventCode, IntPtr reason, Element source)
        {
            CheckResult(SciterPostEvent(he.Handle, (int)eventCode, source != null ? source.Handle : he.Handle, reason));
        }

        /// <summary>
        /// SciterCallMethod - calls behavior specific method.
        /// </summary>
        public bool CallBehaviorMethod(Element he, BehaviorMethods methodId, out object result)
        {
            result = null;
            switch (methodId)
            {
                case BehaviorMethods.DoClick:
                    var methodParam = new METHOD_PARAMS() { methodID = (int)METHOD_PARAMS.BEHAVIOR_METHOD_IDENTIFIERS.DO_CLICK };
                    var res = SciterCallBehaviorMethod(he.Handle, ref methodParam);

                    if (res == ScDomResult.SCDOM_OK_NOT_HANDLED)
                        return false;
                    
                    CheckResult(res);
                    return true;
            }

            return false;
        }
        /// <summary>
        /// SciterIsElementVisible - deep visibility, determines if element visible - has no visiblity:hidden and no display:none defined 
        /// for itself or for any its parents.
        /// </summary> 
        public bool IsElementVisible(Element he)
        {
            var visible = default(bool);
            CheckResult(SciterIsElementVisible(he.Handle, out visible));

            return visible;
        }

        /// <summary>
        /// SciterIsElementEnabled - deep enable state, determines if element enabled - is not disabled by itself or no one 
        /// </summary>
        public bool IsElementEnabled(Element he)
        {
            var enabled = default(bool);
            CheckResult(SciterIsElementEnabled(he.Handle, out enabled));

            return enabled;
        }

        /// <summary>
        /// Scroll to view.
        /// </summary>
        public void ScrollToView(Element he, bool top)
        {
            CheckResult(SciterScrollToView(he.Handle, top));
        }

        /// <summary>
        /// Returns element index within parent's collection
        /// </summary>
        public int GetElementIndex(Element he)
        {
            var index = default(int);
            CheckResult(SciterGetElementIndex(he.Handle, out index));

            return index;
        }

        /// <summary>
        /// Gets element location
        /// </summary>
        public Rectangle GetElementLocation(Element he, ElementLocation areas)
        {
            Rectangle rect;
            CheckResult(SciterGetElementLocation(he.Handle, out rect, (ELEMENT_AREA)areas));
            rect.Width -= rect.Left;
            rect.Height -= rect.Top;
            return rect;
        }

        /// <summary>
        /// User friendly version of SetElementText
        /// </summary>
        public void SetElementText(Element he, String text)
        {
            CheckResult(SciterSetElementText(he.Handle, text, text.Length));
        }

        /// <summary>
        /// Updates element html
        /// </summary>
        public void SetElementHtml(Element he, String text, SET_ELEMENT_HTML loc)
        {
            var bytes = MarshalUtility.StringToByteUtf8(text);
            CheckResult(SciterSetElementHtml(he.Handle, bytes, bytes.Length, loc));
        }

        /// <summary>
        /// Marks DOM object as unused (a.k.a. AddRef).
        /// </summary>
        public void UseElement(IntPtr he)
        {
            CheckResult(SciterUseElement(he));
        }

        /// <summary>
        /// Unuses element ignoring any possible exception
        /// </summary>
        public void UnuseElement(IntPtr he)
        {
            var result = SciterUnuseElement(he);
            Debug.Assert(result == ScDomResult.SCDOM_OK);
        }

        /// <summary>
        /// Returns parent element satisfying given css selector
        /// </summary>
        public Element SelectParent(Element element, string cssSelector, int depth)        
        {
            var parent = default(IntPtr);
            CheckResult(SciterSelectParent(element.Handle, MarshalUtility.StringToAnsi(cssSelector), depth, out parent));

            return Element.Create(parent);
        }

        /// <summary>
        /// Selects child elements
        /// </summary>
        public void SelectElements(Element he, String cssSelector, Predicate<Element> selector)
        {
            var result = SciterSelectElements(he.Handle, MarshalUtility.StringToAnsi(cssSelector),
                (IntPtr handle, IntPtr arg) =>
                {
                    var element = Element.CreateInternal(handle);
                    var selected = selector(element);

                    element.Drop();
                    return selected ? 1 : 0;
                }, IntPtr.Zero);

            CheckResult(result);
        }

        /// <summary>
        /// Get number of element's attributes.
        /// </summary>
        public int GetAttributeCount(Element he)
        {
            int count;

            CheckResult(SciterGetAttributeCount(he.Handle, out count));
            return count;
        }

        /// <summary>
        /// Get value of any element's attribute by name.
        /// </summary>
        public string GetAttribute(IntPtr he, string name)
        {
            var value = default(IntPtr);
            CheckResult(SciterGetAttributeByName(he, MarshalUtility.StringToAnsi(name), out value));
            return Marshal.PtrToStringUni(value);
        }

        /// <summary>
        /// Get value of any element's attribute by attribute's number.
        /// </summary>
        public KeyValuePair<string, string> GetAttribute(Element he, int index)
        {
            IntPtr name;
            IntPtr value;
            CheckResult(SciterGetNthAttribute(he.Handle, index, out name, out value));

            return new KeyValuePair<string, string>(MarshalUtility.PtrToStringAnsi(name), Marshal.PtrToStringUni(value));
        }

        /// <summary>
        /// Set attribute's value.
        /// </summary>
        public void SetAttributeByName(Element he, string name, string value)
        {
            CheckResult(SciterSetAttributeByName(he.Handle, MarshalUtility.StringToAnsi(name), value));
        }

        /// <summary>
        /// Get element's text.
        /// </summary>
        public string GetElementType(Element element)
        {
            IntPtr type;
            CheckResult(SciterGetElementType(element.Handle, out type));

            return MarshalUtility.PtrToStringAnsi(type);
        }

        /// <summary>
        /// Get inner text of the element as LPWSTR (utf16 words).
        /// </summary>
        public string GetElementText(Element element)
        {
            IntPtr text;
            CheckResult(SciterGetElementText(element.Handle, out text));

            return Marshal.PtrToStringUni(text);
        }

        /// <summary>
        /// Get text of the element and information where child elements are placed.
        /// </summary>
        public string GetElementHtml(Element element, bool outer)
        {
            IntPtr text;
            CheckResult(SciterGetElementHtml(element.Handle, out text, outer));

            return MarshalUtility.PtrToStringUtf8(text);
        }

        /// <summary>
        /// Insert element at index position of parent.
        /// It is not an error to insert element which already has parent - it will be disconnected first, but 
        /// you need to update elements parent in this case. 
        /// </summary>
        public void InsertElement(Element child, Element element, int pos)
        {
            CheckResult(SciterInsertElement(child.Handle, element.Handle, pos));
        }

        /// <summary>
        /// Create new element as copy of existing element, new element is a full (deep) copy of the element and
        /// is disconnected initially from the DOM.
        /// Element created with ref_count = 1 thus you \b must call Sciter_UnuseElement on returned handler.
        /// </summary>
        public ElementRef Clone(Element element)
        {
            var he = default(IntPtr);

            CheckResult(SciterCloneElement(element.Handle, out he));
            return new ElementRef(he);
        }

        /// <summary>
        /// Apply changes and refresh element area in its window.
        /// </summary>
        public void UpdateElement(Element element, bool forceUpdate)
        {
            CheckResult(SciterUpdateElement(element.Handle, forceUpdate));
        }

        /// <summary>
        /// Get IntPtr of containing window.
        /// </summary>
        public IntPtr GetElementHwnd(Element he, bool root)
        {
            IntPtr IntPtr;

            CheckResult(SciterGetElementHwnd(he.Handle, out IntPtr, root));
            return IntPtr;
        }

        /// <summary>
        /// Create new element, the element is disconnected initially from the DOM. 
        /// Element created with ref_count = 1 thus you must call Sciter_UnuseElement on returned handler.
        /// </summary>
        public ElementRef CreateElement(string tag, string text)
        {
            var he = default(IntPtr);
            CheckResult(SciterCreateElement(MarshalUtility.StringToAnsi(tag), text, out he));
            return new ElementRef(he);
        }

        /// <summary>
        /// Get root DOM element of HTML document.
        /// </summary>
        public Element GetRootElement(IntPtr IntPtr)
        {
            var he = default(IntPtr);

            CheckResult(SciterGetRootElement(IntPtr, out he));
            return Element.Create(he);
        }

        /// <summary>
        /// Get Element handle by its UID.
        /// </summary>
        public Element GetElementByUID(IntPtr elementIntPtr, int uid)
        {
            var he = default(IntPtr);

            CheckResult(SciterGetElementByUID(elementIntPtr, uid, out he));
            return Element.Create(he);
        }

        /// <summary>
        /// Get Element UID.
        /// </summary>
        public int GetElementUID(Element element)
        {
            var uid = default(int);

            CheckResult(SciterGetElementUID(element.Handle, out uid));
            return uid;
        }

        /// <summary>
        /// Get value of any element's attribute by name.
        /// </summary>
        public string GetAttributeByName(Element element, string name)
        {
            var value = default(IntPtr);
            CheckResult(SciterGetAttributeByName(element.Handle, MarshalUtility.StringToAnsi(name), out value));
            return Marshal.PtrToStringUni(value);
        }

        /// <summary>
        /// Remove all attributes from the element.
        /// </summary>
        public void ClearAttributes(Element element)
        {
            CheckResult(SciterClearAttributes(element.Handle));
        }

        /// <summary>
        /// Get number of child elements.
        /// </summary>
        public int GetChildrenCount(Element he)
        {
            int count;
            CheckResult(SciterGetChildrenCount(he.Handle, out count));

            return count;
        }

        /// <summary>
        /// Get handle of every element's child element.
        /// </summary>
        public Element GetNthChild(Element he, int index)
        {
            IntPtr element;
            CheckResult(SciterGetNthChild(he.Handle, index, out element));

            return Element.Create(element);
        }

        /// <summary>
        /// Get element's style attribute.
        /// </summary>
        public string GetStyleAttribute(Element he, String name)
        {
            var value = default(IntPtr);
            CheckResult(SciterGetStyleAttribute(he.Handle, MarshalUtility.StringToAnsi(name), out value));
            return Marshal.PtrToStringUni(value);
        }

        /// <summary>
        /// Set element's style attribute.
        /// </summary>
        public void SetStyleAttribute(Element he, String name, String value)
        {
            CheckResult(SciterSetStyleAttribute(he.Handle, MarshalUtility.StringToAnsi(name), value));
        }

        /// <summary>
        /// Calls behavior specific method
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public object CallScriptingMethod(Element he, string name, object[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Attach/Detach ElementEventProc to the element 
        /// See Sciter::event_handler.
        /// </summary>
        public void DetachEventHandler(Element he, ISciterBehavior behavior)
        {            
            var r = SciterDetachEventHandler(he.Handle, SciterHostApi.ElementEventProcEntryPoint, InstanceProtector.Protect(behavior));

            // DetachEventHandler can return SCDOM_PASSIVE_HANDLE if element was detached from the tree
            CheckResult(r == ScDomResult.SCDOM_PASSIVE_HANDLE ? ScDomResult.SCDOM_OK : r);
        }

        /// <summary>
        /// Attach ElementEventProc to the element and subscribe it to events providede by subscription parameter
        /// See Sciter::attach_event_handler.
        /// </summary>
        public void AttachEventHandler(Element he, ISciterBehavior behavior)
        {
            CheckResult(SciterAttachEventHandler(he.Handle, SciterHostApi.ElementEventProcEntryPoint, InstanceProtector.Protect(behavior)));
        }

        /// <summary>
        /// Shows block element (DIV) in popup window.
        /// </summary>
        public void ShowPopup(Element he, Element anchor, PopupPlacement placement)
        {
            CheckResult(SciterShowPopup(he.Handle, anchor.Handle, (POPUP_PLACEMENT)placement));
        }

        /// <summary>
        /// Shows block element (DIV) in popup window at given position.
        /// </summary>
        public void ShowPopup(Element he, Point localtion, bool animate)
        {
            CheckResult(SciterShowPopupAt(he.Handle, localtion, animate));
        }

        /// <summary>
        /// Removes popup window.
        /// </summary>
        public void HidePopup(Element he)
        {
            CheckResult(SciterHidePopup(he.Handle));
        }

        /// <summary>
        /// Gets current element state
        /// </summary>
        public ElementState GetElementState(Element he)
        {
            ELEMENT_STATE_BITS state = 0;
            CheckResult(SciterGetElementState(he.Handle, out state));

            return (ElementState)state;
        }

        /// <summary>
        /// Gets current element state
        /// </summary>
        public void SetElementState(Element he, ElementState stateToSet, ElementState stateToClear, bool update)
        {
            CheckResult(SciterSetElementState(he.Handle, (ELEMENT_STATE_BITS)stateToSet, (ELEMENT_STATE_BITS)stateToClear, update));
        }

        /// <summary>
        /// Set the mouse capture to the specified element.
        /// </summary>
        /// <param name="element"></param>
        public void SetCapture(Element element)
        {
            CheckResult(SciterSetCapture(element.Handle));
        }

        /// <summary>
        /// Attach/Detach ElementEventProc to the Sciter window. 
        /// All events will start first here (in SINKING phase) and if not consumed will end up here.
        /// You can install Window EventHandler only once - it will survive all document reloads.
        /// </summary>
        public void WindowAttachEventHandler(IntPtr hWnd, ISciterBehavior bhv, EVENT_GROUPS evt)
        {
            CheckResult(SciterWindowAttachEventHandler(hWnd, SciterHostApi.ElementEventProcEntryPoint, InstanceProtector.Protect(bhv), evt));
        }

        /// <summary>
        /// Attach/Detach ElementEventProc to the Sciter window. 
        /// All events will start first here (in SINKING phase) and if not consumed will end up here.
        /// You can install Window EventHandler only once - it will survive all document reloads.
        /// </summary>
        public void WindowDetachEventHandler(IntPtr hWnd, ISciterBehavior bhv)
        {
            CheckResult(SciterWindowDetachEventHandler(hWnd, SciterHostApi.ElementEventProcEntryPoint, InstanceProtector.Protect(bhv)));
        }

        /// <summary>
        /// Returns parent of the element
        /// </summary>
        public Element GetParentElement(Element element)
        {
            var parent = default(IntPtr);
            CheckResult(SciterGetParentElement(element.Handle, out parent));

            return Element.Create(parent);
        }
        #endregion

        #region Private implementation

        /// <summary>
        /// Checks result of api call
        /// </summary>
        private static void CheckResult(ScDomResult result)
        {
            if (result != ScDomResult.SCDOM_OK && result != ScDomResult.SCDOM_OK_NOT_HANDLED)
                throw new InvalidOperationException(String.Format("Sciter api failed with {0} result", result));

            Debug.Assert(result == ScDomResult.SCDOM_OK);
        }
        #endregion
    }
}
