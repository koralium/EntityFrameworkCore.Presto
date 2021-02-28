using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Data.Presto
{
    public class PrestoFactory : DbProviderFactory
    {
        public PrestoFactory()
        {
        }

        public static readonly PrestoFactory Instance = new PrestoFactory();

        public override DbCommand CreateCommand()
            => new PrestoCommand();

        public override DbConnection CreateConnection()
            => new PrestoConnection();

        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
            => new PrestoConnectionStringBuilder();

        public override DbParameter CreateParameter()
            => new PrestoParameter();
    }
}
