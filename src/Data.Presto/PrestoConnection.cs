using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Text;

namespace Data.Presto
{
    public class PrestoConnection : DbConnection
    {
        private ConnectionState _state;
        private string _connectionString = string.Empty;
        private readonly List<WeakReference<PrestoCommand>> _commands = new List<WeakReference<PrestoCommand>>();

        internal PrestoConnectionStringBuilder ConnectionOptions { get; set; }

        public override string ConnectionString
        {
            get => _connectionString;
            set
            {
                if (State != ConnectionState.Closed)
                {
                    throw new InvalidOperationException();
                }
                _connectionString = value ?? string.Empty;
                ConnectionOptions = new PrestoConnectionStringBuilder(value);
            }
        }

        public override string Database => ConnectionOptions.Catalog;

        public override string DataSource => ConnectionOptions.DataSource;

        public override string ServerVersion => string.Empty;

        public override ConnectionState State => _state;

        public override void ChangeDatabase(string databaseName)
        {
            ConnectionOptions.Catalog = databaseName;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }

            base.Dispose(disposing);
        }

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
                    _commands.RemoveAt(i);
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
            var command = new PrestoCommand
            {
                Connection = this,
                CommandTimeout = DefaultTimeout,
                Transaction = Transaction
            };
            _commands.Add(new WeakReference<PrestoCommand>(command));
            return command;
        }

        public virtual int DefaultTimeout { get; set; } = 120;

        protected internal virtual PrestoTransaction Transaction { get; set; }
    }
}
