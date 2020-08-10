using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;

namespace EntityFrameworkCore.Presto.Storage.Internal
{
    public class PrestoListTypeMapping : RelationalTypeMapping
    {

        public PrestoListTypeMapping(
            [NotNull] string storeType,
            [NotNull] Type clrType,
            DbType? dbType = null,
            bool unicode = false,
            int? size = null) 
            : base(storeType, clrType, dbType, unicode, size)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
