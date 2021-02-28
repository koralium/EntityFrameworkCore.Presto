using Data.Presto.Decoders;
using Data.Presto.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Presto.Types
{
    class RealType : PrestoType
    {
        public override Type Type => typeof(float);

        public override ColumnDecoder CreateDecoder()
        {
            return new RealDecoder();
        }
    }
}
