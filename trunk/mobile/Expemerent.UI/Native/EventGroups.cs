using System;
using System.Collections.Generic;
using System.Text;

namespace Expemerent.UI.Native
{
    [Flags]
    public enum EVENT_GROUPS
    {
        /// <summary>
        /// attached/detached 
        ///</summary>
        HANDLE_INITIALIZATION = 0x0000,

        /// <summary>
        /// mouse events 
        ///</summary>
        HANDLE_MOUSE = 0x0001,

        /// <summary>
        /// key events 
        ///</summary>
        HANDLE_KEY = 0x0002,

        /// <summary>
        /// focus events, if this flag is set it also means that element it attached to is focusable 
        ///</summary>
        HANDLE_FOCUS = 0x0004,

        /// <summary>
        /// scroll events 
        ///</summary>
        HANDLE_SCROLL = 0x0008,

        /// <summary>
        /// timer event 
        ///</summary>
        HANDLE_TIMER = 0x0010,

        /// <summary>
        /// size changed event 
        ///</summary>
        HANDLE_SIZE = 0x0020,

        /// <summary>
        /// drawing request (event) 
        ///</summary>
        HANDLE_DRAW = 0x0040,

        /// <summary>
        /// requested data () has been delivered 
        ///</summary>
        HANDLE_DATA_ARRIVED = 0x080,

        /// <summary>
        /// secondary, synthetic events: BUTTON_CLICK, HYPERLINK_CLICK, etc.,  a.k.a. notifications from intrinsic behaviors 
        ///</summary>
        HANDLE_BEHAVIOR_EVENT = 0x0100,

        /// <summary>
        /// behavior specific methods 
        ///</summary>
        HANDLE_METHOD_CALL = 0x0200,

        /// <summary>
        /// behavior specific methods 
        /// </summary>
        HANDLE_SCRIPTING_METHOD_CALL = 0x0400, 

        /// <summary>
        /// all of them 
        ///</summary>
        HANDLE_ALL = 0x07FF,

        /// <summary>
        /// disable INITIALIZATION events to be sent. normally engine sends BEHAVIOR_DETACH / BEHAVIOR_ATTACH events unconditionally, this flag allows to disable this behavior 
        ///</summary>
        DISABLE_INITIALIZATION = unchecked((int)0x80000000),

        /// <summary>
        /// bubbling (emersion) phase
        /// </summary>
        BUBBLING = 0,        

        /// <summary>
        /// capture (immersion) phase, this flag is or'ed with EVENTS codes below
        /// </summary>
        SINKING  = 0x08000,  
        
        /// <summary>
        /// event already processed.
        /// </summary>
        HANDLED  = 0x10000   
    }
}
