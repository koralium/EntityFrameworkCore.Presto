using Data.Presto.Decoders;
using Data.Presto.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Presto.Types
{
    class DoubleType : PrestoType
    {
        public override Type Type => typeof(double);

        public override ColumnDecoder CreateDecoder()
        {
            return new DoubleDecoder();
        }
    }
}
