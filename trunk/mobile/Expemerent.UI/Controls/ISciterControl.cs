using Expemerent.UI.Controls;
using System.Windows.Forms;
using Expemerent.UI.Dom;

namespace Expemerent.UI.Controls
{
    /// <summary>
    /// Defines interface for classes that can host Sciter controls
    /// </summary>
    public interface ISciterControl
    {
        /// <summary>
        /// Gets reference to the control root element 
        /// </summary>
        Element RootElement { get; }

        /// <summary>
        /// Gets a collection of components owned by the control
        /// </summary>
        ControlsCollection SciterControls { get; }

        /// <summary>
        /// Gets a parent windows forms control
        /// </summary>
        Control Control { get; }
    }
}
