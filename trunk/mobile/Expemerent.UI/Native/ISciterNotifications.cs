using System;

namespace Expemerent.UI.Native
{
    /// <summary>
    /// Defines all notification messages sent by sciter
    /// </summary>
    internal interface ISciterNotifications
    {
        /// <summary>
        /// Occurs when all external data has been loaded
        /// </summary>
        void FireDocumentComplete(DocumentCompleteEventArgs e);

        /// <summary>
        /// Occurs when the sciter is about to download a referred resource. 
        /// </summary>
        void FireLoadData(LoadDataEventArgs e);

        /// <summary>
        /// Occurs when download process completed 
        /// </summary>
        void FireDataLoaded(DataLoadedEventArgs e);

        /// <summary>
        /// Occurs when behavior should be attached to the DOM element
        /// </summary>
        void FireAttachBehavior(AttachBehaviorEventArgs e);

        /// <summary>
        /// Occurs when sciter wants to notify host
        /// </summary>
        /// <param name="e"></param>
        void FireCallbackHost(CallbackHostEventArgs e);
    }
}