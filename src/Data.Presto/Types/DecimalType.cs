using Data.Presto.Decoders;
using Data.Presto.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Data.Presto.Types
{
    class DecimalType : PrestoType
    {
        public override Type Type => typeof(decimal);

        public override ColumnDecoder CreateDecoder()
        {
            return new DecimalDecoder();
        }
    }
}
