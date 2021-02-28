using Data.Presto.Decoders;
using Data.Presto.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Presto.Types
{
    class TinyintType : PrestoType
    {
        public override Type Type => typeof(sbyte);

        public override ColumnDecoder CreateDecoder()
        {
            return new TinyintDecoder();
        }
    }
}
