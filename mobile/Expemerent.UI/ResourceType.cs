using System;
using Expemerent.UI.Native;

namespace Expemerent.UI
{
    /// <summary>
    /// Resource type enumeration
    /// </summary>
    public enum ResourceType
    {
        /// <summary>
        /// Html resource
        /// </summary>
        Html = RESOURCE_TYPE.RT_DATA_HTML,

        /// <summary>
        /// Image resource
        /// </summary>
        Image = RESOURCE_TYPE.RT_DATA_IMAGE,

        /// <summary>
        /// Style resource
        /// </summary>
        Style = RESOURCE_TYPE.RT_DATA_STYLE,

        /// <summary>
        /// Cursor resource
        /// </summary>
        Cursor = RESOURCE_TYPE.RT_DATA_CURSOR,

        /// <summary>
        /// Script resource
        /// </summary>
        Script = RESOURCE_TYPE.RT_DATA_SCRIPT,
    }
}
