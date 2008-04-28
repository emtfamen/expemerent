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
        #region Serialization support
		/// <summary>
        /// Serialization support
        /// </summary>
        protected SciterException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Serialization support
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        } 
	    #endregion    

        /// <summary>
        /// Creates a new instance of the <see cref="SciterException"/> class
        /// </summary>
        public SciterException(string message)
            : base(message)
        {
        }
    }
}
