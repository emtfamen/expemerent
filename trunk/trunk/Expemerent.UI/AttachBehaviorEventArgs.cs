using System;
using System.Diagnostics;
using Expemerent.UI.Behaviors;
using Expemerent.UI.Dom;
using Expemerent.UI.Native;

namespace Expemerent.UI
{
    [Serializable]
    public class AttachBehaviorEventArgs : EventArgs
    {
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
    }
}