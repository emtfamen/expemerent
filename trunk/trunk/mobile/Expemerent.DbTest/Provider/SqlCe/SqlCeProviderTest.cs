using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.IO;
using System.Data;
using System.Data.Common;

namespace Expemerent.DbTest.Provider.SqlCe
{
    internal class SqlCeProviderTest : DbProviderTest
    {
        /// <summary>
        /// Provider factory 
        /// </summary>
        private SqlCeProviderFactory _factory;

        /// <summary>
        /// Provider initialization
        /// </summary>
        public override void Initialize(TestOptions options, MetricReporter reporter)
        {
            base.Initialize(options, reporter);
        }

        /// <summary>
        /// Gets factory instance for the provider
        /// </summary>
        protected override DbProviderFactory Factory
        {
            [DebuggerStepThrough]
            get { return _factory ?? (_factory = new SqlCeProviderFactory()); }
        }

        /// <summary>
        /// Creates new database file
        /// </summary>
        protected override void CreateDatabase()
        {
            var file = new FileInfo(Path.Combine(Options.DatabasePath, DatabaseFile));
            if (file.Exists)
                file.Delete();

            using (var engine = new SqlCeEngine())
            {
                engine.LocalConnectionString = ConnectionString;
                engine.CreateDatabase();
            }
        }

        /// <summary>
        /// Gets database file location
        /// </summary>
        protected override string DatabaseFile
        {
            [DebuggerStepThrough]
            get { return Path.Combine(Options.DatabasePath, "sql.ce.db"); }
        }

        /// <summary>
        /// Gets statemnts with database schema definition
        /// </summary>
        protected override IEnumerable<String> Schema
        {
            get
            {
                return new List<string>()
                {
                    @"CREATE TABLE Messages(MessageID int primary key, Subject nvarchar(256), Body ntext)"
                };
            }
        }
    }
}
