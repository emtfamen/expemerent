using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Security;
using System.Runtime.InteropServices;
using System.Text;
using Expemerent.UI.Dom;
using Expemerent.UI.Behaviors;

namespace Expemerent.UI.Native
{
    #region Sciter dom interface
    /// <summary>
    /// Sciter dom interface
    /// </summary>
    [SuppressUnmanagedCodeSecurityAttribute]
    internal partial class SciterDomApi
    {
        #region Function prototypes

        /// <summary>
        /// Marks DOM object as unused (a.k.a. AddRef).
        /// </summary>
        [DllImport(SciterHostApi.SciterDll, EntryPoint="Sciter_UseElement")]
        private static extern ScDomResult SciterUseElement(IntPtr he);

        /// <summary>
        /// Marks DOM object as unused (a.k.a. Release). Get handle of every element's child element.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll, EntryPoint="Sciter_UnuseElement")]
        private static extern ScDomResult SciterUnuseElement(IntPtr he);

        /// <summary>
        /// Get root DOM element of HTML document.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterGetRootElement(IntPtr hwnd, out IntPtr phe);

        /// <summary>
        /// Get focused DOM element of HTML document.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterGetFocusElement(IntPtr hwnd, IntPtr phe);

        /// <summary>
        /// Find DOM element by coordinate.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterFindElement(IntPtr hwnd, Point pt, out IntPtr phe);

        /// <summary>
        /// Get number of child elements.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterGetChildrenCount(IntPtr he, out int count);

        /// <summary>
        /// Get handle of every element's child element.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterGetNthChild(IntPtr he, int n, out IntPtr phe);

        /// <summary>
        /// Get parent element.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterGetParentElement(IntPtr he, out IntPtr p_parent_he);

        /// <summary>
        /// Get text of the element and information where child elements are placed.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterGetElementHtml(IntPtr he, /*[MarshalAs(UnmanagedType.LPStr)]*/ out IntPtr utf8bytes, bool outer);

        /// <summary>
        /// Get inner text of the element as LPWSTR (utf16 words).
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterGetElementText(IntPtr he, /*[MarshalAs(UnmanagedType.LPWStr)]*/ out IntPtr utf16words);

        /// <summary>
        /// Set inner text of the element from LPCWSTR buffer (utf16 words).
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterSetElementText(IntPtr he, [MarshalAs(UnmanagedType.LPWStr)] string utf16words, int length);

        /// <summary>
        /// Get number of element's attributes.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterGetAttributeCount(IntPtr he, out int p_count);

        /// <summary>
        /// Get value of any element's attribute by attribute's number.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterGetNthAttribute(IntPtr he, int n, /*[MarshalAs(UnmanagedType.LPStr)]*/ out IntPtr p_name, /*[MarshalAs(UnmanagedType.LPWStr)]*/ out IntPtr p_value);

        /// <summary>
        /// Get value of any element's attribute by name.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterGetAttributeByName(IntPtr he, /*[MarshalAs(UnmanagedType.LPStr)]*/ Byte[] name, /*[MarshalAs(UnmanagedType.LPWStr)]*/ out IntPtr p_value);

        /// <summary>
        /// Set attribute's value.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterSetAttributeByName(IntPtr he, /*[MarshalAs(UnmanagedType.LPStr)]*/ Byte[] name, [MarshalAs(UnmanagedType.LPWStr)] String value);

        /// <summary>
        /// Remove all attributes from the element.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterClearAttributes(IntPtr he);

        /// <summary>
        /// Get element index. 
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterGetElementIndex(IntPtr he, out int p_index);

        /// <summary>
        /// Get element's text.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterGetElementType(IntPtr he, /*[MarshalAs(UnmanagedType.LPStr)]*/ out IntPtr p_type);

        /// <summary>
        /// Get element's style attribute.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterGetStyleAttribute(IntPtr he, /*[MarshalAs(UnmanagedType.LPStr)]*/ Byte[] name, /*[MarshalAs(UnmanagedType.LPWStr)]*/ out IntPtr p_value);

