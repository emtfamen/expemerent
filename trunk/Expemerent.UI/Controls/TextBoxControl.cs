using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Expemerent.UI.Behaviors;
using System.ComponentModel;

namespace Expemerent.UI.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class TextBoxControl : InputControl
    {
        /// <summary>
        /// Element text
        /// </summary>
        private string _text; 

        /// <summary>
        /// Creates a new instance of the <see cref="TextBoxControl"/> class
        /// </summary>
        public TextBoxControl()
        {
        }

        /// <summary>
        /// Occurs when button was pressed
        /// </summary>
        public event EventHandler TextChanged;

        /// <summary>
        /// Raises onclick event
        /// </summary>
        protected virtual void OnTextChanged(EventArgs e)
        {
            var handler = TextChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Gets or sets slider value 
        /// </summary>
        public String Text
        {
            [DebuggerStepThrough]
            get { return _text ?? String.Empty; }
            set 
            { 
                _text = value;
                var element = Element;
                if (element != null)
                {
                    element.Text = value;
                    element.Update();
                }
            }
        }

        /// <summary>
        /// Handles <see cref="SciterBehavior.OnAttached"/>
        /// </summary>
        protected override void OnAttached(ElementEventArgs e)
        {
            base.OnAttached(e);

            if (_text != null)
                Element.Text = _text;
            else
                _text = Element.Text;
        }

        /// <summary>
        /// Handles notifications from instrnic behaviors
        /// </summary>
        protected override void OnBehaviorEvent(BehaviorEventArgs e)
        {
            if (e.Phase == Phase.Bubbling && e.BehaviorEvent == BehaviorEventType.EditValueChanged)
            {
                _text = Element.Text;

                OnTextChanged(EventArgs.Empty);
                e.Handled = true;
            }

            base.OnBehaviorEvent(e);
        }
    }
}
