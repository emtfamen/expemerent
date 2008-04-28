using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Expemerent.UI.Behaviors;
using Expemerent.UI.Dom;
using Keys = System.Windows.Forms.Keys;
using System.Security;

namespace Expemerent.UI.Native
{
#pragma warning disable 0649
    /// <summary>
    /// Native interface to the htmlayout.dll
    /// </summary>
    [SuppressUnmanagedCodeSecurityAttribute]
    internal static partial class SciterHostApi
    {
        #region Host notification enum
        private enum HTMLAYOUT_NOTIFICATION
        {
            /// <summary>
            /// This notification is sent on parsing the document and while handling 
            /// &lt;INPUT&gt;, &lt;TEXTAREA&gt;, &lt;SELECT&gt; and &lt;WIDGET&gt; tags.
            /// </summary>
            HLN_CREATE_CONTROL = 0xAFF + 0x01,

            /// <summary>
            /// Notifies that HtmLayout is about to download a referred resource. 
            /// </summary>
            HLN_LOAD_DATA = 0xAFF + 0x02,

            /// <summary>
            /// This notification is sent when control creation process has completed. 
            /// </summary>
            HLN_CONTROL_CREATED = 0xAFF + 0x03,

            /// <summary>
            /// This notification indicates that external data (for example image) download process completed.
            /// </summary>
            HLN_DATA_LOADED = 0xAFF + 0x04,

            /// <summary>
            /// This notification is sent when all external data (for example image) has been downloaded.
            /// </summary>
            HLN_DOCUMENT_COMPLETE = 0xAFF + 0x05,

            /// <summary>
            /// This notification instructs host application to update its UI.
            /// </summary>
            HLN_UPDATE_UI = 0xAFF + 0x06,

            /// <summary>
            /// This notification is sent when HTMLayout destroys its controls.
            /// </summary>
            HLN_DESTROY_CONTROL = 0xAFF + 0x07,

            /// <summary>
            /// his notification is sent on parsing the document and while processing elements having non empty style.behavior attribute value.
            /// </summary>
            HLN_ATTACH_BEHAVIOR = 0xAFF + 0x08,

            /// <summary>
            /// This notification is sent after DOM element has changed its behavior(s) 
            /// </summary>
            HLN_BEHAVIOR_CHANGED = 0xAFF + 0x09,


            /// <summary>
            /// This notification is sent when dialog window created but document is not loaded
            /// </summary>
            HLN_DIALOG_CREATED = 0xAFF + 0x10,

            /// <summary>
            /// This notification is sent when dialog window is about to be closed - happens while handling WM_CLOSE in dialog
            /// </summary>
            HLN_DIALOG_CLOSE_RQ = 0xAFF + 0x0A,

            /// <summary>
            /// This notification is sent when document is loaded and parsed in full.
            /// </summary>
            HLN_DOCUMENT_LOADED = 0xAFF + 0x0B,
        } 
        #endregion

        #region Notification data structs
        /// <summary>
        /// Notification callback structure.
        /// </summary>
        private struct HL_NMHDR
        {
            public IntPtr hwnd;
            public int from;
            public HTMLAYOUT_NOTIFICATION code;
        } ;

        private struct SCN_LOAD_DATA
        {
            public IntPtr hwnd;
            public int from;
            public int code;

            //[MarshalAs(UnmanagedType.LPWStr)]
            public IntPtr uri; /**< [in] Zero terminated string, fully qualified uri, for example "http://server/folder/file.ext".*/
            public IntPtr outData; /**< [in,out] pointer to loaded data to return. if data exists in the cache then this field contain pointer to it*/
            public int outDataSize; /**< [in,out] loaded data size to return.*/
            public RESOURCE_TYPE dataType; /**< [in] SciterResourceType */

            ///element requested download, in case of context_menu:url( menu-url ) it is an element for which context menu was requested
            public IntPtr principal;

            /// <summary>
            /// n/a
            /// </summary>
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

        private struct SCN_DATA_LOADED
        {
            public IntPtr hwnd;
            public int from;
            public int code;

            //[MarshalAs(UnmanagedType.LPWStr)]
            public IntPtr uri; /**< [in] zero terminated string, fully qualified uri, for example "http://server/folder/file.ext".*/
            public IntPtr data; /**< [in] pointer to loaded data.*/
            public int dataSize; /**< [in] loaded data size (in bytes).*/
            public RESOURCE_TYPE dataType; /**< [in] SciterResourceType */
            public int status;

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
        private struct SCN_ATTACH_BEHAVIOR
        {
            public IntPtr hwnd;
            public int from;
            public int code;

