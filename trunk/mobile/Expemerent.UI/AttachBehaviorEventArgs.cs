using System;
using System.Diagnostics;
using Expemerent.UI.Behaviors;
using Expemerent.UI.Dom;
using Expemerent.UI.Native;

namespace Expemerent.UI
{
    /// <summary>
    /// Specifies possible behavior events
    /// </summary>
    public enum EventGroups
    {
        /// <summary>
        /// attached/detached 
        ///</summary>
        Initialization = EVENT_GROUPS.HANDLE_INITIALIZATION,

        /// <summary>
        /// mouse events 
        ///</summary>
        Mouse = EVENT_GROUPS.HANDLE_MOUSE,

        /// <summary>
        /// key events 
        ///</summary>
        Key = EVENT_GROUPS.HANDLE_KEY,

        /// <summary>
        /// focus events, if this flag is set it also means that element it attached to is focusable 
        ///</summary>
        Focus = EVENT_GROUPS.HANDLE_FOCUS,

        /// <summary>
        /// scroll events 
        ///</summary>
        Scroll = EVENT_GROUPS.HANDLE_SCROLL,

        /// <summary>
        /// timer event 
        ///</summary>
        Timer = EVENT_GROUPS.HANDLE_TIMER,

        /// <summary>
        /// size changed event 
        ///</summary>
        Size = EVENT_GROUPS.HANDLE_SIZE,

        /// <summary>
        /// drawing request (event) 
        ///</summary>
        Draw = EVENT_GROUPS.HANDLE_DRAW,

        /// <summary>
        /// requested data () has been delivered 
        ///</summary>
        DataArrived = EVENT_GROUPS.HANDLE_DATA_ARRIVED,

        /// <summary>
        /// secondary, synthetic events: BUTTON_CLICK, HYPERLINK_CLICK, etc.,  a.k.a. notifications from intrinsic behaviors 
        ///</summary>
        BehaviorEvent = EVENT_GROUPS.HANDLE_BEHAVIOR_EVENT,

        /// <summary>
        /// behavior specific methods 
        ///</summary>
        MethodCall = EVENT_GROUPS.HANDLE_METHOD_CALL,

        /// <summary>
        /// behavior specific methods 
        /// </summary>
        ScriptingMethodCall = EVENT_GROUPS.HANDLE_SCRIPTING_METHOD_CALL,

        /// <summary>
        /// all of them 
        ///</summary>
        All = EVENT_GROUPS.HANDLE_ALL,
    }

    [Serializable]
    public class AttachBehaviorEventArgs : EventArgs
    {
        private EventGroups? _eventGroups; 

        /// <summary>
        /// Creates a new instance of the <see cref="AttachBehaviorEventArgs"/> class
        /// </summary>
        public AttachBehaviorEventArgs()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AttachBehaviorEventArgs"/> class
        /// </summary>
        /// <param name="notification"></param>
        internal AttachBehaviorEventArgs(Element element, String behaviorName)
        {
            Element = element;
            BehaviorName = behaviorName;
        }

        /// <summary>
        /// Gets behavior name property
        /// </summary>
        public string BehaviorName { get; protected internal set; }

        /// <summary>
        /// Gets target element 
        /// </summary>
        public Element Element { get; protected internal set; }

        /// <summary>
        /// Gets or sets behavior instance
        /// </summary>
        public SciterBehavior Behavior { get; set; }

        /// <summary>
        /// Gets or sets event grops that behavior will process
        /// </summary>
        /// <remarks>
        /// If not overriden will use value of the <see cref="SciterBehavior.EventGroups"/> property
        /// </remarks>
        public EventGroups EventGroups
        {
            get 
            {
                if (!_eventGroups.HasValue)
                    return Behavior != null ? Behavior.EventGroups : EventGroups.All;
                else
                    return _eventGroups.Value;
 
            }
            set { _eventGroups = value; }
        }
    }
}
