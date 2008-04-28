using System;
using System.Text;

namespace Expemerent.UI
{
    /// <summary>
    /// Evaluates value
    /// </summary>
    internal delegate TValue LazyValueCallback<TValue>();

    /// <summary>
    /// Defines structure with lazy evaluated value
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    internal struct LazyValue<TValue> where TValue : class
    {
        /// <summary>
        /// Callback to evaluate value
        /// </summary>
        private LazyValueCallback<TValue> _lazyValue;

        /// <summary>
        /// Value holder
        /// </summary>
        private TValue _value;

        /// <summary>
        /// Gets contained value
        /// </summary>
        public TValue Value 
        { 
            get 
            {
                if (_value == null && _lazyValue != null)
                {
                    _value = _lazyValue();
                    _lazyValue = null;
                }

                return _value;
            }
            set
            {
                _value = value;
                _lazyValue = null;
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="LazyValue"/> object
        /// </summary>
        public LazyValue(LazyValueCallback<TValue> lazyValue)
        {
            _value = default(TValue);
            _lazyValue = lazyValue;
        }
    }
}
