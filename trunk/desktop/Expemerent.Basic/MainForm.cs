using System;
using System.ComponentModel;
using Expemerent.UI;
using Expemerent.UI.Controls;
using System.Windows.Forms;
using Expemerent.UI.Dom;
using System.IO;
using Expemerent.UI.Protocol;
using Expemerent.UI.Behaviors;

namespace Expemerent.Basic
{
    public partial class MainForm : SciterForm
    {
        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs evt)
        {
            base.OnLoad(evt);
            
            var contentControl = new SciterControl();

            var openButton = new ButtonControl() { Selector = "#open" };
            var reloadButton = new ButtonControl() { Selector = "#reload" };
            var content = new BindableControl() { Selector = "#content" };

            Action resizeContent = () => 
                { 
                    var rect = content.Element.GetLocation(ElementLocation.ContentBox);
                    contentControl.Top = rect.Top;
                    contentControl.Left = rect.Left;
                    contentControl.Width = rect.Width;
                    contentControl.Height = rect.Height;
                };

            content.Attached += (s, e) => resizeContent();
            content.Size += (s, e) => resizeContent();

            reloadButton.Click += (s, e) =>
                {
                    contentControl.Reload();
                };

            openButton.Click += (s, e) =>
                {
                    var dlg = new OpenFileDialog() { Filter = "Html files (*.htm)|*.htm|All files (*.*)|*.*" };
                    if (dlg.ShowDialog() == DialogResult.OK)
                        contentControl.LoadHtmlResource(FileProtocol.ProtocolPrefix + dlg.FileName);
                };

            SciterControls.Add(openButton);
            SciterControls.Add(reloadButton);
            SciterControls.Add(content);

            ScriptingMethodCall += (s, e) =>
                {
                    switch (e.MethodName)
                    {
                        case "application_name": // Returns application title
                            e.ReturnValue = Text;
                            break;
                    }
                };

            contentControl.HandleCreated += (s, e) => contentControl.RegisterClass<Scripting.Application>();
            contentControl.ScriptingMethodCall += (s, e) =>
                {
                    switch (e.MethodName)
                    {
                        case "application_name": // Returns application title
                            e.ReturnValue = Text;
                            break;
                        case "sum": // Returns sum of two arguments
                            e.ReturnValue = Convert.ToDouble(e.Arguments[0]) + Convert.ToDouble(e.Arguments[1]);
                            break;
                        case "echo":
                            e.ReturnValue = e.Arguments[0];
                            break;
                        case "dict":
                            e.ReturnValue = new { first = 1, second = 2, third = 3 };
                            break;
                        case "callback":
                            contentControl.Call("callback", "hello");
                            contentControl.Call("callback", 1);
                            contentControl.Call("callback", true);
                            contentControl.Call("callback", 1.0);
                            contentControl.Call("callback", DateTime.Now);
                            contentControl.Call("callback", 1M);
                            contentControl.Call("callback", new byte[] { 0x1, 0x2, 0x3} );
                            break;
                    }   
                };

            Controls.Add(contentControl);
            LoadHtmlResource<MainForm>("Html/Default.htm");
        }
    }
}
