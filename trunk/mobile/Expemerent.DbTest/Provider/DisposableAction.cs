using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expemerent.DbTest.Provider
{
    /// <summary>
    /// Delays <see cref="Action"/> till <see cref="IDisposable.Dispose"/>  call
    /// </summary>
    internal class DisposableAction : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly Action _action;

        /// <summary>
        /// Crates a new instance of the <see cref="DisposableAction"/> class
        /// </summary>
        public DisposableAction(Action action)
        {
            #region Arguments checking
            if (action == null)
                throw new ArgumentNullException("action");            
            #endregion

            _action = IgnoreSubsequentCalls(action);
        }

        /// <summary>
        /// Invokes stored <see cref="Action"/>
        /// </summary>
        void IDisposable.Dispose()
        {
            _action();
        }

        /// <summary>
        /// Only the first call will be passed to the <param name="action">action</param>
        /// </summary>
        public static Action IgnoreSubsequentCalls(Action action)
        {
            #region Arguments checking
            if (action == null)
                throw new ArgumentNullException("action");
            #endregion

            return delegate
            {
                if (action != null)
                    action();

                action = null;
            };
        }

        /// <summary>
        /// Only the first call will be passed to the <param name="action">action</param>
        /// </summary>
        public static Action DisableSubsequentCalls(Action action)
        {
            #region Arguments checking
            if (action == null)
                throw new ArgumentNullException("action");
            #endregion

            return delegate
            {
                if (action != null)
                    action();
                else
                    throw new InvalidOperationException("Subsequent calls to the delegate disabled");

                action = null;
            };
        }
    }
}
