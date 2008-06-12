using System;
using System.Collections.Generic;
using System.Text;
using Expemerent.UI.Dom;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;

namespace Expemerent.UI.Controls
{
    /// <summary>
    /// Simple dataGrid without editing support. The first row of the table should contain bindings
    /// </summary>    
    public class DataGridControl : ListControl
    {
        #region Private data
        /// <summary>
        /// Name of the data field attribute
        /// </summary>
        private const string DataFieldAttribute = "datafield";

        /// <summary>
        /// Header size
        /// </summary>
        private int _headerSize;

        /// <summary>
        /// Property to column mapping. 
        /// </summary>
        private List<PropertyDescriptor> _properties;
        #endregion

        /// <summary>
        /// This method is called when element and dataSource should be disconnected
        /// </summary>
        protected override void UnwireDataSource()
        {
            base.UnwireDataSource();

            _properties = null;
            _headerSize = 0;
        }

        /// <summary>
        /// This method is called when element and dataSource are ready to be connected
        /// </summary>
        protected override void WireDataSource()
        {
            var element = Element;
            var props = DataManager.GetItemProperties();
            if (element.Children.Count == 0)
            {
                _headerSize = 0;
                _properties = new List<PropertyDescriptor>(props.Count);

                foreach (PropertyDescriptor prop in props)
                    _properties.Add(prop);
            }
            else
            {
                var template = element.Children[0];
                var cells = template.Children;

                _headerSize = element.Children.Count;
                _properties = new List<PropertyDescriptor>(cells.Count);
                foreach (var cell in cells)
                {
                    var propName = cell.Attributes[DataFieldAttribute];
                    if (!String.IsNullOrEmpty(propName))
                    {
                        var prop = props[propName];
                        _properties.Add(prop);
                    }
                }
            }

            base.WireDataSource();
        }
        
        /// <summary>
        /// Updates item text & type
        /// </summary>
        protected override void UpdateItem(int position, object item, Element element)
        {
            var container = GetItemsContainer();
            var row = container[position];

            for (int i = 0; i < _properties.Count; ++i)
            {
                var prop = _properties[i];
                var cellText = prop != null ? ConvertUtility.GetValue(prop, item) : String.Empty;

                row.Children[i].Text = cellText;
            }

            UpdateItemType(position, element);
        }

        /// <summary>
        /// Creates an <see cref="Element"/> to insert in the list
        /// </summary>
        protected override ElementRef CreateItem(int position, object item)
        {
            var rowRef = Element.CreateElement("tr", String.Empty);
            var row = rowRef.Element;

            for (int i = 0; i < _properties.Count; ++i)
            {
                var prop = _properties[i];
                var cellText = prop != null ? ConvertUtility.GetValue(prop, item) : String.Empty;
                
                using (var cell = Element.CreateElement("td", cellText))
                    row.InsertElement(cell.Element, i);
            }

            UpdateItemType(position, row);
            return rowRef;
        }

        /// <summary>
        /// Returns container where items should be created
        /// </summary>
        protected override IItemsContainer GetItemsContainer()
        {
            return new ElementContainer(Element, _headerSize);
        }

        #region IItemsContainer implementation
        /// <summary>
        /// The <see cref="IItemsContainer"/> for the select <see cref="Element"/> 
        /// </summary>
        protected class ElementContainer : IItemsContainer
        {
            /// <summary>
            /// Hardcoded header row size 
            /// </summary>
            protected int HeaderSize { get; private set; }

            /// <summary>
            /// Container instance
            /// </summary>
            protected Element Element { get; private set; }

            /// <summary>
            /// Creates a new instance of the <see cref="ElementContainer"/> class
            /// </summary>
            public ElementContainer(Element element, int headerSize)
            {
                #region Arguments checking
                if (element == null)
                    throw new ArgumentNullException("element");
                if (headerSize < 0)
                    throw new ArgumentNullException("headerSize");
                #endregion

                Element = element;
                HeaderSize = headerSize;
            }

            /// <summary>
            /// Inserts element at specified position
            /// </summary>
            public void InsertElement(Element element, int position)
            {
                Element.InsertElement(element, position + HeaderSize);
            }

            /// <summary>
            /// Determines the index of specified element in the container
            /// </summary>
            public int IndexOf(Element element)
            {
                var index = Element.Children.IndexOf(element);
                if (index > 0)
                    return index - HeaderSize;

                return -1;
            }

            /// <summary>
            /// Gets amount of items in the container
            /// </summary>
            public int Count
            {
                [DebuggerStepThrough]
                get
                {
                    var count = Element.Children.Count - HeaderSize;
                    return count < 0 ? 0 : count;
                }
            }

            /// <summary>
            /// Returns element at the specified index 
            /// </summary>
            public Element this[int index]
            {
                [DebuggerStepThrough]
                get { return Element.Children[index + HeaderSize]; }
            }

            /// <summary>
            /// Gets items enumerator
            /// </summary>
            public IEnumerator<Element> GetEnumerator()
            {
                var counter = HeaderSize;
                foreach (var item in Element.Children)
                {
                    if (counter > 0)
                        counter--;
                    else
                        yield return item;
                }
            }

            /// <summary>
            /// Gets items enumerator
            /// </summary>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
        #endregion
    }
}
