using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Data.Presto.Decoders
{
    class RealDecoder : PrimitiveDecoder<float>
    {
        protected override string DataTypeName => "real";

        protected override float DecodeSingleValue(ref Utf8JsonReader reader)
        {
            return reader.GetSingle();
        }

        public override float GetFloat(in int index)
        {
            return GetTVal(index);
        }

        public override double GetDouble(in int index)
        {
            return GetTVal(index);
        }

        public override decimal GetDecimal(in int index)
        {
            return (decimal)GetTVal(index);
        }
    }
}
