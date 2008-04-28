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
                //View.DocumentComplete += (s, e) =>
                //{
                //    _elementBehavior = new SciterBehavior();
                //    var titleElement = View.RootElement.Find("#title");
                //    var titleHandle = titleElement.Use(ElementRefType.Weak);

                //    titleElement.InnerHtml = "Ля-Ля!";                
                //    _elementBehavior.Attached += delegate
                //    {
                //        // Handle protectes element between calls
                //        titleElement = titleHandle.Element;
                //        titleElement.InnerHtml += ": Сделали";
                //        titleElement.Update();
                //    };

                //    _elementBehavior.Mouse += (s1, e1) =>
                //    {
                //        // Handle protectes element between calls
                //        titleElement = titleHandle.Element;
                //        titleElement.InnerHtml = String.Format("Мыша: {0}", e1.MouseEvent);
                //        titleElement.Update();
                //    };

                //    _elementBehavior.Detached += delegate
                //    {
                //        MessageBox.Show("Detached");
                //    };

                //    titleElement.AttachBehavior(_elementBehavior);
                //};

                View.CallbackHost += (s, e) => Trace.WriteLine(String.Format("First: {0}, Second: {1}", e.First, e.Second));
                View.LoadResource(GetType(), "Html/Default.htm");

                var input = new TextBoxControl() { Selector = "#input_text" };
                var label = new TextBoxControl() { Selector = "#static_text" };
                var slider = new SliderControl() { Selector = "#slider" };
                var slider_text = new TextBoxControl() { Selector = "#slider_text" };
                var title = new TextBoxControl() { Selector = "#title" };

                title.Mouse += (s, e) => { title.Text = String.Format("Мыша: {0}", e.MouseEvent); };

                slider_text.Text = "10";
                label.IsEnabled = false;

                input.DataBindings.Add("Text", label, "Text", false, DataSourceUpdateMode.OnPropertyChanged);
                slider.DataBindings.Add("Value", slider_text, "Text", false, DataSourceUpdateMode.OnPropertyChanged);

                SciterControls.Add(title);
                SciterControls.Add(slider);
                SciterControls.Add(input);
                SciterControls.Add(label);
                SciterControls.Add(slider_text);

                slider.Value = slider.Value + 7;
            }
        }
    }
}
