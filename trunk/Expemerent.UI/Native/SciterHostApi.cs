using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Keys = System.Windows.Forms.Keys;
using System.Security;
using Expemerent.UI.Behaviors;
using Expemerent.UI.Dom;

namespace Expemerent.UI.Native
{
#pragma warning disable 0649
    /// <summary>
    /// Native interface to the sciter-x.dll
    /// </summary>
    [SuppressUnmanagedCodeSecurityAttribute]
    internal static partial class SciterHostApi
    {
        #region Host specific enums
        internal enum SCITER_NOTIFICATION
        {
            /// <summary>
            /// Notifies that HtmLayout is about to download a referred resource. 
            /// </summary>
            SC_LOAD_DATA = 0x01,

            /// <summary>
            /// This notification indicates that external data (for example image) download process completed.
            /// </summary>
            SC_DATA_LOADED = 0x02,

            /// <summary>
            /// This notification is sent when all external data (for example image) has been downloaded.
            /// </summary>
            SC_DOCUMENT_COMPLETE = 0x03,

            /// <summary>
            /// This notification is sent on parsing the document and while processing elements having non empty style.behavior attribute value.
            /// </summary>
            SC_ATTACH_BEHAVIOR = 0x04,

            /// <summary>
            /// This notification is sent on  
            /// 1) stdin, stdout and stderr operations and
            /// 2) view.hostCallback(p1,p2) calls from script
            /// </summary>
            SC_CALLBACK_HOST = 0x05,

            LOAD_OK = 0, // do default loading if data not set
            LOAD_DISCARD = 1, // discard request completely
            LOAD_DELAYED = 2, // data will be delivered later by the host
        }
        #endregion

        #region Host specific data
        /// <summary>
        /// Notification callback structure.
        /// </summary>
        internal struct SCN_CALLBACK_NOTIFICATION
        {
            public SCITER_NOTIFICATION code; /**< [in] one of the codes above.*/
            public IntPtr hwnd; /**< [in] HWND of the window this callback was attached to.*/
        } ;

        internal struct SCN_LOAD_DATA
        {
            public SCITER_NOTIFICATION code; /**< [in] one of the codes above.*/
            public IntPtr hwnd; /**< [in] HWND of the window this callback was attached to.*/

            //[MarshalAs(UnmanagedType.LPWStr)]
            public IntPtr uri; /**< [in] Zero terminated string, fully qualified uri, for example "http://server/folder/file.ext".*/
            public IntPtr outData; /**< [in,out] pointer to loaded data to return. if data exists in the cache then this field contain pointer to it*/
            public int outDataSize; /**< [in,out] loaded data size to return.*/
            public RESOURCE_TYPE dataType; /**< [in] SciterResourceType */

            public IntPtr request_id;

            public IntPtr principal;
            public IntPtr initiator;

            public String GetUri()
            {
                return Marshal.PtrToStringUni(uri);
            }

            public byte[] GetData()
            {
                return MarshalUtility.MarshalData(outData, outDataSize);
            }
        } ;

        internal struct SCN_DATA_LOADED
        {
            public SCITER_NOTIFICATION code; /**< [in] one of the codes above.*/
            public IntPtr hwnd; /**< [in] HWND of the window this callback was attached to.*/

            //[MarshalAs(UnmanagedType.LPWStr)]
            public IntPtr uri; /**< [in] zero terminated string, fully qualified uri, for example "http://server/folder/file.ext".*/
            public IntPtr data; /**< [in] pointer to loaded data.*/
            public int dataSize; /**< [in] loaded data size (in bytes).*/
            public RESOURCE_TYPE dataType; /**< [in] SciterResourceType */

            public String GetUri()
            {
                return Marshal.PtrToStringUni(uri);
            }

            public byte[] GetData()
            {
                return MarshalUtility.MarshalData(data, dataSize);
            }
        } ;

        [StructLayout(LayoutKind.Sequential)]
        internal struct SCN_ATTACH_BEHAVIOR
        {
            public SCITER_NOTIFICATION code; /**< [in] one of the codes above.*/
            public IntPtr hwnd; /**< [in] HWND of the window this callback was attached to.*/

