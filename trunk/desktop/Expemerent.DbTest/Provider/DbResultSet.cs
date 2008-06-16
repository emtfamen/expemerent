using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;

namespace Expemerent.DbTest.Provider
{
    internal abstract class DbResultSet : IDisposable
    {
        /// <summary>
        /// Reads records from absolute position
        /// </summary>
        public abstract void ReadAbsolute(int position, int count, Action<IDataRecord> process);

        /// <summary>
        /// Gets amount of records in the resultset
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// Releases resources
        /// </summary>
        public abstract void Dispose();
    }
}
