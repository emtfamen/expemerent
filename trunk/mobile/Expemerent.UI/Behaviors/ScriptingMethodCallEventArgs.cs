using System;
using Expemerent.UI.Dom;
using System.Diagnostics;
using Expemerent.UI.Native;

namespace Expemerent.UI.Behaviors
{
    /// <summary>
    /// Represents event args for the <see cref="SciterBehavior.ScriptingMethodCall"/> event
    /// </summary>
    public class ScriptingMethodCallEventArgs : ElementEventArgs
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ScriptingMethodCall"/> class
        /// </summary>
        internal ScriptingMethodCallEventArgs(Element element)
            : base(element)
        {
        }

        /// <summary>
        /// Gets method name
        /// </summary>
        public string MethodName { get; internal protected set; }

        /// <summary>
        /// Method arguments
        /// </summary>
        public object[] Arguments { get; internal protected set; }

        /// <summary>
        /// Gets or sets return value
        /// </summary>
        public object ReturnValue { get; set; }
    }
}