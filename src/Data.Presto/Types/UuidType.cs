using Data.Presto.Decoders;
using Data.Presto.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Presto.Types
{
    class UuidType : PrestoType
    {
        public override Type Type => typeof(Guid);

        public override ColumnDecoder CreateDecoder()
        {
            return new UuidDecoder();
        }
    }
}
