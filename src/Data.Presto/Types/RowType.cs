using Data.Presto.Decoders;
using Data.Presto.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Presto.Types
{
    class RowType : PrestoType
    {
        private readonly IReadOnlyList<PrestoColumn> _columns;

        public RowType(IReadOnlyList<PrestoColumn> columns)
        {
            _columns = columns;
        }

        public override Type Type => typeof(object);

        public override ColumnDecoder CreateDecoder()
        {
            return new RowDecoder(_columns);
        }
    }
}
