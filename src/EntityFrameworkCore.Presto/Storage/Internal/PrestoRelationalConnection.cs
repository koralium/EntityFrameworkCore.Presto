using Data.Presto;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace EntityFrameworkCore.Presto.Storage.Internal
{
    public class PrestoRelationalConnection : RelationalConnection
    {
        public PrestoRelationalConnection([NotNull] RelationalConnectionDependencies dependencies) : base(dependencies)
        {
        }

        protected override DbConnection CreateDbConnection()
        {
            var conn = new PrestoConnection();
            conn.ConnectionString = this.ConnectionString;
            return conn;
        }
    }
}