            public IntPtr element; /**< [in] target DOM element handle*/

            //[MarshalAs(UnmanagedType.LPStr)]
            public IntPtr behaviorName; /**< [in] zero terminated string, string appears as value of CSS behavior:"???" attribute.*/

            public IntPtr elementProc; /**< [out] pointer to ElementEventProc function.*/
            public IntPtr elementTag; /**< [out] tag value, passed as is into pointer ElementEventProc function.*/

            public EVENT_GROUPS elementEvents { get { return EVENT_GROUPS.HANDLE_ALL; } set { ; } }

            public String GetBehaviorName()
            {
                return MarshalUtility.PtrToStringAnsi(behaviorName);
            }
        } ;

        [StructLayout(LayoutKind.Explicit)]
        internal struct SCN_CALLBACK_HOST
        {
            public enum ChannelType
            {
                StdIn,
                StdOut,
                StdErr
            }

            /// <summary>
            /// one of the codes above
            /// </summary>
            [FieldOffset(0)]
            public SCITER_NOTIFICATION code;

            /// <summary>
            /// HWND of the window this callback was attached to
            /// </summary>
            [FieldOffset(1 * 4)]
            public IntPtr hwnd;

            /// <summary>
            /// 0 - stdin, 1 - stdout, 2 - stderr
            /// </summary>
            [FieldOffset(2 * 4)]
            public ChannelType channel;

            /// <summary>
            /// in, parameter #1
            /// </summary>
            [FieldOffset(3 * 4)]
            public JsonValue p1;

            /// <summary>
            /// in, parameter #2
            /// </summary>
            [FieldOffset(3 * 4 + JsonValue.SizeOf)]
            public JsonValue p2;

            /// <summary>
            /// out, retval
            /// </summary>
            [FieldOffset(3 * 4 + 2 * JsonValue.SizeOf)]
            public JsonValue r;
        }

        #endregion

        #region Private data
        /// <summary>
        /// Name of the SciterDll 
        /// </summary>
        private const string SciterDll = "sciter-x.dll";

        /// <summary>
        /// SciterHostCallback
        /// </summary>
        private delegate uint SciterHostCallback(IntPtr pns, IntPtr callbackParam);

        /// <summary>
        /// Storing callback delegate to prevent it from being GC
        /// </summary>
        private static readonly SciterHostCallback _nativeCallback;

        /// <summary>
        /// Storing callback delegate to prevent it from being GC
        /// </summary>
        private static readonly ElementEventProc _nativeElementProc;

        /// <summary>
        /// Marshalled element entry point 
        /// </summary>
        public static readonly IntPtr ElementEventProcEntryPoint;

        /// <summary>
        /// Cached instance of sciterApi object
        /// </summary>
        private static readonly SciterDomApi _sciterApi;
        #endregion

        #region Intialization
        /// <summary>
        /// Static initialization
        /// </summary>
        static SciterHostApi()
        {
            _nativeCallback = Host_NativeCallback;
            _nativeElementProc = Behavior_NativeCallbackI4;

            _sciterApi = CreateSciterApiInterface();

            ElementEventProcEntryPoint = Marshal.GetFunctionPointerForDelegate(_nativeElementProc);
        } 
        #endregion

        #region Imports section
        /// <summary>
        /// SciterGetDomApi
        /// </summary>
        [DllImport(SciterDll, EntryPoint = "SciterGetDomApi")]
        private static extern IntPtr SciterGetDomApi_Native();

        /// <summary>
        /// SciterSetCallback
        /// </summary>
        [DllImport(SciterDll)]
        private static extern void SciterSetCallback(IntPtr hWndSciter, SciterHostCallback cb, IntPtr cbParam);

        /// <summary>
        /// Load HTML from in memory buffer with base.
        /// </summary>
        [DllImport(SciterDll)]
        private static extern bool SciterLoadHtml(IntPtr hWndSciter, [MarshalAs(UnmanagedType.LPArray)] byte[] html, int htmlSize, [MarshalAs(UnmanagedType.LPWStr)] string baseUrl);

