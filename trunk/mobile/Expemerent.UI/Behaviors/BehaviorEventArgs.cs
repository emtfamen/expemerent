using System;
using Expemerent.UI.Dom;
using Expemerent.UI.Native;

namespace Expemerent.UI.Behaviors
{
    /// <summary>
    /// Reason of the behavior event
    /// </summary>
    public enum BehaviorEventReason
    {
        /// <summary>
        /// Mouse
        /// </summary>
        MouseClick = BEHAVIOR_EVENT_PARAMS.EVENT_REASON.BY_MOUSE_CLICK,

        /// <summary>
        /// Keyboard
        /// </summary>
        KeyClick = BEHAVIOR_EVENT_PARAMS.EVENT_REASON.BY_KEY_CLICK,

        /// <summary>
        /// synthesized, programmatically generated.
        /// </summary>
        Synthesized = BEHAVIOR_EVENT_PARAMS.EVENT_REASON.SYNTHESIZED,

        /// <summary>
        /// single char insertion
        /// </summary>
        InsChar = BEHAVIOR_EVENT_PARAMS.EVENT_REASON.BY_INS_CHAR,

        /// <summary>
        /// character range insertion, clipboard
        /// </summary>
        InsChars = BEHAVIOR_EVENT_PARAMS.EVENT_REASON.BY_INS_CHARS, 

        /// <summary>
        /// single char deletion
        /// </summary>
        DelChar = BEHAVIOR_EVENT_PARAMS.EVENT_REASON.BY_DEL_CHAR,  

        /// <summary>
        /// character range deletion (selection)
        /// </summary>
        DelChars = BEHAVIOR_EVENT_PARAMS.EVENT_REASON.BY_DEL_CHARS, 
    }

    /// <summary>
    /// Kind of behavior event
    /// </summary>
    public enum BehaviorEventType
    {
        /// <summary>
        /// Click on button
        /// </summary>
        ButtonClick = BEHAVIOR_EVENT_PARAMS.BEHAVIOR_EVENTS.BUTTON_CLICK, 
        
        /// <summary>
        /// Mouse down or key down in button
        /// </summary>
        ButtonPress = BEHAVIOR_EVENT_PARAMS.BEHAVIOR_EVENTS.BUTTON_PRESS, 

        /// <summary>
        /// checkbox/radio/slider changed its state/value 
        /// </summary>
        ButtonStateChanged = BEHAVIOR_EVENT_PARAMS.BEHAVIOR_EVENTS.BUTTON_STATE_CHANGED, 

        /// <summary>
        /// before text change
        /// </summary>
        EditValueChanging = BEHAVIOR_EVENT_PARAMS.BEHAVIOR_EVENTS.EDIT_VALUE_CHANGING, 

        /// <summary>
        /// after text change
        /// </summary>
        EditValueChanged = BEHAVIOR_EVENT_PARAMS.BEHAVIOR_EVENTS.EDIT_VALUE_CHANGED, 

        /// <summary>
        /// selection in &lt;select&gt; changed
        /// </summary>
        SelectSelectionChanged = BEHAVIOR_EVENT_PARAMS.BEHAVIOR_EVENTS.SELECT_SELECTION_CHANGED, 
        
        /// <summary>
        /// node in select expanded/collapsed, heTarget is the node
        /// </summary>
        SelectStateChanged = BEHAVIOR_EVENT_PARAMS.BEHAVIOR_EVENTS.SELECT_STATE_CHANGED, 

        /// <summary>
        /// request to show popup just received, here DOM of popup element can be modifed.
        /// </summary>
        PopupRequest = BEHAVIOR_EVENT_PARAMS.BEHAVIOR_EVENTS.POPUP_REQUEST, 
        
        /// <summary>
        /// popup element has been measured and ready to be shown on screen, here you can use functions like ScrollToView.
        /// </summary>
        PopupReady = BEHAVIOR_EVENT_PARAMS.BEHAVIOR_EVENTS.POPUP_READY, 
        
        /// <summary>
        /// popup element is closed, here DOM of popup element can be modifed again - e.g. some items can be removed to free memory.
        /// </summary>
        PopupDismissed = BEHAVIOR_EVENT_PARAMS.BEHAVIOR_EVENTS.POPUP_DISMISSED, 

        /// <summary>
        /// menu item activated by mouse hover or by keyboard,
        /// </summary>
        MenuItemActive = BEHAVIOR_EVENT_PARAMS.BEHAVIOR_EVENTS.MENU_ITEM_ACTIVE, 

