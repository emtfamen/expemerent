using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Expemerent.UI.Dom;

namespace Expemerent.UI.Controls
{
    /// <summary>
    /// Collection of sciter controls
    /// </summary>
    public sealed class ControlsCollection : Collection<BindableControl>
    {
        /// <summary>
        /// Collection owner
        /// </summary>
        private readonly ISciterControl _owner;

        /// <summary>
        /// Creates a new instance of the <see cref="ControlsCollection"/> class
        /// </summary>
        /// <param name="owner"></param>
        internal ControlsCollection(ISciterControl owner)
        {
            Debug.Assert(owner != null, "Owner cannot be null");
            _owner = owner;
        }

        /// <summary>
        /// Handles item insert
        /// </summary>
        protected override void InsertItem(int index, BindableControl item)
        {
            #region Arguments checking
            if (item == null)
                throw new ArgumentException("item");
            if (Object.ReferenceEquals(item.Parent, _owner))
                throw new ArgumentException("Control cannot be added to the collection more than one time", "item");            
            #endregion

            item.SetParent(_owner);

            base.InsertItem(index, item);
        }

        /// <summary>
        /// Handles item change
        /// </summary>
        protected override void SetItem(int index, BindableControl item)
        {
            #region Arguments checking
            if (item == null)
                throw new ArgumentException("item");
            if (Object.ReferenceEquals(item.Parent, _owner))
                throw new ArgumentException("Control cannot be added to the collection more than one time", "item");
            #endregion

            this[index].SetParent(null);
            
            item.SetParent(_owner);
            base.SetItem(index, item);
        }

        /// <summary>
        /// Handles item remove
        /// </summary>
        protected override void RemoveItem(int index)
        {
            this[index].SetParent(null);

            base.RemoveItem(index);
        }

        /// <summary>
        /// Frees element 
        /// </summary>
        internal void FreeElements()        
        {
            foreach (var item in Items)
                item.SetParent(null);

            Clear();
        }

        /// <summary>
        /// Updates SciterControls bindings
        /// </summary>
        internal void UpdateElements()
        {
            foreach (var item in this.Items)
                item.UpdateDomElement();
        }

        /// <summary>
        /// Finds a control associated with the specified Element
        /// </summary>
        internal BindableControl FindControl(Element element)
        {
            foreach (var item in Items)
                if (item.ElementHandle == element.Handle)
                    return item;

            return null;
        }
    }
}
