using System;
using System.Collections.Generic;
using System.Text;

namespace Expemerent.UI.Behaviors
{
    /// <summary>
    /// Simple 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class BehaviorAttribute : Attribute
    {
        public BehaviorAttribute(string behaviorName)
        {
            #region Arguments checking
            if (String.IsNullOrEmpty(behaviorName))
                throw new ArgumentNullException("behaviorName"); 
            #endregion

            BehaviorName = behaviorName;
        }

        /// <summary>
        /// Gets or sets name of the behavior
        /// </summary>
        public string BehaviorName { get; private set; }

        /// <summary>
        /// Gets or sets which events behavior should receive
        /// </summary>
        public EventGroups EventGroups { get; set; }
    }
}
