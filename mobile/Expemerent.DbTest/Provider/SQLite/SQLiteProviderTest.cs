using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;
using System.Diagnostics;
using System.Data;
using System.Data.Common;


namespace Expemerent.DbTest.Provider.SQLite
{
    internal class SQLiteProviderTest : DbProviderTest
    {
        /// <summary>
        /// Provider factory 
        /// </summary>
        private SQLiteFactory _factory;

        /// <summary>
        /// Provider initialization
        /// </summary>
        public override void Initialize(TestOptions options, MetricReporter reporter)
        {
            base.Initialize(options, reporter);
        }

        /// <summary>
        /// Opens active connection to the database
        /// </summary>
        protected override DbConnection CreateConnection()
        {
            return new SQLiteConnection(ConnectionString);
        }

        /// <summary>
        /// Gets factory instance for the provider
        /// </summary>
        protected override DbProviderFactory Factory
        {
            [DebuggerStepThrough]
            get { return _factory ?? (_factory = new SQLiteFactory()); }
        }

        /// <summary>
        /// Creates new database file
        /// </summary>
        protected override void CreateDatabase()
        {
            var file = new FileInfo(Path.Combine(Options.DatabasePath, DatabaseFile));
            if (file.Exists)
                file.Delete();
            
            SQLiteConnection.CreateFile(file.FullName);
        }

        /// <summary>
        /// Gets database file location
        /// </summary>
        protected override string DatabaseFile
        {
            [DebuggerStepThrough]
            get { return Path.Combine(Options.DatabasePath, "sql.lite.db"); } 
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
                    @"CREATE TABLE Messages(MessageID int, Subject text, Body text, primary key (MessageID))"
                };
            }
        }
    }
}
