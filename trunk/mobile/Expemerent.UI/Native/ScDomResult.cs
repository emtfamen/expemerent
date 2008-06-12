using System;
using System.Collections.Generic;
using System.Text;

namespace Expemerent.UI.Native
{
    /// <summary>
    /// Result value for Sciter DOM functions
    /// </summary>
    internal enum ScDomResult : int
    {
        /// <summary>
        /// Function completed successfully
        /// </summary>
        SCDOM_OK = 0,

        /// <summary>
        /// Invalid IntPtr
        /// </summary>
        SCDOM_INVALID_HWND = 1,

        /// <summary>
        /// Invalid IntPtr
        /// </summary>
        SCDOM_INVALID_HANDLE = 2,

        /// <summary>
        /// Attempt to use IntPtr which is not marked by Sciter_UseElement()
        /// </summary>
        SCDOM_PASSIVE_HANDLE = 3,

        /// <summary>
        /// Parameter is invalid, e.g. pointer is null
        /// </summary>
        SCDOM_INVALID_PARAMETER = 4,

        /// <summary>
        /// Operation failed, e.g. invalid html in #SciterSetElementHtml()
        /// </summary>
        SCDOM_OPERATION_FAILED = 5,

        /// <summary>
        /// ???
        /// </summary>
        SCDOM_OK_NOT_HANDLED = (-1)
    }
}
