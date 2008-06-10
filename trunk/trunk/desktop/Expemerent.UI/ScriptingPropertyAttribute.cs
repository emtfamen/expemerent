using System;
using System.Collections.Generic;
using System.Text;

namespace Expemerent.UI
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ScriptingPropertyAttribute : Attribute
    {
        /// <summary>
        /// Optional property name
        /// </summary>
        public string Name { get; set; }

    }
}
