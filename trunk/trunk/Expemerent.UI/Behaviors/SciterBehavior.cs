using System;
using System.Diagnostics;
using Expemerent.UI.Dom;
using Expemerent.UI.Native;
using System.ComponentModel;

namespace Expemerent.UI.Behaviors
{
    /// <summary>
    /// Base implementation of behavior interface
    /// </summary>
    public class SciterBehavior : ISciterBehavior
    {
        #region Event keys
        /// <summary>
        /// Handles attach event
        /// </summary>
        private readonly static object AttachedEvent = new object();

        /// <summary>
        /// Handles detach event
        /// </summary>
        private readonly static object DetachedEvent = new object();

        /// <summary>
        /// Handles timer event
        /// </summary>
        private readonly static object TimerEvent = new object();

        /// <summary>
        /// Handles size event
        /// </summary>
        private readonly static object SizeEvent = new object();

        /// <summary>
        /// Handles draw event
        /// </summary>
        private readonly static object DrawEvent = new object();

        /// <summary>
        /// Handles data arrived event
        /// </summary>
        private readonly static object DataArrivedEvent = new object();

        /// <summary>
        /// Handles method call
        /// </summary>
        private readonly static object MethodCallEvent = new object();

        /// <summary>
        /// Handles mouse event
        /// </summary>
        private readonly static object MouseEvent = new object();

        /// <summary>
        /// Handles keyboard event
        /// </summary>
        private readonly static object KeyEvent = new object();

        /// <summary>
        /// Handles scripting method
        /// </summary>
        private readonly static object ScriptingMethodCallEvent = new object();

        /// <summary>
        /// Handles behavior event
        /// </summary>
        private readonly static object BehaviorEventEvent = new object();

        /// <summary>
        /// Handles focus event
        /// </summary>
        private readonly static object FocusEvent = new object();
        #endregion

        #region Private data
        /// <summary>
        /// Subscriptions collection
        /// </summary>
        private EventHandlerList _events;

        /// <summary>
        /// Gets a subscriptions collection
        /// </summary>
        protected EventHandlerList Events
        {
            [DebuggerStepThrough]
            get { return _events ?? (_events = new EventHandlerList()); }
        }

        /// <summary>
        /// Gets a value indicating whether behavior have a subscribers
        /// </summary>
        protected bool HasEvents
        {
            [DebuggerStepThrough]
            get { return _events != null; }
        }

        /// <summary>
        /// Specifies which notifications should be recieved by the behavior
        /// </summary>
        protected internal virtual EventGroups EventGroups { [DebuggerStepThrough] get { return EventGroups.All; } }
        #endregion

        #region ISciterBehavior Members

        /// <summary>
        /// Handles behavior initialization
        /// </summary>
        void ISciterBehavior.ProcessAttach(ElementEventArgs e)
        {
            e.Handled = true;
            OnAttached(e);
        }

        /// <summary>
        /// Handles behavior initialization
        /// </summary>
        void ISciterBehavior.ProcessDettach(ElementEventArgs e)
        {
            e.Handled = true;
            OnDetached(e);
        }

        /// <summary>
        /// Handles mouse events
        /// </summary>
        void ISciterBehavior.ProcessMouse(MouseEventArgs e)
        {
            OnMouse(e);
        }

        /// <summary>
        /// Handles keyboard events
        /// </summary>
        void ISciterBehavior.ProcessKey(KeyEventArgs e)
        {
            OnKey(e);
        }

        /// <summary>
        /// Handles focus events
        /// </summary>
        void ISciterBehavior.ProcessFocus(FocusEventArgs e)
        {
            OnFocus(e);
        }

        /// <summary>
        /// Handles draw events
        /// </summary>
        void ISciterBehavior.ProcessDraw(DrawEventArgs e)
        {
            OnDraw(e);
        }

        /// <summary>
        /// Handles timer event
        /// </summary>
        void ISciterBehavior.ProcessTimer(ElementEventArgs e)
        {
            OnTimer(e);
        }

        /// <summary>
        /// Handles secondary behavior event
        /// </summary>
        void ISciterBehavior.ProcessBehaviorEvent(BehaviorEventArgs e)
        {
            OnBehaviorEvent(e);
        }

        /// <summary>
        /// Handles method call event
        /// </summary>
        void ISciterBehavior.ProcessMethodCall(MethodCallEventArgs e)
        {
            OnMethodCall(e);
        }