        /// <summary>
        /// Set element's style attribute.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterSetStyleAttribute(IntPtr he, /*[MarshalAs(UnmanagedType.LPStr)]*/ Byte[] name, [MarshalAs(UnmanagedType.LPWStr)] String value);

        /// <summary>
        /// Get bounding rectangle of the element.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterGetElementLocation(IntPtr he, out Rectangle p_location, ELEMENT_AREA areas);

        /// <summary>
        /// Scroll to view.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterScrollToView(IntPtr he, bool toTopOfView);

        /// <summary>
        /// Apply changes and refresh element area in its window.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterUpdateElement(IntPtr he, bool renderNow);

        /// <summary>
        /// refresh element area in its window.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterRefreshArea(IntPtr he, Rectangle rc);

        /// <summary>
        /// Set the mouse capture to the specified element.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterSetCapture(IntPtr he);

        /// <summary>
        /// Releases the mouse capture from the specified element.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterReleaseCapture(IntPtr he);

        /// <summary>
        /// Get IntPtr of containing window.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterGetElementHwnd(IntPtr he, out IntPtr p_IntPtr, bool rootWindow);

        /// <summary>
        /// Combine given URL with URL of the document element belongs to.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterCombineURL(IntPtr he, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder szUrlBuffer, int urlBufferSize);

        /// <summary>
        /// Call specified function for every element in a DOM that meets specified CSS selectors.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterSelectElements(IntPtr he, /*[MarshalAs(UnmanagedType.LPStr)]*/ Byte[] CSS_selectors, [MarshalAs(UnmanagedType.FunctionPtr)] SciterElementCallback callback, IntPtr param);

        /// <summary>
        /// Find parent of the element by CSS selector. 
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterSelectParent(IntPtr he, /*[MarshalAs(UnmanagedType.LPStr)]*/ Byte[] selector, int depth, out IntPtr heFound);

        /// <summary>
        /// Set inner or outer html of the element.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterSetElementHtml(IntPtr he, /*[MarshalAs(UnmanagedType.LPStr)]*/ byte[] html, int htmlLength, SET_ELEMENT_HTML where);

        /// <summary>
        /// Get Element UID.
        /// </summary>
        /// <remarks>Element UID support functions. 
        /// Element UID is unique identifier of the DOM element. 
        /// UID is suitable for storing it in structures associated with the view/document.
        /// Access to the element using IntPtr is more effective but table space of handles is limited.
        /// It is not recommended to store IntPtr handles between function calls.
        /// </remarks>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterGetElementUID(IntPtr he, out int puid);

        /// <summary>
        /// Get Element handle by its UID.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterGetElementByUID(IntPtr hwnd, int uid, out IntPtr phe);

        /// <summary>
        /// Shows block element (DIV) in popup window.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterShowPopup(IntPtr hePopup, IntPtr heAnchor, POPUP_PLACEMENT placement);

        /// <summary>
        /// Shows block element (DIV) in popup window at given position.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterShowPopupAt(IntPtr hePopup, Point pos, bool animate);

        /// <summary>
        /// Removes popup window.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterHidePopup(IntPtr he);

        /// <summary>
        /// Get/set state bits, stateBits*** accept or'ed values above
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterGetElementState(IntPtr he, out ELEMENT_STATE_BITS pstateBits);

        /// <summary>
        /// Get/set state bits, stateBits*** accept or'ed values above
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterSetElementState(IntPtr he, ELEMENT_STATE_BITS stateBitsToSet, ELEMENT_STATE_BITS stateBitsToClear, bool updateView);

        /// <summary>
        /// Create Element.CreateElement, the element is disconnected initially from the DOM. 
        /// Element created with ref_count = 1 thus you must call Sciter_UnuseElement on returned handler.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterCreateElement(/*[MarshalAs(UnmanagedType.LPStr)]*/ Byte[] tagname, [MarshalAs(UnmanagedType.LPWStr)] String textOrNull, out IntPtr phe);

