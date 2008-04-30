using System;
using System.Diagnostics;
using Expemerent.UI;
using Expemerent.UI.Behaviors;
using Expemerent.UI.Controls;
using System.Windows.Forms;
using Expemerent.UI.Dom;
using Expemerent.UI.Native;

namespace Expemerent.Basic
{
    public partial class MainForm : SciterForm
    {
        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs args)
        {
            base.OnLoad(args);

            using (var scope = ElementScope.Create())
            {
                var input = new TextBoxControl() { Selector = "#input_text" };
                var label = new TextBoxControl() { Selector = "#static_text" };
                var slider = new SliderControl() { Selector = "#slider" };
                var slider_text = new TextBoxControl() { Selector = "#slider_text" };
                var title = new TextBoxControl() { Selector = "#title" };

                title.Mouse += (s, e) => { title.Text = String.Format("Мыша: {0}", e.MouseEvent); };

                slider_text.Text = "10";
                label.IsEnabled = false;

                input.DataBindings.Add("Text", label, "Text");
                slider.DataBindings.Add("Value", slider_text, "Text");

                SciterControls.Add(title);
                SciterControls.Add(slider);
                SciterControls.Add(input);
                SciterControls.Add(label);
                SciterControls.Add(slider_text);

                LoadResource<MainForm>("Html/Default.htm");

                slider.Value = slider.Value + 7;
            }
        }
    }
}
