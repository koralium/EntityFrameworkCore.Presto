using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Data.Presto.Decoders
{
    class DecimalDecoder : PrimitiveDecoder<decimal>
    {
        protected override string DataTypeName => "decimal";

        protected override decimal DecodeSingleValue(ref Utf8JsonReader reader)
        {
            var val = reader.GetString();

            if (!decimal.TryParse(val, out var v))
            {
                throw new InvalidOperationException($"{val} is not a decimal value");
            }

            return v;
        }

        public override float GetFloat(in int index)
        {
            return (float)GetTVal(index);
        }

        public override double GetDouble(in int index)
        {
            return (double)GetTVal(index);
        }

        public override decimal GetDecimal(in int index)
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
