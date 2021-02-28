using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Data.Presto.Decoders
{
    class SmallintDecoder : PrimitiveDecoder<short>
    {
        protected override string DataTypeName => "smallint";

        protected override short DecodeSingleValue(ref Utf8JsonReader reader)
        {
            return reader.GetInt16();
        }

        public override byte GetByte(in int index)
        {
            var value = GetTVal(index);

            if (value > byte.MaxValue)
            {
                throw new InvalidOperationException("Trying to get a int16 from a int32 value and the value is larger than byte MaxValue");
            }
            if (value < byte.MinValue)
            {
                throw new InvalidOperationException("Trying to get a int16 from a int32 value and the value is smaller than byte MinValue");
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
