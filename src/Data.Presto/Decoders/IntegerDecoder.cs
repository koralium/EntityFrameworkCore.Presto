using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Data.Presto.Decoders
{
    class IntegerDecoder : PrimitiveDecoder<int>
    {
        protected override string DataTypeName => "integer";

        protected override int DecodeSingleValue(ref Utf8JsonReader reader)
        {
            return reader.GetInt32();
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
            var value = GetTVal(index);

            //Check bounds
            if (value > short.MaxValue)
            {
                throw new InvalidOperationException("Trying to get a int16 from a int32 value and the value is larger than int16 MaxValue");
            }
            if (value < short.MinValue)
            {
                throw new InvalidOperationException("Trying to get a int16 from a int32 value and the value is smaller than int16 MinValue");
            }

            return (short)value;
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
