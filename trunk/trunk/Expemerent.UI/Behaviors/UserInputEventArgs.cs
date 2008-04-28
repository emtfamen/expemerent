using System;
using System.Text;
using Expemerent.UI.Dom;

namespace Expemerent.UI.Behaviors
{
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
        public bool Control { get; protected internal set; }
        
        /// <summary>
        /// Gets a value indicating whether the Shift key was pressed.
        /// </summary>
        public bool Shift { get; protected internal set; }

        /// <summary>
        /// Gets a value indicating whether the Alt key was pressed.
        /// </summary>
        public bool Alt { get; protected internal set; }
    }
}
