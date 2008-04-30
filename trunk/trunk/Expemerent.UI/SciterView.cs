using System;
using System.Diagnostics;
using Expemerent.UI.Dom;
using Expemerent.UI.Native;
using Expemerent.UI.Protocol;
using System.Windows.Forms;

namespace Expemerent.UI
{
    /// <summary>
    /// View class for working with sciter in forms and controls
    /// </summary>
    public class SciterView : ISciterNotifications
    {
        #region Properties
        /// <summary>
        /// Gets instance of sciter dom api
        /// </summary>
        private static SciterDomApi SciterDomApi
        {
            [DebuggerStepThrough]
            get { return SciterHostApi.SciterDomApi; }
        }

        /// <summary>
        /// Gets or sets value of the window handle
        /// </summary>
        internal IntPtr HandleInternal { get; set; }

        /// <summary>
        /// Gets value of the window handle
        /// </summary>
        protected IntPtr Handle
        {
            get
            {
                #region Precondition checking
                if (HandleInternal == IntPtr.Zero)
                    throw new InvalidOperationException("View should be attached to window before use");
                
                #endregion

                return HandleInternal;
            }
        }

        /// <summary>
        /// Gets or sets the window hook instance
        /// </summary>
        internal WindowHook Hook { get; private set; }

        #endregion

        #region Construction
        /// <summary>
        /// Creates a new instance of the <see cref="SciterView"/> class
        /// </summary>
        protected SciterView()
        {
        } 
        #endregion

        #region Public events
        /// <summary>
        /// Occurs when window going to be created
        /// </summary>
        public event EventHandler<EventArgs> Created;

        /// <summary>
        /// Occurs when window has been destroyed
        /// </summary>
        public event EventHandler<EventArgs> Destroyed;

        /// <summary>
        /// Occurs when all external data has been loaded
        /// </summary>
        public event EventHandler<DocumentCompleteEventArgs> DocumentComplete;

        /// <summary>
        /// Occurs when download process completed 
        /// </summary>
        public event EventHandler<DataLoadedEventArgs> DataLoaded;

        /// <summary>
        /// Occurs when the sciter is about to download a referred resource. 
        /// </summary>
        public event EventHandler<LoadDataEventArgs> LoadData;

        /// <summary>
        /// Occurs when behavior should be attached to the DOM element
        /// </summary>
        public event EventHandler<AttachBehaviorEventArgs> AttachBehavior;

        /// <summary>
        /// Occurs when sciter wants to notify host
        /// </summary>
        public event EventHandler<CallbackHostEventArgs> CallbackHost;
        #endregion

