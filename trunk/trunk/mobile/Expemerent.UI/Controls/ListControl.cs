using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Expemerent.UI.Dom;
using Expemerent.UI.Native;
using CurrencyManager = System.Windows.Forms.CurrencyManager;
using ItemChangedEventArgs = System.Windows.Forms.ItemChangedEventArgs;

namespace Expemerent.UI.Controls
{
    /// <summary>
    /// Provides a common implementation of TextBox 
    /// </summary>
    public abstract class ListControl : BindableControl
    {
        #region IItemsContainer interface
        /// <summary>
        /// Base interface for containers of items
        /// </summary>
        public interface IItemsContainer : IEnumerable<Element>
        {
            /// <summary>
            /// Inserts element at specified position
            /// </summary>
            void InsertElement(Element element, int position);

            /// <summary>
            /// Determines the index of specified element in the container
            /// </summary>
            int IndexOf(Element element);

            /// <summary>
            /// Gets amount of items in the container
            /// </summary>
            int Count { get; }

            /// <summary>
            /// Returns element at the specified index 
            /// </summary>
            Element this[int index] { get; }
        }
        #endregion

        #region Defines possible list item types
        /// <summary>
        /// Type of the item to insert
        /// </summary>
        protected enum ListItemType
        {
            /// <summary>
            /// Ordinary item
            /// </summary>
            Item,

            /// <summary>
            /// Selected item
            /// </summary>
            Selected
        } 
        #endregion

        #region Private data
        /// <summary>
        /// DataSource instance 
        /// </summary>
        private object _dataSource;

        /// <summary>
        /// CurrencyManager
        /// </summary>
        protected CurrencyManager _dataManager;        
        #endregion        

        #region Public properties
        /// <summary>
        /// Gets or sets dataSource control
        /// </summary>
        public object DataSource
        {
            [DebuggerStepThrough]
            get { return _dataSource; }
            set
            {
                if (_dataSource != value)
                {
                    _dataSource = value;
                    UpdateDataSource();
                }
            }
        } 
        #endregion

        #region Protected properties
        /// <summary>
        /// CurrencyManager
        /// </summary>
        protected CurrencyManager DataManager
        {
            get { return _dataManager; }
            private set
            {
                if (!ReferenceEquals(_dataManager, value))
                {
                    if (_dataManager != null)
                        UnwireDataSource();

                    _dataManager = value;

                    if (_dataManager != null)
                        WireDataSource();
                }
            }
        }
        #endregion

        #region Private implementation
        /// <summary>
        /// Removes dataSource subscriptions
        /// </summary>
        protected virtual void UnwireDataSource()
        {
            ClearItems();

            Debug.Assert(DataManager != null, "Cannot unwire null data manager");

            DataManager.ItemChanged -= manager_ItemChanged;
            DataManager.ListChanged -= manager_ListChanged;
            DataManager.PositionChanged -= manager_PositionChanged;
            DataManager.MetaDataChanged -= manager_MetaDataChanged;
        }

        /// <summary>
        /// Subscribes to the dataSource events. This method called when all conditions are ready to bind data
        /// </summary>
        protected virtual void WireDataSource()
        {
            Debug.Assert(DataManager != null, "CurrencyManager already wired");

            DataManager.ItemChanged += manager_ItemChanged;
            DataManager.ListChanged += manager_ListChanged;
            DataManager.PositionChanged += manager_PositionChanged;
            DataManager.MetaDataChanged += manager_MetaDataChanged;

            ClearItems();
        }

        /// <summary>
        /// Handles DOM element changes
        /// </summary>
        protected internal override void UpdateDomElement()
        {
            base.UpdateDomElement();
            UpdateDataSource();
        }
        #endregion

        #region DataManger events handling
        /// <summary>
        /// Handles <see cref="CurrencyManager.MetaDataChanged"/> event
        /// </summary>
        private void manager_MetaDataChanged(object sender, EventArgs e)
        {
            UpdateDataSource();
        }

        /// <summary>
        /// Handles <see cref="CurrencyManager.PositionChanged"/> event
        /// </summary>
        private void manager_PositionChanged(object sender, EventArgs e)
        {
            ProcessPositionChanged();
        }

        /// <summary>
        /// Processes position changed event
        /// </summary>
        private void ProcessPositionChanged()
        {
            var elements = GetItemsContainer();
            for (int i = 0; i < elements.Count; ++i)
                UpdateItemType(i, elements[i]);

            Element.Update();
        }

