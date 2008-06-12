using System;
using Expemerent.UI.Native;

namespace Expemerent.UI
{
    public class CallbackHostEventArgs : EventArgs
    {
        /// <summary>
        /// Event args destination
        /// </summary>
        public enum HostChannelType
        {
            /// <summary>
            /// Standart input 
            /// </summary>
            StdIn,

            /// <summary>
            /// Standart output
            /// </summary>
            StdOut,

            /// <summary>
            /// Standart errors
            /// </summary>
            StdErr
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CallbackHostEventArgs"/> class
        /// </summary>
        public CallbackHostEventArgs(HostChannelType channelType, object first, object second)
        {
            ChannelType = channelType;
            First = first;
            Second = second;
        }

        /// <summary>
        /// Gets ChannelType
        /// </summary>
        public HostChannelType ChannelType { get; private set; }

        /// <summary>
        /// Gets first parameter
        /// </summary>
        public object First { get; private set; }

        /// <summary>
        /// Gets second parameter
        /// </summary>
        public object Second { get; private set; }

        /// <summary>
        /// Gets or sets return value of the event
        /// </summary>
        public object ReturnValue { get; set; }
    }
}
