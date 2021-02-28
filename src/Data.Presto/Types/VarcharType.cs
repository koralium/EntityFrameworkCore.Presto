using Data.Presto.Decoders;
using Data.Presto.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Presto.Types
{
    class VarcharType : PrestoType
    {
        public override Type Type => typeof(string);

        public override ColumnDecoder CreateDecoder()
        {
            return new VarcharDecoder();
        }
    }
}
