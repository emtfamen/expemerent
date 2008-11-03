using System;
using System.Collections.Generic;
using System.Text;

using Expemerent.UI.Native;

namespace Expemerent.UI.Dom
{
    /// <summary>
    /// Possible types of the element location
    /// </summary>
    [Flags]
    public enum ElementLocation
    {
        /// <summary>
        /// or this flag if you want to get Sciter window relative coordinates,
        /// otherwise it will use nearest windowed container e.g. popup window.
        /// </summary>
        RootRelative = ELEMENT_AREA.ROOT_RELATIVE, 

        /// <summary>
        /// "or" this flag if you want to get coordinates relative to the origin
        /// of element iself.
        /// </summary>
        SelfRelative = ELEMENT_AREA.SELF_RELATIVE, 

        /// <summary>
        /// position relative to view - HTMLayout window
        /// </summary>
        ViewRelative = ELEMENT_AREA.VIEW_RELATIVE,

        /// <summary>
        /// content (inner)  box
        /// </summary>
        ContentBox = ELEMENT_AREA.CONTENT_BOX,   

        /// <summary>
        /// content + paddings
        /// </summary>
        PaddingBox = ELEMENT_AREA.PADDING_BOX,   

        /// <summary>
        /// content + paddings + border
        /// </summary>
        BorderBox = ELEMENT_AREA.BORDER_BOX,   


        /// <summary>
        /// content + paddings + border + margins 
        /// </summary>
        MarginBox = ELEMENT_AREA.MARGIN_BOX,  

        /// <summary>
        /// relative to content origin - location of background image (if it set no-repeat)
        /// </summary>
        BackImageArea = ELEMENT_AREA.BACK_IMAGE_AREA, 

        /// <summary>
        /// relative to content origin - location of foreground image (if it set no-repeat)
        /// </summary>
        ForeImageArea = ELEMENT_AREA.FORE_IMAGE_AREA, 

        /// <summary>
        /// scroll_area - scrollable area in content box 
        /// </summary>
        ScrollableArea = ELEMENT_AREA.SCROLLABLE_AREA,   
    }
}
