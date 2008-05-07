using System;
using System.ComponentModel;
using System.Windows.Forms;
using Expemerent.UI.Behaviors;
using System.Diagnostics;
using Expemerent.UI.Dom;
using System.Collections.Generic;
using System.Xml;

namespace Expemerent.UI.Controls
{    
    /// <summary>
    /// Base implementation of control with databinding support
    /// </summary>
    public class BindableControl : SciterBehavior, IBindableComponent, INotifyPropertyChanged, IAttributeAccessor, IStyleAccessor, IDisposable
    {
        #region Event keys
        /// <summary>
        /// Event key for the <see cref="ParentChanged"/> event
        /// </summary>
        private readonly static object ParentChangedEvent = new object();

        /// <summary>
        /// Event key for the <see cref="PropertyChanged"/> event
        /// </summary>
        private readonly static object PropertyChangedEvent = new object();

        /// <summary>
        /// Event key for the <see cref="Validated"/> event
        /// </summary>
        private readonly static object ValidatedEvent = new object();

        /// <summary>
        /// Event key for the <see cref="Validating"/> event
        /// </summary>
        private readonly static object ValidatingEvent = new object();

        /// <summary>
        /// Event key for the <see cref="Validating"/> event
        /// </summary>
        private readonly static object BindingContextChangedEvent = new object();

        /// <summary>
        /// Event key for the <see cref="Disposing"/> event
        /// </summary>
        private readonly static object DisposedEvent = new object();
        #endregion

        #region Class data
        /// <summary>
        /// See <see cref="BindingContext"/> property
        /// </summary>
        private BindingContext _bindingContext;

        /// <summary>
        /// See <see cref="DataBindings"/> property
        /// </summary>
        private ControlBindingsCollection _bindings;

        /// <summary>
        /// Reference to the bound element
        /// </summary>
        private ElementRef _elementRef;

        /// <summary>
        /// CSS selector for the element
        /// </summary>
        private String _selector;

        /// <summary>
        /// Parent control of the component
        /// </summary>
        private ISciterControl _parent;

        /// <summary>
        /// Bitmask of element states to set
        /// </summary>
        private ElementState _stateToSet;

        /// <summary>
        /// Bitmask of element states to clear
        /// </summary>
        private ElementState _stateToClear;

        /// <summary>
        /// "Offline" collection of element attributes
        /// </summary>
        private Dictionary<string, string> _controlAttributes;

        /// <summary>
        /// "Offline" collection of element styles
        /// </summary>
        private Dictionary<string, string> _controlStyle;

        /// <summary>
        /// Attribues collection
        /// </summary>
        private Dictionary<string, string> ControlAttributes { get { return _controlAttributes ?? (_controlAttributes = new Dictionary<string, string>()); } }

        /// <summary>
        /// Styles collection
        /// </summary>
        private Dictionary<string, string> ControlStyle { get { return _controlStyle ?? (_controlStyle = new Dictionary<string, string>()); } }

        #endregion