        /// <summary>
        /// This function is used in response to SCN_LOAD_DATA request. 
        /// </summary>
        [DllImport(SciterDll)]
        private static extern bool SciterDataReady(IntPtr hWndSciter, [MarshalAs(UnmanagedType.LPWStr)] string uri, [MarshalAs(UnmanagedType.LPArray)] byte[] data, int dataLength);

        /// <summary>
        /// This function is used in response to SCN_LOAD_DATA request. 
        /// </summary>
        [DllImport(SciterDll)]
        private static extern bool SciterDataReadyAsync(IntPtr hWndSciter, [MarshalAs(UnmanagedType.LPWStr)] string uri, [MarshalAs(UnmanagedType.LPArray)] byte[] data, int dataLength, IntPtr requestId);

        /// <summary>
        /// Calls function defined in sciter script
        /// </summary>
        [DllImport(SciterDll)]
        private static extern bool SciterCall(IntPtr hWnd, [MarshalAs(UnmanagedType.LPStr)] string functionName, int argc, [MarshalAs(UnmanagedType.LPArray)] JsonValue[] argv, out JsonValue retval);

        #endregion

        #region Public interface
        /// <summary>
        /// Returns reference to the sciter VM
        /// </summary>
        [DllImport(SciterDll)]
        public static extern IntPtr SciterGetVM(IntPtr hwnd);

        /// <summary>
        /// SciterNativeDefineClass - register "native" class to for the script.
        /// </summary>
        [DllImport(SciterDll)]
        public static extern bool SciterNativeDefineClass(IntPtr hvm, SciterNativeClassDef pClassDef);

        /// <summary>
        /// TIS_throw - throw error from method or property implementation code.
        /// </summary>
        [DllImport(SciterDll)]
        public static extern bool SciterNativeThrow(IntPtr hvm, [MarshalAs(UnmanagedType.LPWStr)]string errorMsg);

        /// <summary>
        /// Returns reference to the sciter API object
        /// </summary>
        public static SciterDomApi SciterDomApi
        {
            [DebuggerStepThrough]
            get { return _sciterApi; }
        }

        /// <summary>
        /// Calls scripting function
        /// </summary>
        public static object SciterCall(IntPtr hwnd, string functionName, params object[] args)
        {
            var jsonResult = new JsonValue();
            var jsonParams = JsonValue.CreateJsonArray(args);

            try
            {
                SciterCall(hwnd, functionName, jsonParams.Length, jsonParams, out jsonResult);
                return jsonResult.GetValue();
            }
            finally
            {
                JsonValue.FreeJsonArray(jsonParams);
                jsonResult.Clear();
            }
        }

        /// <summary>
        /// Call sciter WndProc without calling to default wnd proc
        /// </summary>
        [DllImport("sciter-x.dll")]
        public static extern IntPtr SciterProcND(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool pbHandled);

        /// <summary>
        /// Load HTML file.
        /// </summary>
        [DllImport("sciter-x.dll")]
        public static extern bool SciterLoadFile(IntPtr hWndSciter, [MarshalAs(UnmanagedType.LPWStr)] string fileName);

        /// <summary>
        /// Load HTML from in memory buffer with base.
        /// </summary>
        public static bool SciterLoadHtml(IntPtr hWndSciter, byte[] html, string baseUrl)
        {
            Debug.Assert(html != null, "Html parameter cannot be null");

            return SciterLoadHtml(hWndSciter, html, html.Length, baseUrl);
        }

        /// <summary>
        /// Load HTML from in memory buffer with base.
        /// </summary>
        public static bool SciterLoadHtml(IntPtr hWndSciter, string html, string baseUrl)
        {
            Debug.Assert(html != null, "Html parameter cannot be null");

            var bytes = MarshalUtility.StringToByteUtf8(html, true);
            return SciterLoadHtml(hWndSciter, bytes, bytes.Length, baseUrl);
        }

        /// <summary>
        /// Assings host callback to the sciter window
        /// </summary>
        /// <param name="hWndSciter"></param>
        /// <param name="ntf"></param>
        public static void SciterSetCallback(IntPtr hWndSciter, ISciterNotifications ntf)
        {
            SciterSetCallback(hWndSciter, _nativeCallback, InstanceProtector.Protect(ntf));
        }

