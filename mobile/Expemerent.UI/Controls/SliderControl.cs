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
        /// Creates a new instance of the <see cref="SliderControl"/> class
        /// </summary>
        public SliderControl()
        {
        }

        /// <summary>
        /// Occurs when button was pressed
        /// </summary>
        public event EventHandler ValueChanged;

        /// <summary>
        /// Raises onclick event
        /// </summary>
        protected virtual void OnValueChanged(EventArgs e)
        {
            var handler = ValueChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Gets or sets slider value 
        /// </summary>
        public int Value
        {
            get 
            {
                var value = GetAttribute("value");
                if (!String.IsNullOrEmpty(value))
                    return Int32.Parse(value);
                
                return 0;
            }
            set { SetAttribute("value", value.ToString()); }
        }

        /// <summary>
        /// Handles notifications from instrnic behaviors
        /// </summary>
        protected override void OnBehaviorEvent(BehaviorEventArgs e)
        {
            if (e.Phase == Phase.Bubbling && e.BehaviorEvent == BehaviorEventType.ButtonStateChanged)
            {
                OnValueChanged(EventArgs.Empty);
                e.Handled = true;
            }

            base.OnBehaviorEvent(e);
        }
    }
}
