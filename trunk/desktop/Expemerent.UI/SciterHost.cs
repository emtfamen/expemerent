using System;
using System.Diagnostics;
using Expemerent.UI.Behaviors;

namespace Expemerent.UI
{
    public class SciterHost : SciterBehavior, ISciterHost
    {
        #region Event keys
        /// <summary>
        /// Event key for the <see cref="Destroyed"/> event
        /// </summary>
        private readonly static object DestroyedEvent = new object();

        /// <summary>
        /// Event key for the <see cref="DocumentComplete"/> event
        /// </summary>
        private readonly static object DocumentCompleteEvent = new object();

        /// <summary>
        /// Event key for the <see cref="LoadData"/> event
        /// </summary>
        private readonly static object LoadDataEvent = new object();

        /// <summary>
        /// Event key for the <see cref="DataLoaded"/> event
        /// </summary>
        private readonly static object DataLoadedEvent = new object();

        /// <summary>
        /// Event key for the <see cref="AttachBehavior"/> event
        /// </summary>
        private readonly static object AttachBehaviorEvent = new object();

        /// <summary>
        /// Event key for the <see cref="CallbackHost"/> event
        /// </summary>
        private readonly static object CallbackHostEvent = new object();
        #endregion

        #region Public events
        /// <summary>
        /// Occurs when the <see cref="SciterView"/> going to be destroyed
        /// </summary>
        public event EventHandler Destroyed
        {
            add { Events.AddHandler(DestroyedEvent, value); }
            remove { Events.RemoveHandler(DestroyedEvent, value); }
        }

        /// <summary>
        /// Occurs when the document with all external data has been loaded
        /// </summary>
        public event EventHandler<DocumentCompleteEventArgs> DocumentComplete
        {
            add { Events.AddHandler(DocumentCompleteEvent, value); }
            remove { Events.RemoveHandler(DocumentCompleteEvent, value); }
        }

        /// <summary>
        /// Occurs when the sciter is about to download a referred resource. 
        /// </summary>
        public event EventHandler<LoadDataEventArgs> LoadData
        {
            add { Events.AddHandler(LoadDataEvent, value); }
            remove { Events.RemoveHandler(LoadDataEvent, value); }
        }

        /// <summary>
        /// Occurs when download process completed 
        /// </summary>
        public event EventHandler<DataLoadedEventArgs> DataLoaded
        {
            add { Events.AddHandler(DataLoadedEvent, value); }
            remove { Events.RemoveHandler(DataLoadedEvent, value); }
        }

        /// <summary>
        /// Occurs when behavior should be attached to the DOM element
        /// </summary>
        public event EventHandler<AttachBehaviorEventArgs> AttachBehavior
        {
            add { Events.AddHandler(AttachBehaviorEvent, value); }
            remove { Events.RemoveHandler(AttachBehaviorEvent, value); }
        }

        /// <summary>
        /// Occurs when sciter wants to notify host
        /// </summary>
        public event EventHandler<CallbackHostEventArgs> CallbackHost
        {
            add { Events.AddHandler(CallbackHostEvent, value); }
            remove { Events.RemoveHandler(CallbackHostEvent, value); }
        }
        #endregion

        #region Protected implementation
        /// <summary>
        /// Handles download process complete
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnProcessDataLoaded(DataLoadedEventArgs e)
        {
            if (HasEvents)
            {
                var handler = (EventHandler<DataLoadedEventArgs>)Events[DataLoadedEvent];
                if (handler != null)
                {
                    handler(this, e);
                }
            }
        }

        /// <summary>
        /// Handles all external data has been load
        /// </summary>
        protected virtual void OnProcessDocumentComplete(DocumentCompleteEventArgs e)
        {
            if (HasEvents)
            {
                var handler = (EventHandler<DocumentCompleteEventArgs>)Events[DocumentCompleteEvent];
                if (handler != null)
                {
                    handler(this, e);
                }
            }
        }

        /// <summary>
        /// Sciter is about to download a referred resource. 
        /// </summary>
        protected virtual void OnProcessLoadData(LoadDataEventArgs e)
        {
            if (HasEvents)
            {
                var handler = (EventHandler<LoadDataEventArgs>)Events[LoadDataEvent];
                if (handler != null)
                {
                    handler(this, e);
                }
            }
        }

        /// <summary>
        /// Handles behavior attach events
        /// </summary>
        protected virtual void OnProcessAttachBehavior(AttachBehaviorEventArgs e)
        {
            if (HasEvents)
            {
                var handler = (EventHandler<AttachBehaviorEventArgs>)Events[AttachBehaviorEvent];
                if (handler != null)
                {
                    handler(this, e);
                }
            }
        }

        /// <summary>
        /// Handles sciter notifications
        /// </summary>
        protected virtual void OnProcessCallbackHost(CallbackHostEventArgs e)
        {
            Debug.WriteLine(String.Format("Host event: [{0}, {1}]", e.First, e.Second));

            if (HasEvents)
            {
                var handler = (EventHandler<CallbackHostEventArgs>)Events[CallbackHostEvent];
                if (handler != null)
                {
                    handler(this, e);
                }
            }
        }

        /// <summary>
        /// Handles destroyed event
        /// </summary>
        protected virtual void OnProcessDestroyed(EventArgs e)
        {
            if (HasEvents)
            {
                var handler = (EventHandler)Events[DestroyedEvent];
                if (handler != null)
                {
                    handler(this, e);
                }
            }
        }

        /// <summary>
        /// Raises <see cref="Destroyed"/> event
        /// </summary>
        void ISciterHost.ProcessDestroyed(EventArgs e)
        {
            if (HasEvents)
            {
                var handler = (EventHandler)Events[DestroyedEvent];
                if (handler != null)
                {
                    handler(this, e);
                }
            }
        }
        
        #endregion

        #region ISciterHost implementation
        /// <summary>
        /// Occurs when all external data has been loaded
        /// </summary>
        void ISciterNotifications.ProcessDocumentComplete(DocumentCompleteEventArgs e)
        {
            OnProcessDocumentComplete(e);
        }

        /// <summary>
        /// Occurs when the sciter is about to download a referred resource. 
        /// </summary>
        void ISciterNotifications.ProcessLoadData(LoadDataEventArgs e)
        {
            OnProcessLoadData(e);
        }

        /// <summary>
        /// Occurs when download process completed 
        /// </summary>
        void ISciterNotifications.ProcessDataLoaded(DataLoadedEventArgs e)
        {
            OnProcessDataLoaded(e);
        }

        /// <summary>
        /// Occurs when behavior should be attached to the DOM element
        /// </summary>
        void ISciterNotifications.ProcessAttachBehavior(AttachBehaviorEventArgs e)
        {
            OnProcessAttachBehavior(e);
        }

        /// <summary>
        /// Occurs when sciter wants to notify host
        /// </summary>
        void ISciterNotifications.ProcessCallbackHost(CallbackHostEventArgs e)
        {
            OnProcessCallbackHost(e);
        }  
        #endregion
    }
}
