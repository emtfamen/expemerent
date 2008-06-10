using System;
using System.Collections.Generic;
using System.Text;

namespace Expemerent.UI.Native
{
    [Flags]
    public enum ELEMENT_STATE_BITS
    {
        /// <summary>
        ///selector :link,    any element having href attribute
        ///</summary>
        STATE_LINK = 0x00000001,
        /// <summary>
        ///selector :hover,   element is under the cursor, mouse hover  
        ///</summary>
        STATE_HOVER = 0x00000002,
        /// <summary>
        ///selector :active,  element is activated, e.g. pressed  
        ///</summary>
        STATE_ACTIVE = 0x00000004,
        /// <summary>
        ///selector :focus,   element is in focus  
        ///</summary>
        STATE_FOCUS = 0x00000008,
        /// <summary>
        ///selector :visited, aux flag - not used internally now.
        ///</summary>
        STATE_VISITED = 0x00000010,
        /// <summary>
        ///selector :current, current item in collection, e.g. current <option> in <select>
        ///</summary>
        STATE_CURRENT = 0x00000020,
        /// <summary>
        ///selector :checked, element is checked (or selected), e.g. check box or itme in multiselect
        ///</summary>
        STATE_CHECKED = 0x00000040,
        /// <summary>
        ///selector :disabled, element is disabled, behavior related flag.
        ///</summary>
        STATE_DISABLED = 0x00000080,
        /// <summary>
        ///selector :read-only, element is read-only, behavior related flag.
        ///</summary>
        STATE_READONLY = 0x00000100,
        /// <summary>
        ///selector :expanded, element is in expanded state - nodes in tree view e.g. <options> in <select>
        ///</summary>
        STATE_EXPANDED = 0x00000200,
        /// <summary>
        ///selector :collapsed, mutually exclusive with EXPANDED
        ///</summary>
        STATE_COLLAPSED = 0x00000400,
        /// <summary>
        ///selector :incomplete, element has images (back/fore/bullet) requested but not delivered.
        ///</summary>
        STATE_INCOMPLETE = 0x00000800,
        /// <summary>
        ///selector :animating, is currently animating 
        ///</summary>
        STATE_ANIMATING = 0x00001000,
        /// <summary>
        ///selector :focusable, shall accept focus
        ///</summary>
        STATE_FOCUSABLE = 0x00002000,
        /// <summary>
        ///selector :anchor, first element in selection (<select miltiple>), STATE_CURRENT is the current.
        ///</summary>
        STATE_ANCHOR = 0x00004000,
        /// <summary>
        ///selector :synthetic, synthesized DOM elements - e.g. all missed cells in tables (<td>) are getting this flag
        ///</summary>
        STATE_SYNTHETIC = 0x00008000,
        /// <summary>
        ///selector :owns-popup, anchor(owner) element of visible popup. 
        ///</summary>
        STATE_OWNS_POPUP = 0x00010000,
        /// <summary>
        ///selector :tab-focus, element got focus by tab traversal. engine set it together with :focus.
        ///</summary>
        STATE_TABFOCUS = 0x00020000,
        /// <summary>
        ///selector :empty - element is empty. 
        ///</summary>
        STATE_EMPTY = 0x00040000,
        /// <summary>
        ///selector :busy, element is busy. HTMLayoutRequestElementData will set this flag if
        ///external data was requested for the element. When data will be delivered engine will reset this flag on the element. 
        ///</summary>
        STATE_BUSY = 0x00080000,


        /// <summary>
        ///drag over the block that can accept it (so is current drop target). Flag is set for the drop target block. At any given moment of time it can be only one such block.
        ///</summary>
        STATE_DRAG_OVER = 0x00100000,
        /// <summary>
        ///active drop target. Multiple elements can have this flag when D&D is active. 
        ///</summary>
        STATE_DROP_TARGET = 0x00200000,
        /// <summary>
        ///dragging/moving - the flag is set for the moving element (copy of the drag-source).
        ///</summary>
        STATE_MOVING = 0x00400000,
        /// <summary>
        ///dragging/copying - the flag is set for the copying element (copy of the drag-source).
        ///</summary>
        STATE_COPYING = 0x00800000,
        /// <summary>
        ///is set in element that is being dragged.
        ///</summary>
        STATE_DRAG_SOURCE = 0x00C00000,

        /// <summary>
        ///this element is in popup state and presented to the user - out of flow now
        ///</summary>
        STATE_POPUP = 0x40000000,
        /// <summary>
        ///pressed - close to active but has wider life span - e.g. in MOUSE_UP it is still on, so behavior can check it in MOUSE_UP to discover CLICK condition.
        ///</summary>
        STATE_PRESSED = 0x04000000,
        /// <summary>
        ///has more than one child.    
        ///</summary>
        STATE_HAS_CHILDREN = 0x02000000,
        /// <summary>
        ///has single child.
        ///</summary>
        STATE_HAS_CHILD = 0x01000000,

        /// <summary>
        ///selector :ltr, the element or one of its nearest container has @dir and that dir has "ltr" value
        ///</summary>
        STATE_IS_LTR = 0x20000000,
        /// <summary>
        ///selector :rtl, the element or one of its nearest container has @dir and that dir has "rtl" value
        ///</summary>
        STATE_IS_RTL = 0x10000000,
    }
}