        /// <summary>
        /// This function is used in response to SCN_LOAD_DATA request. 
        /// </summary>
        public static bool SciterDataReady(IntPtr hWndSciter, String uri, byte[] buffer)
        {
            return SciterDataReady(hWndSciter, uri, buffer, buffer == null ? 0 : buffer.Length);
        }

        /// <summary>
        /// This function is used in response to SCN_LOAD_DATA request. 
        /// </summary>
        public static bool SciterDataReadyAsync(IntPtr hWndSciter, IntPtr requestId, String uri, byte[] buffer)
        {
            return SciterDataReadyAsync(hWndSciter, uri, buffer, buffer == null ? 0 : buffer.Length, requestId);
        }

        #endregion

        #region Private implementation
        /// <summary>
        /// Creates an instance of SciterApi object
        /// </summary>
        private static SciterDomApi CreateSciterApiInterface()
        {
            IntPtr api = SciterGetDomApi_Native();
            return (SciterDomApi)Marshal.PtrToStructure(api, typeof(SciterDomApi));
        }
        #endregion

        #region Host events processing
        
        #region Host events partial methods
        unsafe static partial void Host_HandleAttachBehavior(ISciterNotifications host, IntPtr pns);
        unsafe static partial void Host_HandleDataLoaded(ISciterNotifications host, IntPtr pns);
        unsafe static partial void Host_HandleLoadData(ISciterNotifications host, IntPtr pns);
        unsafe static partial void Host_HandleDocumentComplete(ISciterNotifications host, IntPtr pns);
        #endregion

        /// <summary>
        /// Bridge to sciter host callbacks
        /// </summary>
        private unsafe static uint Host_NativeCallback(IntPtr pns, IntPtr callbackParam)
        {
            using (var prot = ElementScope.Create())
            {
                var host = InstanceProtector.GetInstance(callbackParam) as ISciterNotifications;
                if (host != null)
                {
                    var ntf = (SCN_CALLBACK_NOTIFICATION*)pns;
                    switch (ntf->code)
                    {
                        case SCITER_NOTIFICATION.SC_ATTACH_BEHAVIOR:
                            Host_HandleAttachBehavior(host, pns);
                            break;
                        case SCITER_NOTIFICATION.SC_DATA_LOADED:
                            Host_HandleDataLoaded(host, pns);
                            break;
                        case SCITER_NOTIFICATION.SC_LOAD_DATA:
                            Host_HandleLoadData(host, pns);
                            break;
                        case SCITER_NOTIFICATION.SC_CALLBACK_HOST:
                            Host_HandleCallbackHost(host, pns);
                            break;
                        case SCITER_NOTIFICATION.SC_DOCUMENT_COMPLETE:
                            Host_HandleDocumentComplete(host, pns);
                            break;
                        default:
                            Debug.Fail(String.Format("Invalid notification code: {0}", ntf->code));
                            break;
                    }
                }

                Debug.Assert(host != null, "Behavior object has been garbage collected");
                return 0;
            }
        }

        /// <summary>
        /// Handles host callback notification
        /// </summary>
        private unsafe static void Host_HandleCallbackHost(ISciterNotifications host, IntPtr pns)
        {
            var datantf = (SCN_CALLBACK_HOST*)pns;
            var e = new CallbackHostEventArgs((CallbackHostEventArgs.HostChannelType)datantf->channel, datantf->p1.GetValue(), datantf->p2.GetValue());
            host.ProcessCallbackHost(e);

            if (e.ReturnValue != null)
                datantf->r.SetValue(e.ReturnValue);
        }
        #endregion

        #region Behavior events processing

