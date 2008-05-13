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
            using (var scope = ElementScope.Create())
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

                Controls.Add(contentControl);

                LoadHtmlResource<MainForm>("Html/Default.htm");
            }
        }
    }
}
