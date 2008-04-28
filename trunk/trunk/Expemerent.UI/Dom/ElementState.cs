using System;
using Expemerent.UI.Native;

namespace Expemerent.UI.Dom
{
    /// <summary>
    /// Possible element states
    /// </summary>
    [Flags]
    public enum ElementState
    {
        /// <summary>
        /// selector :link, any element having href attribute
        ///</summary>
        Link = ELEMENT_STATE_BITS.STATE_LINK,
        
        /// <summary>
        /// selector :hover, element is under the cursor, mouse hover 
        ///</summary>
        Hover = ELEMENT_STATE_BITS.STATE_HOVER,
        /// <summary>
        /// selector :active, element is activated, e.g. pressed 
        ///</summary>
        Active = ELEMENT_STATE_BITS.STATE_ACTIVE,

        /// <summary>
        /// selector :focus, element is in focus 
        ///</summary>
        Focus = ELEMENT_STATE_BITS.STATE_FOCUS,

        /// <summary>
        /// selector :visited, aux flag - not used internally now.
        ///</summary>
        Visited = ELEMENT_STATE_BITS.STATE_VISITED,
        
        /// <summary>
        /// selector :current, current item in collection, e.g. current <option> in <select>
        ///</summary>
        Current = ELEMENT_STATE_BITS.STATE_CURRENT,
        
        /// <summary>
        /// selector :checked, element is checked (or selected), e.g. check box or itme in multiselect
        ///</summary>
        Checked = ELEMENT_STATE_BITS.STATE_CHECKED,
        
        /// <summary>
        /// selector :disabled, element is disabled, behavior related flag.
        ///</summary>
        Disabled = ELEMENT_STATE_BITS.STATE_DISABLED,
        
        /// <summary>
        /// selector :read-only, element is read-only, behavior related flag.
        ///</summary>
        Readonly = ELEMENT_STATE_BITS.STATE_READONLY,
        
        /// <summary>
        /// selector :expanded, element is in expanded state - nodes in tree view e.g. <options> in <select>
        ///</summary>
        Expanded = ELEMENT_STATE_BITS.STATE_EXPANDED,
        
        /// <summary>
        /// selector :collapsed, mutually exclusive with EXPANDED
        ///</summary>
        Collapsed = ELEMENT_STATE_BITS.STATE_COLLAPSED,
        
        /// <summary>
        /// selector :incomplete, element has images (back/fore/bullet) requested but not delivered.
        ///</summary>
        Incomplete = ELEMENT_STATE_BITS.STATE_INCOMPLETE,
        
        /// <summary>
        /// selector :animating, is currently animating 
        ///</summary>
        Animating = ELEMENT_STATE_BITS.STATE_ANIMATING,
        
        /// <summary>
        /// selector :focusable, shall accept focus
        ///</summary>
        Focusable = ELEMENT_STATE_BITS.STATE_FOCUSABLE,

        /// <summary>
        /// selector :anchor, first element in selection (<select miltiple>), ELEMENT_STATE_BITS.STATE_CURRENT is the current.
        ///</summary>
        Anchor = ELEMENT_STATE_BITS.STATE_ANCHOR,

        /// <summary>
        /// selector :synthetic, synthesized DOM elements - e.g. all missed cells in tables (<td>) are getting this flag
        ///</summary>
        Synthetic = ELEMENT_STATE_BITS.STATE_SYNTHETIC,

        /// <summary>
        /// selector :owns-popup, anchor(owner) element of visible popup. 
        ///</summary>
        OwnsPopup = ELEMENT_STATE_BITS.STATE_OWNS_POPUP,

        /// <summary>
        /// selector :tab-focus, element got focus by tab traversal. engine set it together with :focus.
        ///</summary>
        TabFocus = ELEMENT_STATE_BITS.STATE_TABFOCUS,

        /// <summary>
        /// selector :empty - element is empty. 
        ///</summary>
        Empty = ELEMENT_STATE_BITS.STATE_EMPTY,

        /// <summary>
        /// selector :busy, element is busy. HTMLayoutRequestElementData will set this flag if
        /// external data was requested for the element. When data will be delivered engine will reset this flag on the element. 
        ///</summary>
        Busy = ELEMENT_STATE_BITS.STATE_BUSY,

        /// <summary>
        /// drag over the block that can accept it (so is current drop target). Flag is set for the drop target block. At any given moment of time it can be only one such block.
        ///</summary>
        DragOver = ELEMENT_STATE_BITS.STATE_DRAG_OVER,

        /// <summary>
        /// active drop target. Multiple elements can have this flag when D&D is active. 
        ///</summary>
        DropTarget = ELEMENT_STATE_BITS.STATE_DROP_TARGET,

        /// <summary>
        /// dragging/moving - the flag is set for the moving block.
        ///</summary>
        Moving = ELEMENT_STATE_BITS.STATE_MOVING,
        
        /// <summary>
        /// dragging/copying - the flag is set for the copying block.
        ///</summary>
        Copying = ELEMENT_STATE_BITS.STATE_COPYING,

        /// <summary>
        /// this element is in popup state and presented to the user - out of flow now
        ///</summary>
        Popup = ELEMENT_STATE_BITS.STATE_POPUP,

        /// <summary>
        /// pressed - close to active but has wider life span - e.g. in MOUSE_UP it 
        /// is still on, so behavior can check it in MOUSE_UP to discover CLICK condition.
        ///</summary>
        Pressed = ELEMENT_STATE_BITS.STATE_PRESSED,

        /// <summary>
        /// has more than one child. 
        ///</summary>
        HasСhildren = ELEMENT_STATE_BITS.STATE_HAS_CHILDREN,

        /// <summary>
        /// has single child.is still on, so behavior can check it in MOUSE_UP to discover CLICK condition.
        ///</summary>
        HasChild = ELEMENT_STATE_BITS.STATE_HAS_CHILD,        
    } 
}
