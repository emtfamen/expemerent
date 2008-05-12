using System;
using System.Text;
using Expemerent.UI.Dom;
using Expemerent.UI.Native;

namespace Expemerent.UI.Behaviors
{
    [Flags]
    public enum KeyboardState
    {
        /// <summary>
        /// Empty state
        /// </summary>
        None = 0,

        /// <summary>
        /// The CONTROL modifier key.
        /// </summary>
        Control = KEYBOARD_STATES.CONTROL_KEY_PRESSED,

        /// <summary>
        /// The SHIFT modifier key.
        /// </summary>
        Shift = KEYBOARD_STATES.SHIFT_KEY_PRESSED,

        /// <summary>
        /// The ALT modifier key.
        /// </summary>
        Alt = KEYBOARD_STATES.ALT_KEY_PRESSED 
    }

    /// <summary>
    /// Base class for all events related to the user input
    /// </summary>
    public class UserInputEventArgs : InputEventArgs
    {
        /// <summary>
        /// Creates a new instance of the <see cref="UserInputEventArgs"/> class
        /// </summary>
        public UserInputEventArgs(Element element, Phase phase)
            :base(element, phase)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the Control key was pressed.
        /// </summary>
        public KeyboardState KeyboardState { get; protected internal set; }
    }
}
