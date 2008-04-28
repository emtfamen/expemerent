using System;

namespace Expemerent.UI
{
    /// <summary>
    /// This notification is sent when all external data (for example image) has been downloaded.
    /// </summary>
    [Serializable]
    public class DocumentCompleteEventArgs : EventArgs
    {
        /// <summary>
        /// Default instance
        /// </summary>
        public new static readonly DocumentCompleteEventArgs Empty = new DocumentCompleteEventArgs();
    }
}
