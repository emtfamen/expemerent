using System;
using Expemerent.UI.Dom;
using Expemerent.UI.Native;

namespace Expemerent.UI.Behaviors
{
    /// <summary>
    /// Represents focus event parameters
    /// </summary>
    public class FocusEventArgs : InputEventArgs
    {       
        /// <summary>
        /// Creates a new instance of the <see cref="FocusEventArgs"/> class
        /// </summary>
        internal FocusEventArgs(Element element, Phase phase)
            : base(element, phase)
        {

        }

        /// <summary>
        /// TRUE if focus is being set by mouse click
        /// </summary>
        public bool IsMouseClick { get; internal set; }

        /// <summary>
        /// Cancels focus transfer when validation failed
        /// </summary>
        public bool Cancel { get; internal set; }

        /// <summary>
        /// True if a control lost its focus
        /// </summary>
        public bool IsLostFocus { get; internal set; }
    }
}