        /// <summary>
        /// Handles <see cref="CurrencyManager.ListChanged"/> event
        /// </summary>
        private void manager_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    {
                        var container = GetItemsContainer();
                        var item = DataManager.List[e.NewIndex];
                        using (var childHandle = CreateItem(e.NewIndex, item))
                        {
                            container.InsertElement(childHandle.Element, e.NewIndex);
                        }
                        Element.Update();
                    }
                    break;
                case ListChangedType.ItemChanged:
                    {
                        var container = GetItemsContainer();
                        var child = container[e.NewIndex];
                        UpdateItem(e.NewIndex, DataManager.List[e.NewIndex], child);
                        Element.Update();
                    }
                    break;
                case ListChangedType.ItemDeleted:
                    {
                        var container = GetItemsContainer();
                        var child = container[e.NewIndex];
                        child.Delete();
                        
                        ProcessPositionChanged();
                        Element.Update();
                    }
                    break;
                case ListChangedType.ItemMoved:
                    {
                        var container = GetItemsContainer();
                        var item = DataManager.List[e.OldIndex];
                        var child = container[e.NewIndex];
                        UpdateItem(e.NewIndex, item, child);
                        Element.Update();
                    }
                    break;
                case ListChangedType.PropertyDescriptorAdded:
                case ListChangedType.PropertyDescriptorChanged:
                case ListChangedType.PropertyDescriptorDeleted:
                case ListChangedType.Reset:
                    UpdateDataSource();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Handles <see cref="CurrencyManager.ItemChanged"/> event
        /// </summary>
        private void manager_ItemChanged(object sender, ItemChangedEventArgs e)
        {
            ProcessItemChanged(e.Index);
            Element.Update();
        }

        /// <summary>
        /// Processes item changed event
        /// </summary>
        private void ProcessItemChanged(int index)
        {
            if (index >= 0)
            {
                var elements = GetItemsContainer();
                var item = DataManager.List[index];

                UpdateItem(index, item, elements[index]);

                // Selected value changed
                OnPropertyChanged(new PropertyChangedEventArgs(String.Empty));
            }
        } 
        #endregion

        #region Private implementation
        /// <summary>
        /// Updates list content with information from the datasource
        /// </summary>
        protected virtual void UpdateDataSource()
        {
            if (Element != null && BindingContext != null && DataSource != null)
            {
                DataManager = (CurrencyManager)BindingContext[DataSource];
                
                var container = GetItemsContainer();
                for (var i = 0; i < DataManager.List.Count; ++i)
                {
                    using (var item = CreateItem(i, DataManager.List[i]))
                        container.InsertElement(item.Element, i);
                }

                // Force selected item update (needed to refresh caption in the dropdown)
                ProcessItemChanged(DataManager.Position);
                Element.Update();
            }
            else
                DataManager = null;
        } 
        #endregion

        #region List item data extraction
        /// <summary>
        /// Returns item type for specified position
        /// </summary>
        protected virtual ListItemType GetItemType(int position)
        {
            Debug.Assert(DataManager != null, "DataManager should be initialized");
            return position == DataManager.Position ? ListItemType.Selected : ListItemType.Item;
        }
        #endregion

        #region Working with DOM items
        /// <summary>
        /// Updates element type
        /// </summary>
        protected virtual void UpdateItemType(int position, Element element)
        {
            var itemType = GetItemType(position);
            switch (itemType)
            {
                case ListItemType.Item:
                    element.SetState(ElementState.None, ElementState.Checked | ElementState.Current);
                    break;
                case ListItemType.Selected:
                    element.SetState(ElementState.Checked | ElementState.Current, ElementState.None);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Updates item text & type
        /// </summary>
        protected abstract void UpdateItem(int position, object item, Element element);

        /// <summary>
        /// Creates an <see cref="Element"/> to insert in the list
        /// </summary>
        protected abstract ElementRef CreateItem(int position, object item);
        
        /// <summary>
        /// Returns container where items should be created
        /// </summary>
        protected abstract IItemsContainer GetItemsContainer();

        /// <summary>
        /// Clears content of the list control
        /// </summary>
        private void ClearItems()
        {
            if (ElementRef != null)
            {
                // Scope will free deleted items as soon as possible
                using (var scope = ElementScope.Create())
                {
                    var container = GetItemsContainer();
                    var items = new List<Element>(container);
                    foreach (var item in items)
                        item.Delete();
                }
            }
        }
        #endregion
    }
}
