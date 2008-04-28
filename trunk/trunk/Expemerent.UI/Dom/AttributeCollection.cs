using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Expemerent.UI.Native;


namespace Expemerent.UI.Dom
{
    /// <summary>
    /// Collection of element attribues
    /// </summary>
    public sealed class AttributeCollection : IEnumerable<KeyValuePair<string, string>>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="AttributeCollection"/> class
        /// </summary>
        /// <param name="element"></param>
        internal AttributeCollection(Element element)
        {
            Element = element;
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
        /// Gets a number of element's attribues
        /// </summary>
        public int Count
        {
            [DebuggerStepThrough]
            get { return SciterDomApi.GetAttributeCount(Element); }
        }

        /// <summary>
        /// Gets or sets attribute value by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string this[string name]
        {
            get { return SciterDomApi.GetAttributeByName(Element, name); }
            set { SciterDomApi.SetAttributeByName(Element, name, value); }
        }

        /// <summary>
        /// Gets element 
        /// </summary>
        private Element Element { get; set; }

        #region IEnumerable<KeyValuePair<string,string>> Members

        /// <summary>
        /// Gets attribues enumerator
        /// </summary>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return SciterDomApi.GetAttribute(Element, i);
            }
        }

        /// <summary>
        /// Gets attribues enumerator
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Clears attributes collection
        /// </summary>
        public void Clear()
        {
            SciterDomApi.ClearAttributes(Element);
        }
    }
}