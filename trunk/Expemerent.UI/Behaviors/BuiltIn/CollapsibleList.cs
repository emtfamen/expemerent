using System;
using System.Collections.Generic;
using System.Text;
using Expemerent.UI.Dom;

namespace Expemerent.UI.Behaviors.BuiltIn
{
    /// <summary>
    /// collapsible-list behavior implementation
    /// </summary>
    [Behavior("collapsible-list")]
    public class CollapsibleList : ExpandableList
    {
        /// <summary>
        /// Set current item
        /// </summary>
        protected override void SetCurrentItem(Element ctl, Element item)
        {
            // get previously expanded item:
            Element prev = ctl.Find(":root > :expanded");
            Element prev_current = ctl.Find(":root > :current");

            if (prev_current != item && prev_current != null)
                prev_current.SetState(ElementState.None, ElementState.Current);

            if (prev == item)
            {
                prev.SetState(ElementState.Current | ElementState.Collapsed);
            }
            else
            {
                if (prev != null)
                    prev.SetState(ElementState.Collapsed); // collapse old one

                item.SetState(ElementState.Current | ElementState.Expanded); // set new expanded.
            }
            item.ScrollToView();
        }
    }
}
