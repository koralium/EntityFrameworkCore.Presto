using Data.Presto.Decoders;
using Data.Presto.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Presto.Types
{
    class TimestampType : PrestoType
    {
        public override Type Type => typeof(DateTime);

        public override ColumnDecoder CreateDecoder()
        {
            return new TimestampDecoder();
        }
    }
}
