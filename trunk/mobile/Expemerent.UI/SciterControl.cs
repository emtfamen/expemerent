using System;
using System.Windows.Forms;
using System.Diagnostics;
using Expemerent.UI.Controls;

namespace Expemerent.UI
{
    /// <summary>
    /// Default implementation of <see cref="Form"/> with sciter content
    /// </summary>
    public class SciterControl : Control, ISciterControl
    {
        /// <summary>
        /// See <see cref="View"/> property
        /// </summary>
        private SciterView _view;

        /// <summary>
        /// See <see cref="SciterControls"/> collection
        /// </summary>
        private ControlsCollection _sciterControls;

        /// <summary>
        /// Gets sciter view
        /// </summary>
        public SciterView View
        {
            [DebuggerStepThrough]
            get { return _view; }
        }

        /// <summary>
        /// Gets a collection of sciter controls
        /// </summary>
        public ControlsCollection SciterControls
        {
            [DebuggerStepThrough]
            get { return _sciterControls ?? (_sciterControls = new ControlsCollection(this)); }
        }

        /// <summary>
        /// Gets parent windows forms control
        /// </summary>
        public Control Control
        {
            [DebuggerStepThrough]
            get { return this; }
        }

        /// <summary>
        /// Handles <see cref="HandleCreated"/> event
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            if (Site == null || !Site.DesignMode)
                _view = SciterView.Attach(this);

            base.OnHandleCreated(e);
        }

        /// <summary>
        /// Disposes used resources
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && _sciterControls != null)
                _sciterControls.FreeElements();

            base.Dispose(disposing);
        }
    }
}
