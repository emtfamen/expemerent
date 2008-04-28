using System;
using System.Diagnostics;
using Expemerent.UI.Native;

namespace Expemerent.UI.Dom
{
    /// <summary>
    /// Represents a style of the HTML element
    /// </summary>
    public sealed class Style
    {
        /// <summary>
        /// Gets or set HTML element
        /// </summary>
        private readonly Element _element;

        /// <summary>
        /// Creates a new instance of the <see cref="Style"/> class
        /// </summary>        
        internal Style(Element element)
        {
            _element = element;
        }

        /// <summary>
        /// Gets instance of sciter dom api
        /// </summary>
        private static SciterDomApi SciterDomApi
        {
            [DebuggerStepThrough]
            get { return SciterHostApi.SciterDomApi; }
        }

        /// <summary>
        /// Gets or sets style attribute
        /// </summary>
        public string this[string name]
        {
            [DebuggerStepThrough]
            get { return SciterDomApi.GetStyleAttribute(_element, name); }
            [DebuggerStepThrough]
            set { SciterDomApi.SetStyleAttribute(_element, name, value); }
        }
    }
}