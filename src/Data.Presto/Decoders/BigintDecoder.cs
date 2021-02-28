using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Data.Presto.Decoders
{
    class BigintDecoder : PrimitiveDecoder<long>
    {
        protected override string DataTypeName => "bigint";

        protected override long DecodeSingleValue(ref Utf8JsonReader reader)
        {
            return reader.GetInt64();
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
            var value = GetTVal(index);

            //Check bounds
            if (value > int.MaxValue)
            {
                throw new InvalidOperationException("Trying to get a int32 from a int64 value and the value is larger than int32 MaxValue");
            }
            if (value < int.MinValue)
            {
                throw new InvalidOperationException("Trying to get a int32 from a int64 value and the value is smaller than int32 MinValue");
            }

            return (int)value;
        }

        public override long GetInt64(in int index)
        {
            return GetTVal(index);
        }
    }
}
