using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Expemerent.UI.Behaviors;
using System.ComponentModel;

namespace Expemerent.UI.Controls
{
    /// <summary>
    /// Provides a common implementation of TextBox 
    /// </summary>
    public class TextBoxControl : InputControl
    {
        /// <summary>
        /// Event key for the <see cref="TextChanged"/> event
        /// </summary>
        private readonly static object TextChangedEvent = new object();

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
        public event EventHandler TextChanged
        {
            add { Events.AddHandler(TextChangedEvent, value); }
            remove { Events.RemoveHandler(TextChangedEvent, value); }
        }

        /// <summary>
        /// Raises onclick event
        /// </summary>
        protected virtual void OnTextChanged(EventArgs e)
        {
            if (HasEvents)
            {
                var handler = (EventHandler)Events[TextChangedEvent];
                if (handler != null)
                    handler(this, e);
            }
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
            }

            base.OnBehaviorEvent(e);
        }
    }
}
