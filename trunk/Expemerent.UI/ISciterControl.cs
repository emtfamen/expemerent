using Expemerent.UI.Controls;
using System.Windows.Forms;

namespace Expemerent.UI
{
    /// <summary>
    /// Defines interface to the Windows.Forms controls with sciter content
    /// </summary>
    public interface ISciterControl
    {
        /// <summary>
        /// Gets View instance for this control
        /// </summary>
        SciterView View { get; }

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
