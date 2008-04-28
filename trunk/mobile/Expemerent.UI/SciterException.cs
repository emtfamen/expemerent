using System;
using System.Runtime.Serialization;

namespace Expemerent.UI
{
    /// <summary>
    /// Exception text for all sciter operations
    /// </summary>
    [Serializable]
    public class SciterException : ApplicationException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SciterException"/> class
        /// </summary>
        public SciterException(string message)
            : base(message)
        {
        }
    }
}
