using System;
using Expemerent.UI.Dom;
using Expemerent.UI.Native;
using System.Windows.Forms;
using System.Text;

namespace Expemerent.UI.Behaviors
{
    /// <summary>
    /// Defines event types
    /// </summary>
    public enum KeyEventType 
    {
        KeyDown = KEY_PARAMS.KEY_EVENTS.KEY_DOWN,
        KeyUp = KEY_PARAMS.KEY_EVENTS.KEY_UP,
        KeyChar = KEY_PARAMS.KEY_EVENTS.KEY_CHAR
    } 

    /// <summary>
    /// Represents keyboard events
    /// </summary>
    public class KeyEventArgs : UserInputEventArgs
    {
        /// <summary>
        /// Creates a new instance of the <see cref="KeyEventArgs"/> class
        /// </summary>
        internal KeyEventArgs(Element element, Phase phase)
            : base(element, phase)
        {
        }

        /// <summary>
        /// Gets or sets type of the event
        /// </summary>
        public KeyEventType KeyEventType { get; protected internal set; }

        /// <summary>
        /// Gets the keyboard code
        /// </summary>
        public int KeyValue { get; protected internal set; }

        /// <summary>
        /// Gets the keyboard code
        /// </summary>
        public Keys KeyCode { get { return (Keys)KeyValue & Keys.KeyCode; } }

        /// <summary>
        /// Gets the character for the KeyPress event
        /// </summary>
        public char KeyChar 
        { 
            get 
            {
                return Encoding.Default.GetChars(BitConverter.GetBytes(KeyValue))[0];
            } 
        }
    }
}
