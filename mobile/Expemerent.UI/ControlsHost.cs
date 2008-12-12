using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Expemerent.UI.Behaviors;
using Expemerent.UI.Controls;
using Expemerent.UI.Dom;
using Expemerent.UI.Native;

namespace Expemerent.UI
{
    /// <summary>
    /// Allows to reuse most of implementation between <see cref="SciterForm"/> and <see cref="SciterControl"/>
    /// </summary>
    internal sealed class ControlsHost : SciterHost, ISciterControl
    {
        #region Private data
        /// <summary>
        /// See <see cref="AutoValidate"/> property
        /// </summary>
        private AutoValidate _autoValidate = AutoValidate.Disable;

        /// <summary>
        /// Collection of created behaviors
        /// </summary>
        private Dictionary<String, SciterBehavior> _behaviors;

        /// <summary>
        /// Pending LoadHtml request
        /// </summary>
        private Action<SciterView> _loadHtmlRequest; 
        #endregion

        #region Properties
        /// <summary>
        /// Gets the host owner
        /// </summary>
        public Control Control { get; private set; }

        /// <summary>
        /// Gets a value indicating whether Behaviors collection has been created
        /// </summary>
        private bool HasBehaviors
        {
            [DebuggerStepThrough]
            get { return _behaviors != null; }
        }

        /// <summary>
        /// Gets a collection of created behaviors
        /// </summary>
        private Dictionary<String, SciterBehavior> Behaviors
        {
            [DebuggerStepThrough]
            get { return _behaviors ?? (_behaviors = new Dictionary<String, SciterBehavior>()); }
            set { _behaviors = value; }
        }

        /// <summary>
        /// Instance of the view object 
        /// </summary>
        private SciterView _view;

        /// <summary>
        /// Gets or sets instance of the <see cref="SciterView"/> class
        /// </summary>
        public SciterView View
        {
            [DebuggerStepThrough]
            get { return _view; }
            private set
            {
                #region Precondition checking
                if (_view != null && value != null)
                    throw new InvalidOperationException("It is not allowed to replace an existing View object");
                #endregion

                EVENT_GROUPS events = EVENT_GROUPS.HANDLE_METHOD_CALL
                    | EVENT_GROUPS.HANDLE_SCRIPTING_METHOD_CALL
                    | EVENT_GROUPS.HANDLE_INITIALIZATION
                    | EVENT_GROUPS.HANDLE_FOCUS;

                _view = value;
                if (_view != null)
                    _view.AttachEventHandler(this, events);
            }
        }

        /// <summary>
        /// Returns collection of the sciter controls 
        /// </summary>
        public ControlsCollection SciterControls { get; private set; }

        /// <summary>
        /// Gets or sets auto validate options
        /// </summary>
        public AutoValidate AutoValidate
        {
            get { return _autoValidate; }
            set
            {
                #region Arguments checking
                if (value == AutoValidate.Inherit)
                    throw new ArgumentOutOfRangeException("value");                
                #endregion

                _autoValidate = value;
            }
        }

        /// <summary>
        /// Gets the root element (will be a document.rootElement)
        /// </summary>
        public Element RootElement
        {
            [DebuggerStepThrough]
            get { return View != null ? View.RootElement : null; }
        }
        #endregion

        #region Construction
        /// <summary>
        /// Creates a new instance of the <see cref="SciterHost"/> class
        /// </summary>
        public ControlsHost(Control owner)
        {
            #region Parameters checking
            if (owner == null)
                throw new ArgumentNullException("owner");
            #endregion

            Control = owner;
            SciterControls = new ControlsCollection(this);
        } 
        #endregion

        #region Public interface
        /// <summary>
        /// Attaches sciter to the host control. Control handle should be created at this point
        /// </summary>
        public void AttachToControl()
        {
            View = SciterView.Attach(Control.Handle, this);

            if (_loadHtmlRequest != null)
                _loadHtmlRequest(View);
        }

        /// <summary>
        /// Releases resources after the host window has been destroyed
        /// </summary>
        public void DetachFromControl()
        {
            View = null;
            Behaviors = null;
        }

        /// <summary>
        /// Loads Html from the resource. The html loading can be delayed if window handle was not created.
        /// </summary>
        /// <typeparam name="TResourceBase">Used as a base URI in the data requests</typeparam>
        /// <param name="resourceName">Relative resource name</param>
        public void LoadResource<TResourceBase>(string resourceName)
        {
            #region Arguments checking
            if (String.IsNullOrEmpty(resourceName))
                throw new ArgumentNullException("resourceName");
            #endregion

            _loadHtmlRequest = view => view.LoadResource(typeof(TResourceBase), resourceName);

            if (View != null)
                _loadHtmlRequest(View);
        }

