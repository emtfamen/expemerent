using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Expemerent.UI.Behaviors;
using Expemerent.UI.Dom;

namespace Expemerent.UI.Controls
{
    public class ListBoxControl : ListControl
    {
        /// <summary>
        /// Defines key for <see cref="Format"/> event delegate
        /// </summary>
        private readonly static object FormatEvent = new object();

        /// <summary>
        /// Provides access to the DisplayMember property
        /// </summary>
        private PropertyDescriptor _displayMemberProperty;

        /// <summary>
        /// Provides access to the ValueMember property
        /// </summary>
        private PropertyDescriptor _valueMemberProperty;

        /// <summary>
        /// See <see cref="DisplayMember"/> property
        /// </summary>
        private string _displayMember;

        /// <summary>
        /// See <see cref="ValueMember"/> property
        /// </summary>
        private string _valueMember;

        /// <summary>
        /// Gets or sets index of the selected value
        /// </summary>
        public int SelectedIndex
        {
            [DebuggerStepThrough]
            get { return DataManager != null ? DataManager.Position : 0; }
            set
            {
                if (DataManager != null)
                    DataManager.Position = value;
            }
        }

        /// <summary>
        /// Gets or sets selected value in list
        /// </summary>
        public object SelectedValue
        {
            [DebuggerStepThrough]
            get { return DataManager != null ? GetItemValue(DataManager.Current) : null; }
            set
            {
                if (DataManager != null)
                {
                    for (int i = 0; i < DataManager.List.Count; i++)
                    {
                        var item = DataManager.List[i];
                        if (GetItemValue(item) == value)
                        {
                            DataManager.Position = i;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the property to use as the actual value for the items in the Control
        /// </summary>
        public string ValueMember
        {
            [DebuggerStepThrough]
            get { return _valueMember; }
            set
            {
                _valueMember = value;
                UpdateDataSource();
            }
        }

        /// <summary>
        /// Gets or sets the property to display for this Control
        /// </summary>
        public string DisplayMember
        {
            [DebuggerStepThrough]
            get { return _displayMember; }
            set
            {
                _displayMember = value;
                UpdateDataSource();
            }
        }

        /// <summary>
        /// Occurs when list item should be formatted
        /// </summary>
        public event ConvertEventHandler Format
        {
            add { Events.AddHandler(FormatEvent, value); }
            remove { Events.RemoveHandler(FormatEvent, value); }
        } 

        /// <summary>
        /// Invokes list item formatting
        /// </summary>
        protected String PerformFormat(object item)
        {
            if (HasEvents)
            {
                ConvertEventHandler handler = (ConvertEventHandler)Events[FormatEvent];
                if (handler != null)
                {
                    var e = new ConvertEventArgs(item, typeof(String));
                    handler(this, e);
                    return e.Value != null ? e.Value.ToString() : null;
                }
            }

            return null;
        }

        /// <summary>
        /// Converts item to value
        /// </summary>
        protected virtual Object GetItemValue(object item)
        {
            if (_valueMemberProperty != null)
                return _valueMemberProperty.GetValue(item);

            return item;
        }

        /// <summary>
        /// Converts item to display string
        /// </summary>
        protected virtual String GetItemText(object item)
        {
            var formattedText = PerformFormat(item);
            if (formattedText == null)
            {
                if (_displayMemberProperty != null)
                    formattedText = ConvertUtility.GetValue(_displayMemberProperty, item);
                else
                    formattedText = item != null ? item.ToString() : String.Empty;
            }

            return formattedText;
        }

        /// <summary>
        /// Returns element where items of the collection should be created
        /// </summary>
        protected override Element GetItemsContainer()
        {
            var selectType = Element.Attributes["type"];
            switch (selectType)
            {
                case "select-dropdown":
                    return Element.Find(":root > popup[type='select']");
                case "select":
                    return Element;
                default:
                    Debug.WriteLine(String.Format("Unexpected select type: {0}", selectType));
                    return Element;
            }
        }

        /// <summary>
        /// Updates element type
        /// </summary>
        protected override void UpdateItemType(int position, Element element)
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
        protected override void UpdateItem(int position, object item, Element element)
        {
            element.Text = GetItemText(item);
            UpdateItemType(position, element);
        }

        /// <summary>
        /// Creates an Element to insert in the list
        /// </summary>
        protected override ElementRef CreateItem(int position, object item, CreateElementHandler createElement)
        {
            var itemType = GetItemType(position);
            var elementHandle = createElement("option", GetItemText(item));
            UpdateItemType(position, elementHandle.Element);

            return elementHandle;
        }

        /// <summary>
        /// Removes dataSource subscriptions
        /// </summary>
        protected override void UnwireDataSource()
        {
            _displayMemberProperty = null;
            _valueMemberProperty = null;

            base.UnwireDataSource();
        }

        /// <summary>
        /// Removes dataSource subscriptions
        /// </summary>
        protected override void WireDataSource()
        {
            base.WireDataSource();

            if (DataManager != null)
            {
                var props = DataManager.GetItemProperties();

                _displayMemberProperty = props[DisplayMember ?? String.Empty];
                _valueMemberProperty = props[ValueMember ?? String.Empty];
            }
        }

        #region Handling behavior events
        /// <summary>
        /// Handles <see cref="SciterBehavior.BehaviorEvent"/>
        /// </summary>
        protected override void OnBehaviorEvent(BehaviorEventArgs e)
        {
            base.OnBehaviorEvent(e);

            if (e.Phase == Phase.Bubbling && e.BehaviorEvent == BehaviorEventType.SelectSelectionChanged)
            {
                var container = GetItemsContainer();
                var index = container.Children.IndexOf(e.Source);

                DataManager.Position = index;
            }
        }
        #endregion   
    }
}
