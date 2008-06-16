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
        /// Returns resultset with records filtered by specified criteria
        /// </summary>
        protected override DbResultSet ExecuteResultSet(DbConnection conn, DbTransaction tran, string statement)
        {
            using (var cmd = new SqlCeCommand(statement, (SqlCeConnection)conn, (SqlCeTransaction)tran))
            {
                return new ResultSetImpl(cmd.ExecuteResultSet(ResultSetOptions.Scrollable));
            }
        }

        /// <summary>
        /// Gets statemnts with database schema definition
        /// </summary>
        protected override IEnumerable<String> Schema
        {
            get
            {
                yield return @"CREATE TABLE Messages(MessageID int primary key, Subject varbinary(256), Body image)";
            }
        }

        private class ResultSetImpl : DbResultSet
        {
            /// <summary>
            /// ResultSet instance
            /// </summary>
            private SqlCeResultSet _resultSet;

            /// <summary>
            /// Creates a new instance of the <see cref="ResultSetImpl"/> class
            /// </summary>
            public ResultSetImpl(SqlCeResultSet resultSet)
            {
                _resultSet = resultSet;
            }

            /// <summary>
            /// Reads records from absolute position
            /// </summary>
            public override void ReadAbsolute(int position, int count, Action<IDataRecord> process)
            {
                for (int i = 0; i < count; ++i)
                {
                    if (_resultSet.ReadAbsolute(position + i))
                        process(_resultSet);
                }
            }

            /// <summary>
            /// Gets amount of records in the resultset
            /// </summary>
            public override int Count 
            {
                get { return _resultSet.RecordsAffected; }
            }

            /// <summary>
            /// Releases resources
            /// </summary>
            public override void Dispose()
            {
                _resultSet.Dispose();
            }
        }
    }
}
