using System;
using System.ComponentModel;

namespace Expemerent.UI.Controls
{
    /// <summary>
    /// Utility methods type conversion operations
    /// </summary>
    internal static class ConvertUtility
    {
        /// <summary>
        /// Gets property value from the object and converts it to stirng
        /// </summary>
        /// <remarks>
        /// NOTE: CompactFramework do not support type converters
        /// </remarks>
        public static string GetValue(PropertyDescriptor descr, object item)
        {
            var val = descr.GetValue(item);
            return val != null ? descr.Converter.ConvertToString(val) : String.Empty;
        }
    }
}
