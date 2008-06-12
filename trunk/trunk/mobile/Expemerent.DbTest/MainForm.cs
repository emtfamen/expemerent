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
        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs args)
        {
            base.OnLoad(args);

            var options = new TestOptions()
            {
                DatabasePath = GetDefaultDatabasePath(),
                RecordsCount = 1000
            };

            options.PropertyChanged += (s, e) =>
            {
                if (options.RecordsCount < 1)
                    options.RecordsCount = 1;
            };


            var container = new Container();
            Disposed += (s, evt) => container.Dispose();

            // Update window title 
            DocumentComplete += (s, evt) => Text = String.IsNullOrEmpty(Text) ? RootElement.Find("title").Text : Text;

            var providers_list = new BindingSource() 
                { 
                    DataSource = new Type[] 
                    { 
                        typeof(Provider.SqlCe.SqlCeProviderTest),
                        typeof(Provider.SQLite.SQLiteProviderTest)
                    } 
                };
            var providers = new ListBoxControl() { Selector = "#database_provider", DataSource = providers_list };
            providers.Format += (s, e) => e.Value = ((Type)e.Value).Name;

            var database_path = new TextBoxControl() { Selector = "#database_location" };
            database_path.DataBindings.Add("Text", options, "DatabasePath");
            database_path.Validating += (s, e) =>
            {
                e.Cancel = !new DirectoryInfo(database_path.Text).Exists;
                if (e.Cancel)
                    database_path.Attributes["error"] = "true";
                else
                    database_path.Attributes["error"] = null;
            };

            var records_count = new TextBoxControl() { Selector = "#records_count" };
            records_count.DataBindings.Add("Text", options, "RecordsCount");

            var browse_button = new ButtonControl() { Selector = "#folder_browse" };
            browse_button.Click += delegate
            {
                options.DatabasePath = GetDefaultDatabasePath();
            };

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

            container.Add(providers_list);

            SciterControls.Add(records_count);
            SciterControls.Add(metrics_grid);
            SciterControls.Add(browse_button);
            SciterControls.Add(providers);
            SciterControls.Add(start_button);
            SciterControls.Add(database_path);
            LoadHtmlResource<MainForm>("Html/Default.htm");
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
