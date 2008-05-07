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

        #region Element factory delegate
		/// <summary>
        /// Defines a factory method to create new elements
        /// </summary>
        protected delegate ElementRef CreateElementHandler(string tag, string text); 
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
            Debug.Assert(DataManager != null, "Cannot unwire null empty data manager");

            DataManager.ItemChanged -= manager_ItemChanged;
            DataManager.ListChanged -= manager_ListChanged;
            DataManager.PositionChanged -= manager_PositionChanged;
            DataManager.MetaDataChanged -= manager_MetaDataChanged;
        }

        /// <summary>
        /// Removes dataSource subscriptions
        /// </summary>
        protected virtual void WireDataSource()
        {
            Debug.Assert(DataManager != null, "CurrencyManager already wired");

            DataManager.ItemChanged += manager_ItemChanged;
            DataManager.ListChanged += manager_ListChanged;
            DataManager.PositionChanged += manager_PositionChanged;
            DataManager.MetaDataChanged += manager_MetaDataChanged;
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
            var elements = GetItemsContainer().Children;
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
                        using (var childHandle = CreateItem(e.NewIndex, item, Element.CreateElement))
                        {
                            container.InsertElement(childHandle.Element, e.NewIndex);
                        }
                        Element.Update();
                    }
                    break;
                case ListChangedType.ItemChanged:
                    {
                        var container = GetItemsContainer();
                        var child = container.Children[e.NewIndex];
                        UpdateItem(e.NewIndex, DataManager.List[e.NewIndex], child);
                        Element.Update();
                    }
                    break;
                case ListChangedType.ItemDeleted:
                    {
                        var container = GetItemsContainer();
                        var child = container.Children[e.NewIndex];
                        child.Delete();
                        
                        ProcessPositionChanged();
                        Element.Update();
                    }
                    break;
                case ListChangedType.ItemMoved:
                    {
                        var container = GetItemsContainer();
                        var item = DataManager.List[e.OldIndex];
                        var child = container.Children[e.NewIndex];
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

                UpdateItem(index, item, elements.Children[index]);

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
                
                ClearItems();
                var container = GetItemsContainer();

                for (var i = 0; i < DataManager.List.Count; ++i)
                {
                    using (var item = CreateItem(i, DataManager.List[i], Element.CreateElement))
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
        protected abstract void UpdateItemType(int position, Element element);

        /// <summary>
        /// Updates item text & type
        /// </summary>
        protected abstract void UpdateItem(int position, object item, Element element);

        /// <summary>
        /// Creates an Element to insert in the list
        /// </summary>
        protected abstract ElementRef CreateItem(int position, object item, CreateElementHandler itemFactory);
        
        /// <summary>
        /// Returns element where items of the collection should be created
        /// </summary>
        protected abstract Element GetItemsContainer();

        /// <summary>
        /// Clears content of the list control
        /// </summary>
        private void ClearItems()
        {
            // Scope will free deleted items as soon as possible
            using (var scope = ElementScope.Create())
            {
                var container = GetItemsContainer();
                var items = new List<Element>(container.Children);
                foreach (var item in items)
                    item.Delete();
            }
        }
        #endregion
    }
}