            public IntPtr element; /**< [in] target DOM element handle*/

            //[MarshalAs(UnmanagedType.LPStr)]
            public IntPtr behaviorName; /**< [in] zero terminated string, string appears as value of CSS behavior:"???" attribute.*/

            public IntPtr elementProc; /**< [out] pointer to ElementEventProc function.*/
            public IntPtr elementTag; /**< [out] tag value, passed as is into pointer ElementEventProc function.*/
            public EVENT_GROUPS elementEvents; /**< [out] EVENT_GROUPS bit flags, event groups elementProc subscribed to. */

            public String GetBehaviorName()
            {
                return MarshalUtility.PtrToStringAnsi(behaviorName);
            }
        } ;     
        #endregion

        #region Private data
        /// <summary>
        /// Name of the SciterDll 
        /// </summary>
        private const string HtmlayoutDll = "htmlayout.dll";
        
        /// <summary>
        /// Storing callback delegate to prevent it from being GC
        /// </summary>        
        private static readonly HtmlayoutHostCallback _nativeCallback;

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
        /// HtmlayoutHostCallback
        /// </summary>
        private delegate IntPtr HtmlayoutHostCallback(int uMsg, IntPtr wParam, IntPtr lParam, IntPtr vParam);

        /// <summary>
        /// Sets callback method for htmlayout notifications
        /// </summary>
        [DllImport(HtmlayoutDll, EntryPoint = "HTMLayoutSetCallback")]
        private static extern void HTMLayoutSetCallback(IntPtr hWndHTMLayout, HtmlayoutHostCallback cb, IntPtr cbParam);

        /// <summary>
        /// Load HTML from in memory buffer with base.
        /// </summary>
        [DllImport(HtmlayoutDll, EntryPoint = "HTMLayoutLoadHtmlEx")]
        private static extern bool HtmlayoutLoadHtml(IntPtr hWndSciter, [MarshalAs(UnmanagedType.LPArray)] Byte[] html, int htmlSize, [MarshalAs(UnmanagedType.LPWStr)] string baseUrl);

        /// <summary>
        /// This function is used in response to SCN_LOAD_DATA request. 
        /// </summary>
        [DllImport(HtmlayoutDll, EntryPoint = "HTMLayoutDataReady")]
        private static extern bool HtmlayoutDataReady(IntPtr hWndSciter, [MarshalAs(UnmanagedType.LPWStr)] string uri, [MarshalAs(UnmanagedType.LPArray)] byte[] data, int dataLength);
        #endregion

        #region Public interface
        /// <summary>
        /// Returns reference to the sciter VM
        /// </summary>
        public static IntPtr SciterGetVM(IntPtr hwnd)
        {
            throw new NotSupportedException("NotSupported in Htmlayout");
        }

        /// <summary>
        /// SciterNativeDefineClass - register "native" class to for the script.
        /// </summary>
        public static bool SciterNativeDefineClass(IntPtr hvm, SciterNativeClassDef pClassDef)
        {
            throw new NotSupportedException("NotSupported in Htmlayout");
        }

        /// <summary>
        /// TIS_throw - throw error from method or property implementation code.
        /// </summary>
        public static bool SciterNativeThrow(IntPtr hvm, string errorMsg)
        {
            throw new NotSupportedException("NotSupported in Htmlayout");
        }


        /// <summary>
        /// Returns reference to the sciter API object
        /// </summary>
        public static SciterDomApi SciterDomApi
        {
            [DebuggerStepThrough]
            get { return _sciterApi; }
        }

        /// <summary>
        /// Call sciter WndProc without calling to default wnd proc
        /// </summary>
        [DllImport(HtmlayoutDll, EntryPoint = "HTMLayoutProcND")]
        public static extern IntPtr SciterProcND(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool pbHandled);

        /// <summary>
        /// Load HTML file.
        /// </summary>
        [DllImport(HtmlayoutDll, EntryPoint = "HTMLayoutLoadFile")]
        public static extern bool SciterLoadFile(IntPtr hWndSciter, [MarshalAs(UnmanagedType.LPWStr)] string fileName);

        /// <summary>
        /// Calls scripting function
        /// </summary>
        public static object SciterCall(IntPtr hwnd, string functionName, params object[] args)
        {
            throw new NotImplementedException("NotSupported in Htmlayout");
        }

        /// <summary>
        /// Load HTML from in memory buffer with base.
        /// </summary>
        public static bool SciterLoadHtml(IntPtr hWndSciter, byte[] html, string baseUrl)
        {
            Debug.Assert(html != null, "Html parameter cannot be null");

            return HtmlayoutLoadHtml(hWndSciter, html, html.Length, baseUrl);
        }

