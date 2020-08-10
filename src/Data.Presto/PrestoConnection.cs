using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace Data.Presto
{
    public class PrestoConnection : DbConnection
    {
        private string _connectionString = string.Empty;
        private ConnectionState _state;
        private readonly List<WeakReference<PrestoCommand>> _commands = new List<WeakReference<PrestoCommand>>();

        internal PrestoConnectionStringBuilder ConnectionOptions { get; set; }

        public override string ConnectionString 
        {
            get => _connectionString;
            set
            {
                if(State != ConnectionState.Closed)
                {
                    throw new InvalidOperationException();
                }
                _connectionString = value ?? string.Empty;
                ConnectionOptions = new PrestoConnectionStringBuilder(value);
            }
        }

        public override string Database => throw new NotImplementedException();

        public override string DataSource => ConnectionOptions.DataSource;

        public override string ServerVersion => string.Empty;

        public override ConnectionState State => _state;

        public override void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        protected override DbProviderFactory DbProviderFactory => PrestoFactory.Instance;

        public override void Close()
        {
            if (State != ConnectionState.Open)
            {
                return;
            }

            for (var i = _commands.Count - 1; i >= 0; i--)
            {
                var reference = _commands[i];
                if (reference.TryGetTarget(out var command))
                {
                    // NB: Calls RemoveCommand()
                    command.Dispose();
                }
                else
                {
                    _commands.RemoveAt(i);
                }
            }

            Debug.Assert(_commands.Count == 0);
            _state = ConnectionState.Closed;
            OnStateChange(new StateChangeEventArgs(ConnectionState.Open, ConnectionState.Closed));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }

            base.Dispose(disposing);
        }

        public override void Open()
        {
            if (State == ConnectionState.Open)
            {
                return;
            }

            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new InvalidOperationException("No connection string set.");
            }

            _state = ConnectionState.Open;

            OnStateChange(new StateChangeEventArgs(ConnectionState.Closed, ConnectionState.Open));
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return Transaction = new PrestoTransaction(this, isolationLevel);
        }

        protected override DbCommand CreateDbCommand()
        {
            return new PrestoCommand
            {
                Connection = this,
                CommandTimeout = DefaultTimeout,
                Transaction = Transaction
            };
        }

        public virtual int DefaultTimeout { get; set; } = 120;

        protected internal virtual PrestoTransaction Transaction { get; set; }
    }
}
