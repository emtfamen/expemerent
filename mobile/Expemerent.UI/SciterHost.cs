using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Expemerent.UI.Controls;
using System.Diagnostics;
using System.Windows.Forms;
using Expemerent.UI.Native;
using Expemerent.UI.Behaviors;
using Expemerent.UI.Dom;

namespace Expemerent.UI
{
    /// <summary>
    /// Base implementation of the <see cref="ISciterHost"/> interface
    /// </summary>
    /// <remarks>
    /// Allows to reuse most of implementation between <see cref="SciterForm"/> and <see cref="SciterControl"/>
    /// </remarks>
    internal class SciterHost : SciterBehavior, ISciterHost
    {
        #region Event keys
        /// <summary>
        /// Event key for the <see cref="Destroyed"/> event
        /// </summary>
        private readonly static object DestroyedEvent = new object();

        /// <summary>
        /// Event key for the <see cref="DocumentComplete"/> event
        /// </summary>
        private readonly static object DocumentCompleteEvent = new object();

        /// <summary>
        /// Event key for the <see cref="LoadData"/> event
        /// </summary>
        private readonly static object LoadDataEvent = new object();

        /// <summary>
        /// Event key for the <see cref="DataLoaded"/> event
        /// </summary>
        private readonly static object DataLoadedEvent = new object();

        /// <summary>
        /// Event key for the <see cref="AttachBehavior"/> event
        /// </summary>
        private readonly static object AttachBehaviorEvent = new object();

        /// <summary>
        /// Event key for the <see cref="CallbackHost"/> event
        /// </summary>
        private readonly static object CallbackHostEvent = new object(); 
        #endregion

        #region Private data
        /// <summary>
        /// Instance of the view object 
        /// </summary>
        private SciterView _view;

        /// <summary>
        /// Host owner
        /// </summary>
        private ISciterControl _owner;

        /// <summary>
        /// See <see cref="SciterControls"/> property
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
        #endregion

        #region Properties
        /// <summary>
        /// Gets the host owner
        /// </summary>
        protected ISciterControl Owner
        {
            [DebuggerStepThrough]
            get { return _owner; } 
        }

        /// <summary>
        /// Gets a handle of the host window
        /// </summary>
        protected IntPtr Handle
        {
            [DebuggerStepThrough]
            get { return _owner != null ? _owner.Control.Handle : IntPtr.Zero; } 
        }

        /// <summary>
        /// Gets a value indicating whether Behaviors collection has been created
        /// </summary>
        protected bool HasBehaviors
        {
            [DebuggerStepThrough]
            get { return _behaviors != null; }
        }

        /// <summary>
        /// Gets a collection of created behaviors
        /// </summary>
        protected Dictionary<String, SciterBehavior> Behaviors
        {
            [DebuggerStepThrough]
            get { return _behaviors ?? (_behaviors = new Dictionary<String, SciterBehavior>()); }
            set { _behaviors = value; }
        }

        /// <summary>
        /// Gets a value indicating whether controls collection has been created
        /// </summary>
        protected bool HasSciterControls
        {
            [DebuggerStepThrough]
            get { return _sciterControls != null; }
        }

        /// <summary>
        /// Returns collection of the sciter controls 
        /// </summary>
        public ControlsCollection SciterControls
        {
            [DebuggerStepThrough]
            get { return _sciterControls ?? (_sciterControls = new ControlsCollection(_owner)); }
        }

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
                
                _view = value;
                if (_view != null)
                    _view.AttachEventHandler(this, EVENT_GROUPS.HANDLE_METHOD_CALL | EVENT_GROUPS.HANDLE_SCRIPTING_METHOD_CALL | EVENT_GROUPS.HANDLE_INITIALIZATION);                    
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
        public SciterHost(ISciterControl owner)
        {
            #region Parameters checking
            if (owner == null)
                throw new ArgumentNullException("owner");
            #endregion

            _owner = owner;
        } 
        #endregion

        #region Public events
        /// <summary>
        /// Occurs when the <see cref="SciterView"/> going to be destroyed
        /// </summary>
        public event EventHandler Destroyed
        {
            add { Events.AddHandler(DestroyedEvent, value); }
            remove { Events.RemoveHandler(DestroyedEvent, value); }
        }

        /// <summary>
        /// Occurs when the document with all external data has been loaded
        /// </summary>
        public event EventHandler<DocumentCompleteEventArgs> DocumentComplete
        {
            add { Events.AddHandler(DocumentCompleteEvent, value); }
            remove { Events.RemoveHandler(DocumentCompleteEvent, value); }
        }

        /// <summary>
        /// Occurs when the sciter is about to download a referred resource. 
        /// </summary>
        public event EventHandler<LoadDataEventArgs> LoadData
        {
            add { Events.AddHandler(LoadDataEvent, value); }
            remove { Events.RemoveHandler(LoadDataEvent, value); }
        }

