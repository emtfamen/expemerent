using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace Expemerent.DbTest.Provider
{
    /// <summary>
    /// .NETCF provider factory implementation
    /// </summary>
    internal abstract class DbProviderFactory
    {
        /// <summary>
        /// Creates a new connection
        /// </summary>
        public abstract DbConnection CreateConnection();

        /// <summary>
        /// Creates a new command
        /// </summary>
        public abstract DbCommand CreateCommand();

        /// <summary>
        /// Creates a new parameter
        /// </summary>
        public abstract DbParameter CreateParameter();
    }
}
