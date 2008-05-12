using System;
using System.Collections.Generic;
using System.Diagnostics;
using Form = System.Windows.Forms.Form;
using Control = System.Windows.Forms.Control;
using Expemerent.UI.Behaviors;
using Expemerent.UI.Controls;
using Expemerent.UI.Dom;
using Expemerent.UI.Native;

namespace Expemerent.UI
{
    /// <summary>
    /// Default implementation of <see cref="Form"/> with sciter content. 
    /// </summary>
    /// <remarks>
    /// <see cref="SciterForm"/> and <see cref="SciterControl"/> classes provide an extended services working with Htmlayout/Sciter.    
    /// [*] SciterControls management
    /// [*] Behaviors management
    /// </remarks>
    public class SciterControl : Control, ISciterControl, ISciterBehavior
    {
        #region Private data
        /// <summary>
        /// Key for the <see cref="AttachBehavior"/> event
        /// </summary>
        private static readonly object AttachBehaviorEvent = new Object();

        /// <summary>
        /// Key for the <see cref="AttachBehavior"/> event
        /// </summary>
        private static readonly object DocumentCompleteEvent = new Object();

        /// <summary>
        /// Key for the <see cref="CallbackHost"/> event
        /// </summary>
        private static readonly object CallbackHostEvent = new Object();

        /// <summary>
        /// See <see cref="View"/> property
        /// </summary>
        private SciterView _view;

        /// <summary>
        /// See <see cref="SciterControls"/> collection
        /// </summary>
        private ControlsCollection _sciterControls;

        /// <summary>
        /// Collection of created behaviors
        /// </summary>
        private Dictionary<String, SciterBehavior> _behaviors;

        /// <summary>
        /// Pending LoadHtml request
        /// </summary>
        private Action<SciterView> _loadHtmlRequest;

        /// <summary>
        /// Gets sciter view
        /// </summary>
        private SciterView View
        {
            [DebuggerStepThrough]
            get { return _view; }
            set
            {
                if (_view != null && value != null)
                    throw new InvalidOperationException("It is not allowed to replace an existing View object");

                _view = value;
                if (_view != null)
                {
                    SciterDomApi.WindowAttachEventHandler(Handle, this, EVENT_GROUPS.HANDLE_METHOD_CALL | EVENT_GROUPS.HANDLE_SCRIPTING_METHOD_CALL);

                    _view.AttachBehavior += (s, e) => OnAttachBehavior(e);
                    _view.DocumentComplete += (s, e) => OnDocumentComplete(e);
                    _view.CallbackHost += (s, e) => OnCallbackHost(e);
                }
            }
        }

        /// <summary>
        /// Gets instance of sciter dom api
        /// </summary>
        private static SciterDomApi SciterDomApi
        {
            [DebuggerStepThrough]
            get { return SciterHostApi.SciterDomApi; }
        }
        #endregion

        #region Public interface
        /// <summary>
        /// Occurs when control needs a behavior instance
        /// </summary>
        public event EventHandler<AttachBehaviorEventArgs> AttachBehavior
        {
            add { Events.AddHandler(AttachBehaviorEvent, value); }
            remove { Events.AddHandler(AttachBehaviorEvent, value); }
        }

        /// <summary>
        /// Occurs when control needs a behavior instance
        /// </summary>
        public event EventHandler<AttachBehaviorEventArgs> DocumentComplete
        {
            add { Events.AddHandler(DocumentCompleteEvent, value); }
            remove { Events.AddHandler(DocumentCompleteEvent, value); }
        }

        /// <summary>
        /// Occurs when control needs a behavior instance
        /// </summary>
        public event EventHandler<CallbackHostEventArgs> CallbackHost
        {
            add { Events.AddHandler(CallbackHostEvent, value); }
            remove { Events.AddHandler(CallbackHostEvent, value); }
        }

        /// <summary>
        /// Gets a collection of sciter controls
        /// </summary>
        public ControlsCollection SciterControls
        {
            [DebuggerStepThrough]
            get { return _sciterControls ?? (_sciterControls = new ControlsCollection(this)); }
        }

        /// <summary>
        /// Returns true if collection of sciter controls has been created
        /// </summary>
        private bool IsControlsCollectionCreated 
        {
            [DebuggerStepThrough]
            get { return _sciterControls != null; }
        }

