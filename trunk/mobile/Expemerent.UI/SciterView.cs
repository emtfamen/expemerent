using System;
using System.Diagnostics;
using Expemerent.UI.Dom;
using Expemerent.UI.Native;
using Expemerent.UI.Protocol;
using System.Windows.Forms;
using Expemerent.UI.Behaviors.BuiltIn;
using Expemerent.UI.Behaviors;
using System.IO;

namespace Expemerent.UI
{
    /// <summary>
    /// View class for working with sciter in forms and controls
    /// </summary>
    public class SciterView 
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
        /// Gets or sets host instance
        /// </summary>
        internal ISciterHost Host { get; set; }

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
        /// Static initialization
        /// </summary>
        static SciterView()
        {
            SciterFactory.RegisterBehavior<AccessKeys>();
            SciterFactory.RegisterBehavior<ExpandableList>();
            SciterFactory.RegisterBehavior<CollapsibleList>();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SciterView"/> class
        /// </summary>
        protected SciterView()
        {
        } 
        #endregion

        #region Message processing
        /// <summary>
        /// Controls should route all window events throgh this method. 
        /// </summary>
        private IntPtr ProcessMessage(IntPtr wnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            if (msg == User32.WM_DESTROY)
                Host.ProcessDestroyed(EventArgs.Empty);

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
            SciterHostApi.SciterSetCallback(Handle, Host);
        }
        #endregion

        #region Public interface
        /// <summary>
        /// Attaches SciterView to the existing window
        /// </summary>
        public static SciterView Attach(ISciterHost host)
        {
            SciterView view = new SciterView();
            view.Host = host;
            view.HandleInternal = host.Handle;
            view.Hook = WindowHook.Install(host.Handle, view.ProcessMessage);
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
        /// Loads html from file 
        /// </summary>
        public void LoadResource(String resourceName)
        {
            var text = SciterFactory.ResolveBinResource(resourceName, ResourceType.Html);

            if (text == null)
                throw new FileNotFoundException(String.Format("Resource {0} not found", resourceName));

            SciterHostApi.SciterLoadHtml(Handle, text, resourceName);
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

            var resourceName = ResProtocol.ProtocolPrefix +
                    baseType.AssemblyQualifiedName +
                    ResProtocol.PathSeparator + resource;

            var text = SciterFactory.ResolveBinResource(resourceName, ResourceType.Html);
            
            if (text == null)
                throw new FileNotFoundException(String.Format("Resource {0} not found", resourceName));

            SciterHostApi.SciterLoadHtml(Handle, text, resourceName);
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
        /// Registers scripting class 
        /// </summary>
        public void RegisterClass<TType>() where TType : new()
        {
            Scripting.RegisterClass<TType>(this);
        }

        /// <summary>
        /// Attached event handler to the Window
        /// </summary>
        internal void AttachEventHandler(ISciterBehavior handler, EVENT_GROUPS events)
        {
            SciterDomApi.WindowAttachEventHandler(Handle, handler, events);
        }
        #endregion
    }
}
