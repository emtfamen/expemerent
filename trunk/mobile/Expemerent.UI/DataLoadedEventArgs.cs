using System;
using System.Diagnostics;
using Expemerent.UI.Native;

namespace Expemerent.UI
{
    [Serializable]
    public class DataLoadedEventArgs : EventArgs
    {
        /// <summary>
        /// loaded data to return. if data exists in the cache then this field contain pointer to it
        /// </summary>
        private LazyValue<byte[]> _data;

        /// <summary>
        /// Requested uri
        /// </summary>
        private string _uri;

        /// <summary>
        /// Creates a new instance of the <see cref="LoadDataEventArgs"/> class
        /// </summary>
        internal DataLoadedEventArgs(String uri, LazyValueCallback<byte[]> dataEval)
        {
            _uri = uri;
            _data = new LazyValue<byte[]>(dataEval);
        }

        /// <summary>
        /// Gets request Uri
        /// </summary>
        public String Uri
        {
            [DebuggerStepThrough]
            get { return _uri; }
        }

        /// <summary>
        /// Gets or sets requested data
        /// </summary>
        public byte[] Data
        {
            get { return _data.Value; }
        }
    }
}
