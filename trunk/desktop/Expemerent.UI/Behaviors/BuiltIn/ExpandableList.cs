using System;
using System.Collections.Generic;
using System.Text;
using Expemerent.UI.Dom;
using Keys = System.Windows.Forms.Keys;

namespace Expemerent.UI.Behaviors.BuiltIn
{
    /// <summary>
    /// expandable-list behavior implementation
    /// </summary>
    [Behavior("expandable-list")]
    public class ExpandableList : SciterBehavior
    {
        /// <summary>
        /// Specifies which notifications should be recieved by the behavior
        /// </summary>
        protected internal override EventGroups EventGroups
        {
            get { return EventGroups.BehaviorEvent | EventGroups.Focus | EventGroups.Key | EventGroups.Mouse | EventGroups.Initialization; }
        }

        /// <summary>
        /// Handles <see cref="Mouse"/> events
        /// </summary>
        protected override void OnMouse(MouseEventArgs e)
        {
            if (e.Phase == Phase.Bubbling && (e.MouseEvent == MouseEvent.MouseDown || e.MouseEvent == MouseEvent.MouseDblClick))
            {
                if (e.MouseButtons == MouseButtons.Left)
                {
                    var ctl = e.Element;
                    var item = FindTargetItem(ctl, e.Target);

                    if (item != null)
                        SetCurrentItem(ctl, item);

                    e.Handled = true;
                }
            }

            base.OnMouse(e);
        }

        /// <summary>
        /// Handles <see cref="Key"/> events
        /// </summary>
        protected override void OnKey(KeyEventArgs e)
        {
            if (e.Phase == Phase.Bubbling && (e.KeyEventType == KeyEventType.KeyDown) && e.KeyboardState == KeyboardState.None)
            {
                var ctl = e.Element;
                switch (e.KeyCode)
                {
                    case Keys.Down:
                        {
                            var c = ctl.Find(":current");
                            var ch = ctl.Children;
                            var idx = c != null ? c.ElementIndex + 1 : 0;                            
                            if (idx < ch.Count)
                            {
                                var nc = ch[idx];
                                SetCurrentItem(ctl, nc);
                            }
                            e.Handled = true;
                        }
                        break;
                    case Keys.Up:
                        {
                            var c = ctl.Find(":current");
                            var ch = ctl.Children;
                            var idx = c != null ? c.ElementIndex - 1 : ch.Count - 1;                            
                            if (idx >= 0)
                            {
                                var nc = ch[idx];
                                SetCurrentItem(ctl, nc);
                            }
                            e.Handled = true;
                        }
                        break;
                    case Keys.Space:
                        {
                            var c = ctl.Find(":current");
                            if (c != null)
                                SetCurrentItem(ctl, c);
                        }
                        break;
                    default:
                        break;
                }
            }
            base.OnKey(e);
        }

        /// <summary>
        /// Handles <see cref="Focus"/> events
        /// </summary>
        protected override void OnFocus(FocusEventArgs e)
        {
            base.OnFocus(e);
        }

        /// <summary>
        /// Handles <see cref="Behavior"/> events
        /// </summary>        
        protected override void OnBehaviorEvent(BehaviorEventArgs e)
        {
            if (e.Phase == Phase.Bubbling && (e.BehaviorEvent == BehaviorEventType.ActivateChild))
            {
                var item = FindTargetItem(e.Element, e.Target);
                if (item != null)
                {
                    SetCurrentItem(e.Element, item);
                    e.Handled = true;
                }
            }

            base.OnBehaviorEvent(e);
        }

        /// <summary>
        /// Handles <see cref="Attached"/> event
        /// </summary>
        protected override void OnAttached(ElementEventArgs e)
        {
            
            // Scope will free all allocated child elements
            using (var scope = ElementScope.Create())
            {
                var ctl = e.Element;
                var children = ctl.Children;
                var got_one = false;
                for (int i = children.Count - 1; i >= 0; --i)
                {
                    var t = children[i];
                    if (!String.IsNullOrEmpty(t.Attributes["default"]) && !got_one)
                    {
                        t.SetState(ElementState.Current | ElementState.Expanded);
                        got_one = true;
                    }
                    else
                    {
                        t.SetState(ElementState.Collapsed);
                    }
                }
            }
            
            base.OnAttached(e);
        }

        /// <summary>
        /// Set current item
        /// </summary>
        protected virtual void SetCurrentItem(Element ctl, Element item)
        {
            // previously selected item
            var prev_current = ctl.Find(":root > :current");
            var prev = ctl.Find(":root > :expanded");

            if (prev_current != null && prev_current != item)
                prev_current.SetState(ElementState.None, ElementState.Current);

            if (prev != null)
            {
                if (prev == item) 
                    return;

                prev.SetState(ElementState.None, ElementState.Current | ElementState.Expanded);
            }
            item.SetState(ElementState.Current | ElementState.Expanded);
            item.ScrollToView();
        }

        /// <summary>
        /// Find target item
        /// </summary>
        private Element FindTargetItem(Element ctl, Element target)
        {
            if (target == ctl)
                return null;

            if (target == null)
                return null;

            var parent = target.Parent;
            if (parent == null)
                return target;

            if (target.Test("li > .caption"))
                return parent;

            return FindTargetItem(ctl, parent);
        }
    }
}
