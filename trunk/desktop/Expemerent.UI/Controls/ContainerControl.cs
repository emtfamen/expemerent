using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using Expemerent.UI.Dom;

namespace Expemerent.UI.Controls
{
    /// <summary>
    /// Provides implementation for the "container" controls
    /// </summary>
    public class ContainerControl : BindableControl, ISciterControl
    {
        #region Private data
        /// <summary>
        /// See <see cref="SciterControls"/> property
        /// </summary>
        private ControlsCollection _sciterControls; 
        #endregion
        
        #region ISciterControl implementation
        /// <summary>
        /// Gets a root element fot this control (control itself)
        /// </summary>
        Element ISciterControl.RootElement
        {
            [DebuggerStepThrough]
            get { return Element; }
        }

        /// <summary>
        /// Gets a collection of components owned by the control
        /// </summary>
        public ControlsCollection SciterControls
        {
            [DebuggerStepThrough]
            get { return _sciterControls ?? (_sciterControls = new ControlsCollection(this)); }
        }

        /// <summary>
        /// Gets a parent windows forms control
        /// </summary>
        Control ISciterControl.Control
        {
            get { return Parent != null ? Parent.Control : null; }
        } 
        #endregion

        #region Internal implementation
        /// <summary>
        /// Forces control to update handle of the DOM element
        /// </summary>
        protected internal override void UpdateDomElement()
        {
            base.UpdateDomElement();

            if (_sciterControls != null)
                _sciterControls.UpdateElements();
        } 
        #endregion

        #region Dispose implementation
        /// <summary>
        /// Disposes resources used by the component
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && _sciterControls != null)
                _sciterControls.FreeElements();

            base.Dispose(disposing);
        } 
        #endregion
    }
}
