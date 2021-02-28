using Data.Presto.Decoders;
using Data.Presto.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Presto.Types
{
    class BooleanType : PrestoType
    {
        public override Type Type => typeof(bool);

        public override ColumnDecoder CreateDecoder()
        {
            return new BooleanDecoder();
        }
    }
}
