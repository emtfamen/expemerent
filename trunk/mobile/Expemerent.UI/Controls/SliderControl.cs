using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Expemerent.UI.Behaviors;

namespace Expemerent.UI.Controls
{
    public class SliderControl : InputControl    
    {
        /// <summary>
        /// Event key for the <see cref="ValueChanged"/> event
        /// </summary>
        private static readonly object ValueChangedEvent = new object();

        /// <summary>
        /// Creates a new instance of the <see cref="SliderControl"/> class
        /// </summary>
        public SliderControl()
        {
        }

        /// <summary>
        /// Occurs when button was pressed
        /// </summary>
        public event EventHandler ValueChanged
        {
            add { Events.AddHandler(ValueChangedEvent, value); }
            remove { Events.RemoveHandler(ValueChangedEvent, value); }
        }

        /// <summary>
        /// Raises onclick event
        /// </summary>
        protected virtual void OnValueChanged(EventArgs e)
        {
            if (HasEvents)
            {
                var handler = (EventHandler)Events[ValueChangedEvent];
                if (handler != null)
                    handler(this, e);
            }
        }

        /// <summary>
        /// Gets or sets slider value 
        /// </summary>
        public int Value
        {
            get 
            {
                var value = Attributes["value"];
                if (!String.IsNullOrEmpty(value))
                    return Int32.Parse(value);
                
                return 0;
            }
            set { Attributes["value"] = value.ToString(); }
        }

        /// <summary>
        /// Handles notifications from instrnic behaviors
        /// </summary>
        protected override void OnBehaviorEvent(BehaviorEventArgs e)
        {
            if (e.Phase == Phase.Bubbling && e.BehaviorEvent == BehaviorEventType.ButtonStateChanged)
            {
                OnValueChanged(EventArgs.Empty);
            }

            base.OnBehaviorEvent(e);
        }
    }
}
