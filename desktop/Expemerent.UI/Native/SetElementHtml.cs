using System;
using System.Collections.Generic;
using System.Text;

namespace Expemerent.UI.Native
{
    /// <summary>
    /// Set inner or outer html of the element flags
    /// </summary>
    internal enum SET_ELEMENT_HTML 
    {
        SIH_REPLACE_CONTENT = 0,
        SIH_INSERT_AT_START = 1,
        SIH_APPEND_AFTER_LAST = 2,
        SOH_REPLACE = 3,
        SOH_INSERT_BEFORE = 4,
        SOH_INSERT_AFTER = 5
    };
}
