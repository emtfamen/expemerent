using System;
using System.Diagnostics;

namespace Expemerent.UI
{
    /// <summary>
    /// Notifies that HtmLayout is about to download a referred resource. 
    /// </summary>
    [Serializable]
    public class LoadDataEventArgs : EventArgs
    {
        /// <summary>
        /// loaded data to return. if data exists in the cache then this field contain pointer to it
        /// </summary>
        private LazyValue<byte[]> _data;

        /// <summary>
        /// Requested uri
        /// </summary>
        private String _uri;

        /// <summary>
        /// Creates a new instance of the <see cref="LoadDataEventArgs"/> class
        /// </summary>
        internal LoadDataEventArgs(String uri, LazyValueCallback<byte[]> dataEval)
        {
            _uri = uri;
            _data = new LazyValue<byte[]>(dataEval);
        }

        /// <summary>
        /// data already exists in the cache
        /// </summary>
        public bool IsCached { get; protected internal set; }

        /// <summary>
        /// Gets request Uri
        /// </summary>
        public String Uri
        {
            [DebuggerStepThrough]
            get { return _uri; }
        }

        /// <summary>
        /// Gets resource type 
        /// </summary>
        public ResourceType ResourceType { get; internal set; }

        /// <summary>
        /// Gets request id for async data loading
        /// </summary>
        public IntPtr RequestId { get; internal set; }

        /// <summary>
        /// Data buffer has been changed
        /// </summary>
        internal bool IsDataAvailable { get; private set; }

        /// <summary>
        /// Gets or sets requested data
        /// </summary>
        public byte[] GetData()
        {
            return _data.Value;
        }

        /// <summary>
        /// Sets requested data
        /// </summary>
        public void SetData(byte[] buffer)
        {
            IsDataAvailable = true;
            _data.Value = buffer;
        }
    }
}