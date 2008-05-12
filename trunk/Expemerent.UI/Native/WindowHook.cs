using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Security;
using Expemerent.UI.Dom;

namespace Expemerent.UI.Native
{
    /// <summary>
    /// The WndProcCallback method is used when a hooked window's message map contains the hooked message.
    /// </summary>
    /// <param name="hwnd">The handle to the window for which the message was received.</param>
    /// <param name="msg"></param>
    /// <param name="wParam">The message's parameters (part 1).</param>
    /// <param name="lParam">The message's parameters (part 2).</param>
    /// <param name="handled">The invoked function sets this to true if it
    /// handled the message. If the value is false when the callback
    /// returns, the next window procedure in the wndproc chain is called.
    /// </param>
    /// <returns>Returns a value specified for the given message.</returns>
    internal delegate IntPtr WndProcCallback(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled);

    /// <summary>
    /// Window hook implementation
    /// </summary>
    internal sealed class WindowHook 
    {
        /// <summary>
        /// Control instance to hook
        /// </summary>
        private readonly Control _control;
        
        /// <summary>
        /// Stored instance of the ProcessMessage delegete to prevent it from GC
        /// </summary>
        private readonly User32.WndProcCallback _mainWndProc;

        /// <summary>
        /// Ptr to the original ProcessMessage
        /// </summary>
        private IntPtr _originalWndProc;

        /// <summary>
        /// Installed callback
        /// </summary>
        private WndProcCallback _callback;

        /// <summary>
        /// Occurs when window handle has been destroyed
        /// </summary>
        public event EventHandler HandleDestroyed;

        /// <summary>
        /// Installs hook on the existing control. 
        /// This operation should be done before creation of the window handle
        /// </summary>
        public static WindowHook Install(Control control, WndProcCallback callback)
        {
            return new WindowHook(control, callback);
        }

        /// <summary>
        /// Makes a connection between a specified window handle
        /// and the callback to be called when that message is received.
        /// </summary>
        private WindowHook(Control control, WndProcCallback callback)
        {
            _control = control;

            _mainWndProc = WindowProc;
            _callback = callback;

            _control.HandleDestroyed += control_HandleDestroyed;
            
            // Subclassing window
            _originalWndProc = User32.SetWindowLong(_control.Handle, User32.GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(_mainWndProc));
        }

        /// <summary>
        /// The event handler called when a control's handle is destroyed.
        /// We remove the HookedProcInformation from hwndDict and
        /// put it back into ctlDict in case the control get re-
        /// created and we still want to hook its messages.
        /// </summary>
        private void control_HandleDestroyed(object sender, EventArgs e)
        {
            revertWndProc();
        }
       
        /// <summary>
        /// This is a generic wndproc. It is the callback for all hooked
        /// windows. If we get into this function, we look up the hwnd in the
        /// global list of all hooked windows to get its message map. If the
        /// message received is present in the message map, its callback is
        /// invoked with the parameters listed here.
        /// </summary>
        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            using (var scope = ElementScope.Create())
            {
                Debug.Assert(_originalWndProc != IntPtr.Zero, "Window hook was not installed");

                bool handled = false;

                IntPtr retval = _callback(hwnd, msg, wParam, lParam, ref handled);
                if (handled)
                    return retval;

                return User32.CallWindowProc(_originalWndProc, hwnd, msg, wParam, lParam);
            }
        }

        /// <summary>
        /// Reverts ProcessMessage to the original one
        /// </summary>
        private void revertWndProc()
        {
            Debug.Assert(_originalWndProc != IntPtr.Zero, "Window hook was not installed");            
            
            User32.SetWindowLong(_control.Handle, User32.GWL_WNDPROC, _originalWndProc);
            
            _control.HandleDestroyed -= control_HandleDestroyed;
            _originalWndProc = IntPtr.Zero;

            OnHandleDestroyed(EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when window handle has been destroyed and hook detached
        /// </summary>
        /// <param name="e"></param>
        private void OnHandleDestroyed(EventArgs e)
        {
            var handle = HandleDestroyed;
            if (handle != null)
                handle(this, e);
        }
    }
}
