using System;
using System.Collections.Generic;
using System.Text;
using Expemerent.UI.Behaviors;
using System.Diagnostics;

namespace Expemerent.UI.Controls
{
    /// <summary>
    /// Control interface to button behavior
    /// </summary>
    public class ButtonControl : InputControl
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ButtonControl"/> class
        /// </summary>
        public ButtonControl()
        {
        }

        /// <summary>
        /// Occurs when button was pressed
        /// </summary>
        public event EventHandler Click;

        /// <summary>
        /// Raises onclick event
        /// </summary>
        protected virtual void OnClick(EventArgs e)
        {
            var handler = Click;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Handles notifications from instrnic behaviors
        /// </summary>
        protected override void OnBehaviorEvent(BehaviorEventArgs e)
        {
            if (e.Phase == Phase.Bubbling && e.BehaviorEvent == BehaviorEventType.ButtonPress)
            {
                OnClick(EventArgs.Empty);
                e.Handled = true;
            }

            base.OnBehaviorEvent(e);
        }
    }
}