        /// <summary>
        /// Handles data arrived event
        /// </summary>
        void ISciterBehavior.ProcessDataArrived(DataArrivedEventArgs e)
        {
            OnDataArrived(e);
        }

        /// <summary>
        /// Handles size changes
        /// </summary>
        void ISciterBehavior.ProcessSize(ElementEventArgs e)
        {
            OnSize(e);
        }

        /// <summary>
        /// Handles scripting calls
        /// </summary>
        void ISciterBehavior.ProcessScriptingMethodCall(ScriptingMethodCallEventArgs e)
        {
            OnScriptingMethodCall(e);
        }

        /// <summary>
        /// Handles scroll
        /// </summary>
        void ISciterBehavior.ProcessScroll(ElementEventArgs e)
        {
            e.Handled = false;
        }

        #endregion

        #region Public events
        /// <summary>
        /// Handles attach event
        /// </summary>
        public event EventHandler<ElementEventArgs> Attached
        {
            add { Events.AddHandler(AttachedEvent, value); }
            remove { Events.RemoveHandler(AttachedEvent, value); }
        }

        /// <summary>
        /// Handles detach event
        /// </summary>
        public event EventHandler<ElementEventArgs> Detached
        {
            add { Events.AddHandler(DetachedEvent, value); }
            remove { Events.RemoveHandler(DetachedEvent, value); }
        }

        /// <summary>
        /// Handles timer event
        /// </summary>
        public event EventHandler<ElementEventArgs> Timer
        {
            add { Events.AddHandler(TimerEvent, value); }
            remove { Events.RemoveHandler(TimerEvent, value); }
        }

        /// <summary>
        /// Handles size event
        /// </summary>
        public event EventHandler<ElementEventArgs> Size
        {
            add { Events.AddHandler(SizeEvent, value); }
            remove { Events.RemoveHandler(SizeEvent, value); }
        }

        /// <summary>
        /// Handles draw event
        /// </summary>
        public event EventHandler<DrawEventArgs> Draw
        {
            add { Events.AddHandler(DrawEvent, value); }
            remove { Events.RemoveHandler(DrawEvent, value); }
        }

        /// <summary>
        /// Handles data arrived event
        /// </summary>
        public event EventHandler<DataArrivedEventArgs> DataArrived
        {
            add { Events.AddHandler(DataArrivedEvent, value); }
            remove { Events.RemoveHandler(DataArrivedEvent, value); }
        }

        /// <summary>
        /// Handles method call
        /// </summary>
        public event EventHandler<MethodCallEventArgs> MethodCall
        {
            add { Events.AddHandler(MethodCallEvent, value); }
            remove { Events.RemoveHandler(MethodCallEvent, value); }
        }

        /// <summary>
        /// Handles mouse event
        /// </summary>
        public event EventHandler<MouseEventArgs> Mouse
        {
            add { Events.AddHandler(MouseEvent, value); }
            remove { Events.RemoveHandler(MouseEvent, value); }
        }

        /// <summary>
        /// Handles keyboard event
        /// </summary>
        public event EventHandler<KeyEventArgs> Key
        {
            add { Events.AddHandler(KeyEvent, value); }
            remove { Events.RemoveHandler(KeyEvent, value); }
        }

        /// <summary>
        /// Handles scripting method
        /// </summary>
        public event EventHandler<ScriptingMethodCallEventArgs> ScriptingMethodCall
        {
            add { Events.AddHandler(ScriptingMethodCallEvent, value); }
            remove { Events.RemoveHandler(ScriptingMethodCallEvent, value); }
        }

        /// <summary>
        /// Handles behavior event
        /// </summary>
        public event EventHandler<BehaviorEventArgs> BehaviorEvent
        {
            add { Events.AddHandler(BehaviorEventEvent, value); }
            remove { Events.RemoveHandler(BehaviorEventEvent, value); }
        }

        /// <summary>
        /// Handles focus event
        /// </summary>
        public event EventHandler<FocusEventArgs> Focus
        {
            add { Events.AddHandler(FocusEvent, value); }
            remove { Events.RemoveHandler(FocusEvent, value); }
        }

        #endregion

        #region Protected implementation
        /// <summary>
        /// Raises <see cref="Attached"/> event
        /// </summary>
        protected virtual void OnAttached(ElementEventArgs e)
        {
            if (HasEvents)
            {
                var handler = (EventHandler<ElementEventArgs>)Events[AttachedEvent];
                if (handler != null)
                {
                    e.Handled = true;
                    handler(this, e);
                }
            }
        }