        /// <summary>
        /// Create Element.CreateElement as copy of existing element, Element.CreateElement is a full (deep) copy of the element and
        /// is disconnected initially from the DOM.
        /// Element created with ref_count = 1 thus you \b must call Sciter_UnuseElement on returned handler.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterCloneElement(IntPtr he, out IntPtr phe);

        /// <summary>
        /// Insert element at index position of parent.
        /// It is not an error to insert element which already has parent - it will be disconnected first, but 
        /// you need to update elements parent in this case. 
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterInsertElement(IntPtr he, IntPtr hparent, int index);

        /// <summary>
        /// Take element out of its container (and DOM tree). 
        /// Element will be destroyed when its reference counter will become zero
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterDetachElement(IntPtr he);

        /// <summary>
        /// Start Timer for the element. 
        /// Element will receive on_timer event
        /// To stop timer call SciterSetTimer( element, 0 );
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterSetTimer(IntPtr he, int milliseconds);

        /// <summary>
        /// Attach/Detach ElementEventProc to the element 
        /// See htmlayout::event_handler.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterDetachEventHandler(IntPtr he, IntPtr pep, IntPtr tag);

        /// <summary>
        /// Attach ElementEventProc to the element and subscribe it to events providede by subscription parameter
        /// See Sciter::attach_event_handler.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterAttachEventHandler(IntPtr he, IntPtr pep, IntPtr tag);

        /// <summary>
        /// Attach/Detach ElementEventProc to the Sciter window. 
        /// All events will start first here (in SINKING phase) and if not consumed will end up here.
        /// You can install Window EventHandler only once - it will survive all document reloads.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterWindowAttachEventHandler(IntPtr hwnd, /*[MarshalAs(UnmanagedType.FunctionPtr)] ElementEventProc*/ IntPtr pep, IntPtr tag, EVENT_GROUPS subscription);

        /// <summary>
        /// Attach/Detach ElementEventProc to the Sciter window. 
        /// All events will start first here (in SINKING phase) and if not consumed will end up here.
        /// You can install Window EventHandler only once - it will survive all document reloads.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterWindowDetachEventHandler(IntPtr hwnd, /*[MarshalAs(UnmanagedType.FunctionPtr)] ElementEventProc*/ IntPtr pep, IntPtr tag);

        /// <summary>
        /// SendEvent - sends sinking/bubbling event to the child/parent chain of element element.
        /// First event will be send in SINKING mode (with SINKING flag) - from root to element element itself.
        /// Then from element element to its root on parents chain without SINKING flag (bubbling phase).
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterSendEvent(IntPtr he, int appEventCode, IntPtr heSource, IntPtr reason, out bool handled);

        /// <summary>
        /// PostEvent - post sinking/bubbling event to the child/parent chain of element element.
        /// Function will return immediately posting event into input queue of the application. 
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterPostEvent(IntPtr he, int appEventCode, IntPtr heSource, IntPtr reason);

        /// <summary>
        /// SciterCallMethod - calls behavior specific method.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterCallBehaviorMethod(IntPtr he, ref METHOD_PARAMS param);

        /// <summary>
        /// SciterRequestElementData - request data download for the element.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterRequestElementData(IntPtr he, [MarshalAs(UnmanagedType.LPWStr)] String url, uint dataType, IntPtr initiator);

        /// <summary>
        /// SciterSendRequest - send GET or POST request for the element
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterHttpRequest(IntPtr he, [MarshalAs(UnmanagedType.LPWStr)] String url, uint dataType, uint requestType, IntPtr requestParams, uint nParams);

        /// <summary>
        /// SciterGetScrollInfo - get scroll info of element with overflow:scroll or auto.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterGetScrollInfo(IntPtr he, out Point scrollPos, out Rectangle viewRect, out Size contentSize);

        /// <summary>
        /// SciterSetScrollPos - set scroll position of element with overflow:scroll or auto.
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterSetScrollPos(IntPtr he, Point scrollPos, bool smooth);