        /// <summary>
        /// Occurs when download process completed 
        /// </summary>
        public event EventHandler<DataLoadedEventArgs> DataLoaded
        {
            add { Events.AddHandler(DataLoadedEvent, value); }
            remove { Events.RemoveHandler(DataLoadedEvent, value); }
        }

        /// <summary>
        /// Occurs when behavior should be attached to the DOM element
        /// </summary>
        public event EventHandler<AttachBehaviorEventArgs> AttachBehavior
        {
            add { Events.AddHandler(AttachBehaviorEvent, value); }
            remove { Events.RemoveHandler(AttachBehaviorEvent, value); }
        }

        /// <summary>
        /// Occurs when sciter wants to notify host
        /// </summary>
        public event EventHandler<CallbackHostEventArgs> CallbackHost
        {
            add { Events.AddHandler(CallbackHostEvent, value); }
            remove { Events.RemoveHandler(CallbackHostEvent, value); }
        } 
        #endregion

        #region Public interface
        /// <summary>
        /// Attaches sciter to the host control. Control handle should be created at this point
        /// </summary>
        public void AttachToControl()
        {
            View = SciterView.Attach(this);

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
        /// Causes validation in all sciter controls
        /// </summary>
        public void PerformValidation()
        {
            if (HasSciterControls)
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

        #region ISciterHost implementation
        /// <summary>
        /// Gets HWND handle of the host window
        /// </summary>
        IntPtr ISciterHost.Handle { get { return Handle; } }

        /// <summary>
        /// Raises <see cref="Destroyed"/> event
        /// </summary>
        void ISciterHost.ProcessDestroyed(EventArgs e)
        {
            if (HasSciterControls)
                SciterControls.FreeElements();

            if (HasEvents)
            {
                var handler = (EventHandler)Events[DestroyedEvent];
                if (handler != null)
                {
                    handler(_owner, e);
                }
            }
        }

        /// <summary>
        /// Occurs when all external data has been loaded
        /// </summary>
        void ISciterNotifications.ProcessDocumentComplete(DocumentCompleteEventArgs e)
        {
            if (HasSciterControls)
                SciterControls.UpdateElements();

            if (HasEvents)
            {
                var handler = (EventHandler<DocumentCompleteEventArgs>)Events[DocumentCompleteEvent];
                if (handler != null)
                {
                    handler(_owner, e);
                }
            }
        }

        /// <summary>
        /// Occurs when the sciter is about to download a referred resource. 
        /// </summary>
        void ISciterNotifications.ProcessLoadData(LoadDataEventArgs e)
        {
            if (HasEvents)
            {
                var handler = (EventHandler<LoadDataEventArgs>)Events[LoadDataEvent];
                if (handler != null)
                {
                    handler(_owner, e);
                }
            }
        }

        /// <summary>
        /// Occurs when download process completed 
        /// </summary>
        void ISciterNotifications.ProcessDataLoaded(DataLoadedEventArgs e)
        {
            if (HasEvents)
            {
                var handler = (EventHandler<DataLoadedEventArgs>)Events[DataLoadedEvent];
                if (handler != null)
                {
                    handler(_owner, e);
                }
            }
        }

        /// <summary>
        /// Occurs when behavior should be attached to the DOM element
        /// </summary>
        void ISciterNotifications.ProcessAttachBehavior(AttachBehaviorEventArgs e)
        {

            var behavior = default(SciterBehavior);
            var behaviorName = e.BehaviorName;
            if (!String.IsNullOrEmpty(behaviorName))
            {
                if (!(HasBehaviors && Behaviors.TryGetValue(behaviorName, out behavior)) && HasEvents)
                {
                    var handler = (EventHandler<AttachBehaviorEventArgs>)Events[AttachBehaviorEvent];
                    if (handler != null)
                    {
                        // Hiding source element 
                        var newEvent = new AttachBehaviorEventArgs() { BehaviorName = behaviorName };
                        handler(this, newEvent);
                        behavior = newEvent.Behavior;

                        Behaviors[behaviorName] = behavior;
                    }
                }
            }

            // If Behavior not fould - leave the old one
            e.Behavior = behavior ?? e.Behavior;
        }

        /// <summary>
        /// Occurs when sciter wants to notify host
        /// </summary>
        void ISciterNotifications.ProcessCallbackHost(CallbackHostEventArgs e)
        {
            Debug.WriteLine(String.Format("Host event: [{0}, {1}]", e.First, e.Second));

            if (HasEvents)
            {
                var handler = (EventHandler<CallbackHostEventArgs>)Events[CallbackHostEvent];
                if (handler != null)
                {
                    handler(_owner, e);
                }
            }
        } 
        #endregion
    }
}
