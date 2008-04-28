using System;
using System.Collections.Generic;
using System.Text;

namespace Expemerent.UI
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ScriptingMethodAttribute : Attribute
    {
        /// <summary>
        /// Optional method name
        /// </summary>
        public string Name { get; set; }
    }
}
