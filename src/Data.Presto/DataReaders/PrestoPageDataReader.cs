using Data.Presto.Decoders;
using Data.Presto.Models;
using Data.Presto.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Data.Presto.DataReaders
{
    /// <summary>
    /// Reads a single presto page
    /// </summary>
    class PrestoPageDataReader : DbDataReader
    {
        private readonly string[] _columnNames;
        private readonly Dictionary<string, int> _nameToOrdinal;
        private int currentIndex = -1;
        private readonly int _size;

        private readonly IReadOnlyList<ColumnDecoder> _columnDecoders;

        public PrestoPageDataReader(IReadOnlyList<PrestoColumn> columns, int size, IReadOnlyList<ColumnDecoder> columnDecoders)
        {
            _size = size;
            _columnNames = columns.Select(x => x.Name).ToArray();

            _nameToOrdinal = new Dictionary<string, int>();
            for (int i = 0; i < _columnNames.Length; i++)
            {
                _nameToOrdinal.Add(_columnNames[i], i);
            }

            _columnDecoders = columnDecoders;
        }

        public override object this[int ordinal] => GetValue(ordinal);

        public override object this[string name] => GetValue(GetOrdinal(name));

        public override int Depth => 0;

        public override int FieldCount => _columnNames.Length;

        public override bool HasRows => _size > 0;

        public override bool IsClosed => false;

        public override int RecordsAffected => -1;

        public override bool GetBoolean(int ordinal)
        {
            return _columnDecoders[ordinal].GetBoolean(currentIndex);
        }

        public override byte GetByte(int ordinal)
        {
            return _columnDecoders[ordinal].GetByte(currentIndex);
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override char GetChar(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override string GetDataTypeName(int ordinal)
        {
            return _columnDecoders[ordinal].GetDataTypeName();
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return _columnDecoders[ordinal].GetDateTime(currentIndex);
        }

        public override decimal GetDecimal(int ordinal)
        {
            return _columnDecoders[ordinal].GetDecimal(currentIndex);
        }

        public override double GetDouble(int ordinal)
        {
            return _columnDecoders[ordinal].GetDouble(currentIndex);
        }

        public override IEnumerator GetEnumerator()
        {
            return new DbEnumerator(this, false);
        }

        public override Type GetFieldType(int ordinal)
        {
            return _columnDecoders[ordinal].GetFieldType();
        }

        public override float GetFloat(int ordinal)
        {
            return _columnDecoders[ordinal].GetFloat(currentIndex);
        }

        public override Guid GetGuid(int ordinal)
        {
            return _columnDecoders[ordinal].GetGuid(currentIndex);
        }

        public override short GetInt16(int ordinal)
        {
            return _columnDecoders[ordinal].GetInt16(currentIndex);
        }

        public override int GetInt32(int ordinal)
        {
            return _columnDecoders[ordinal].GetInt32(currentIndex);
        }

        public override long GetInt64(int ordinal)
        {
            return _columnDecoders[ordinal].GetInt64(currentIndex);
        }

        public override string GetName(int ordinal)
        {
            return _columnNames[ordinal];
        }

        public override int GetOrdinal(string name)
        {
            if (_nameToOrdinal.TryGetValue(name, out int ordinal))
            {
                return ordinal;
            }
            throw new InvalidOperationException($"Column '{name}' does not exist in the result.");
        }

        public override string GetString(int ordinal)
        {
            return _columnDecoders[ordinal].GetString(currentIndex);
        }

        public override object GetValue(int ordinal)
        {
            return _columnDecoders[ordinal].GetValue(currentIndex);
        }

        public override int GetValues(object[] values)
        {
            if (values.Length < _columnDecoders.Count)
            {
                throw new InvalidOperationException("The values array is smaller than the number of columns");
            }

            for (int i = 0; i < _columnDecoders.Count; i++)
            {
                values[i] = GetValue(i);
            }

            return _columnDecoders.Count;
        }

        public override bool IsDBNull(int ordinal)
        {
            return _columnDecoders[ordinal].IsDbNull(currentIndex);
        }

        public override T GetFieldValue<T>(int ordinal)
        {
            return _columnDecoders[ordinal].GetFieldValue<T>(currentIndex);
        }

        public void ToJson(Utf8JsonWriter jsonWriter)
        {
            jsonWriter.WriteStartObject();
            for (int i = 0; i < _columnNames.Length; i++)
            {
                jsonWriter.WritePropertyName(_columnNames[i]);
                _columnDecoders[i].WriteJson(currentIndex, jsonWriter);
            }
            jsonWriter.WriteEndObject();
        }

        public override bool NextResult()
        {
            return false;
        }

        public override bool Read()
        {
            currentIndex++;
            if (currentIndex >= _size)
            {
                return false;
            }
            return true;
        }
    }
}
