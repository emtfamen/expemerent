using System;
using System.Collections.Generic;
using System.Text;
using Expemerent.UI.Behaviors;
using System.Diagnostics;

namespace Expemerent.UI.Controls
{
    /// <summary>
    /// Base implementation of the Button control
    /// </summary>
    public class ButtonControl : InputControl
    {
        /// <summary>
        /// Event key for the <see cref="Click"/> event
        /// </summary>
        private readonly static object ClickEvent = new object();

        /// <summary>
        /// Creates a new instance of the <see cref="ButtonControl"/> class
        /// </summary>
        public ButtonControl()
        {
        }

        /// <summary>
        /// Occurs when button was pressed
        /// </summary>
        public event EventHandler Click
        {
            add { Events.AddHandler(ClickEvent, value); }
            remove { Events.RemoveHandler(ClickEvent, value); }
        }

        /// <summary>
        /// Raises onclick event
        /// </summary>
        protected virtual void OnClick(EventArgs e)
        {
            if (HasEvents)
            {
                var handler = (EventHandler)Events[ClickEvent];
                if (handler != null)
                    handler(this, e);
            }
        }

        /// <summary>
        /// Handles notifications from instrnic behaviors
        /// </summary>
        protected override void OnBehaviorEvent(BehaviorEventArgs e)
        {
            if (e.Phase == Phase.Bubbling && e.BehaviorEvent == BehaviorEventType.ButtonPress)
            {
                OnClick(EventArgs.Empty);
            }

            base.OnBehaviorEvent(e);
        }
    }
}
