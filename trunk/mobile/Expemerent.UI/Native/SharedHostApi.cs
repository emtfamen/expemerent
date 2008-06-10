using System;
using System.Collections.Generic;
using System.Text;
using Expemerent.UI.Behaviors;
using Expemerent.UI.Dom;
using Keys = System.Windows.Forms.Keys;
using System.Diagnostics;

namespace Expemerent.UI.Native
{
    /// <summary>
    /// Native interface to the sciter-x.dll
    /// </summary>
    internal static partial class SciterHostApi
    {
        /// <summary>
        /// Handles draw event
        /// </summary>
        unsafe static partial void Behavior_HandleDraw(ISciterBehavior behavior, IntPtr he, IntPtr prms, ref bool handled)
        {
            var datantf = (DRAW_PARAMS*)prms;
            var e = new DrawEventArgs(Element.Create(he))
            {
                Hdc = datantf->hdc,
                DrawArea = datantf->area,
                EventType = (DrawEventType)datantf->cmd
            };

            try
            {
                behavior.ProcessDraw(e);
            }
            finally
            {
                if (e.IsGraphicsCreated)
                    e.ReleaseGraphics();
            }

            handled = e.Handled;
        }

        /// <summary>
        /// Handles timer event
        /// </summary>
        unsafe static partial void Behavior_HandleTimer(ISciterBehavior behavior, IntPtr he, IntPtr prms, ref bool handled)
        {
            var e = new ElementEventArgs(Element.Create(he));
            behavior.ProcessTimer(e);

            handled = e.Handled;
        }

        /// <summary>
        /// Handles method call event
        /// </summary>
        unsafe static partial void Behavior_HandleMethodCall(ISciterBehavior behavior, IntPtr he, IntPtr prms, ref bool handled)
        {
            var datantf = (METHOD_PARAMS*)prms;
            var methodId = (METHOD_PARAMS.BEHAVIOR_METHOD_IDENTIFIERS)datantf->methodID;
            switch (methodId)
            {
                case METHOD_PARAMS.BEHAVIOR_METHOD_IDENTIFIERS.XCALL:
                    {
                        var data = (XCALL_PARAMS*)datantf;
                        var e = new ScriptingMethodCallEventArgs(Element.Create(he))
                        {
                            Arguments = data->GetArgs(),
                            MethodName = data->GetName()
                        };

                        behavior.ProcessScriptingMethodCall(e);
                        if (e.Handled)
                            data->result.SetValue(e.ReturnValue);

                        handled = e.Handled;
                    }
                    break;
                default:
                    {
                        var e = new MethodCallEventArgs(Element.Create(he));
            behavior.ProcessMethodCall(e);

            handled = e.Handled;
        }
                    break;
            }
        }

        /// <summary>
        /// Handles data arrived event
        /// </summary>
        unsafe static partial void Behavior_HandleDataArrived(ISciterBehavior behavior, IntPtr he, IntPtr prms, ref bool handled)
        {
            var datantf = (DATA_ARRIVED_PARAMS*)prms;
            var e = new DataArrivedEventArgs(Element.Create(he));

            behavior.ProcessDataArrived(e);

            handled = e.Handled;
        }

        /// <summary>
        /// Handles size event
        /// </summary>
        unsafe static partial void Behavior_HandleSize(ISciterBehavior behavior, IntPtr he, IntPtr prms, ref bool handled)
        {
            var e = new ElementEventArgs(Element.Create(he));
            behavior.ProcessSize(e);

            handled = e.Handled;
        }

        /// <summary>
        /// Handles scroll event
        /// </summary>
        unsafe static partial void Behavior_HandleScroll(ISciterBehavior behavior, IntPtr he, IntPtr prms, ref bool handled)
        {
            var e = new ElementEventArgs(Element.Create(he));
            behavior.ProcessScroll(e);

            handled = e.Handled;
        }

        /// <summary>
        /// Handles keyboard event
        /// </summary>
        unsafe static partial void Behavior_HandleKey(ISciterBehavior behavior, IntPtr he, IntPtr prms, ref bool handled)
        {
            var datantf = (KEY_PARAMS*)prms;
            var e = new KeyEventArgs(Element.Create(he), (Phase)datantf->cmd & Phase.All)
            {
                Target = Element.Create(datantf->target),
                KeyboardState = (KeyboardState)datantf->alt_state,
                KeyEventType = (KeyEventType)((int)datantf->cmd & (int)~Phase.All),
                KeyValue = datantf->key_code
            };

            behavior.ProcessKey(e);

            handled = e.Handled;
        }

        /// <summary>
        /// Handles initialization event
        /// </summary>
        unsafe static partial void Behavior_HandleInitialization(ISciterBehavior behavior, IntPtr he, IntPtr prms, ref bool handled)
        {
            var datantf = (INITIALIZATION_PARAMS*)prms;
            var e = new ElementEventArgs(Element.Create(he));

            switch (datantf->cmd)
            {
                case INITIALIZATION_PARAMS.INITIALIZATION_EVENTS.BEHAVIOR_ATTACH:
                    behavior.ProcessAttach(e);
                    break;
                case INITIALIZATION_PARAMS.INITIALIZATION_EVENTS.BEHAVIOR_DETACH:
                    behavior.ProcessDettach(e);
                    break;
                default:
                    Debug.Fail(String.Format("Invalid enum value: {0}", datantf->cmd));
                    break;
            }

            handled = e.Handled;
        }