        /// <summary>
        /// SciterIsElementVisible - deep visibility, determines if element visible - has no visiblity:hidden and no display:none defined 
        /// for itself or for any its parents.
        /// </summary> 
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterIsElementVisible(IntPtr he, out bool pVisible);

        /// <summary>
        /// SciterIsElementEnabled - deep enable state, determines if element enabled - is not disabled by itself or no one 
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterIsElementEnabled(IntPtr he, out bool pEnabled);

        /// <summary>
        /// SciterSortElements
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterSortElements(IntPtr he, uint firstIndex, uint lastIndex, [MarshalAs(UnmanagedType.FunctionPtr)] ElementComparator cmpFunc, IntPtr cmpFuncParam);

        /// <summary>
        /// SciterTraverseUIEvent - traverse (sink-and-bubble) MOUSE or KEY event.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterTraverseUIEvent(EVENT_GROUPS evt, IntPtr eventCtlStruct, out bool bOutProcessed);

        ///** SciterRange*** - range manipulation routines. 
        // ATTN: Not completed yet */
        //enum ADVANCE_TO 
        //{
        // GO_FIRST = 0,
        // GO_LAST = 1,
        // GO_NEXT = 2,
        // GO_PREV = 3,
        // GO_NEXT_CHAR = 4,
        // GO_PREV_CHAR = 5,
        //};
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterRangeCreate(); //IntPtr element, HRANGE* pRange, BOOL outer);
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterRangeFromSelection(); //IntPtr element, HRANGE* pRange);
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterRangeFromPositions(); //IntPtr element, HPOSITION* pStart, HPOSITION* pEnd);
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterRangeUse(); //HRANGE range);
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterRangeFree(); //HRANGE range);
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterRangeAdvancePos(); //HRANGE range, uint cmd, INT* c, HPOSITION* pPos);
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterRangeToHtml(); //HRANGE range, LPBYTE* pHtmlUtf8Bytes, uint* numBytes);
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterRangeReplace(); //HRANGE range, LPBYTE htmlUtf8Bytes, uint numBytes);
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterRangeInsertHtml(); //HPOSITION* pPos, LPBYTE htmlUtf8Bytes, uint numBytes);

        /// <summary>
        /// CallScriptingMethod - calls scripting method defined for the element.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterCallScriptingMethod(IntPtr he, [MarshalAs(UnmanagedType.LPStr)] String name, [MarshalAs(UnmanagedType.LPArray)] JsonValue[] argv, int argc, out JsonValue retval);

        /// <summary>
        /// Attach HWND to the element.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterAttachHwndToElement(IntPtr he, IntPtr hwnd);

        /// <summary>
        /// get value of the element. 'value' is value of correspondent behavior attached to the element or its text.
        /// HELEMENT, element which value will be retrieved.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterGetValue(IntPtr he, ref JsonValue pval);

        /// <summary>
        /// set value of the element.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterSetValue(IntPtr he, ref JsonValue pval);

        /// <summary>
        /// get 'expando' of the element. 'expando' is a scripting object (of class Element) 
        /// that is assigned to the DOM element. 'expando' could be null as they are created on demand by script.
        /// </summary>
        [DllImport(SciterHostApi.SciterDll)]
        private static extern ScDomResult SciterGetExpando(IntPtr he, ref JsonValue pval, int forceCreation);

        #endregion

        /// <summary>
        /// Releases the mouse capture from the specified element.
        /// </summary>
        /// <param name="element"></param>
        public void ReleaseCapture(Element element)
        {
            SciterReleaseCapture(element.Handle);
        }

        /// <summary>
        /// Deletes element from the DOM Tree
        /// </summary>        
        public void DeleteElement(Element element)
        {
            IntPtr parent;
            CheckResult(SciterGetParentElement(element.Handle, out parent));
            CheckResult(SciterDetachElement(element.Handle));
            CheckResult(SciterUpdateElement(parent, true));
        }
    }

    #endregion
}