        /// <summary>
        /// Raises <see cref="Detached"/> event
        /// </summary>
        protected virtual void OnDetached(ElementEventArgs e)
        {
            if (HasEvents)
            {
                var handler = (EventHandler<ElementEventArgs>)Events[DetachedEvent];
                if (handler != null)
                {
                    e.Handled = true;
                    handler(this, e);
                }
            }
        }

        /// <summary>
        /// Raises <see cref="Mouse"/> event
        /// </summary>
        protected virtual void OnMouse(MouseEventArgs e)
        {
            if (HasEvents)
            {
                var handler = (EventHandler<MouseEventArgs>)Events[MouseEvent];
                if (handler != null)
                {
                    e.Handled = true;
                    handler(this, e);
                }
            }
        }

        /// <summary>
        /// Raises <see cref="Timer"/> event
        /// </summary>
        protected virtual void OnTimer(ElementEventArgs e)
        {
            if (HasEvents)
            {
                var handler = (EventHandler<ElementEventArgs>)Events[TimerEvent];
                if (handler != null)
                {
                    e.Handled = true;
                    handler(this, e);
                }
            }
        }

        /// <summary>
        /// Raises <see cref="Draw"/> event
        /// </summary>
        protected virtual void OnDraw(DrawEventArgs e)
        {
            if (HasEvents)
            {
                var handler = (EventHandler<DrawEventArgs>)Events[DrawEvent];
                if (handler != null)
                {
                    e.Handled = true;
                    handler(this, e);
                }
            }
        }

        /// <summary>
        /// Raises <see cref="Focus"/> event
        /// </summary>
        protected virtual void OnFocus(FocusEventArgs e)
        {
            if (HasEvents)
            {
                var handler = (EventHandler<FocusEventArgs>)Events[FocusEvent];
                if (handler != null)
                {
                    e.Handled = true;
                    handler(this, e);
                }
            }
        }

        /// <summary>
        /// Raises <see cref="Key"/> event
        /// </summary>
        protected virtual void OnKey(KeyEventArgs e)
        {
            if (HasEvents)
            {
                var handler = (EventHandler<KeyEventArgs>)Events[KeyEvent];
                if (handler != null)
                {
                    e.Handled = true;
                    handler(this, e);
                }
            }
        }

        /// <summary>
        /// Raises <see cref="ScriptingMethodCall"/> event
        /// </summary>
        protected virtual void OnScriptingMethodCall(ScriptingMethodCallEventArgs e)
        {
            if (HasEvents)
            {
                var handler = (EventHandler<ScriptingMethodCallEventArgs>)Events[ScriptingMethodCallEvent];
                if (handler != null)
                {
                    e.Handled = true;
                    handler(this, e);
                }
            }
        }

        /// <summary>
        /// Raises <see cref="BehaviorEvent"/> event
        /// </summary>
        protected virtual void OnBehaviorEvent(BehaviorEventArgs e)
        {
            if (HasEvents)
            {
                var handler = (EventHandler<BehaviorEventArgs>)Events[BehaviorEventEvent];
                if (handler != null)
                {
                    e.Handled = true;
                    handler(this, e);
                }
            }
        }

        /// <summary>
        /// Raises <see cref="MethodCall"/> event
        /// </summary>
        protected virtual void OnMethodCall(MethodCallEventArgs e)
        {
            if (HasEvents)
            {
                var handler = (EventHandler<MethodCallEventArgs>)Events[MethodCallEvent];
                if (handler != null)
                {
                    e.Handled = true;
                    handler(this, e);
                }
            }
        }

        /// <summary>
        /// Raises <see cref="Size"/> event
        /// </summary>
        protected virtual void OnSize(ElementEventArgs e)
        {
            if (HasEvents)
            {
                var handler = (EventHandler<ElementEventArgs>)Events[SizeEvent];
                if (handler != null)
                {
                    e.Handled = true;
                    handler(this, e);
                }
            }
        }

        /// <summary>
        /// Raises <see cref="DataArrived"/> event
        /// </summary>
        protected virtual void OnDataArrived(DataArrivedEventArgs e)
        {
            if (HasEvents)
            {
                var handler = (EventHandler<DataArrivedEventArgs>)Events[DataArrivedEvent];
                if (handler != null)
                {
                    e.Handled = true;
                    handler(this, e);
                }
            }
        }
        #endregion
    }
}