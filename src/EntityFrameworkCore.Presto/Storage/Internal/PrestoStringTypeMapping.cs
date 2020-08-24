using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace EntityFrameworkCore.Presto.Storage.Internal
{
    public class PrestoStringTypeMapping : StringTypeMapping
    {
        public PrestoStringTypeMapping(string storeType, DbType? dbType = null, bool unicode = false, int? size = null) : base(storeType, dbType, unicode, size)
        {
        }

        protected PrestoStringTypeMapping(RelationalTypeMappingParameters parameters) : base(parameters)
        {
        }
    }
}
