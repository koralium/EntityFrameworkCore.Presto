using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Data.Presto.Decoders
{
    class TinyintDecoder : PrimitiveDecoder<sbyte>
    {
        protected override string DataTypeName => "tinyint";

        protected override sbyte DecodeSingleValue(ref Utf8JsonReader reader)
        {
            return reader.GetSByte();
        }

        public override byte GetByte(in int index)
        {
            var value = GetTVal(index);

            if (value < byte.MinValue)
            {
                throw new InvalidOperationException("Trying to get a byte from a sbyte value and the value is smaller than byte minValue");
            }

            return (byte)value;
        }

        public override short GetInt16(in int index)
        {
            return GetTVal(index);
        }

        public override int GetInt32(in int index)
        {
            return GetTVal(index);
        }

        public override long GetInt64(in int index)
        {
            return GetTVal(index);
        }

        public override void WriteJson(in int index, in Utf8JsonWriter jsonWriter)
        {
            var val = Values[index];
            if (!val.HasValue)
            {
                jsonWriter.WriteNullValue();
            }
            else
            {
                jsonWriter.WriteNumberValue(val.Value);
            }
        }
    }
}
