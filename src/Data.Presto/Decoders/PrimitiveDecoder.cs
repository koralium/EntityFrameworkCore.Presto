using Data.Presto.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Data.Presto.Decoders
{
    internal abstract class PrimitiveDecoder<T> : ColumnDecoder
        where T : struct
    {
        protected List<T?> Values { get; } = new List<T?>();

        protected abstract string DataTypeName { get; }

        public override string GetDataTypeName()
        {
            return DataTypeName;
        }

        public override object GetFieldValue(in int index, Type type)
        {
            var val = GetValue(index);
            if (Equals(type, typeof(T)))
            {
                return val;
            }
            return Convert.ChangeType(val, type);
        }

        public override object GetValue(in int index)
        {
            return Values[index];
        }

        public override bool IsDbNull(in int index)
        {
            return Values[index] == null;
        }

        protected T GetTVal(in int index)
        {
            var v = Values[index];
            if (!v.HasValue)
            {
                throw new InvalidOperationException("Tried to get a value, but the column value is null");
            }
            return v.Value;
        }

        protected abstract T DecodeSingleValue(ref Utf8JsonReader reader);

        internal override void DecodeValue(ref Utf8JsonReader utf8JsonReader)
        {
            if (utf8JsonReader.TokenType == JsonTokenType.Null)
            {
                Values.Add(null);
                return;
            }
            Values.Add(DecodeSingleValue(ref utf8JsonReader));
        }

        internal override void NewBatch(Memory<byte> memory)
        {
        }

        public override int RowCount => Values.Count;

        public override Type GetFieldType()
        {
            return typeof(T);
        }

        public override void AppendOffset()
        {
            Values.Add(null);
        }
    }
}
