using System;

namespace Expemerent.UI
{
    /// <summary>
    /// Defines all notification messages sent by sciter
    /// </summary>
    public interface ISciterNotifications
    {
        /// <summary>
        /// Occurs when all external data has been loaded
        /// </summary>
        void ProcessDocumentComplete(DocumentCompleteEventArgs e);

        /// <summary>
        /// Occurs when the sciter is about to download a referred resource. 
        /// </summary>
        void ProcessLoadData(LoadDataEventArgs e);

        /// <summary>
        /// Occurs when download process completed 
        /// </summary>
        void ProcessDataLoaded(DataLoadedEventArgs e);

        /// <summary>
        /// Occurs when behavior should be attached to the DOM element
        /// </summary>
        void ProcessAttachBehavior(AttachBehaviorEventArgs e);

        /// <summary>
        /// Occurs when sciter wants to notify host
        /// </summary>
        void ProcessCallbackHost(CallbackHostEventArgs e);
    }
}