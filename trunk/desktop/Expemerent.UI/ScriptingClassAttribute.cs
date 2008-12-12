using System;

namespace Expemerent.UI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ScriptingClassAttribute : Attribute
    {
        /// <summary>
        /// Optional class name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Class should be static
        /// </summary>
        public bool IsStatic { get; set; }
    }
}
