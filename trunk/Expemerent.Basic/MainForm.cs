using System;
using System.Diagnostics;
using Expemerent.UI;
using Expemerent.UI.Behaviors;
using Expemerent.UI.Controls;
using System.Windows.Forms;
using Expemerent.UI.Dom;

namespace Expemerent.Basic
{
    public partial class MainForm : SciterForm
    {
        public MainForm()
        {
            LoadResource<MainForm>("Html/Default.htm");
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs args)
        {
            base.OnLoad(args);

            //View.CallbackHost += (s, e) => Trace.WriteLine(String.Format("First: {0}, Second: {1}", e.First, e.Second));
            //View.LoadResource(GetType(), "Html/scintilla.htm");

            //var input = new TextBoxControl() { Selector = "#input_text" };
            //var label = new TextBoxControl() { Selector = "#static_text" };
            //var slider = new SliderControl() { Selector = "#slider" };
            //var slider_text = new TextBoxControl() { Selector = "#slider_text" };
            //var title = new TextBoxControl() { Selector = "#title" };

            //input.DataBindings.Add("Text", label, "Text");
            //slider.DataBindings.Add("Value", slider_text, "Text");

            //SciterControls.Add(title);
            //SciterControls.Add(slider);
            //SciterControls.Add(input);
            //SciterControls.Add(label);
            //SciterControls.Add(slider_text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }
    }
}
