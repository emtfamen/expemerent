using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;

namespace Expemerent.UI.Native
{
    #region Native data types for scripting support
    /// <summary>
    /// Native representation of the scripting class
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal class SciterNativeClassDef
    {
        public static readonly int SizeOf = Marshal.SizeOf(typeof(SciterNativeClassDef));

        public IntPtr name;
        public IntPtr methods;
        public IntPtr properties;
        public IntPtr dtor;
    }
    #endregion

    /// <summary>
    /// Interface to the scripting VM
    /// </summary>
    internal class Scripting
    {
        #region Public interface
        /// <summary>
        /// Registers scripting class
        /// </summary>
        public static void RegisterClass<TType>(SciterView view)
        {
            
        } 
        #endregion
    }
}