        #region Events support
        /// <summary>
        /// Raises <see cref="Created"/> event
        /// </summary>
        protected virtual void OnCreated(EventArgs e)
        {
            var handler = Created;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises <see cref="Destroyed"/> event
        /// </summary>
        protected virtual void OnDestroyed(EventArgs e)
        {
            var handler = Destroyed;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        
        #endregion

        #region Message processing
        /// <summary>
        /// Controls should route all window events throgh this method. 
        /// </summary>
        private static IntPtr ProcessMessage(IntPtr wnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            handled = false;
            return SciterHostApi.SciterProcND(wnd, msg, wparam, lparam, ref handled);
        }

        /// <summary>
        /// Performs sciter initialization
        /// </summary>
        private void InitializeSciter()
        {
            Hook.HandleDestroyed += delegate 
            { 
                HandleInternal = IntPtr.Zero; 
                Hook = null; 
            };

            // Processing WM_CREATE through sciter
            bool handled = false;
            SciterHostApi.SciterProcND(Handle, User32.WM_CREATE, IntPtr.Zero, IntPtr.Zero, ref handled);
            SciterHostApi.SciterSetCallback(Handle, this);
        }
        #endregion

        #region Public interface
        /// <summary>
        /// Attaches SciterView to the existing window
        /// </summary>
        public static SciterView Attach(Control control)
        {
            SciterView view = new SciterView()
            {
                HandleInternal = control.Handle,
                Hook = WindowHook.Install(control, SciterView.ProcessMessage)
            };
            view.InitializeSciter();

            return view;
        }

        /// <summary>
        /// Calls script function
        /// </summary>
        public object Call(string functionName, params object[] args)
        {
            #region Arguments checking
            if (String.IsNullOrEmpty(functionName))
                throw new ArgumentNullException("functionName"); 
            #endregion

            return SciterHostApi.SciterCall(Handle, functionName, args);
        }

        /// <summary>
        /// Loads Html text in the sciter window
        /// </summary>
        public void LoadHtml(string baseUri, string html)
        {
            #region Arguments checking
            if (String.IsNullOrEmpty(html))
                throw new ArgumentNullException("html");
            #endregion

            SciterHostApi.SciterLoadHtml(Handle, html, baseUri);
        }

        /// <summary>
        /// Loads Html text in the sciter window
        /// </summary>
        public void LoadHtml(string html)
        {
            LoadHtml(null, html);
        }

        /// <summary>
        /// Loads Html text in the sciter window
        /// </summary>
        public void LoadFile(string file)
        {
            #region Arguments checking
            if (String.IsNullOrEmpty(file))
                throw new ArgumentNullException("file"); 
            #endregion
            
            SciterHostApi.SciterLoadFile(Handle, file);
        }

        /// <summary>
        /// Loads html from the resource
        /// </summary>
        public void LoadResource(Type baseType, String resource)
        {
            #region Arguments checking
            if (baseType == null)
                throw new ArgumentNullException("baseType");

            if (String.IsNullOrEmpty(resource))
                throw new ArgumentNullException("resource"); 
            #endregion

            var nameIndex = resource.LastIndexOf(ResProtocol.PathSeparator);
            var location = resource.Substring(0, nameIndex);
            var baseUri = ResProtocol.ProtocolPrefix +
                    baseType.AssemblyQualifiedName +
                    ResProtocol.PathSeparator + location + ResProtocol.PathSeparator;

            var resourceName = baseUri + resource.Substring(nameIndex + 1);
            var text = SciterFactory.ResolveBinResource(resourceName, ResourceType.Html);
            
            SciterHostApi.SciterLoadHtml(Handle, text, baseUri);
        }

        /// <summary>
        /// Gets reference to the root element
        /// </summary>
        public Element RootElement
        {
            [DebuggerStepThrough]
            get { return SciterDomApi.GetRootElement(Handle); }
        }

        /// <summary>
        /// Creates a new handle without adding it to the tree
        /// </summary>
        public ElementRef CreateElement(String tag, String text)
        {
            return SciterDomApi.CreateElement(tag, text);
        }

        /// <summary>
        /// Registers scripting class 
        /// </summary>
        public void RegisterClass<TType>() where TType : new()
        {
            Scripting.RegisterClass<TType>(this);
        }
        #endregion

        #region ISciterNotifications Members
        /// <summary>
        /// Occurs when all external data has been loaded
        /// </summary>
        void ISciterNotifications.FireDocumentComplete(DocumentCompleteEventArgs e)
        {
            var handler = DocumentComplete;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when the sciter is about to download a referred resource. 
        /// </summary>
        void ISciterNotifications.FireLoadData(LoadDataEventArgs e)
        {
            var handler = LoadData;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when download process completed 
        /// </summary>
        void ISciterNotifications.FireDataLoaded(DataLoadedEventArgs e)
        {
            var handler = DataLoaded;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when behavior should be attached to the DOM element
        /// </summary>
        void ISciterNotifications.FireAttachBehavior(AttachBehaviorEventArgs e)
        {
            var handler = AttachBehavior;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when sciter wants to notify host
        /// </summary>
        void ISciterNotifications.FireCallbackHost(CallbackHostEventArgs e)
        {
            var handler = CallbackHost;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion
    }
}
