using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Expemerent.UI.Native
{
    using HDC = IntPtr;
    using HELEMENT = IntPtr;
    using LPCBYTE = IntPtr;
    using UINT = UInt32;
    using LPCWSTR = IntPtr;
    using BOOL = Boolean;
    using LPCSTR = IntPtr;

    [StructLayout(LayoutKind.Sequential)]
    internal struct INITIALIZATION_PARAMS
    {
        internal enum INITIALIZATION_EVENTS
        {
            BEHAVIOR_DETACH = 0,
            BEHAVIOR_ATTACH = 1
        }

        /// <summary>
        /// INITIALIZATION_EVENTS
        /// </summary>
        internal INITIALIZATION_EVENTS cmd;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DATA_ARRIVED_PARAMS
    {
        /// <summary>
        ///element intiator of HTMLayoutRequestElementData request, 
        ///</summary>
        internal HELEMENT initiator;

        /// <summary>
        ///data buffer
        ///</summary>
        internal LPCBYTE data;

        /// <summary>
        ///size of data
        ///</summary>
        internal UINT dataSize;

        /// <summary>
        ///data type passed "as is" from HTMLayoutRequestElementData
        ///</summary>
        internal UINT dataType;

#if HTMLAYOUT
        /// <summary>
        ///status = 0 (dataSize == 0) - unknown error. status = 100..505 - http response status, Note: 200 - OK!  status > 12000 - wininet error code, see ERROR_INTERNET_*** in wininet.h
        ///</summary>
        internal UINT status;

        /// <summary>
        ///requested url 
        ///</summary>
        internal LPCWSTR uri;
#endif
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DRAW_PARAMS
    {
        public enum DRAW_EVENTS
        {
            DRAW_BACKGROUND = 0,
            DRAW_CONTENT = 1,
            DRAW_FOREGROUND = 2,
        };

        /// <summary>
        ///DRAW_EVENTS
        ///</summary>
        internal DRAW_EVENTS cmd;

        /// <summary>
        ///hdc to paint on
        ///</summary>
        internal HDC hdc;

        /// <summary>
        /// element area to paint,  
        /// for DRAW_BACKGROUND/DRAW_FOREGROUND - it is a border box for DRAW_CONTENT - it is a content box
        ///</summary>
        internal Rectangle area;

        /// <summary>
        ///  
        ///</summary>
        internal UINT reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct BEHAVIOR_EVENT_PARAMS
    {
        internal enum BEHAVIOR_EVENTS
        {
            /// <summary>
            ///click on button
            ///</summary>
            BUTTON_CLICK = 0,

            /// <summary>
            ///mouse down or key down in button
            ///</summary>
            BUTTON_PRESS = 1,

            /// <summary>
            ///checkbox/radio/slider changed its state/value 
            ///</summary>
            BUTTON_STATE_CHANGED = 2,

            /// <summary>
            ///before text change
            ///</summary>
            EDIT_VALUE_CHANGING = 3,

            /// <summary>
            ///after text change
            ///</summary>
            EDIT_VALUE_CHANGED = 4,

            /// <summary>
            ///selection in <select> changed
            ///</summary>
            SELECT_SELECTION_CHANGED = 5,

            /// <summary>
            ///node in select expanded/collapsed, heTarget is the node
            ///</summary>
            SELECT_STATE_CHANGED = 6,

            /// <summary>
            ///request to show popup just received, here DOM of popup element can be modifed.
            ///</summary>
            POPUP_REQUEST = 7,

            /// <summary>
            ///popup element has been measured and ready to be shown on screen, here you can use functions like ScrollToView.
            ///</summary>
            POPUP_READY = 8,

            /// <summary>
            ///popup element is closed, here DOM of popup element can be modifed again - e.g. some items can be removed to free memory.
            ///</summary>
            POPUP_DISMISSED = 9,

            /// <summary>
            ///menu item activated by mouse hover or by keyboard,
            ///</summary>
            MENU_ITEM_ACTIVE = 0xA,

            /// <summary>
            ///menu item click, BEHAVIOR_EVENT_PARAMS structure layout BEHAVIOR_EVENT_PARAMS.cmd - MENU_ITEM_CLICK/MENU_ITEM_ACTIVE BEHAVIOR_EVENT_PARAMS.heTarget - the menu item, presumably <li> element BEHAVIOR_EVENT_PARAMS.reason - BY_MOUSE_CLICK | BY_KEY_CLICK
            ///</summary>
            MENU_ITEM_CLICK = 0xB,

            /// <summary>
            ///evt.he is a menu dom element that is about to be shown. You can disable/enable items in it.      
            ///</summary>
            CONTEXT_MENU_SETUP = 0xF,

            /// <summary>
            ///"right-click", BEHAVIOR_EVENT_PARAMS::he is current popup menu HELEMENT being processed or NULL. application can provide its own HELEMENT here (if it is NULL) or modify current menu element.
            ///</summary>
            CONTEXT_MENU_REQUEST = 0x10,

            /// <summary>
            ///broadcast notification, sent to all elements of some container being shown or hidden "grey" event codes  - notfications from behaviors from this SDK 
            ///</summary>
            VISIUAL_STATUS_CHANGED = 0x11,

            /// <summary>
            ///hyperlink click
            ///</summary>
            HYPERLINK_CLICK = 0x80,

            /// <summary>
            ///click on some cell in table header, target = the cell, reason = index of the cell (column number, 0..n)
            ///</summary>
            TABLE_HEADER_CLICK,

            /// <summary>
            ///click on data row in the table, target is the row target = the row, reason = index of the row (fixed_rows..n)
            ///</summary>
            TABLE_ROW_CLICK,

            /// <summary>
            ///mouse dbl click on data row in the table, target is the row target = the row, reason = index of the row (fixed_rows..n)
            ///</summary>
            TABLE_ROW_DBL_CLICK,

            /// <summary>
            ///element was collapsed, so far only behavior:tabs is sending these two to the panels
            ///</summary>
            ELEMENT_COLLAPSED = 0x90,

            /// <summary>
            ///element was expanded,
            ///</summary>
            ELEMENT_EXPANDED,

            /// <summary>
            ///activate (select) child, used for example by accesskeys behaviors to send activation request, e.g. tab on behavior:tabs. 
            ///</summary>
            ACTIVATE_CHILD,

            /// <summary>
            /// command to switch tab programmatically, handled by behavior:tabs use it as HTMLayoutPostEvent(tabsElementOrItsChild, DO_SWITCH_TAB, tabElementToShow, 0);
            /// </summary>
            DO_SWITCH_TAB = ACTIVATE_CHILD,

            /// <summary>
            ///request to virtual grid to initialize its view
            ///</summary>
            INIT_DATA_VIEW,

            /// <summary>
            ///request from virtual grid to data source behavior to fill data in the table parameters passed throug DATA_ROWS_PARAMS structure.
            ///</summary>
            ROWS_DATA_REQUEST,

            /// <summary>
            ///ui state changed, observers shall update their visual states. is sent for example by behavior:richtext when caret position/selection has changed.
            ///</summary>
            UI_STATE_CHANGED,

            /// <summary>
            ///all custom event codes shall be greater than this number. All codes below this will be used solely by application - HTMLayout will not intrepret it  and will do just dispatching. To send event notifications with  these codes use HTMLayoutSend/PostEvent API.
            ///</summary>
            FIRST_APPLICATION_EVENT_CODE = 0x100
        }

        internal enum EVENT_REASON
        {
            BY_MOUSE_CLICK = 0,
            BY_KEY_CLICK = 1,

            /// <summary>
            ///synthesized, programmatically generated.
            ///</summary>
            SYNTHESIZED = 2,

            /// <summary>
            ///single char insertion
            ///</summary>
            BY_INS_CHAR = 3,

            /// <summary>
            ///character range insertion, clipboard
            ///</summary>
            BY_INS_CHARS,

            /// <summary>
            ///single char deletion
            ///</summary>
            BY_DEL_CHAR,

            /// <summary>
            ///character range deletion (selection)
            ///</summary>
            BY_DEL_CHARS,
        }


        /// <summary>
        ///BEHAVIOR_EVENTS
        ///</summary>
        internal BEHAVIOR_EVENTS cmd;

        /// <summary>
        ///target element handler
        ///</summary>
        internal HELEMENT heTarget;

        /// <summary>
        ///source element e.g. in SELECTION_CHANGED it is new selected <option>, in MENU_ITEM_CLICK it is menu item (LI) element
        ///</summary>
        internal HELEMENT he;

        /// <summary>
        ///EVENT_REASON or EDIT_CHANGED_REASON - UI action causing change. In case of custom event notifications this may be any application specific value.
        ///</summary>
        internal EVENT_REASON reason;
    };

    internal enum KEYBOARD_STATES
    {
        CONTROL_KEY_PRESSED = 0x1,
        SHIFT_KEY_PRESSED = 0x2,
        ALT_KEY_PRESSED = 0x4
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct KEY_PARAMS
    {
        internal enum KEY_EVENTS
        {
            KEY_DOWN = 0,
            KEY_UP,
            KEY_CHAR
        }

        /// <summary>
        ///KEY_EVENTS
        ///</summary>
        internal KEY_EVENTS cmd;

        /// <summary>
        ///target element
        ///</summary>
        internal HELEMENT target;

        /// <summary>
        ///key scan code, or character unicode for KEY_CHAR
        ///</summary>
        internal int key_code;

        /// <summary>
        ///KEYBOARD_STATES   
        ///</summary>
        internal KEYBOARD_STATES alt_state;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct FOCUS_PARAMS
    {
        internal enum FOCUS_EVENTS
        {
            FOCUS_LOST = 0,
            FOCUS_GOT = 1,
        }

        /// <summary>
        ///FOCUS_EVENTS
        ///</summary>
        internal FOCUS_EVENTS cmd;

        /// <summary>
        ///target element, for FOCUS_LOST it is a handle of new focus element and for FOCUS_GOT it is a handle of old focus element, can be NULL
        ///</summary>
        internal HELEMENT target;

        /// <summary>
        ///TRUE if focus is being set by mouse click
        ///</summary>
        internal BOOL by_mouse_click;

        /// <summary>
        ///in FOCUS_LOST phase setting this field to TRUE will cancel transfer focus from old element to the new one.
        ///</summary>
        internal BOOL cancel;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MOUSE_PARAMS
    {
        internal enum MOUSE_EVENTS
        {
            MOUSE_ENTER = 0,
            MOUSE_LEAVE = 1,
            MOUSE_MOVE = 2,
            MOUSE_UP = 3,
            MOUSE_DOWN = 4,
            MOUSE_DCLICK = 5,
            MOUSE_WHEEL = 6,
            /// <summary>
            ///mouse pressed ticks
            ///</summary>
            MOUSE_TICK = 7,
            /// <summary>
            ///mouse stay idle for some time
            ///</summary>
            MOUSE_IDLE = 8,

            /// <summary>
            ///item dropped, target is that dropped item 
            ///</summary>
            DROP = 9,
            /// <summary>
            ///drag arrived to the target element that is one of current drop targets.  
            ///</summary>
            DRAG_ENTER = 0xA,
            /// <summary>
            ///drag left one of current drop targets. target is the drop target element.  
            ///</summary>
            DRAG_LEAVE = 0xB,

            /// <summary>
            ///This flag is 'ORed' with MOUSE_ENTER..MOUSE_DOWN codes if dragging operation is in effect. In this case 
            ///</summary>
            DRAGGING = 0x100,
        }

        internal enum CURSOR_TYPE
        {
            /// <summary>
            ///0
            ///</summary>
            CURSOR_ARROW,
            /// <summary>
            ///1
            ///</summary>
            CURSOR_IBEAM,
            /// <summary>
            ///2
            ///</summary>
            CURSOR_WAIT,
            /// <summary>
            ///3
            ///</summary>
            CURSOR_CROSS,
            /// <summary>
            ///4
            ///</summary>
            CURSOR_UPARROW,
            /// <summary>
            ///5
            ///</summary>
            CURSOR_SIZENWSE,
            /// <summary>
            ///6
            ///</summary>
            CURSOR_SIZENESW,
            /// <summary>
            ///7
            ///</summary>
            CURSOR_SIZEWE,
            /// <summary>
            ///8
            ///</summary>
            CURSOR_SIZENS,
            /// <summary>
            ///9 
            ///</summary>
            CURSOR_SIZEALL,
            /// <summary>
            ///10
            ///</summary>
            CURSOR_NO,
            /// <summary>
            ///11
            ///</summary>
            CURSOR_APPSTARTING,
            /// <summary>
            ///12
            ///</summary>
            CURSOR_HELP,
            /// <summary>
            ///13
            ///</summary>
            CURSOR_HAND,
            /// <summary>
            ///14 
            ///</summary>
            CURSOR_DRAG_MOVE,
            /// <summary>
            ///15
            ///</summary>
            CURSOR_DRAG_COPY,
        }

        internal enum MOUSE_BUTTONS
        {
            MAIN_MOUSE_BUTTON = 1, //aka left button
            PROP_MOUSE_BUTTON = 2, //aka right button
            MIDDLE_MOUSE_BUTTON = 4,
        }

        /// <summary>
        ///MOUSE_EVENTS
        ///</summary>
        internal MOUSE_EVENTS cmd;
        /// <summary>
        ///target element
        ///</summary>
        internal HELEMENT target;
        /// <summary>
        ///position of cursor, element relative
        ///</summary>
        internal Point pos;
        /// <summary>
        ///position of cursor, document root relative
        ///</summary>
        internal Point pos_document;
        /// <summary>
        ///MOUSE_BUTTONS or MOUSE_WHEEL_DELTA
        ///</summary>
        internal UINT button_state;
        /// <summary>
        ///KEYBOARD_STATES 
        ///</summary>
        internal KEYBOARD_STATES alt_state;
        /// <summary>
        ///CURSOR_TYPE to set, see CURSOR_TYPE
        ///</summary>
        internal CURSOR_TYPE cursor_type;
        /// <summary>
        ///mouse is over icon (foreground-image, foreground-repeat:no-repeat)
        ///</summary>
        internal BOOL is_on_icon;

        /// <summary>
        ///element that is being dragged over, this field is not NULL if (cmd & DRAGGING) != 0
        ///</summary>
        ///TODO: Only in htmlayout
        ///internal HELEMENT dragging;

        /// <summary>
        /// see DRAGGING_TYPE. 
        ///</summary>
        ///TODO: Only in htmlayout
        ///internal UINT dragging_mode;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct METHOD_PARAMS
    {
        internal enum BEHAVIOR_METHOD_IDENTIFIERS 
        {
            DO_CLICK = 0,
            GET_TEXT_VALUE = 1,
            /// <summary>
            /// p - TEXT_VALUE_PARAMS
            ///</summary>
            SET_TEXT_VALUE,

            /// <summary>
            /// p - TEXT_EDIT_SELECTION_PARAMS
            ///</summary>
            TEXT_EDIT_GET_SELECTION,

            /// <summary>
            /// p - TEXT_EDIT_SELECTION_PARAMS Replace selection content or insert text at current caret position. Replaced text will be selected. 
            ///</summary>
            TEXT_EDIT_SET_SELECTION,
            /// <summary>
            /// p - TEXT_EDIT_REPLACE_SELECTION_PARAMS 
            ///</summary>
            TEXT_EDIT_REPLACE_SELECTION,

            /// <summary>
            /// Set value of type="vscrollbar"/"hscrollbar"
            ///</summary>
            SCROLL_BAR_GET_VALUE,
            SCROLL_BAR_SET_VALUE,

            TEXT_EDIT_GET_CARET_POSITION,// get current caret position, it returns rectangle that is relative to origin of the editing element. p - TEXT_CARET_POSITION_PARAMS

            /// <summary>
            /// p - TEXT_SELECTION_PARAMS, OutputStreamProc will receive stream of WCHARs
            ///</summary>
            TEXT_EDIT_GET_SELECTION_TEXT,
            /// <summary>
            /// p - TEXT_SELECTION_PARAMS, OutputStreamProc will receive stream of BYTEs - utf8 encoded html fragment.
            ///</summary>
            TEXT_EDIT_GET_SELECTION_HTML,

            /// <summary>
            /// p - XCALL_PARAMS
            ///</summary>
            XCALL = 0xff,
            FIRST_APPLICATION_METHOD_ID = 0x100
        }

        /// <summary>
        /// see: #BEHAVIOR_METHOD_IDENTIFIERS
        /// </summary>
        internal int methodID;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct XCALL_PARAMS
    {
        /// <summary>
        /// see: #BEHAVIOR_METHOD_IDENTIFIERS
        /// </summary>
        internal int methodID;

        ///<summary>
        /// method name
        ///</summary>
        internal LPCSTR name;

        /// <summary>
        /// argument count
        ///</summary>
        internal UINT argc;

        /// <summary>
        /// vector of arguments [SCITER_VALUE]
        ///</summary>
        internal IntPtr argv;

        /// <summary>
        /// return value
        ///</summary>
        internal JsonValue result;

        /// <summary>
        /// Returns method name
        /// </summary>
        public string GetName()
        {
            return MarshalUtility.PtrToStringAnsi(name);
        }

        /// <summary>
        /// Returns arguments array
        /// </summary>
        public object[] GetArgs()
        {
            var args = new object[argc];
            for (int i = 0; i < argc; i++)
            {
                var json_val = (JsonValue)Marshal.PtrToStructure(new IntPtr(argv.ToInt64() + i * JsonValue.SizeOf), typeof(JsonValue));
                args[i] = json_val.GetValue();
            }

            return args;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SCRIPTING_METHOD_PARAMS
    {
        ///<summary>
        /// method name
        ///</summary>
        internal LPCSTR name;

        /// <summary>
        /// vector of arguments [SCITER_VALUE]
        ///</summary>
        internal IntPtr argv;

        /// <summary>
        /// argument count
        ///</summary>
        internal UINT argc;

        /// <summary>
        /// return value
        ///</summary>
        internal JsonValue result;

        /// <summary>
        /// Returns method name
        /// </summary>
        public string GetName()
        {
            return MarshalUtility.PtrToStringAnsi(name);
        }

        /// <summary>
        /// Returns arguments array
        /// </summary>
        public object[] GetArgs()
        {
            var args = new object[argc];
            for (int i = 0; i < argc; i++)
            {
                var json_val = (JsonValue)Marshal.PtrToStructure(new IntPtr(argv.ToInt64() + i * JsonValue.SizeOf), typeof(JsonValue));
                args[i] = json_val.GetValue();
            }

            return args;
        }
    }
}
