using System;

namespace Expemerent.UI
{
    /// <summary>
    /// Defines host interface to interact with <see cref="SciterView"/>
    /// </summary>
    public interface ISciterHost : ISciterNotifications
    {
        /// <summary>
        /// Gets HWND handle of the host window
        /// </summary>
        IntPtr Handle { get; }

        /// <summary>
        /// Raises <see cref="Destroyed"/> event
        /// </summary>
        void ProcessDestroyed(EventArgs e);
    }
}
