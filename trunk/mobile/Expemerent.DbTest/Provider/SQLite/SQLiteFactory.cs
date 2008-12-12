using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.Data.Common;

namespace Expemerent.DbTest.Provider.SQLite
{
    /// <summary>
    /// Simplified version of the DbProviderFactory for NETCF 
    /// </summary>
    internal class SQLiteFactory : DbProviderFactory
    {
        public override DbConnection CreateConnection()
        {
            return new SQLiteConnection();
        }

        public override DbCommand CreateCommand()
        {
            return new SQLiteCommand();
        }

        public override DbParameter CreateParameter()
        {
            return new SQLiteParameter();
        }
    }
}
