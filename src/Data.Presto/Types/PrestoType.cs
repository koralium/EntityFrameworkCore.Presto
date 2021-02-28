using Data.Presto.Decoders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Presto.Models
{
    internal abstract class PrestoType
    {
        public abstract Type Type { get; }
        public abstract ColumnDecoder CreateDecoder();
    }
}
