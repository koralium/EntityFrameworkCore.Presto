using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Data.Presto.Decoders
{
    class BooleanDecoder : ColumnDecoder
    {
        private readonly List<bool?> values = new List<bool?>();

        public override int RowCount => values.Count;

        public override string GetDataTypeName()
        {
            return "boolean";
        }

        public override bool GetBoolean(in int index)
        {
            var val = values[index];
            if (val.HasValue)
            {
                return val.Value;
            }
            throw new ArgumentException($"The boolean value at index {index} was null.");
        }

        public override object GetFieldValue(in int index, Type type)
        {
            if (Equals(type, typeof(bool)))
            {
                return GetValue(index);
            }
            return Convert.ChangeType(GetValue(index), type);
        }

        public override object GetValue(in int index)
        {
            return GetBoolean(index);
        }

        public override bool IsDbNull(in int index)
        {
            return values[index] == null;
        }

        internal override void DecodeValue(ref Utf8JsonReader utf8JsonReader)
        {
            if(utf8JsonReader.TokenType == JsonTokenType.True)
            {
                values.Add(true);
            }
            else if (utf8JsonReader.TokenType == JsonTokenType.False)
            {
                values.Add(false);
            }
            else if (utf8JsonReader.TokenType == JsonTokenType.Null)
            {
                values.Add(null);
            }
            else
            {
                throw new Exception("Expected boolean value");
            }
        }

        internal override void NewBatch(Memory<byte> memory)
        {
            //Bools do not use the memory representation
        }

        public override Type GetFieldType()
        {
            return typeof(bool);
        }

        public override void WriteJson(in int index, in Utf8JsonWriter jsonWriter)
        {
            var val = values[index];
            if (!val.HasValue)
            {
                jsonWriter.WriteNullValue();
            }
            else
            {
                jsonWriter.WriteBooleanValue(val.Value);
            }
        }

        public override void AppendOffset()
        {
            values.Add(null);
        }
    }
}
