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
        /// Returns resultset with records filtered by specified criteria
        /// </summary>
        protected override DbResultSet ExecuteResultSet(DbConnection conn, DbTransaction tran, string statement)
        {
            var tableName = String.Format("tmp_{0}", Guid.NewGuid().ToString().Replace('-', '_'));
            ExecuteStatement(conn, tran, String.Format("CREATE TEMP TABLE {0} AS {1}", tableName, statement));

            var cmd = new SQLiteCommand(String.Format("SELECT * FROM {0} WHERE OID BETWEEN @First AND @Last", tableName), (SQLiteConnection)conn, (SQLiteTransaction)tran);
            cmd.Parameters.Add("@First", DbType.Int32);
            cmd.Parameters.Add("@Last", DbType.Int32);
            return new ResultSetImpl(cmd);
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
                yield return @"CREATE TABLE Messages(MessageID int, Subject blob, Body blob, primary key (MessageID))";
            }
        }

        internal class ResultSetImpl : DbResultSet
        {
            /// <summary>
            /// Prepared command for record lookup
            /// </summary>
            private SQLiteCommand _command;

            /// <summary>
            /// Creates a new instance of the <see cref="ResultSetImpl"/> class
            /// </summary>
            public ResultSetImpl(SQLiteCommand command)
            {
                _command = command;
            }

            /// <summary>
            /// Reads record by absolute position
            /// </summary>
            public override void ReadAbsolute(int position, int count, Action<IDataRecord> process)
            {
                // ROWID is "1" based
                var rowid = position + 1;
                _command.Parameters[0].Value = rowid;
                _command.Parameters[1].Value = rowid + count - 1;
                using (var reader = _command.ExecuteReader())
                {
                    while (reader.Read())
                        process(reader);
                }
            }

            public override int Count
            {
                get { throw new NotImplementedException(); }
            }

            /// <summary>
            /// Releases used resources
            /// </summary>
            public override void Dispose()
            {
                _command.Dispose();
            }
        }
    }
}
