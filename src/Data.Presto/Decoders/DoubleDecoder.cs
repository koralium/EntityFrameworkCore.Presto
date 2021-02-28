using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Data.Presto.Decoders
{
    class DoubleDecoder : PrimitiveDecoder<double>
    {
        protected override string DataTypeName => "double";

        protected override double DecodeSingleValue(ref Utf8JsonReader reader)
        {
            return reader.GetDouble();
        }

        public override float GetFloat(in int index)
        {
            return (float)GetTVal(index);
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
