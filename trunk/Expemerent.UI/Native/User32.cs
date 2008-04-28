using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Expemerent.UI.Native
{
    /// <summary>
    /// Useful Win32 imports
    /// </summary>
    [SuppressUnmanagedCodeSecurityAttribute]
    internal static class User32
    {
        #region Constants
        /// <summary>
        /// Name of the import library. different for Win32 and WinCE 
        /// </summary>
        public const string ImportLibrary = "user32.dll";

        /// <summary>
        /// WndProc identifier
        /// </summary>
        public const int GWL_WNDPROC = -4;

        /// <summary>
        /// WM_CREATE messageId
        /// </summary>
        public const int WM_CREATE = 0x1;

        /// <summary>
        /// WM_DESTROY messageId
        /// </summary>
        public const int WM_DESTROY = 0x2; 
        #endregion

        #region Public interface
        /// <summary>
        /// A callback to a Win32 window procedure (wndproc):
        /// </summary>
        public delegate IntPtr WndProcCallback(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Allows to change window style
        /// </summary>
        [DllImport(ImportLibrary)]
        public static extern IntPtr SetWindowLong(
            IntPtr hwnd, int nIndex, IntPtr dwNewLong);

        /// <summary>
        /// Calls window procedure
        /// </summary>
        [DllImport(ImportLibrary)]
        public static extern IntPtr CallWindowProc(
            IntPtr lpPrevWndFunc, IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam); 
        #endregion
    } 
}