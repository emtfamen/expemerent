using System;
using System.Collections.Generic;
using System.Text;
using Expemerent.UI.Native;

namespace Expemerent.UI.Dom
{
    /// <summary>
    /// Popup placement
    /// </summary>
    public enum PopupPlacement
    {
        BelowAnchor = POPUP_PLACEMENT.BELOW_ANCHOR,
        AboveAnchor = POPUP_PLACEMENT.ABOVE_ANCHOR,
        LeftSideAnchor = POPUP_PLACEMENT.LEFT_SIDE_ANCHOR,
        RightSideAnchor = POPUP_PLACEMENT.RIGHT_SIDE_ANCHOR
    }
}