        /// <summary>
        /// menu item click,
        /// </summary>
        MenuItemClick = BEHAVIOR_EVENT_PARAMS.BEHAVIOR_EVENTS.MENU_ITEM_CLICK,  

        //   BEHAVIOR_EVENT_PARAMS structure layout
        //   BEHAVIOR_EVENT_PARAMS.cmd - MENU_ITEM_CLICK/MENU_ITEM_ACTIVE   
        //   BEHAVIOR_EVENT_PARAMS.heTarget - owner(anchor) of the menu
        //   BEHAVIOR_EVENT_PARAMS.element - the menu item, presumably <li> element
        //   BEHAVIOR_EVENT_PARAMS.reason - BY_MOUSE_CLICK | BY_KEY_CLICK

        /// <summary>
        /// "right-click", BEHAVIOR_EVENT_PARAMS::element is current popup menu HELEMENT being processed or NULL.
        /// application can provide its own HELEMENT here (if it is NULL) or modify current menu element.
        /// </summary>
        ContextMenuRequest = BEHAVIOR_EVENT_PARAMS.BEHAVIOR_EVENTS.CONTEXT_MENU_REQUEST, 

        /// <summary>
        /// broadcast notification, sent to all elements of some container being shown or hidden   
        /// </summary>
        VisualStatusChange = BEHAVIOR_EVENT_PARAMS.BEHAVIOR_EVENTS.VISIUAL_STATUS_CHANGED, 

        //// "grey" event codes  - notfications from behaviors from this SDK 
        //BEHAVIOR_EVENTS.HYPERLINK_CLICK = 0x80, // hyperlink click
        //BEHAVIOR_EVENTS.TABLE_HEADER_CLICK, // click on some cell in table header, 
        ////     target = the cell, 
        ////     reason = index of the cell (column number, 0..inder)
        //BEHAVIOR_EVENTS.TABLE_ROW_CLICK, // click on data row in the table, target is the row
        ////     target = the row, 
        ////     reason = index of the row (fixed_rows..inder)
        //BEHAVIOR_EVENTS.TABLE_ROW_DBL_CLICK, // mouse dbl click on data row in the table, target is the row
        ////     target = the row, 
        ////     reason = index of the row (fixed_rows..inder)

        /// <summary>
        /// element was collapsed, so far only behavior:tabs is sending these two to the panels
        /// </summary>
        ElementCollapsed = BEHAVIOR_EVENT_PARAMS.BEHAVIOR_EVENTS.ELEMENT_COLLAPSED, 

        /// <summary>
        /// element was expanded,
        /// </summary>
        ElementExpanded = BEHAVIOR_EVENT_PARAMS.BEHAVIOR_EVENTS.ELEMENT_EXPANDED, 

        /// <summary>
        /// activate (select) child, 
        /// used for example by accesskeys behaviors to send activation request, e.g. tab on behavior:tabs. 
        /// </summary>
        ActivateChild = BEHAVIOR_EVENT_PARAMS.BEHAVIOR_EVENTS.ACTIVATE_CHILD, 

        //BEHAVIOR_EVENTS.DO_SWITCH_TAB = ACTIVATE_CHILD, // command to switch tab programmatically, handled by behavior:tabs 
        //// use it as HTMLayoutPostEvent(tabsElementOrItsChild, DO_SWITCH_TAB, tabElementToShow, 0);

        //BEHAVIOR_EVENTS.FIRST_APPLICATION_EVENT_CODE = 0x100
        //// all custom event codes shall be greater
        //// than this number. All codes below this will be used
        //// solely by application - HTMLayout will not intrepret it 
        //// and will do just dispatching.
        //// To send event notifications with  these codes use
        //// HTMLayoutSend/PostEvent API.
    }

    public class BehaviorEventArgs : InputEventArgs
    {
        /// <summary>
        /// Crates a new instance of the <see cref=""/>
        /// </summary>
        internal BehaviorEventArgs(Element element, Phase phase)
            : base(element, phase)
        {
        }

        /// <summary>
        /// Gets or sets kind of the event
        /// </summary>
        public BehaviorEventType BehaviorEvent { get; internal set; }

        /// <summary>
        /// Gets or sets event reason
        /// </summary>
        public BehaviorEventReason Reason { get; internal set; }

        /// <summary>
        /// Gets or sets target element 
        /// </summary>
        public Element Source { get; internal set; }
    }
}