        /// <summary>
        /// Handles scripting call
        /// </summary>
        unsafe static partial void Behavior_HandleScriptingMethodCall(ISciterBehavior behavior, IntPtr he, IntPtr prms, ref bool handled)
        {
            var datantf = (SCRIPTING_METHOD_PARAMS*)prms;
            var e = new ScriptingMethodCallEventArgs(Element.Create(he))
            {
                Arguments = datantf->GetArgs(),
                MethodName = datantf->GetName()
            };

            behavior.ProcessScriptingMethodCall(e);
            if (e.Handled)
                datantf->result.SetValue(e.ReturnValue);

            handled = e.Handled;
        }

        /// <summary>
        /// Handles mouse event
        /// </summary>
        unsafe static partial void Behavior_HandleMouseEvent(ISciterBehavior behavior, IntPtr he, IntPtr prms, ref bool handled)
        {
            var datantf = (MOUSE_PARAMS*)prms;
            var e = new MouseEventArgs(Element.Create(he), (Phase)datantf->cmd & Phase.All)
            {
                MouseButtons = (MouseButtons)datantf->button_state,
                Target = Element.Create(datantf->target),
                IsOverIcon = datantf->is_on_icon,
                Position = datantf->pos,
                PositionInDoc = datantf->pos_document,
                CursorType = (CursorType)datantf->cursor_type,
                MouseEvent = (MouseEvent)datantf->cmd & (MouseEvent)~Phase.All,
                KeyboardState = (KeyboardState)datantf->alt_state,
            };

            behavior.ProcessMouse(e);

            handled = e.Handled;
        }

        /// <summary>
        /// Handles focus change event
        /// </summary>
        unsafe static partial void Behavior_HandleFocusEvent(ISciterBehavior behavior, IntPtr he, IntPtr prms, ref bool handled)
        {
            var datantf = (FOCUS_PARAMS*)prms;
            var e = new FocusEventArgs(Element.Create(he), (Phase)datantf->cmd & Phase.All)
            {
                IsLostFocus = !((datantf->cmd & FOCUS_PARAMS.FOCUS_EVENTS.FOCUS_GOT) == FOCUS_PARAMS.FOCUS_EVENTS.FOCUS_GOT),
                Target = Element.Create(datantf->target),
                IsMouseClick = datantf->by_mouse_click,
            };

            behavior.ProcessFocus(e);
            if (e.Handled)
                datantf->cancel = e.Cancel;

            handled = e.Handled;
        }

        /// <summary>
        /// Handles behavior event
        /// </summary>
        unsafe static partial void Behavior_HandleBehaviorEvent(ISciterBehavior behavior, IntPtr he, IntPtr prms, ref bool handled)
        {
            var datantf = (BEHAVIOR_EVENT_PARAMS*)prms;
            var e = new BehaviorEventArgs(Element.Create(he), (Phase)datantf->cmd & Phase.All)
            {
                Source = Element.Create(datantf->he),
                Target = Element.Create(datantf->heTarget),
                BehaviorEvent = (BehaviorEventType)datantf->cmd & (BehaviorEventType)~Phase.All,
                Reason = (BehaviorEventReason)datantf->reason
            };

            behavior.ProcessBehaviorEvent(e);

            handled = e.Handled;
        }


        /// <summary>
        /// Handles behavior attach
        /// </summary>
        unsafe static partial void Host_HandleAttachBehavior(ISciterNotifications host, IntPtr pns)
        {
            var datantf = (SCN_ATTACH_BEHAVIOR*)pns;
            var e = new AttachBehaviorEventArgs(Element.Create(datantf->element), datantf->GetBehaviorName());
                
            host.ProcessAttachBehavior(e);

            e.Behavior = e.Behavior ?? SciterFactory.ResolveBehavior(e.BehaviorName);
            if (e.Behavior != null)
            {
                datantf->elementProc = ElementEventProcEntryPoint;
                datantf->elementTag = InstanceProtector.Protect(e.Behavior);
                datantf->elementEvents = (EVENT_GROUPS)e.EventGroups;
            }
        }

        /// <summary>
        /// Handles load data notification
        /// </summary>
        unsafe static partial void Host_HandleLoadData(ISciterNotifications host, IntPtr pns)
        {
            var datantf = (SCN_LOAD_DATA*)pns;
            var e = new LoadDataEventArgs(datantf->GetUri(), () => datantf->GetData())
            {
                IsCached = datantf->outData != IntPtr.Zero,
                //TODO: Htmlayout do not have such property
                //RequestId = datantf->request_id,
                ResourceType = (ResourceType)datantf->dataType
            };
            host.ProcessLoadData(e);

            byte[] bytes = null;
            if (e.IsDataAvailable)
                bytes = e.GetData();
            else
                bytes = SciterFactory.ResolveBinResource(e.Uri, e.ResourceType);

            SciterDataReady(datantf->hwnd, e.Uri, bytes);
        }

        /// <summary>
        /// Handles data loaded notification
        /// </summary>
        unsafe static partial void Host_HandleDataLoaded(ISciterNotifications host, IntPtr pns)
        {
            var datantf = (SCN_DATA_LOADED*)pns;
            var e = new DataLoadedEventArgs(datantf->GetUri(), () => datantf->GetData());
            host.ProcessDataLoaded(e);
        }

        /// <summary>
        /// Handles document complete event
        /// </summary>
        static unsafe partial void Host_HandleDocumentComplete(ISciterNotifications host, IntPtr pns)
        {
            host.ProcessDocumentComplete(DocumentCompleteEventArgs.Empty);
        }
    }
}
