using Data.Presto.Decoders;
using Data.Presto.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Presto.Types
{
    class BigintType : PrestoType
    {
        public override Type Type => typeof(long);

        public override ColumnDecoder CreateDecoder()
        {
            return new BigintDecoder();
        }
    }
}
