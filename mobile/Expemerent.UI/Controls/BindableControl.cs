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
    /// Base implementation of data binding support
    /// </summary>
    public class BindableControl : SciterBehavior, IBindableComponent, INotifyPropertyChanged, IDisposable
    {
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
        private Dictionary<string, string> _attributes;

        /// <summary>
        /// "Offline" collection of element styles
        /// </summary>
        private Dictionary<string, string> _styles;
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
        /// Gets or sets a value indicating whether validation is performed
        /// </summary>
        public bool CausesValidation { get; set; } 
        /// <summary>
        /// Attribues collection
        /// </summary>
        private Dictionary<string, string> Attributes { get { return _attributes ?? (_attributes = new Dictionary<string, string>()); } }

        /// <summary>
        /// Styles collection
        /// </summary>
        private Dictionary<string, string> Styles { get { return _styles ?? (_styles = new Dictionary<string, string>()); } }

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
                }
            }
        }
        #endregion

        #region Events Support
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when a control should be validated.
        /// </summary>
        public event CancelEventHandler Validating;

        /// <summary>
        /// Occurs when a control has been validated.
        /// </summary>
        public event EventHandler Validated;

        /// <summary>
        /// Occurs when <see cref="Parent"/> of the control has been changed
        /// </summary>
        public event EventHandler ParentChanged;

        /// <summary>
        /// Occurs when component has been disposed
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Raises <see cref="Disposed"/> event
        /// </summary>
        protected virtual void OnDisposed(EventArgs e)
        {
            var handler = Disposed;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises <see cref="Validating"/> event
        /// </summary>
        protected virtual void OnValidating(CancelEventArgs e)
        {
            var handler = Validating;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises <see cref="Validating"/> event
        /// </summary>
        protected virtual void OnValidated(EventArgs e)
        {
            var handler = Validated;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises <see cref="PropertyChanged"/> event
        /// </summary>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises <see cref="ParentChanged"/> event
        /// </summary>
        protected virtual void OnParentChanged(EventArgs e)
        {
            var handler = ParentChanged;
            if (handler != null)
                handler(this, e);
        }
        #endregion

        #region Protected imterface
        /// <summary>
        /// Gets element attribute
        /// </summary>
        protected string GetStyle(string name)
        {
            var val = default(String);
            var element = Element;
            if (element != null)
            {
                val = element.Style[name];
                Styles[name] = val;
            }
            else if (!Styles.TryGetValue(name, out val))
            {
                val = String.Empty;
            }

            return val ?? String.Empty;
        }

        /// <summary>
        /// Sets element attribute
        /// </summary>
        protected void SetStyle(string name, string value)
        {
            Styles[name] = value;
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
        protected string GetAttribute(string name)
        {
            var val = String.Empty;
            var element = Element;
            if (element != null)
            {
                val = element.Attributes[name];
                Attributes[name] = val;
            }
            else if (!Attributes.TryGetValue(name, out val))
            {
                val = String.Empty;
            }

            return val ?? String.Empty;
        }

        /// <summary>
        /// Sets element attribute
        /// </summary>
        protected void SetAttribute(string name, string value)
        {
            Attributes[name] = value;
            var element = Element;
            if (element != null)
            {
                element.Attributes[name] = value;
                element.Update();
            }

            OnPropertyChanged(new PropertyChangedEventArgs(String.Empty));
        }

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
            if (Parent != null && Parent.View != null)
            {
                var rootElement = Parent.View.RootElement;
                return rootElement != null ? rootElement.Find(Selector) : null;
            }

            return null;
        }

        /// <summary>
        /// Updates DOM element
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

            if (_attributes != null)
            {
                foreach (var item in _attributes)
                {
                    element.Attributes[item.Key] = item.Value;
                }
            }

            if (_styles != null)
            {
                foreach (var item in _styles)
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
        protected virtual void PerformValidation(FocusEventArgs e)
        {
            var validateArgs = new CancelEventArgs();
            OnValidating(validateArgs);

            if (!(e.Cancel = validateArgs.Cancel))
                OnValidated(EventArgs.Empty);            
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
        }

        /// <summary>
        /// Handles <see cref="SciterBehavior.Focus"/> event
        /// </summary>
        protected override void OnFocusEvent(FocusEventArgs e)
        {
            if (e.Phase == Phase.Sinking && e.IsLostFocus)
            {
                if (CausesValidation) 
                    PerformValidation(e);

                e.Handled = true;
            }

            base.OnFocusEvent(e);
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
    }
}