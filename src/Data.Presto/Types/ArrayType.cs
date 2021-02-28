using Data.Presto.Decoders;
using Data.Presto.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Data.Presto.Types
{
    class ArrayType : PrestoType
    {
        private readonly PrestoType _innerType;
        public override Type Type => typeof(IList);

        public ArrayType(PrestoType innerType)
        {
            _innerType = innerType;
        }

        public override ColumnDecoder CreateDecoder()
        {
            return new ArrayDecoder(_innerType.CreateDecoder());
        }
    }
}
