using Data.Presto.Client;
using Data.Presto.DataReaders;
using Data.Presto.Internal;
using Data.Presto.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Presto
{
    public class PrestoCommand : DbCommand
    {
        private PrestoParameterCollection _parameters;

        public override string CommandText { get; set; }
        public override int CommandTimeout { get; set; }

        internal DbDataReader DataReader { get; private set; }

        public override CommandType CommandType
        {
            get => CommandType.Text;
            set
            {
                if (value != CommandType.Text)
                {
                    throw new ArgumentException();
                }
            }
        }

        public override bool DesignTimeVisible { get; set; }
        public override UpdateRowSource UpdatedRowSource { get; set; }
        protected override DbConnection DbConnection
        {
            get
            {
                return PrestoConnection;
            }
            set
            {
                PrestoConnection = (PrestoConnection)value;
            }
        }

        internal PrestoConnection PrestoConnection { get; private set; }

        public new virtual PrestoParameterCollection Parameters
        {
            get
            {
                if (_parameters == null)
                {
                    _parameters = new PrestoParameterCollection();
                }
                return _parameters;
            }
        }

        protected override DbParameterCollection DbParameterCollection => Parameters;

        protected override DbTransaction DbTransaction { get; set; }

        public override void Cancel()
            => Dispose(true);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DataReader?.Dispose();
                
            }
        }

        public override int ExecuteNonQuery()
        {
            using var reader = ExecuteDbDataReader(CommandBehavior.Default);
            return reader.RecordsAffected;
        }

        public override object ExecuteScalar()
        {
            using var reader = ExecuteDbDataReader(CommandBehavior.Default);

            return reader.Read()
                ? reader.GetValue(0)
                : null;
        }

        public override void Prepare()
        {
            throw new NotImplementedException();
        }

        protected override DbParameter CreateDbParameter()
        {
            return new PrestoParameter();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(CommandTimeout));
            return AsyncHelper.RunSync(() => ExecuteDbDataReaderAsync(behavior, tokenSource.Token));
        }

        protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
        {
            //Check so no other data reader has been open with this command
            if (DataReader != null)
            {
                throw new InvalidOperationException("Another data reader has already been opened");
            }

            var client = new PrestoClient(PrestoConnection.ConnectionOptions);

            var sqlQuery = ParameterUtils.DeparameterizeStatement(this);
            var decodeResult = await client.Query(sqlQuery, cancellationToken);

            if(decodeResult.State == Models.PrestoState.Failed)
            {
                throw new InvalidOperationException(decodeResult.ErrorMessage);
            }

            DataReader = new PrestoDataReader(decodeResult, client, PrestoConnection.ConnectionOptions.Streaming);

            return DataReader;
        }
    }
}