        /// <summary>
        /// Load HTML from in memory buffer with base.
        /// </summary>
        public static bool SciterLoadHtml(IntPtr hWndSciter, string html, string baseUrl)
        {
            Debug.Assert(html != null, "Html parameter cannot be null");

            var bytes = MarshalUtility.StringToByteUtf8(html, true);
            return HtmlayoutLoadHtml(hWndSciter, bytes, bytes.Length, baseUrl);
        }

        /// <summary>
        /// Assings host callback to the sciter window
        /// </summary>
        /// <param name="hWndSciter"></param>
        /// <param name="ntf"></param>
        public static void SciterSetCallback(IntPtr hWndSciter, ISciterNotifications ntf)
        {
            Debug.Assert(ntf != null, "Notification callback cannot be null");

            HTMLayoutSetCallback(hWndSciter, _nativeCallback, InstanceProtector.Protect(ntf));
        }

        /// <summary>
        /// This function is used in response to SCN_LOAD_DATA request. 
        /// </summary>
        public static bool SciterDataReady(IntPtr hWndSciter, String uri, byte[] buffer)
        {
            return HtmlayoutDataReady(hWndSciter, uri, buffer, buffer == null ? 0 : buffer.Length);
        }

        /// <summary>
        /// This function is used in response to SCN_LOAD_DATA request. 
        /// </summary>
        public static bool SciterDataReadyAsync(IntPtr hWndSciter, IntPtr requestId, String uri, byte[] buffer)
        {
            throw new NotImplementedException("Async ops not supported");
        }

        #endregion

        #region Private implementation
        /// <summary>
        /// Creates an instance of SciterApi object
        /// </summary>
        private static SciterDomApi CreateSciterApiInterface()
        {
            return new SciterDomApi();
        }

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
                bool handled = false;
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

        #region Host events partial methods
        unsafe static partial void Host_HandleAttachBehavior(ISciterNotifications host, IntPtr pns);
        unsafe static partial void Host_HandleDataLoaded(ISciterNotifications host, IntPtr pns);
        unsafe static partial void Host_HandleLoadData(ISciterNotifications host, IntPtr pns);
        unsafe static partial void Host_HandleDocumentComplete(ISciterNotifications host, IntPtr pns); 
        #endregion

        /// <summary>
        /// Bridge to sciter host callbacks
        /// </summary>
        private unsafe static IntPtr Host_NativeCallback(int uMsg, IntPtr wParam, IntPtr pns, IntPtr vParam)
        {
            using (var prot = ElementScope.Create())
            {
                var host = InstanceProtector.GetInstance(vParam) as ISciterNotifications;
                if (host != null)
                {
                    var ntf = (HL_NMHDR*)pns;
                    switch (ntf->code)
                    {
                        case HTMLAYOUT_NOTIFICATION.HLN_ATTACH_BEHAVIOR:
                            Host_HandleAttachBehavior(host, pns);
                            break;
                        case HTMLAYOUT_NOTIFICATION.HLN_DATA_LOADED:
                            Host_HandleDataLoaded(host, pns);
                            break;
                        case HTMLAYOUT_NOTIFICATION.HLN_LOAD_DATA:
                            Host_HandleLoadData(host, pns);
                            break;
                        case HTMLAYOUT_NOTIFICATION.HLN_DOCUMENT_COMPLETE:
                            Host_HandleDocumentComplete(host, pns);
                            break;
                        case HTMLAYOUT_NOTIFICATION.HLN_BEHAVIOR_CHANGED:
                        case HTMLAYOUT_NOTIFICATION.HLN_CONTROL_CREATED:
                        case HTMLAYOUT_NOTIFICATION.HLN_CREATE_CONTROL:
                        case HTMLAYOUT_NOTIFICATION.HLN_DESTROY_CONTROL:
                        case HTMLAYOUT_NOTIFICATION.HLN_DIALOG_CLOSE_RQ:
                        case HTMLAYOUT_NOTIFICATION.HLN_DIALOG_CREATED:
                        case HTMLAYOUT_NOTIFICATION.HLN_DOCUMENT_LOADED:
                        case HTMLAYOUT_NOTIFICATION.HLN_UPDATE_UI:
                            break;
                        default:
                            //There is a lot of notifications besides the HLN_
                            //Debug.WriteLine(String.Format("Invalid notification code: {0}", ntf->code));
                            break;
                    }
                }

                Debug.Assert(host != null, "Behavior object has been garbage collected");
                return IntPtr.Zero;
            }
        }
        #endregion
    }

#pragma warning restore 0649
}