        #region Construction
        /// <summary>
        /// Creates a new instance of the <see cref="BindableControl"/> class
        /// </summary>
        public BindableControl()
        {
            CausesValidation = true;
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets accessor to the attributes collection
        /// </summary>
        public IAttributeAccessor Attributes { get { return this; } }

        /// <summary>
        /// Gets accessor to the styles collection
        /// </summary>
        public IStyleAccessor Style { get { return this; } }

        /// <summary>
        /// Gets or sets a value indicating whether the control causes validation to be performed on any controls that require validation when it receives focus.
        /// </summary>
        public bool CausesValidation { get; set; } 

        /// <summary>
        /// Gets or sets enabled state
        /// </summary>
        public bool IsEnabled
        {
            get { return (GetElementState() & ElementState.Disabled) != ElementState.Disabled; }
            set
            {
                var stateToSet = value ? 0 : ElementState.Disabled;
                var stateToClear = value ? ElementState.Disabled : 0;

                SetElementState(stateToSet, stateToClear);
            }
        }

        /// <summary>
        /// Gets or sets Visible attribute
        /// </summary>
        public bool IsVisible
        {
            get { return String.Compare(GetStyle("visibility"), "visible", StringComparison.InvariantCultureIgnoreCase) == 0; }
            set { SetStyle("visibility", value ? "visible" : "hidden"); }
        }
        
        /// <summary>
        /// Gets parent windows.forms control
        /// </summary>
        private Control ParentControl
        {
            [DebuggerStepThrough]
            get { return Parent != null ? Parent.Control : null; } 
        }

        /// <summary>
        /// Strong reference to the element handle
        /// </summary>
        private ElementRef ElementRef
        {
            [DebuggerStepThrough]
            get { return _elementRef; }
            set
            {
                if (_elementRef != null)
                    _elementRef.Dispose();

                _elementRef = value;                
            }
        }

        /// <summary>
        /// Gets HELEMENT handle
        /// </summary>
        internal IntPtr ElementHandle 
        { 
            [DebuggerStepThrough]
            get { return ElementRef != null ? ElementRef.ElementHandle : IntPtr.Zero; } 
        }

        /// <summary>
        /// Gets DOM element associated with the control
        /// </summary>
        public Element Element
        {
            [DebuggerStepThrough]
            get { return ElementRef != null ? ElementRef.Element : null; }
            private set
            {
                ReleaseDomElement();
                if (value != null)
                    SetDomElement(value);
            }
        }

        /// <summary>
        /// Gets or sets css selector used to select dom element
        /// </summary>
        public String Selector
        {
            [DebuggerStepThrough]
            get { return _selector; }
            set
            {
                _selector = value;
                UpdateDomElement();
            }
        }

        /// <summary>
        /// Gets or sets parent control
        /// </summary>
        public ISciterControl Parent
        {
            get { return _parent; }
            set
            {
                if (Object.ReferenceEquals(_parent, value))
                {
                    if (_parent != null)
                        _parent.SciterControls.Remove(this);

                    if (value != null)
                        value.SciterControls.Add(this);
                }
            }
        }

        /// <summary>
        /// Gets the collection of data-binding objects for this <see cref="T:System.Windows.Forms.IBindableComponent"></see>.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Windows.Forms.ControlBindingsCollection"></see> for this <see cref="T:System.Windows.Forms.IBindableComponent"></see>. 
        /// </returns>
        public ControlBindingsCollection DataBindings
        {
            [DebuggerStepThrough]
            get { return _bindings = _bindings ?? new ControlBindingsCollection(this); }
        }

        /// <summary>
        /// Gets or sets the collection of currency managers for the <see cref="T:System.Windows.Forms.IBindableComponent"></see>. 
        /// </summary>
        /// <returns>
        /// The collection of <see cref="T:System.Windows.Forms.BindingManagerBase"></see> objects for this <see cref="T:System.Windows.Forms.IBindableComponent"></see>.
        /// </returns>
        public BindingContext BindingContext 
        {
            [DebuggerStepThrough]
            get { return _bindingContext ?? (ParentControl != null ? ParentControl.BindingContext : null); }
            set 
            {
                if (!ReferenceEquals(_bindingContext, value))
                {
                    _bindingContext = value;
                    ProcessBindingContextChange();
                    OnBindingContextChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Sets focus on this element
        /// </summary>
        public void SetFocus()
        {
            Element.SetState(ElementState.Focus, ElementState.None);
        }
        #endregion

        #region Events Support
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { Events.AddHandler(PropertyChangedEvent, value); }
            remove { Events.RemoveHandler(PropertyChangedEvent, value); }
        }

        /// <summary>
        /// Occurs when a control should be validated.
        /// </summary>
        public event CancelEventHandler Validating
        {
            add { Events.AddHandler(ValidatingEvent, value); }
            remove { Events.RemoveHandler(ValidatingEvent, value); }
        }

        /// <summary>
        /// Occurs when a control has been validated.
        /// </summary>
        public event EventHandler Validated
        {
            add { Events.AddHandler(ValidatedEvent, value); }
            remove { Events.RemoveHandler(ValidatedEvent, value); }
        }

        /// <summary>
        /// Occurs when <see cref="Parent"/> of the control has been changed
        /// </summary>
        public event EventHandler ParentChanged
        {
            add { Events.AddHandler(ParentChangedEvent, value); }
            remove { Events.RemoveHandler(ParentChangedEvent, value); }
        }

        /// <summary>
        /// Occurs when <see cref="BindingContext"/> of the control has been changed
        /// </summary>
        public event EventHandler BindingContextChanged
        {
            add { Events.AddHandler(BindingContextChangedEvent, value); }
            remove { Events.AddHandler(BindingContextChangedEvent, value); }
        }

        /// <summary>
        /// Occurs when component has been disposed
        /// </summary>
        public event EventHandler Disposed
        {
            add { Events.AddHandler(DisposedEvent, value); }
            remove { Events.RemoveHandler(DisposedEvent, value); }
        }

        /// <summary>
        /// Raises <see cref="Disposed"/> event
        /// </summary>
        protected virtual void OnDisposed(EventArgs e)
        {
            if (HasEvents)
            {
                var handler = (EventHandler)Events[DisposedEvent];
                if (handler != null)
                    handler(this, e);
            }
        }

        /// <summary>
        /// Raises <see cref="Validating"/> event
        /// </summary>
        protected virtual void OnValidating(CancelEventArgs e)
        {
            if (HasEvents)
            {
                var handler = (CancelEventHandler)Events[ValidatingEvent];
                if (handler != null)
                    handler(this, e);
            }
        }

        /// <summary>
        /// Raises <see cref="Validating"/> event
        /// </summary>
        protected virtual void OnValidated(EventArgs e)
        {
            if (HasEvents)
            {
                var handler = (EventHandler)Events[ValidatedEvent];
                if (handler != null)
                    handler(this, e);
            }
        }

        /// <summary>
        /// Raises <see cref="PropertyChanged"/> event
        /// </summary>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (HasEvents)
            {
                var handler = (PropertyChangedEventHandler)Events[PropertyChangedEvent];
                if (handler != null)
                    handler(this, e);
            }
        }

        /// <summary>
        /// Raises <see cref="ParentChanged"/> event
        /// </summary>
        protected virtual void OnParentChanged(EventArgs e)
        {
            if (HasEvents)
            {
                var handler = (EventHandler)Events[ParentChangedEvent];
                if (handler != null)
                    handler(this, e);
            }
        }

        /// <summary>
        /// Raises <see cref="BindingContextChanged"/> event
        /// </summary>
        protected virtual void OnBindingContextChanged(EventArgs e)
        {
            if (HasEvents)
            {
                var handler = (EventHandler)Events[BindingContextChangedEvent];
                if (handler != null)
                    handler(this, e);
            }
        }
        #endregion

        #region Protected imterface
        /// <summary>
        /// Gets element state 
        /// </summary>
        protected ElementState GetElementState()
        {
            var element = Element;
            return element != null ? element.State : _stateToSet;
        }

        /// <summary>
        /// Updates element states
        /// </summary>
        protected void SetElementState(ElementState stateToSet, ElementState stateToClear)
        {
            _stateToSet = (_stateToSet & ~stateToClear) | stateToSet;
            _stateToClear = (_stateToClear & ~stateToSet) | stateToClear;

            var element = Element;
            if (element != null)
            {
                element.SetState(_stateToSet, _stateToClear);
                element.Update();
            }

            OnPropertyChanged(new PropertyChangedEventArgs(String.Empty));
        }

        /// <summary>
        /// Returns <see cref="Element"/> to bind to
        /// </summary>
        protected virtual Element GetElement()
        {
            var rootElement = Parent != null ? Parent.RootElement : null;
            if (rootElement != null)
                return rootElement.Find(Selector);

            return null;
        }

        /// <summary>
        /// Forces control to update handle of the DOM element
        /// </summary>
        protected internal virtual void UpdateDomElement()
        {
            var element = GetElement();
            if (!ReferenceEquals(Element, element))
            {
                ReleaseDomElement();
                SetDomElement(element);
            }
        }

        /// <summary>
        /// Processes binding context changes
        /// </summary>
        protected virtual void ProcessBindingContextChange()
        {
            if (_bindings != null)
            {
                for (var i = 0; i < _bindings.Count; ++i)
                {
                    BindingContext.UpdateBinding(BindingContext, _bindings[i]);
                }
            }
        }

        /// <summary>
        /// Handles <see cref="SciterBehavior.OnAttached"/>
        /// </summary>
        protected override void OnAttached(ElementEventArgs e)
        {
            var element = Element;
            element.SetState(_stateToSet, _stateToClear);

            if (_controlAttributes != null)
            {
                foreach (var item in _controlAttributes)
                {
                    element.Attributes[item.Key] = item.Value;
                }
            }

            if (_controlStyle != null)
            {
                foreach (var item in _controlStyle)
                {
                    element.Style[item.Key] = item.Value;
                }
            }
            
            base.OnAttached(e);
        }

        /// <summary>
        /// Handles <see cref="SciterBehavior.OnDetached"/>
        /// </summary>
        protected override void OnDetached(ElementEventArgs e)
        {
            base.OnDetached(e);
        }

        /// <summary>
        /// Performs control validation
        /// </summary>
        /// <returns>true if validation succeeded</returns>
        protected internal virtual bool PerformValidation()
        {
            var validateArgs = new CancelEventArgs();
            OnValidating(validateArgs);

            if (!validateArgs.Cancel)
                OnValidated(EventArgs.Empty);

            return !validateArgs.Cancel;
        }
        #endregion

        #region Private implementation
        /// <summary>
        /// Assigns a new dom element
        /// </summary>
        private void SetDomElement(Element element)
        {
            Debug.Assert(ElementRef == null);
            if (element != null)
            {
                ElementRef = element.Use();
                
                element.AttachBehavior(this);                
                GC.ReRegisterForFinalize(this);

                // Forces element update after property synchronization
                element.Update();
            }
        }

        /// <summary>
        /// Releases associated DOM element
        /// </summary>
        private void ReleaseDomElement()
        {
            var element = Element;
            if (element != null)
            {
                GC.SuppressFinalize(this);
                element.DetachBehavior(this);

                ElementRef = null;
            }
        }

        /// <summary>
        /// Unconditionaly assingns new parent object and updates DOM element
        /// </summary>
        internal void SetParent(ISciterControl parent)
        {
            BindingContext = null;

            _parent = parent;

            ProcessBindingContextChange();
            UpdateDomElement();

            OnParentChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Gets element attribute
        /// </summary>
        private string GetStyle(string name)
        {
            var val = default(String);
            var element = Element;
            if (element != null)
                val = element.Style[name];
            else if (!ControlStyle.TryGetValue(name, out val))
                val = String.Empty;

            return val ?? String.Empty;
        }

        /// <summary>
        /// Sets element attribute
        /// </summary>
        private void SetStyle(string name, string value)
        {
            ControlStyle[name] = value;
            var element = Element;
            if (element != null)
            {
                element.Style[name] = value;
                element.Update();
            }

            OnPropertyChanged(new PropertyChangedEventArgs(String.Empty));
        }

        /// <summary>
        /// Gets element attribute
        /// </summary>
        private string GetAttribute(string name)
        {
            var val = String.Empty;
            var element = Element;
            if (element != null)
                val = element.Attributes[name];
            else if (!ControlAttributes.TryGetValue(name, out val))
                val = String.Empty;

            return val ?? String.Empty;
        }

        /// <summary>
        /// Sets element attribute
        /// </summary>
        private void SetAttribute(string name, string value)
        {
            ControlAttributes[name] = value;
            var element = Element;
            if (element != null)
            {
                element.Attributes[name] = value;
                element.Update();
            }

            OnPropertyChanged(new PropertyChangedEventArgs(String.Empty));
        }

        /// <summary>
        /// Gets amount of element's attributes
        /// </summary>
        private int GetAttributeCount()
        {
            var element = Element;
            if (element != null)
                return element.Attributes.Count;
            else
                return ControlAttributes.Count;
        }

        /// <summary>
        /// Gets amount of element's attributes
        /// </summary>
        private void ClearAttributes()
        {
            var element = Element;
            if (element != null)
                element.Attributes.Clear();
            
            ControlAttributes.Clear();
        }
        #endregion

        #region Dispose implementation
        /// <summary>
        /// Disposes resources used by the component
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Parent = null;
                Debug.Assert(ElementRef == null, "Element handle should be cleared on dispose");
                OnDisposed(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Disposes resources used by the component
        /// </summary>
        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalization
        /// </summary>
        ~BindableControl()
        {
            Dispose(false);
        } 
	    #endregion    
    
        #region IComponent Members

        /// <summary>
        /// See <see cref="IComponent.Site"/>
        /// </summary>
        ISite IComponent.Site { get; set; }
        #endregion        

        #region IStyleAccessor implementation
        /// <summary>
        /// Gets or sets style attribute
        /// </summary>
        string IStyleAccessor.this[string name] 
        {
            get { return GetStyle(name); }
            set { SetStyle(name, value); }
        }
        #endregion

        #region IAttributeAccessor implementation
        /// <summary>
        /// Gets or sets element attribute
        /// </summary>
        string IAttributeAccessor.this[string name] 
        {
            get { return GetAttribute(name); }
            set { SetAttribute(name, value); }
        }

        /// <summary>
        /// Gets a number of element's attribues
        /// </summary>
        int IAttributeAccessor.Count { get { return GetAttributeCount(); } }

        /// <summary>
        /// Clears attributes collection
        /// </summary>
        void IAttributeAccessor.Clear()
        {
            ClearAttributes();
        }
        #endregion

        #region IEnumerable<KeyValuePair<string,string>> Members
        /// <summary>
        /// Gets attribues enumerator
        /// </summary>
        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            var element = Element;
            if (element != null)
                return element.Attributes.GetEnumerator();
            else
                return ControlAttributes.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Gets attribues enumerator
        /// </summary>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Attributes.GetEnumerator();
        }

        #endregion
    }
}