using Data.Presto.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Text;

namespace Data.Presto
{
    public class PrestoDataReader : DbDataReader
    {
        private readonly PrestoCommandExecutor _prestoCommandExecutor;
        private readonly bool _closeConnection;
        private readonly Stopwatch _timer;
        private bool _closed;
        private int _recordsAffected = -1;
        private List<object> currentInstance;

        internal PrestoDataReader(PrestoCommandExecutor prestoCommandExecutor)
        {
            _prestoCommandExecutor = prestoCommandExecutor;
            _prestoCommandExecutor.GetReader();
        }

        public override object this[int ordinal] => throw new NotImplementedException();

        public override object this[string name] => throw new NotImplementedException();

        public override int Depth => throw new NotImplementedException();

        public override int FieldCount => _prestoCommandExecutor.GetFieldCount().Result;

        public override bool HasRows => true;

        public override bool IsClosed => _prestoCommandExecutor.GetReader().Completion.IsCompleted;

        public override int RecordsAffected => -1;

        public override bool GetBoolean(int ordinal)
        {
            return Convert.ToBoolean(currentInstance[ordinal]);
            throw new NotImplementedException();
        }

        public override byte GetByte(int ordinal)
        {
            return Convert.ToByte(currentInstance[ordinal]);
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override char GetChar(int ordinal)
        {
            return Convert.ToChar(currentInstance[ordinal]);
            throw new NotImplementedException();
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override string GetDataTypeName(int ordinal)
        {
            return _prestoCommandExecutor.GetDataTypeName(ordinal).Result;
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return Convert.ToDateTime(currentInstance[ordinal]);
        }

        public override decimal GetDecimal(int ordinal)
        {
            return Convert.ToDecimal(currentInstance[ordinal]);
        }

        public override double GetDouble(int ordinal)
        {
            return Convert.ToDouble(currentInstance[ordinal]);
        }

        public override IEnumerator GetEnumerator()
        {
            return new DbEnumerator(this, closeReader: false);
        }

        public override Type GetFieldType(int ordinal)
        {
            return _prestoCommandExecutor.GetFieldType(ordinal).Result;
        }

        public override float GetFloat(int ordinal)
        {
            return Convert.ToSingle(currentInstance[ordinal]);
        }

        public override Guid GetGuid(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override short GetInt16(int ordinal)
        {
            return Convert.ToInt16(currentInstance[ordinal]);
        }

        public override int GetInt32(int ordinal)
        {
            return Convert.ToInt32(currentInstance[ordinal]);
        }

        public override long GetInt64(int ordinal)
        {
            return Convert.ToInt64(currentInstance[ordinal]);
        }

        public override string GetName(int ordinal)
        {
            return _prestoCommandExecutor.GetColumnName(ordinal).Result;
        }

        public override int GetOrdinal(string name)
        {
            return _prestoCommandExecutor.GetOrdinal(name).Result;
        }

        public override string GetString(int ordinal)
        {
            return Convert.ToString(currentInstance[ordinal]);
        }

        public override object GetValue(int ordinal)
        {
            var o = currentInstance[ordinal];

            return o;
        }

        public override int GetValues(object[] values)
        {
            int i = 0;
            for (; i < values.Length && i < currentInstance.Count; i++)
            {
                values[i] = currentInstance[i];
            }
            return i;
        }

        public override T GetFieldValue<T>(int ordinal)
        {
            var o = currentInstance[ordinal];

            return _prestoCommandExecutor.ConvertValue<T>(ordinal, o);
        }

        public override bool IsDBNull(int ordinal)
        {
            var o = currentInstance[ordinal];
            return o == null;
        }

        public override bool NextResult()
        {
            return false;
        }

        public override bool Read()
        {
            while(true)
            {
                if (_prestoCommandExecutor.GetReader().Completion.IsCompleted)
                {
                    return false;
                }
                if (_prestoCommandExecutor.GetReader().TryRead(out var obj))
                {
                    currentInstance = obj;
                    return true;
                }
            }
        }
    }
}
