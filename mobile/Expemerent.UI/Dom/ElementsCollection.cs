using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Expemerent.UI.Native;

namespace Expemerent.UI.Dom
{
    /// <summary>
    /// Represents a collection of HTML elements
    /// </summary>
    public abstract class ElementsCollection : IEnumerable<Element>
    {
        /// <summary>
        /// Gets the number of elements contained in the collection 
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        public abstract Element this[int index] { get; }

        /// <summary>
        /// Gets element index in the collection
        /// </summary>
        public abstract int IndexOf(Element element);

        #region IEnumerable<Element> Members

        /// <summary>
        /// Gets elements enumerator
        /// </summary>
        public abstract IEnumerator<Element> GetEnumerator();

        /// <summary>
        /// Gets elements enumerator
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

    /// <summary>
    /// Represents a collection of arbitrary HTML elements
    /// </summary>
    internal sealed class AllElementsCollection : ElementsCollection
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ElementsCollection"/> class
        /// </summary>
        internal AllElementsCollection(List<Element> source)
        {
            Elements = source.AsReadOnly();
        }

        /// <summary>
        /// Gets the number of elements contained in the collection 
        /// </summary>
        public override int Count
        {
            [DebuggerStepThrough]
            get { return Elements.Count; }
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        public override Element this[int index]
        {
            [DebuggerStepThrough]
            get { return Elements[index]; }
        }

        /// <summary>
        /// Gets or sets elements collection
        /// </summary>
        private ReadOnlyCollection<Element> Elements { get; set; }

        /// <summary>
        /// Gets elements enumerator
        /// </summary>
        public override IEnumerator<Element> GetEnumerator()
        {
            return Elements.GetEnumerator();
        }

        /// <summary>
        /// Gets element index in the collection
        /// </summary>
        public override int IndexOf(Element element)
        {
            return Elements.IndexOf(element);
        }
    }

    /// <summary>
    /// Represents a collection of children element for HTML element
    /// </summary>
    internal sealed class ChildrenCollection : ElementsCollection
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ChildrenCollection"/> class
        /// </summary>
        public ChildrenCollection(Element parent)
        {
            Parent = parent;
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
        /// Gets or sets parent element
        /// </summary>
        public Element Parent { get; private set; }

        /// <summary>
        /// Gets the number of elements contained in the collection 
        /// </summary>        
        public override int Count
        {
            [DebuggerStepThrough]
            get { return SciterDomApi.GetChildrenCount(Parent); }
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        public override Element this[int index]
        {
            [DebuggerStepThrough]
            get { return SciterDomApi.GetNthChild(Parent, index); }
        }

        /// <summary>
        /// Gets elements enumerator
        /// </summary>
        public override IEnumerator<Element> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        /// <summary>
        /// Gets element index in the collection
        /// </summary>
        public override int IndexOf(Element element)
        {
            return SciterDomApi.GetElementIndex(element);
        }
    }
}