        /// <summary>
        /// Gets root dom element of the currently loaded document
        /// </summary>
        public Element RootElement
        {
            [DebuggerStepThrough]
            get { return View != null ? View.RootElement : null; }
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
        #endregion

        #region Protected properties
        /// <summary>
        /// Collection of created behaviors
        /// </summary>
        protected Dictionary<String, SciterBehavior> Behaviors
        {
            [DebuggerStepThrough]
            get { return _behaviors ?? (_behaviors = new Dictionary<string, SciterBehavior>()); }
            private set { _behaviors = null; }
        } 
        #endregion

        #region Event handling
        /// <summary>
        /// Raises attach behavior event
        /// </summary>
        protected virtual void OnCallbackHost(CallbackHostEventArgs e)
        {
            Debug.WriteLine(String.Format("Host event: [{0}, {1}]", e.First, e.Second));

            var handler = (EventHandler<CallbackHostEventArgs>)Events[CallbackHostEvent];
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises attach behavior event
        /// </summary>
        protected virtual void OnDocumentComplete(DocumentCompleteEventArgs e)
        {
            if (IsControlsCollectionCreated)
                SciterControls.UpdateElements();

            var handler = (EventHandler<DocumentCompleteEventArgs>)Events[DocumentCompleteEvent];
            if (handler != null)
                handler(this, DocumentCompleteEventArgs.Empty);
        }

        /// <summary>
        /// Raises attach behavior event
        /// </summary>
        protected virtual void OnAttachBehavior(AttachBehaviorEventArgs e)
        {
            var behavior = default(SciterBehavior);
            if (_behaviors == null || !_behaviors.TryGetValue(e.BehaviorName, out behavior))
            {
                var handler = (EventHandler<AttachBehaviorEventArgs>)Events[AttachBehaviorEvent];
                if (handler != null)
                {
                    // Hiding source element
                    var newEvent = new AttachBehaviorEventArgs() { BehaviorName = e.BehaviorName };
                    handler(this, newEvent);
                    behavior = newEvent.Behavior;
                }
            }
            
            e.Behavior = behavior ?? e.Behavior;
        }

        /// <summary>
        /// Handles <see cref="HandleCreated"/> event
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            if (Site == null || !Site.DesignMode)
                View = SciterView.Attach(this);

            if (View != null && _loadHtmlRequest != null)
                _loadHtmlRequest(View);

            base.OnHandleCreated(e);
        }

        /// <summary>
        /// Handles <see cref="HandleDestroyed"/> event
        /// </summary>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            Debug.Assert(View != null && View.HandleInternal == IntPtr.Zero, "View should be detached from window at this point");

            View = null;
            Behaviors = null;
        }

        /// <summary>
        /// Causes validation in all sciter controls
        /// </summary>
        protected void PerformValidation()
        {
            if (IsControlsCollectionCreated)
            {
                var firstFailed = default(BindableControl);
                foreach (var item in SciterControls)
                {
                    if (item.CausesValidation)
                    {
                        if (!item.PerformValidation())
                        {
                            firstFailed = firstFailed ?? item;
                        }
                    }
                }

                if (firstFailed != null)
                    firstFailed.SetFocus();
            }
        }

        #endregion

        #region ISciterControl implementation
        /// <summary>
        /// Gets parent windows forms control
        /// </summary>
        Control ISciterControl.Control
        {
            [DebuggerStepThrough]
            get { return this; }
        }

        /// <summary>
        /// Gets the root element (will be a document.rootElement)
        /// </summary>
        Element ISciterControl.RootElement
        {
            [DebuggerStepThrough]
            get { return View != null ? View.RootElement : null; }
        } 
        #endregion

        #region Dispose
        /// <summary>
        /// Disposes used resources
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && _sciterControls != null)
                _sciterControls.FreeElements();

            base.Dispose(disposing);
        } 
        #endregion

        #region ISciterBehavior Members

        /// <summary>
        /// Handles behavior initialization
        /// </summary>
        void ISciterBehavior.ProcessAttach(ElementEventArgs e)
        {
        }

        /// <summary>
        /// Handles behavior deinitialization
        /// </summary>
        void ISciterBehavior.ProcessDettach(ElementEventArgs e)
        {
        }

        /// <summary>
        /// Handles mouse events
        /// </summary>
        void ISciterBehavior.ProcessMouse(MouseEventArgs e)
        {
        }

        /// <summary>
        /// Handles keyboard events
        /// </summary>
        void ISciterBehavior.ProcessKey(KeyEventArgs e)
        {
        }

        /// <summary>
        /// Handles focus events
        /// </summary>
        void ISciterBehavior.ProcessFocus(FocusEventArgs e)
        {
        }

        /// <summary>
        /// Handles draw events
        /// </summary>
        void ISciterBehavior.ProcessDraw(DrawEventArgs e)
        {
        }

        /// <summary>
        /// Handles timer event
        /// </summary>
        void ISciterBehavior.ProcessTimer(ElementEventArgs e)
        { 
        }

        /// <summary>
        /// Handles secondary behavior event
        /// </summary>
        void ISciterBehavior.ProcessBehaviorEvent(BehaviorEventArgs e)
        {
        }

        /// <summary>
        /// Handles method call event
        /// </summary>
        void ISciterBehavior.ProcessMethodCall(MethodCallEventArgs e)
        {
        }

        /// <summary>
        /// Handles data arrived event
        /// </summary>
        void ISciterBehavior.ProcessDataArrived(DataArrivedEventArgs e)
        {
        }

        /// <summary>
        /// Handles size changes
        /// </summary>
        void ISciterBehavior.ProcessSize(ElementEventArgs e)
        {
        }

        /// <summary>
        /// Handles scripting calls
        /// </summary>
        void ISciterBehavior.ProcessScriptingMethodCall(ScriptingMethodCallEventArgs e)
        {
        }

        /// <summary>
        /// Handles scroll
        /// </summary>
        void ISciterBehavior.ProcessScroll(ElementEventArgs e)
        {
        }
        #endregion
    }
}