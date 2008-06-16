using System;
using Expemerent.UI;
using Expemerent.UI.Controls;
using BindingSource = System.Windows.Forms.BindingSource;
using System.ComponentModel;
using Expemerent.DbTest.Provider;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Expemerent.DbTest
{
    public partial class MainForm : SciterForm
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Form initialization
        /// </summary>
        protected override void OnLoad(EventArgs args)
        {
            base.OnLoad(args);
            AutoValidate = AutoValidate.EnablePreventFocusChange;

            var options = new TestOptions()
            {
                DatabasePath = GetDefaultDatabasePath(),
                RecordsCount = 1000
            };

            var container = new Container();
            Disposed += (s, evt) => container.Dispose();

            // Update window title 
            DocumentComplete += (s, evt) => Text = String.IsNullOrEmpty(Text) ? RootElement.Find("title").Text : Text;

            var providers_list = new BindingSource();

            var providers = new ListBoxControl() { Selector = "#database_provider", DataSource = providers_list };
            providers.Format += (s, e) => e.Value = ((Type)e.Value).Name;            

            var new_database = new CheckBoxControl() { Selector = "#new_database" };
            new_database.DataBindings.Add("Checked", options, "CreateDatabase");

            var records_count = new TextBoxControl() { Selector = "#records_count" };
            records_count.DataBindings.Add("Text", options, "RecordsCount", true);

            var records_count_binding = records_count.DataBindings["Text"];
            records_count_binding.BindingComplete += (s, e) =>
            {
                if (options.RecordsCount < 0)
                {
                    records_count.Attributes["error"] = "true";
                    e.Cancel = true;
                }
                else
                    records_count.Attributes["error"] = null;
            };

            var selection_test = new CheckBoxControl() { Selector = "#selection_test" };
            selection_test.DataBindings.Add("Checked", options, "SelectionTest");

            var resultset_test = new CheckBoxControl() { Selector = "#resultset_test" };
            resultset_test.DataBindings.Add("Checked", options, "ResultSetTest");

            var metrics_grid = new DataGridControl() { Selector = "#metrics_grid" };
            var start_button = new ButtonControl() { Selector = "#start_tests" };
            start_button.Click += delegate
            {
                if (PerformValidation())
                {
                    metrics_grid.DataSource = Enumerable.Empty<Metric>();
                    metrics_grid.Element.Update(true);

                    var metricResults = RunProviderTests((Type)providers_list.Current, options);
                    metrics_grid.DataSource = metricResults;
                }
            };

            providers_list.CurrentItemChanged += (s, e) =>
            {
                var exists = IsDatabaseExists((Type)providers_list.Current, options);
                if (exists)
                {
                    new_database.IsEnabled = true;
                }
                else
                {
                    options.CreateDatabase = true;
                    new_database.IsEnabled = false;
                }
            };
            providers_list.DataSource = new Type[] 
            { 
                typeof(Provider.SqlCe.SqlCeProviderTest),
                typeof(Provider.SQLite.SQLiteProviderTest)
            };

            container.Add(providers_list);

            SciterControls.Add(records_count);
            SciterControls.Add(new_database);
            SciterControls.Add(selection_test);
            SciterControls.Add(resultset_test);
            SciterControls.Add(metrics_grid);
            SciterControls.Add(providers);
            SciterControls.Add(start_button);

            LoadHtmlResource<MainForm>("Html/Default.htm");
        }

        /// <summary>
        /// Returns a value indicating whether database file exists
        /// </summary>
        private bool IsDatabaseExists(Type providerType, TestOptions options)
        {
            var provider = (DbProviderTest)Activator.CreateInstance(providerType);
            return provider.IsDatabaseExists(options);
        }

        /// <summary>
        /// Starts tests on the selected provider
        /// </summary>
        private IList<Metric> RunProviderTests(Type type, TestOptions options)
        {
            var reporter = new MetricReporter();
            var provider = (DbProviderTest)Activator.CreateInstance(type);

            provider.Initialize(options, reporter);
            provider.RunTests();

            return reporter.Metrics;
        }

        /// <summary>
        /// Gets path to the default database folder
        /// </summary>
        private string GetDefaultDatabasePath()
        {
            const string fileScheme = "file:///";
            var assemblyPath = GetType().Assembly.GetName().CodeBase;
            if (assemblyPath.StartsWith(fileScheme))
                assemblyPath = assemblyPath.Substring(fileScheme.Length);

            return Path.GetDirectoryName(assemblyPath);
        }
    }
}
