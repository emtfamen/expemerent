using System;
using Expemerent.UI.Dom;
using Expemerent.UI.Native;

namespace Expemerent.UI.Behaviors
{
    /// <summary>
    /// Represents arguments for the <see cref="SciterBehavior.DataArrived"/> event
    /// </summary>
    public class DataArrivedEventArgs : ElementEventArgs
    {
        /// <summary>
        /// element intiator of HTMLayoutRequestElementData request, 
        /// </summary>
        public Element Source { get; internal protected set; }

        public IntPtr data; // data buffer
        public uint dataSize; // size of data
        public uint dataType; // data text passed "as is" from HTMLayoutRequestElementData


        /// <summary>
        /// Creates a new instance of the <see cref="DataArrivedEventArgs"/> class
        /// </summary>
        internal DataArrivedEventArgs(Element element) : base(element)
        {
            
        }
    }
}