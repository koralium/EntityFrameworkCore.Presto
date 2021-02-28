using Data.Presto.Decoders;
using Data.Presto.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Presto.Types
{
    class IntegerType : PrestoType
    {
        public override Type Type => typeof(int);

        public override ColumnDecoder CreateDecoder()
        {
            return new IntegerDecoder();
        }
    }
}
