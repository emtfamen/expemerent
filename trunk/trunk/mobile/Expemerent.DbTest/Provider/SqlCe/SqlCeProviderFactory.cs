using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlServerCe;
using System.Data.Common;

namespace Expemerent.DbTest.Provider.SqlCe
{
    internal class SqlCeProviderFactory : DbProviderFactory
    {
        public override DbConnection CreateConnection()
        {
            return new SqlCeConnection();
        }

        public override DbCommand CreateCommand()
        {
            return new SqlCeCommand();
        }

        public override DbParameter CreateParameter()
        {
            return new SqlCeParameter();
        }
    }
}
