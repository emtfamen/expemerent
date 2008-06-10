using System;
using System.Collections.Generic;
using System.Text;
using Keys = System.Windows.Forms.Keys;
using Expemerent.UI.Dom;
using System.Diagnostics;

namespace Expemerent.UI.Behaviors.BuiltIn
{
    /// <summary>
    /// Supports accesskeys html element attribute
    /// </summary>
    /// <remarks>TODO: Should be fully synchronized with the behavior_accesskeys.cpp</remarks>
    [Behavior("accesskeys")]
    public class AccessKeys : SciterBehavior
    {    
        /// <summary>
        /// Specifies which notifications should be recieved by the behavior
        /// </summary>
        protected internal override EventGroups EventGroups
        {
            get { return EventGroups.Key; }
        }

        /// <summary>
        /// Handles <see cref="Key"/> event
        /// </summary>
        protected override void OnKey(KeyEventArgs e)
        {
            if (e.Phase == Phase.Sinking)
            {
                var keyname = default(string);
                if (e.KeyEventType == KeyEventType.KeyDown && (e.KeyboardState & KeyboardState.Alt) == 0)
                {
                    keyname = GetKeyName(e.KeyCode, e.KeyboardState);
                }
                else if (e.KeyEventType == KeyEventType.KeyChar && e.KeyboardState == KeyboardState.Alt)
                {
                    var chr = e.KeyChar;
                    keyname = chr != '\'' && chr != '\"' ? e.KeyChar.ToString() : null;
                }

                if (!String.IsNullOrEmpty(keyname))
                    e.Handled = ProcessKey(e.Element, keyname);
            }

            base.OnKey(e);
        }

        /// <summary>
        /// Dispatches key
        /// </summary>
        private bool ProcessKey(Element container, string keyname)
        {
            Debug.Assert(!String.IsNullOrEmpty(keyname), "keyname cannot be null");

            var selector = String.Format("[accesskey=='{0}'],[accesskey-alt=='{0}']", keyname);
            var element = container.Find(selector, e => e.IsVisible && e.IsEnabled);

            if (element != null)
            {
                if (element.CallBehaviorMethod(BehaviorMethods.DoClick))
                    return true;

                var parent = element.Parent;
                parent.SendEvent(BehaviorEventType.ActivateChild, element);
            }
            return false;   
        }

        /// <summary>
        /// Creates string name for the key code and key modifiers
        /// </summary>        
        private string GetKeyName(Keys keys, KeyboardState keyboardState)
        {
            var control = String.Empty;
            var shift = String.Empty;
            if ((keyboardState & KeyboardState.Control) == KeyboardState.Control)
                control = "^";
            if ((keyboardState & KeyboardState.Shift) == KeyboardState.Shift)
                shift = "!";

            return control + shift + keys.ToString();
        }        
    }
}
