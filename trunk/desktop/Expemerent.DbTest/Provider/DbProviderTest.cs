using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Diagnostics;
using System.Data.Common;
using System.Text;
using System.IO;

namespace Expemerent.DbTest.Provider
{
    internal abstract class DbProviderTest
    {
        /// <summary>
        /// Collection of unique Ids
        /// </summary>
        private List<int> _uniqueIds;

        /// <summary>
        /// Gets or sets test options
        /// </summary>
        protected TestOptions Options { get; private set; }

        /// <summary>
        /// Gets or sets metrics reporter
        /// </summary>
        protected MetricReporter Reporter { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="DbProviderTest"/> class
        /// </summary>
        public DbProviderTest()
        {
        }

        /// <summary>
        /// Provider initialization
        /// </summary>
        public virtual void Initialize(TestOptions options, MetricReporter reporter)
        {
            #region Arguments checking
            if (options == null)
                throw new ArgumentNullException("options");
            if (reporter == null)
                throw new ArgumentNullException("reporter"); 
            #endregion

            Options = options;
            Reporter = reporter;
        }

        /// <summary>
        /// Executes provider tests
        /// </summary>
        public virtual void RunTests()
        {
            var rnd = new Random(0);
            _uniqueIds = Enumerable.Range(0, Options.RecordsCount).Select(step => rnd.Next()).Distinct().ToList();

            using (Reporter.BeginOperation("Total time"))
            {
                if (Options.CreateDatabase)
                {
                    using (Reporter.BeginOperation("Creating database"))
                        CreateDatabase();
                }

                using (var conn = CreateConnection())
                {
                    using (Reporter.BeginOperation("Openning connection"))
                        conn.Open();

                    if (Options.CreateDatabase)
                    {
                        using (Reporter.BeginOperation("Building Schema"))
                            BuildSchema(conn);

                        using (var tran = conn.BeginTransaction())
                        {
                            using (Reporter.BeginOperation("Loading records"))
                                LoadRecords(conn, tran);

                            tran.Commit();
                        }
                    }

                    if (Options.SelectionTest)
                    {
                        using (var tran = conn.BeginTransaction())
                        {
                            using (Reporter.BeginOperation("Selecting records"))
                                ReadRecords(conn, tran);

                            tran.Commit();
                        }
                    }

                    if (Options.ResultSetTest)
                    {
                        using (var tran = conn.BeginTransaction())
                        {
                            using (Reporter.BeginOperation("ResultSet operations"))
                                ReadResultSet(conn, tran);

                            tran.Commit();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Selects records from result set
        /// </summary>
        private void ReadResultSet(DbConnection conn, DbTransaction tran)
        {
            using (var rs = ExecuteResultSet(conn, tran, "SELECT * FROM Messages ORDER BY messageId"))
            {
                var count = _uniqueIds.Count;

                var rnd = new Random(0);
                for (var i = 0; i < Options.RecordsCount; ++i)
                {
                    rs.ReadAbsolute(rnd.Next(count), 1, Touch);
                }
            }
        }

        /// <summary>
        /// Selecting random records
        /// </summary>
        private void ReadRecords(DbConnection conn, DbTransaction tran)
        {
            var rnd = new Random();

            using (var cmd = Factory.CreateCommand())
            {
                cmd.Connection = conn;
                cmd.Transaction = tran;
                cmd.CommandText = "SELECT * FROM Messages WHERE MessageId = @MessageId";
                cmd.Parameters.Add(CreateParameter("MessageId", 0));

                foreach (var item in _uniqueIds)
                {
                    cmd.Parameters[0].Value = _uniqueIds[rnd.Next(_uniqueIds.Count)];

                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        Touch(reader);
                    }
                }
            }
        }

        /// <summary>
        /// Loads records to the database
        /// </summary>
        protected void LoadRecords(DbConnection conn, DbTransaction tran)
        {
            using (var cmd = Factory.CreateCommand())
            {
                cmd.Connection = conn;
                cmd.Transaction = tran;
                cmd.CommandText = "INSERT INTO Messages (MessageId, Subject, Body) VALUES (@MessageID, @Subject, @Body)";
                cmd.Parameters.Add(CreateParameter("MessageId", 0));
                cmd.Parameters.Add(CreateParameter("Subject", "Subj"));
                cmd.Parameters.Add(CreateParameter("Body", "Body"));
                
                foreach (var item in _uniqueIds)
                {
                    cmd.Parameters[0].Value = item;
                    cmd.Parameters[1].Value = Encoding.UTF8.GetBytes(String.Format("Subject: {0}", item));
                    cmd.Parameters[2].Value = Encoding.UTF8.GetBytes(String.Format("<body>Message: {0}</body>", item));

                    cmd.ExecuteNonQuery();
                }                
            }
        }

        /// <summary>
        /// Creates database parameter 
        /// </summary>
        protected DbParameter CreateParameter(string name, object value)
        {
            var param = Factory.CreateParameter();
            param.ParameterName = name;
            param.Value = value;

            return param;
        }

        /// <summary>
        /// Creates schema in the database
        /// </summary>
        protected virtual void BuildSchema(DbConnection conn)
        {
            foreach (var item in Schema)
                ExecuteStatement(conn, null, item);
        }

        /// <summary>
        /// Creates new database file
        /// </summary>
        protected abstract void CreateDatabase();

        /// <summary>
        /// Executes a simple SQL statement
        /// </summary>
        protected virtual void ExecuteStatement(DbConnection conn, DbTransaction tran, string sql)
        {
            using (var cmd = Factory.CreateCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Transaction = tran;
                cmd.CommandType = CommandType.Text;

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Returns resultset with records filtered by specified criteria
        /// </summary>
        protected abstract DbResultSet ExecuteResultSet(DbConnection conn, DbTransaction tran, string statement);

        /// <summary>
        /// Opens active connection to the database
        /// </summary>
        protected virtual DbConnection CreateConnection()
        {
            var conn = Factory.CreateConnection();
            conn.ConnectionString = ConnectionString;

            return conn;
        }

        /// <summary>
        /// Returns a value indicating whether database file exists
        /// </summary>
        public bool IsDatabaseExists(TestOptions options)
        {
            if (options == null)
                throw new ArgumentNullException("options");

            Options = options;

            return new FileInfo(DatabaseFile).Exists;
        }

        /// <summary>
        /// Gets factory instance for the provider
        /// </summary>
        protected abstract DbProviderFactory Factory
        {
            get;
        }
 
        /// <summary>
        /// Gets connection string
        /// </summary>
        protected virtual string ConnectionString
        {
            [DebuggerStepThrough]
            get { return String.Format("Data Source={0}", DatabaseFile); }
        }

        /// <summary>
        /// Gets path to the database file
        /// </summary>
        protected abstract String DatabaseFile
        {
            get;
        }

        /// <summary>
        /// Gets statemnts with database schema definition
        /// </summary>
        protected abstract IEnumerable<string> Schema
        {
            get;
        }

        /// <summary>
        /// Accesses fields in the record
        /// </summary>
        private void Touch(IDataRecord record)
        {
            for (int i = 0; i < record.FieldCount; i++)
                record.GetValue(i);
        }
    }
}
