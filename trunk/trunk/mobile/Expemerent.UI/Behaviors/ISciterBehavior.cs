using System;
using Expemerent.UI.Native;

namespace Expemerent.UI.Behaviors
{
    /// <summary>
    /// Defines behavior interface
    /// </summary>
    internal interface ISciterBehavior
    {
        /// <summary>
        /// Handles behavior initialization
        /// </summary>
        void ProcessAttach(ElementEventArgs e);

        /// <summary>
        /// Handles behavior deinitialization
        /// </summary>
        void ProcessDettach(ElementEventArgs e);

        /// <summary>
        /// Handles mouse events
        /// </summary>
        void ProcessMouse(MouseEventArgs e);

        /// <summary>
        /// Handles keyboard events
        /// </summary>
        void ProcessKey(KeyEventArgs e);

        /// <summary>
        /// Handles focus events
        /// </summary>
        void ProcessFocus(FocusEventArgs e);

        /// <summary>
        /// Handles draw events
        /// </summary>
        void ProcessDraw(DrawEventArgs e);

        /// <summary>
        /// Handles timer event
        /// </summary>
        void ProcessTimer(ElementEventArgs e);

        /// <summary>
        /// Handles secondary behavior event
        /// </summary>
        void ProcessBehaviorEvent(BehaviorEventArgs e);

        /// <summary>
        /// Handles method call event
        /// </summary>
        void ProcessMethodCall(MethodCallEventArgs e);

        /// <summary>
        /// Handles data arrived event
        /// </summary>
        void ProcessDataArrived(DataArrivedEventArgs e);

        /// <summary>
        /// Handles size changes
        /// </summary>
        void ProcessSize(ElementEventArgs e);

        /// <summary>
        /// Handles scripting calls
        /// </summary>
        void ProcessScriptingMethodCall(ScriptingMethodCallEventArgs e);

        /// <summary>
        /// Handles scroll
        /// </summary>
        void ProcessScroll(ElementEventArgs e);
    }
}
