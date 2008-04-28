using System;
using System.Diagnostics;
using Expemerent.UI.Dom;

namespace Expemerent.UI.Behaviors
{
    /// <summary>
    /// Base class for all behavior events
    /// </summary>
    [Serializable]
    [DebuggerStepThrough]
    public class ElementEventArgs : EventArgs
    {
        /// <summary>
        /// See <see cref="Element"/> property
        /// </summary>
        private readonly Element _element;

        /// <summary>
        /// Creates a new instance of the <see cref="ElementEventArgs"/> class
        /// </summary>
        /// <param name="element"></param>
        internal ElementEventArgs(Element element)
        {
            _element = element;
        }

        /// <summary>
        /// Gets element processing this event
        /// </summary>
        public Element Element
        {
            get { return _element; }
        }

        /// <summary>
        /// Gets or sets handled flag
        /// </summary>
        public bool Handled { get; set; }       
    }
}