        /// <summary>
        /// Loads Html from the resource. The html loading can be delayed if window handle was not created.
        /// </summary>
        /// <param name="resourceName">Resource name</param>
        public void LoadResource(string resourceName)
        {
            #region Arguments checking
            if (String.IsNullOrEmpty(resourceName))
                throw new ArgumentNullException("resourceName");
            #endregion

            _loadHtmlRequest = view => view.LoadResource(resourceName);

            if (View != null)
                _loadHtmlRequest(View);
        }

        /// <summary>
        /// Loads Html from the resource. The html loading can be delayed if window handle was not created.
        /// </summary>
        /// <typeparam name="baseUri">Used as a base URI in the data requests</typeparam>
        /// <param name="html">Html text</param>
        public void LoadHtml(string baseUri, string html)
        {
            #region Arguments checking
            if (String.IsNullOrEmpty(baseUri))
                throw new ArgumentNullException("baseUri");
            if (String.IsNullOrEmpty(html))
                throw new ArgumentNullException("html");
            #endregion

            _loadHtmlRequest = view => view.LoadHtml(baseUri, html);

            if (View != null)
                _loadHtmlRequest(View);
        }

        /// <summary>
        /// Reloads previously loaded document
        /// </summary>
        public void Reload()
        {
            if (View != null && _loadHtmlRequest != null)
                _loadHtmlRequest(View);
        }

        /// <summary>
        /// Calls script function
        /// </summary>
        public object Call(string methodName, params object[] args)
        {
            #region Precondition checking
            if (View == null)
                throw new InvalidOperationException("View should be initialized first");
            #endregion

            return View.Call(methodName, args);
        }

        /// <summary>
        /// Registers scripting class 
        /// </summary>
        public void RegisterClass<TType>() where TType : new()
        {
            #region Precondition checking
            if (View == null)
                throw new InvalidOperationException("View should be initialized first"); 
            #endregion

            Scripting.RegisterClass<TType>(View);
        }

        /// <summary>
        /// Causes validation in all sciter controls
        /// </summary>
        public bool PerformValidation()
        {
            if (SciterControls.Count > 0)
            {
                var firstFailed = default(BindableControl);
                foreach (var item in SciterControls)
                {
                    if (item.CausesValidation)
                    {
                        if (!item.PerformValidation())
                            firstFailed = firstFailed ?? item;
                    }
                }

                if (firstFailed == null)
                    return true;

                firstFailed.SetFocus();
                return false;
            }

            return true;
        } 
        #endregion

        #region ISciterHost implementation
        /// <summary>
        /// Handles destroyed event
        /// </summary>
        protected override void OnProcessDestroyed(EventArgs e)
        {
            SciterControls.FreeElements(); 
            
            base.OnProcessDestroyed(e);
        }

        /// <summary>
        /// Handles all external data has been load
        /// </summary>
        protected override void OnProcessDocumentComplete(DocumentCompleteEventArgs e)
        {
            SciterControls.UpdateElements();
            base.OnProcessDocumentComplete(e);
        }

        /// <summary>
        /// Handles behavior attach events
        /// </summary>
        protected override void OnProcessAttachBehavior(AttachBehaviorEventArgs e)
        {
            var behaviorName = e.BehaviorName;
            if (!String.IsNullOrEmpty(behaviorName))
            {
                var behavior = default(SciterBehavior);
                if (!(HasBehaviors && Behaviors.TryGetValue(behaviorName, out behavior)))
                {
                    base.OnProcessAttachBehavior(e);
                    Behaviors[behaviorName] = e.Behavior;
                }
                else
                    e.Behavior = e.Behavior ?? behavior;
            }
        }
        #endregion

        #region Internal implementation
        /// <summary>
        /// Handles focus change events
        /// </summary>
        protected override void OnFocus(FocusEventArgs e)
        {
            if (e.Phase == Phase.Sinking && e.IsLostFocus && AutoValidate != AutoValidate.Disable)
            {
                var element = e.Element;
                var control = SciterControls.FindControl(element);
                if (control != null && control.CausesValidation)
                {
                    var failed = !control.PerformValidation();
                    if (failed && AutoValidate == AutoValidate.EnablePreventFocusChange)
                    {
                        e.Handled = true;
                        e.Cancel = true;
                        var ar = default(IAsyncResult);
                        ar = Control.BeginInvoke((EventHandler) delegate 
                        {
                            Control.EndInvoke(ar);
                            control.SetFocus(); 
                        });
                    }
                }
            }
            base.OnFocus(e);
        } 
        #endregion
    }
}
