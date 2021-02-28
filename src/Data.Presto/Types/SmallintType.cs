using Data.Presto.Decoders;
using Data.Presto.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Presto.Types
{
    class SmallintType : PrestoType
    {
        public override Type Type => typeof(short);

        public override ColumnDecoder CreateDecoder()
        {
            return new SmallintDecoder();
        }
    }
}
