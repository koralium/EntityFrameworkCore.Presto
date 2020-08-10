using Data.Presto.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Data.Presto
{
    public class PrestoCommand : DbCommand
    {
        private PrestoParameterCollection _parameters;

        private PrestoConnection _prestoConnection;
        private HttpClient _httpClient = new HttpClient();
        public override string CommandText { get; set; }
        public override int CommandTimeout { get; set; }
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
        public override bool DesignTimeVisible { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override UpdateRowSource UpdatedRowSource { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        protected override DbConnection DbConnection 
        {
            get
            {
                return _prestoConnection;
            }
            set
            {
                _prestoConnection = (PrestoConnection)value;
            }
        }

        internal PrestoConnection PrestoConnection => _prestoConnection;

        public new virtual PrestoParameterCollection Parameters
            => _parameters ??= new PrestoParameterCollection();


        protected override DbParameterCollection DbParameterCollection => Parameters;

        protected override DbTransaction DbTransaction { get; set; }

        public override void Cancel()
        {
            throw new NotImplementedException();
        }

        public override int ExecuteNonQuery()
        {
            throw new NotImplementedException();
        }

        public override object ExecuteScalar()
        {
            throw new NotImplementedException();
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
            return new PrestoDataReader(PrestoCommandExecutor.Execute(this).Result);
        }
    }
}
