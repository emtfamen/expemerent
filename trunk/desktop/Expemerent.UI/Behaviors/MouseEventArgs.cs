using System;
using System.Diagnostics;
using Expemerent.UI.Dom;
using Expemerent.UI.Native;
using System.Drawing;

namespace Expemerent.UI.Behaviors
{
    /// <summary>
    /// Mouse event kind
    /// </summary>
    public enum MouseEvent 
    {
        /// <summary>
        /// Mouse enter
        /// </summary>
        MouseEnter = MOUSE_PARAMS.MOUSE_EVENTS.MOUSE_ENTER,

        /// <summary>
        /// Mouse leave
        /// </summary>
        MouseLeave = MOUSE_PARAMS.MOUSE_EVENTS.MOUSE_LEAVE,

        /// <summary>
        /// Mouse move
        /// </summary>
        MouseMove = MOUSE_PARAMS.MOUSE_EVENTS.MOUSE_MOVE,

        /// <summary>
        /// Mouse up
        /// </summary>
        MouseUp = MOUSE_PARAMS.MOUSE_EVENTS.MOUSE_UP,

        /// <summary>
        /// Mouse down
        /// </summary>
        MouseDown = MOUSE_PARAMS.MOUSE_EVENTS.MOUSE_DOWN,

        /// <summary>
        /// MouseDblClick
        /// </summary>
        MouseDblClick = MOUSE_PARAMS.MOUSE_EVENTS.MOUSE_DCLICK,

        /// <summary>
        /// Mouse wheel event
        /// </summary>
        MouseWheel = MOUSE_PARAMS.MOUSE_EVENTS.MOUSE_WHEEL,

        /// <summary>
        /// mouse pressed ticks
        /// </summary>
        MouseTick = MOUSE_PARAMS.MOUSE_EVENTS.MOUSE_TICK, 

        /// <summary>
        /// mouse stay idle for some time
        /// </summary>
        MouseIdle = MOUSE_PARAMS.MOUSE_EVENTS.MOUSE_IDLE, 
    } ;

    /// <summary>
    /// CursorType kind
    /// </summary>
    public enum CursorType
    {
        Arrow = MOUSE_PARAMS.CURSOR_TYPE.CURSOR_ARROW,
        IBeam = MOUSE_PARAMS.CURSOR_TYPE.CURSOR_IBEAM,
        Wait = MOUSE_PARAMS.CURSOR_TYPE.CURSOR_WAIT,
        Cross = MOUSE_PARAMS.CURSOR_TYPE.CURSOR_CROSS, 
        UpArrow = MOUSE_PARAMS.CURSOR_TYPE.CURSOR_UPARROW,
        SizeNWSE = MOUSE_PARAMS.CURSOR_TYPE.CURSOR_SIZENWSE,
        SizeNESW = MOUSE_PARAMS.CURSOR_TYPE.CURSOR_SIZENESW,
        SizeWE = MOUSE_PARAMS.CURSOR_TYPE.CURSOR_SIZEWE,
        SizeNS = MOUSE_PARAMS.CURSOR_TYPE.CURSOR_SIZENS,
        SizeAll = MOUSE_PARAMS.CURSOR_TYPE.CURSOR_SIZEALL,
        No = MOUSE_PARAMS.CURSOR_TYPE.CURSOR_NO,
        AppStarting = MOUSE_PARAMS.CURSOR_TYPE.CURSOR_APPSTARTING,
        Help = MOUSE_PARAMS.CURSOR_TYPE.CURSOR_HELP,
        Hand = MOUSE_PARAMS.CURSOR_TYPE.CURSOR_HAND,
        DragMove = MOUSE_PARAMS.CURSOR_TYPE.CURSOR_DRAG_MOVE,
        DragCopy = MOUSE_PARAMS.CURSOR_TYPE.CURSOR_DRAG_COPY, 
    } ;

    [Flags]
    public enum MouseButtons 
    {
        Left = MOUSE_PARAMS.MOUSE_BUTTONS.MAIN_MOUSE_BUTTON,
        Right = MOUSE_PARAMS.MOUSE_BUTTONS.PROP_MOUSE_BUTTON,
        Middle = MOUSE_PARAMS.MOUSE_BUTTONS.MIDDLE_MOUSE_BUTTON,
    } 

    /// <summary>
    /// Represents a mouse event
    /// </summary>
    [Serializable]
    [DebuggerStepThrough]
    public class MouseEventArgs : UserInputEventArgs
    {
        /// <summary>
        /// Type of the mouse event
        /// </summary>
        public MouseEvent MouseEvent { get; internal set; }
       
        /// <summary>
        /// position of cursor, element relative
        /// </summary>
        public Point Position { get; internal set; }
        
        /// <summary>
        /// position of cursor, document root relative
        /// </summary>
        public Point PositionInDoc { get; internal set; }

        /// <summary>
        /// State of the mouse buttons
        /// </summary>
        public MouseButtons MouseButtons { get; internal set; }
               
        /// <summary>
        /// CURSOR_TYPE to set, see CURSOR_TYPE
        /// </summary>
        public CursorType CursorType { get; internal set; }

        /// <summary>
        /// mouse is over icon (foreground-image, foreground-repeat:no-repeat)
        /// </summary>
        public bool IsOverIcon { get; internal set; } 

        /// <summary>
        /// Creates a new instance of the <see cref="MouseEventArgs"/> class
        /// </summary>        
        internal MouseEventArgs(Element element, Phase phase)
            : base(element, phase)
        {
        }
    }
}