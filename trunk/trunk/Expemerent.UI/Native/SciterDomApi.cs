using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal partial struct SciterDomApi
    {
        #region Function prototypes

        /// <summary>
        /// Marks DOM object as unused (a.k.a. AddRef).
        /// </summary>
        private delegate ScDomResult SciterUseElement(IntPtr he);

        /// <summary>
        /// Marks DOM object as unused (a.k.a. Release). Get handle of every element's child element.
        /// </summary>
        private delegate ScDomResult SciterUnuseElement(IntPtr he);

        /// <summary>
        /// Get root DOM element of HTML document.
        /// </summary>
        private delegate ScDomResult SciterGetRootElement(IntPtr hwnd, out IntPtr phe);

        /// <summary>
        /// Get focused DOM element of HTML document.
        /// </summary>
        private delegate ScDomResult SciterGetFocusElement(IntPtr hwnd, IntPtr phe);

        /// <summary>
        /// Find DOM element by coordinate.
        /// </summary>
        private delegate ScDomResult SciterFindElement(IntPtr hwnd, Point pt, out IntPtr phe);

        /// <summary>
        /// Get number of child elements.
        /// </summary>
        private delegate ScDomResult SciterGetChildrenCount(IntPtr he, out int count);

        /// <summary>
        /// Get handle of every element's child element.
        /// </summary>
        private delegate ScDomResult SciterGetNthChild(IntPtr he, int n, out IntPtr phe);

        /// <summary>
        /// Get parent element.
        /// </summary>
        private delegate ScDomResult SciterGetParentElement(IntPtr he, out IntPtr p_parent_he);

        /// <summary>
        /// Get text of the element and information where child elements are placed.
        /// </summary>
        private delegate ScDomResult SciterGetElementHtml(IntPtr he, /*[MarshalAs(UnmanagedType.LPStr)]*/ out IntPtr utf8bytes, bool outer);

        /// <summary>
        /// Get inner text of the element as LPWSTR (utf16 words).
        /// </summary>
        private delegate ScDomResult SciterGetElementText(IntPtr he, /*[MarshalAs(UnmanagedType.LPWStr)]*/ out IntPtr utf16words);

        /// <summary>
        /// Set inner text of the element from LPCWSTR buffer (utf16 words).
        /// </summary>
        private delegate ScDomResult SciterSetElementText(IntPtr he, [MarshalAs(UnmanagedType.LPWStr)] string utf16words, int length);

        /// <summary>
        /// Get number of element's attributes.
        /// </summary>
        private delegate ScDomResult SciterGetAttributeCount(IntPtr he, out int p_count);

        /// <summary>
        /// Get value of any element's attribute by attribute's number.
        /// </summary>
        private delegate ScDomResult SciterGetNthAttribute(IntPtr he, int n, /*[MarshalAs(UnmanagedType.LPStr)]*/ out IntPtr p_name, /*[MarshalAs(UnmanagedType.LPWStr)]*/ out IntPtr p_value);

        /// <summary>
        /// Get value of any element's attribute by name.
        /// </summary>
        private delegate ScDomResult SciterGetAttributeByName(IntPtr he, /*[MarshalAs(UnmanagedType.LPStr)]*/ Byte[] name, /*[MarshalAs(UnmanagedType.LPWStr)]*/ out IntPtr p_value);

        /// <summary>
        /// Set attribute's value.
        /// </summary>
        private delegate ScDomResult SciterSetAttributeByName(IntPtr he, /*[MarshalAs(UnmanagedType.LPStr)]*/ Byte[] name, [MarshalAs(UnmanagedType.LPWStr)] String value);

        /// <summary>
        /// Remove all attributes from the element.
        /// </summary>
        private delegate ScDomResult SciterClearAttributes(IntPtr he);

        /// <summary>
        /// Get element index. 
        /// </summary>
        private delegate ScDomResult SciterGetElementIndex(IntPtr he, out int p_index);

        /// <summary>
        /// Get element's text.
        /// </summary>
        private delegate ScDomResult SciterGetElementType(IntPtr he, /*[MarshalAs(UnmanagedType.LPStr)]*/ out IntPtr p_type);

        /// <summary>
        /// Get element's style attribute.
        /// </summary>
        private delegate ScDomResult SciterGetStyleAttribute(IntPtr he, /*[MarshalAs(UnmanagedType.LPStr)]*/ Byte[] name, /*[MarshalAs(UnmanagedType.LPWStr)]*/ out IntPtr p_value);

        /// <summary>
        /// Set element's style attribute.
        /// </summary>
        private delegate ScDomResult SciterSetStyleAttribute(IntPtr he, /*[MarshalAs(UnmanagedType.LPStr)]*/ Byte[] name, [MarshalAs(UnmanagedType.LPWStr)] String value);

        /// <summary>
        /// Get bounding rectangle of the element.
        /// </summary>
        private delegate ScDomResult SciterGetElementLocation(IntPtr he, out Rectangle p_location, ELEMENT_AREA areas);

        /// <summary>
        /// Scroll to view.
        /// </summary>
        private delegate ScDomResult SciterScrollToView(IntPtr he, bool toTopOfView);

        /// <summary>
        /// Apply changes and refresh element area in its window.
        /// </summary>
        private delegate ScDomResult SciterUpdateElement(IntPtr he, bool renderNow);

        /// <summary>
        /// refresh element area in its window.
        /// </summary>
        private delegate ScDomResult SciterRefreshArea(IntPtr he, Rectangle rc);

        /// <summary>
        /// Set the mouse capture to the specified element.
        /// </summary>
        private delegate ScDomResult SciterSetCapture(IntPtr he);

        /// <summary>
        /// Releases the mouse capture from the specified element.
        /// </summary>
        private delegate ScDomResult SciterReleaseCapture(IntPtr he);

        /// <summary>
        /// Get IntPtr of containing window.
        /// </summary>
        private delegate ScDomResult SciterGetElementHwnd(IntPtr he, out IntPtr p_IntPtr, bool rootWindow);

        /// <summary>
        /// Combine given URL with URL of the document element belongs to.
        /// </summary>
        private delegate ScDomResult SciterCombineURL(IntPtr he, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder szUrlBuffer, int urlBufferSize);

        /// <summary>
        /// Call specified function for every element in a DOM that meets specified CSS selectors.
        /// </summary>
        private delegate ScDomResult SciterSelectElements(IntPtr he, /*[MarshalAs(UnmanagedType.LPStr)]*/ Byte[] CSS_selectors, [MarshalAs(UnmanagedType.FunctionPtr)] SciterElementCallback callback, IntPtr param);

        /// <summary>
        /// Find parent of the element by CSS selector. 
        /// </summary>
        private delegate ScDomResult SciterSelectParent(IntPtr he, /*[MarshalAs(UnmanagedType.LPStr)]*/ Byte[] selector, int depth, out IntPtr heFound);

        /// <summary>
        /// Set inner or outer html of the element.
        /// </summary>
        private delegate ScDomResult SciterSetElementHtml(IntPtr he, /*[MarshalAs(UnmanagedType.LPStr)]*/ byte[] html, int htmlLength, SET_ELEMENT_HTML where);

        /// <summary>
        /// Get Element UID.
        /// </summary>
        /// <remarks>Element UID support functions. 
        /// Element UID is unique identifier of the DOM element. 
        /// UID is suitable for storing it in structures associated with the view/document.
        /// Access to the element using IntPtr is more effective but table space of handles is limited.
        /// It is not recommended to store IntPtr handles between function calls.
        /// </remarks>
        private delegate ScDomResult SciterGetElementUID(IntPtr he, out int puid);

        /// <summary>
        /// Get Element handle by its UID.
        /// </summary>
        private delegate ScDomResult SciterGetElementByUID(IntPtr hwnd, int uid, out IntPtr phe);

        /// <summary>
        /// Shows block element (DIV) in popup window.
        /// </summary>
        private delegate ScDomResult SciterShowPopup(IntPtr hePopup, IntPtr heAnchor, POPUP_PLACEMENT placement);

        /// <summary>
        /// Shows block element (DIV) in popup window at given position.
        /// </summary>
        private delegate ScDomResult SciterShowPopupAt(IntPtr hePopup, Point pos, bool animate);

        /// <summary>
        /// Removes popup window.
        /// </summary>
        private delegate ScDomResult SciterHidePopup(IntPtr he);

        /// <summary>
        /// Get/set state bits, stateBits*** accept or'ed values above
        /// </summary>
        private delegate ScDomResult SciterGetElementState(IntPtr he, out ELEMENT_STATE_BITS pstateBits);

        /// <summary>
        /// Get/set state bits, stateBits*** accept or'ed values above
        /// </summary>
        private delegate ScDomResult SciterSetElementState(IntPtr he, ELEMENT_STATE_BITS stateBitsToSet, ELEMENT_STATE_BITS stateBitsToClear, bool updateView);

        /// <summary>
        /// Create Element.CreateElement, the element is disconnected initially from the DOM. 
        /// Element created with ref_count = 1 thus you must call Sciter_UnuseElement on returned handler.
        /// </summary>
        private delegate ScDomResult SciterCreateElement(/*[MarshalAs(UnmanagedType.LPStr)]*/ Byte[] tagname, [MarshalAs(UnmanagedType.LPWStr)] String textOrNull, out IntPtr phe);

        /// <summary>
        /// Create Element.CreateElement as copy of existing element, Element.CreateElement is a full (deep) copy of the element and
        /// is disconnected initially from the DOM.
        /// Element created with ref_count = 1 thus you \b must call Sciter_UnuseElement on returned handler.
        /// </summary>
        private delegate ScDomResult SciterCloneElement(IntPtr he, out IntPtr phe);

        /// <summary>
        /// Insert element at index position of parent.
        /// It is not an error to insert element which already has parent - it will be disconnected first, but 
        /// you need to update elements parent in this case. 
        /// </summary>
        private delegate ScDomResult SciterInsertElement(IntPtr he, IntPtr hparent, int index);

        /// <summary>
        /// Take element out of its container (and DOM tree). 
        /// Element will be destroyed when its reference counter will become zero
        /// </summary>
        private delegate ScDomResult SciterDetach(IntPtr he);

        /// <summary>
        /// Start Timer for the element. 
        /// Element will receive on_timer event
        /// To stop timer call SciterSetTimer( element, 0 );
        /// </summary>
        private delegate ScDomResult SciterSetTimer(IntPtr he, int milliseconds);

        /// <summary>
        /// Attach/Detach ElementEventProc to the element 
        /// See htmlayout::event_handler.
        /// </summary>
        private delegate ScDomResult SciterDetachEventHandler(IntPtr he, IntPtr pep, IntPtr tag);

        /// <summary>
        /// Attach ElementEventProc to the element and subscribe it to events providede by subscription parameter
        /// See Sciter::attach_event_handler.
        /// </summary>
        private delegate ScDomResult SciterAttachEventHandler(IntPtr he, IntPtr pep, IntPtr tag);

        /// <summary>
        /// Attach/Detach ElementEventProc to the Sciter window. 
        /// All events will start first here (in SINKING phase) and if not consumed will end up here.
        /// You can install Window EventHandler only once - it will survive all document reloads.
        /// </summary>
        private delegate ScDomResult SciterWindowAttachEventHandler(IntPtr hwnd, /*[MarshalAs(UnmanagedType.FunctionPtr)] ElementEventProc*/ IntPtr pep, IntPtr tag, EVENT_GROUPS subscription);

        /// <summary>
        /// Attach/Detach ElementEventProc to the Sciter window. 
        /// All events will start first here (in SINKING phase) and if not consumed will end up here.
        /// You can install Window EventHandler only once - it will survive all document reloads.
        /// </summary>
        private delegate ScDomResult SciterWindowDetachEventHandler(IntPtr hwnd, /*[MarshalAs(UnmanagedType.FunctionPtr)] ElementEventProc*/ IntPtr pep, IntPtr tag);

        /// <summary>
        /// SendEvent - sends sinking/bubbling event to the child/parent chain of element element.
        /// First event will be send in SINKING mode (with SINKING flag) - from root to element element itself.
        /// Then from element element to its root on parents chain without SINKING flag (bubbling phase).
        /// </summary>
        private delegate ScDomResult SciterSendEvent(IntPtr he, uint appEventCode, IntPtr heSource, uint reason, out bool handled);

        /// <summary>
        /// PostEvent - post sinking/bubbling event to the child/parent chain of element element.
        /// Function will return immediately posting event into input queue of the application. 
        /// </summary>
        private delegate ScDomResult SciterPostEvent(IntPtr he, uint appEventCode, IntPtr heSource, uint reason);

        /// <summary>
        /// SciterCallMethod - calls behavior specific method.
        /// </summary>
        private delegate ScDomResult SciterCallBehaviorMethod(IntPtr he, [MarshalAs(UnmanagedType.LPArray)] METHOD_PARAMS[] param);

        /// <summary>
        /// SciterRequestElementData - request data download for the element.
        /// </summary>
        private delegate ScDomResult SciterRequestElementData(IntPtr he, [MarshalAs(UnmanagedType.LPWStr)] String url, uint dataType, IntPtr initiator);

        /// <summary>
        /// SciterSendRequest - send GET or POST request for the element
        /// </summary>
        private delegate ScDomResult SciterHttpRequest(IntPtr he, [MarshalAs(UnmanagedType.LPWStr)] String url, uint dataType, uint requestType, IntPtr requestParams, uint nParams);

        /// <summary>
        /// SciterGetScrollInfo - get scroll info of element with overflow:scroll or auto.
        /// </summary>
        private delegate ScDomResult SciterGetScrollInfo(IntPtr he, out Point scrollPos, out Rectangle viewRect, out Size contentSize);

        /// <summary>
        /// SciterSetScrollPos - set scroll position of element with overflow:scroll or auto.
        private delegate ScDomResult SciterSetScrollPos(IntPtr he, Point scrollPos, bool smooth);

        /// <summary>
        /// SciterIsElementVisible - deep visibility, determines if element visible - has no visiblity:hidden and no display:none defined 
        /// for itself or for any its parents.
        /// </summary> 
        private delegate ScDomResult SciterIsElementVisible(IntPtr he, out bool pVisible);

        /// <summary>
        /// SciterIsElementEnabled - deep enable state, determines if element enabled - is not disabled by itself or no one 
        /// </summary>
        private delegate ScDomResult SciterIsElementEnabled(IntPtr he, out bool pEnabled);

        /// <summary>
        /// SciterSortElements
        /// </summary>
        private delegate ScDomResult SciterSortElements(IntPtr he, uint firstIndex, uint lastIndex, [MarshalAs(UnmanagedType.FunctionPtr)] ElementComparator cmpFunc, IntPtr cmpFuncParam);

        /// <summary>
        /// SciterTraverseUIEvent - traverse (sink-and-bubble) MOUSE or KEY event.
        /// </summary>
        private delegate ScDomResult SciterTraverseUIEvent(EVENT_GROUPS evt, IntPtr eventCtlStruct, out bool bOutProcessed);

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
        private delegate ScDomResult SciterRangeCreate(); //IntPtr element, HRANGE* pRange, BOOL outer);
        private delegate ScDomResult SciterRangeFromSelection(); //IntPtr element, HRANGE* pRange);
        private delegate ScDomResult SciterRangeFromPositions(); //IntPtr element, HPOSITION* pStart, HPOSITION* pEnd);
        private delegate ScDomResult SciterRangeUse(); //HRANGE range);
        private delegate ScDomResult SciterRangeFree(); //HRANGE range);
        private delegate ScDomResult SciterRangeAdvancePos(); //HRANGE range, uint cmd, INT* c, HPOSITION* pPos);
        private delegate ScDomResult SciterRangeToHtml(); //HRANGE range, LPBYTE* pHtmlUtf8Bytes, uint* numBytes);
        private delegate ScDomResult SciterRangeReplace(); //HRANGE range, LPBYTE htmlUtf8Bytes, uint numBytes);
        private delegate ScDomResult SciterRangeInsertHtml(); //HPOSITION* pPos, LPBYTE htmlUtf8Bytes, uint numBytes);

        /// <summary>
        /// CallScriptingMethod - calls scripting method defined for the element.
        /// </summary>
        private delegate ScDomResult SciterCallScriptingMethod(IntPtr he, [MarshalAs(UnmanagedType.LPStr)] String name, [MarshalAs(UnmanagedType.LPArray)] JsonValue[] argv, int argc, out JsonValue retval);

        /// <summary>
        /// Attach HWND to the element.
        /// </summary>
        private delegate ScDomResult SciterAttachHwndToElement(IntPtr he, IntPtr hwnd);
        #endregion

#pragma warning disable 0169, 0649
        #region Reserved data

        private int _r1;
        private int _r2;
        private int _r3;
        private int _r4;
        #endregion

        #region Sciter callbacks

        /// <summary>
        /// Marks DOM object as unused (a.k.a. AddRef).
        /// </summary>
        private SciterUseElement HTMLayout_UseElement;

        /// <summary>
        /// Marks DOM object as unused (a.k.a. Release). Get handle of every element's child element.
        /// </summary>
        private SciterUnuseElement HTMLayout_UnuseElement;

        /// <summary>
        /// Get root DOM element of HTML document.
        /// </summary>
        private SciterGetRootElement HTMLayoutGetRootElement;

        /// <summary>
        /// Get focused DOM element of HTML document.
        /// </summary>
        private SciterGetFocusElement HTMLayoutGetFocusElement;

        /// <summary>
        /// Find DOM element by coordinate.
        /// </summary>
        private SciterFindElement HTMLayoutFindElement;

        /// <summary>
        /// Get number of child elements.
        /// </summary>
        private SciterGetChildrenCount HTMLayoutGetChildrenCount;

        /// <summary>
        /// Get handle of every element's child element.
        /// </summary>
        private SciterGetNthChild HTMLayoutGetNthChild;

        /// <summary>
        /// Get parent element.
        /// </summary>
        private SciterGetParentElement HTMLayoutGetParentElement;

        /// <summary>
        /// Get text of the element and information where child elements are placed.
        /// </summary>
        private SciterGetElementHtml HTMLayoutGetElementHtml;

        /// <summary>
        /// Get inner text of the element as LPWSTR (utf16 words).
        /// </summary>
        private SciterGetElementText HTMLayoutGetElementInnerText16;

        /// <summary>
        /// Set inner text of the element from LPCWSTR buffer (utf16 words).
        /// </summary>
        private SciterSetElementText HTMLayoutSetElementInnerText16;

        /// <summary>
        /// Get number of element's attributes.
        /// </summary>
        private SciterGetAttributeCount HTMLayoutGetAttributeCount;

        /// <summary>
        /// Get value of any element's attribute by attribute's number.
        /// </summary>
        private SciterGetNthAttribute HTMLayoutGetNthAttribute;

        /// <summary>
        /// Get value of any element's attribute by name.
        /// </summary>
        private SciterGetAttributeByName HTMLayoutGetAttributeByName;

        /// <summary>
        /// Set attribute's value.
        /// </summary>
        private SciterSetAttributeByName HTMLayoutSetAttributeByName;

        /// <summary>
        /// Remove all attributes from the element.
        /// </summary>
        private SciterClearAttributes HTMLayoutClearAttributes;

        /// <summary>
        /// Get element index. 
        /// </summary>
        private SciterGetElementIndex HTMLayoutGetElementIndex;

        /// <summary>
        /// Get element's text.
        /// </summary>
        private SciterGetElementType HTMLayoutGetElementType;

        /// <summary>
        /// Get element's style attribute.
        /// </summary>
        private SciterGetStyleAttribute HTMLayoutGetStyleAttribute;

        /// <summary>
        /// Set element's style attribute.
        /// </summary>
        private SciterSetStyleAttribute HTMLayoutSetStyleAttribute;

        /// <summary>
        /// Get bounding rectangle of the element.
        /// </summary>
        private SciterGetElementLocation HTMLayoutGetElementLocation;

        /// <summary>
        /// Scroll to view.
        /// </summary>
        private SciterScrollToView HTMLayoutScrollToView;

        /// <summary>
        /// Apply changes and refresh element area in its window.
        /// </summary>
        private SciterUpdateElement HTMLayoutUpdateElement;

        /// <summary>
        /// refresh element area in its window.
        /// </summary>
        private SciterRefreshArea HTMLayoutRefreshArea;

        /// <summary>
        /// Set the mouse capture to the specified element.
        /// </summary>
        private SciterSetCapture HTMLayoutSetCapture;

        /// <summary>
        /// Releases the mouse capture from the specified element.
        /// </summary>
        private SciterReleaseCapture HTMLayoutReleaseCapture;

        /// <summary>
        /// Get IntPtr of containing window.
        /// </summary>
        private SciterGetElementHwnd HTMLayoutGetElementHwnd;

        /// <summary>
        /// Combine given URL with URL of the document element belongs to.
        /// </summary>
        private SciterCombineURL HTMLayoutCombineURL;

        /// <summary>
        /// Call specified function for every element in a DOM that meets specified CSS selectors.
        /// </summary>
        private SciterSelectElements HTMLayoutSelectElements;

        /// <summary>
        /// Find parent of the element by CSS selector. 
        /// </summary>
        private SciterSelectParent HTMLayoutSelectParent;

        /// <summary>
        /// Set inner or outer html of the element.
        /// </summary>
        private SciterSetElementHtml HTMLayoutSetElementHtml;

        /// <summary>
        /// Get Element UID.
        /// </summary>
        private SciterGetElementUID HTMLayoutGetElementUID;

        /// <summary>
        /// Get Element handle by its UID.
        /// </summary>
        private SciterGetElementByUID HTMLayoutGetElementByUID;

        /// <summary>
        /// Shows block element (DIV) in popup window.
        /// </summary>
        private SciterShowPopup HTMLayoutShowPopup;

        /// <summary>
        /// Shows block element (DIV) in popup window at given position.
        /// </summary>
        private SciterShowPopupAt HTMLayoutShowPopupAt;

        /// <summary>
        /// Removes popup window.
        /// </summary>
        private SciterHidePopup HTMLayoutHidePopup;

        /// <summary>
        /// Get/set state bits, stateBits*** accept or'ed values above
        /// </summary>
        private SciterGetElementState HTMLayoutGetElementState;

        /// <summary>
        /// Get/set state bits, stateBits*** accept or'ed values above
        /// </summary>
        private SciterSetElementState HTMLayoutSetElementState;

        /// <summary>
        /// Create Element.CreateElement, the element is disconnected initially from the DOM. 
        /// Element created with ref_count = 1 thus you must call Sciter_UnuseElement on returned handler.
        /// </summary>
        private SciterCreateElement HTMLayoutCreateElement;

        /// <summary>
        /// Create Element.CreateElement as copy of existing element, Element.CreateElement is a full (deep) copy of the element and
        /// is disconnected initially from the DOM.
        /// Element created with ref_count = 1 thus you \b must call Sciter_UnuseElement on returned handler.
        /// </summary>
        private SciterCloneElement HTMLayoutCloneElement;

        /// <summary>
        /// Insert element at index position of parent.
        /// It is not an error to insert element which already has parent - it will be disconnected first, but 
        /// you need to update elements parent in this case. 
        /// </summary>
        private SciterInsertElement HTMLayoutInsertElement;

        /// <summary>
        /// Take element out of its container (and DOM tree). 
        /// Element will be destroyed when its reference counter will become zero
        /// </summary>
        private SciterDetach HTMLayoutDetach;

        /// <summary>
        /// Start Timer for the element. 
        /// Element will receive on_timer event
        /// To stop timer call SciterSetTimer( element, 0 );
        /// </summary>
        private SciterSetTimer HTMLayoutSetTimer;

        /// <summary>
        /// Attach/Detach ElementEventProc to the element 
        /// See htmlayout::event_handler.
        /// </summary>
        private SciterDetachEventHandler HTMLayoutDetachEventHandler;

        /// <summary>
        /// Attach ElementEventProc to the element and subscribe it to events providede by subscription parameter
        /// See Sciter::attach_event_handler.
        /// </summary>
        private SciterAttachEventHandler HTMLayoutAttachEventHandler;

        /// <summary>
        /// Attach/Detach ElementEventProc to the Sciter window. 
        /// All events will start first here (in SINKING phase) and if not consumed will end up here.
        /// You can install Window EventHandler only once - it will survive all document reloads.
        /// </summary>
        private SciterWindowAttachEventHandler HTMLayoutWindowAttachEventHandler;

        /// <summary>
        /// Attach/Detach ElementEventProc to the Sciter window. 
        /// All events will start first here (in SINKING phase) and if not consumed will end up here.
        /// You can install Window EventHandler only once - it will survive all document reloads.
        /// </summary>
        private SciterWindowDetachEventHandler HTMLayoutWindowDetachEventHandler;

        /// <summary>
        /// SendEvent - sends sinking/bubbling event to the child/parent chain of element element.
        /// First event will be send in SINKING mode (with SINKING flag) - from root to element element itself.
        /// Then from element element to its root on parents chain without SINKING flag (bubbling phase).
        /// </summary>
        private SciterSendEvent HTMLayoutSendEvent;

        /// <summary>
        /// PostEvent - post sinking/bubbling event to the child/parent chain of element element.
        /// Function will return immediately posting event into input queue of the application. 
        /// </summary>
        private SciterPostEvent HTMLayoutPostEvent;

        /// <summary>
        /// SciterCallMethod - calls behavior specific method.
        /// </summary>
        private SciterCallBehaviorMethod HTMLayoutCallBehaviorMethod;

        /// <summary>
        /// SciterRequestElementData - request data download for the element.
        /// </summary>
        private SciterRequestElementData HTMLayoutRequestElementData;

        /// <summary>
        /// SciterSendRequest - send GET or POST request for the element
        /// </summary>
        private SciterHttpRequest HTMLayoutHttpRequest;

        /// <summary>
        /// SciterGetScrollInfo - get scroll info of element with overflow:scroll or auto.
        /// </summary>
        private SciterGetScrollInfo HTMLayoutGetScrollInfo;

        /// <summary>
        /// SciterSetScrollPos - set scroll position of element with overflow:scroll or auto.
        private SciterSetScrollPos HTMLayoutSetScrollPos;

        /// <summary>
        /// SciterIsElementVisible - deep visibility, determines if element visible - has no visiblity:hidden and no display:none defined 
        /// for itself or for any its parents.
        /// </summary> 
        private SciterIsElementVisible HTMLayoutIsElementVisible;

        /// <summary>
        /// SciterIsElementEnabled - deep enable state, determines if element enabled - is not disabled by itself or no one 
        /// </summary>
        private SciterIsElementEnabled HTMLayoutIsElementEnabled;

        /// <summary>
        /// SciterSortElements - sort children of the element.
        /// </summary>
        private SciterSortElements HTMLayoutSortElements;

        /// <summary>
        /// SciterTraverseUIEvent - traverse (sink-and-bubble) MOUSE or KEY event.
        /// </summary>
        private SciterTraverseUIEvent HTMLayoutTraverseUIEvent;

        private SciterRangeCreate HTMLayoutRangeCreate;
        private SciterRangeFromSelection HTMLayoutRangeFromSelection;
        private SciterRangeFromPositions HTMLayoutRangeFromPositions;
        private SciterRangeUse HTMLayoutRangeUse;
        private SciterRangeFree HTMLayoutRangeFree;
        private SciterRangeAdvancePos HTMLayoutRangeAdvancePos;
        private SciterRangeToHtml HTMLayoutRangeToHtml;
        private SciterRangeReplace HTMLayoutRangeReplace;
        private SciterRangeInsertHtml HTMLayoutRangeInsertHtml;

        /// <summary>
        /// CallScriptingMethod - calls scripting method defined for the element.
        /// </summary>
        private SciterCallScriptingMethod HTMLayoutCallScriptingMethod;

        /// <summary>
        /// Attach HWND to the element.
        /// </summary>
        private SciterAttachHwndToElement HTMLayoutAttachHwndToElement;

        #endregion
#pragma warning restore 0169, 0649

        /// <summary>
        /// Releases the mouse capture from the specified element.
        /// </summary>
        /// <param name="element"></param>
        public void ReleaseCapture(Element element)
        {
            HTMLayoutReleaseCapture(element.Handle);
        }

        /// <summary>
        /// Deletes element from the DOM Tree
        /// </summary>        
        public void DeleteElement(Element element)
        {
            IntPtr parent;
            CheckResult(HTMLayoutGetParentElement(element.Handle, out parent));
            CheckResult(HTMLayoutDetach(element.Handle));
            CheckResult(HTMLayoutUpdateElement(parent, true));
        }
    }

    #endregion
}