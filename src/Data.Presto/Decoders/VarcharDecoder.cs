using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Data.Presto.Decoders
{
    class VarcharDecoder : ColumnDecoder
    {
        private List<string> values = new List<string>();

        public override int RowCount => values.Count;

        public override string GetDataTypeName()
        {
            return "varchar";
        }

        internal override void DecodeValue(ref Utf8JsonReader utf8JsonReader)
        {
            values.Add(utf8JsonReader.GetString());
        }

        internal override void NewBatch(Memory<byte> memory)
        {
        }

        public override string GetString(in int index)
        {
            return values[index];
        }

        public override object GetFieldValue(in int index, Type type)
        {
            var val = GetString(index);
            if (Equals(type, typeof(string)))
            {
                return val;
            }
            return Convert.ChangeType(val, type);
        }

        public override object GetValue(in int index)
        {
            return GetString(index);
        }

        public override bool IsDbNull(in int index)
        {
            return values[index] == null;
        }

        public override Type GetFieldType()
        {
            return typeof(string);
        }

        public override void WriteJson(in int index, in Utf8JsonWriter jsonWriter)
        {
            var val = values[index];
            if (val == null)
            {
                jsonWriter.WriteNullValue();
            }
            else
            {
                jsonWriter.WriteStringValue(val);
            }
        }

        public override void AppendOffset()
        {
            values.Add(null);
        }
    }
}
