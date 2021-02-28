using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Data.Presto.Decoders
{
    class UuidDecoder : PrimitiveDecoder<Guid>
    {
        protected override string DataTypeName => "uuid";

        protected override Guid DecodeSingleValue(ref Utf8JsonReader reader)
        {
            return reader.GetGuid();
        }

        public override Guid GetGuid(in int index)
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
                jsonWriter.WriteStringValue(val.Value);
            }
        }
    }
}
