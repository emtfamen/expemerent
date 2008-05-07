using System;
using System.Diagnostics;
using Expemerent.UI;
using Expemerent.UI.Behaviors;
using Expemerent.UI.Controls;
using System.Windows.Forms;
using Expemerent.UI.Dom;
using Expemerent.UI.Native;
using System.ComponentModel;

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
                components = components ?? new Container();
                var bindingSource = new System.Windows.Forms.BindingSource(components);
                var bindingList = new BindingList<Contact>()  { 
                new Contact() { FirstName = "f1", LastName = "s1" }, 
                new Contact() { FirstName = "f2", LastName = "s2" },
                new Contact() { FirstName = "f3", LastName = "s3" },
                new Contact() { FirstName = "f4", LastName = "s4" } 
            };
                bindingList.AllowEdit = true;
                bindingList.AllowRemove = true;
                bindingList.AllowNew = true;

                bindingSource.DataSource = bindingList;

                var first = new TextBoxControl() { Selector = "#first_name" };
                var second = new TextBoxControl() { Selector = "#last_name" };
                var list = new ListBoxControl() { Selector = "#contacts", DisplayMember = "First" };
                var addnew = new ButtonControl() { Selector = "#addnew" };
                var delete = new ButtonControl() { Selector = "#delete" };

                list.Format += (s, e) => { var contact = ((Contact)e.Value); e.Value = contact.FirstName + ", " + contact.LastName; };
                addnew.Click += (s, e) => { bindingSource.Position = bindingSource.Add(new Contact()); };
                delete.Click += (s, e) => { bindingSource.RemoveCurrent(); };


                first.DataBindings.Add("Text", bindingSource, "FirstName");
                second.DataBindings.Add("Text", bindingSource, "LastName");

                list.DataSource = bindingSource;

                SciterControls.Add(addnew);
                SciterControls.Add(delete);
                SciterControls.Add(first);
                SciterControls.Add(second);
                SciterControls.Add(list);

                LoadResource<MainForm>("Html/Default.htm");
            }
        }

        #region Support classes
        /// <summary>
        /// Contact information
        /// </summary>
        public class Contact
        {
            /// <summary>
            /// Gets or sets First name
            /// </summary>
            public string FirstName { get; set; }

            /// <summary>
            /// Gets or sets Second name
            /// </summary>
            public string LastName { get; set; }

            /// <summary>
            /// Gets or sets contact address
            /// </summary>
            public string Address { get; set; }
        }
        #endregion

    }
}
