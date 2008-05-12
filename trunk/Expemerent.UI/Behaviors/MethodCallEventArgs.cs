using System;
using Expemerent.UI.Dom;
using Expemerent.UI.Native;

namespace Expemerent.UI.Behaviors
{
    /// <summary>
    /// Represents a method call event
    /// </summary>
    public class MethodCallEventArgs : ElementEventArgs
    {
        /// <summary>
        /// Creates a new instance of the <see cref="MethodCallEventArgs"/> class
        /// </summary>
        internal MethodCallEventArgs(Element element) : base(element)
        {
            
        }

        /// <summary>
        /// Gets or sets method id
        /// </summary>
        public BehaviorMethods MethodId { get; protected internal set; }
    }
}