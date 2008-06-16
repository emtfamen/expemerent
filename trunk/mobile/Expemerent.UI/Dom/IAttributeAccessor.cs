using System;
using System.Collections.Generic;
using System.Text;

namespace Expemerent.UI.Dom
{
    /// <summary>
    /// Provides access to the attributes collection of the DOM element
    /// </summary>
    public interface IAttributeAccessor 
    {
        /// <summary>
        /// Returns attributes enumerator
        /// </summary>
        IEnumerator<KeyValuePair<string, string>> GetEnumerator();

        /// <summary>
        /// Gets or sets element attribute
        /// </summary>
        string this[string name] { get; set; }

        /// <summary>
        /// Gets a number of element's attribues
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Clears attributes collection
        /// </summary>
        void Clear();
    }
}
