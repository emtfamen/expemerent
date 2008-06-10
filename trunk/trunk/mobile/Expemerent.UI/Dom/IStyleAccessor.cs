using System;
using System.Collections.Generic;
using System.Text;

namespace Expemerent.UI.Dom
{
    /// <summary>
    /// Provides access to the styles collection of the DOM element
    /// </summary>
    public interface IStyleAccessor
    {
        /// <summary>
        /// Gets or sets style attribute
        /// </summary>
        string this[string name] { get; set; }
    }
}
