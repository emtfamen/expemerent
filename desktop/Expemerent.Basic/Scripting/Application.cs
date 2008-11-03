using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Expemerent.UI;

namespace Expemerent.Basic.Scripting
{
    [ScriptingClass(Name = "Application", IsStatic = false)]
    public class Application
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public Application()
        {
        }

        [ScriptingMethod]
        public string Concat(string str1, string str2)
        {
            return str1 + ":" + str2;
        }

        [ScriptingProperty]
        public string Name
        {
            get { return this.GetType().Name; }
        }
    }
}
