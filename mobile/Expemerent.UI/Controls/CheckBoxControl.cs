using System;
using System.Collections.Generic;
using System.Text;
using Expemerent.UI.Behaviors;
using Expemerent.UI.Dom;
using System.Diagnostics;

namespace Expemerent.UI.Controls
{
    /// <summary>
    /// Control interface to checkbox/radio buttons behavior
    /// </summary>
    public class CheckBoxControl : InputControl
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly static object CheckedChangedEvent = new object();

        /// <summary>
        /// Creates a new instance of the <see cref="InputControl"/> class
        /// </summary>
        public CheckBoxControl()
        {
        }

        /// <summary>
        /// Occurs when button was pressed
        /// </summary>
        public event EventHandler CheckedChanged
        {
            add { Events.AddHandler(CheckedChangedEvent, value); }
            remove { Events.RemoveHandler(CheckedChangedEvent, value); }
        }

        /// <summary>
        /// Raises onclick event
        /// </summary>
        protected virtual void OnCheckedChanged(EventArgs e)
        {
            if (HasEvents)
            {
                var handler = (EventHandler)Events[CheckedChangedEvent];
                if (handler != null)
                    handler(this, e);
            }
        }

        /// <summary>
        /// Gets or sets checked state
        /// </summary>
        public bool Checked
        {
            [DebuggerStepThrough]
            get { return (GetElementState() & ElementState.Checked) == ElementState.Checked; }
            set 
            {
                var stateToSet = value ? ElementState.Checked : 0;
                var stateToClear = value ? 0 : ElementState.Checked;
                
                SetElementState(stateToSet, stateToClear); 
                OnCheckedChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles notifications from instrnic behaviors
        /// </summary>
        protected override void OnBehaviorEvent(BehaviorEventArgs e)
        {
            base.OnBehaviorEvent(e);

            if (e.Phase == Phase.Bubbling && e.BehaviorEvent == BehaviorEventType.ButtonStateChanged)
            {
                OnCheckedChanged(EventArgs.Empty);
                e.Handled = true;
            }   
        }
    }
}