        #region Behavior events partial methods
        unsafe static partial void Behavior_HandleDraw(ISciterBehavior behavior, IntPtr he, IntPtr prms, ref bool handled);
        unsafe static partial void Behavior_HandleInitialization(ISciterBehavior behavior, IntPtr he, IntPtr prms, ref bool handled);
        unsafe static partial void Behavior_HandleMouseEvent(ISciterBehavior behavior, IntPtr he, IntPtr prms, ref bool handled);
        unsafe static partial void Behavior_HandleKey(ISciterBehavior behavior, IntPtr he, IntPtr prms, ref bool handled);
        unsafe static partial void Behavior_HandleFocusEvent(ISciterBehavior behavior, IntPtr he, IntPtr prms, ref bool handled);
        unsafe static partial void Behavior_HandleScroll(ISciterBehavior behavior, IntPtr he, IntPtr prms, ref bool handled);
        unsafe static partial void Behavior_HandleTimer(ISciterBehavior behavior, IntPtr he, IntPtr prms, ref bool handled);
        unsafe static partial void Behavior_HandleSize(ISciterBehavior behavior, IntPtr he, IntPtr prms, ref bool handled);
        unsafe static partial void Behavior_HandleDataArrived(ISciterBehavior behavior, IntPtr he, IntPtr prms, ref bool handled);
        unsafe static partial void Behavior_HandleBehaviorEvent(ISciterBehavior behavior, IntPtr he, IntPtr prms, ref bool handled);
        unsafe static partial void Behavior_HandleScriptingMethodCall(ISciterBehavior behavior, IntPtr he, IntPtr prms, ref bool handled);
        unsafe static partial void Behavior_HandleMethodCall(ISciterBehavior behavior, IntPtr he, IntPtr prms, ref bool handled);
        #endregion

        /// <summary>
        /// Compact framework have a problems with "bool" results
        /// </summary>
        private static int Behavior_NativeCallbackI4(IntPtr tag, IntPtr he, EVENT_GROUPS evtg, IntPtr prms)
        {
            return Behavior_NativeCallback(tag, he, evtg, prms) ? 1 : 0;
        }

        /// <summary>
        /// Bridge to sciter element callbacks
        /// </summary>
        private static unsafe bool Behavior_NativeCallback(IntPtr tag, IntPtr he, EVENT_GROUPS evtg, IntPtr prms)
        {
            using (var prot = ElementScope.Create())
            {
                var handled = false;
                var behavior = InstanceProtector.GetInstance(tag) as ISciterBehavior;
                if (behavior != null)
                {
                    switch (evtg)
                    {
                        case EVENT_GROUPS.HANDLE_INITIALIZATION:
                            Behavior_HandleInitialization(behavior, he, prms, ref handled);
                            break;
                        case EVENT_GROUPS.HANDLE_MOUSE:
                            Behavior_HandleMouseEvent(behavior, he, prms, ref handled);
                            break;
                        case EVENT_GROUPS.HANDLE_KEY:
                            Behavior_HandleKey(behavior, he, prms, ref handled);
                            break;
                        case EVENT_GROUPS.HANDLE_FOCUS:
                            Behavior_HandleFocusEvent(behavior, he, prms, ref handled);
                            break;
                        case EVENT_GROUPS.HANDLE_SCROLL:
                            Behavior_HandleScroll(behavior, he, prms, ref handled);
                            break;
                        case EVENT_GROUPS.HANDLE_TIMER:
                            Behavior_HandleTimer(behavior, he, prms, ref handled);
                            break;
                        case EVENT_GROUPS.HANDLE_SIZE:
                            Behavior_HandleSize(behavior, he, prms, ref handled);
                            break;
                        case EVENT_GROUPS.HANDLE_DRAW:
                            Behavior_HandleDraw(behavior, he, prms, ref handled);
                            break;
                        case EVENT_GROUPS.HANDLE_DATA_ARRIVED:
                            Behavior_HandleDataArrived(behavior, he, prms, ref handled);
                            break;
                        case EVENT_GROUPS.HANDLE_BEHAVIOR_EVENT:
                            Behavior_HandleBehaviorEvent(behavior, he, prms, ref handled);
                            break;
                        case EVENT_GROUPS.HANDLE_METHOD_CALL:
                            Behavior_HandleMethodCall(behavior, he, prms, ref handled);
                            break;
                        case EVENT_GROUPS.HANDLE_SCRIPTING_METHOD_CALL:
                            Behavior_HandleScriptingMethodCall(behavior, he, prms, ref handled);
                            break;
                        default:
                            break;
                    }
                }

                Debug.Assert(behavior != null, "Behavior object has been garbage collected");
                return handled;
            }
        }       
        #endregion
    }

#pragma warning restore 0649
}