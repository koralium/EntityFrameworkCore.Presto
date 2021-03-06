﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Data.Presto.Decoders
{
    class TimestampDecoder : PrimitiveDecoder<DateTime>
    {
        protected override string DataTypeName => throw new NotImplementedException();

        protected override DateTime DecodeSingleValue(ref Utf8JsonReader reader)
        {
            var dateValue = reader.GetString();
            return Convert.ToDateTime(dateValue);
        }

        public override DateTime GetDateTime(in int index)
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
