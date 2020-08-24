using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace EntityFrameworkCore.Presto.Storage.Internal
{
    public class PrestoLongTypeMapping : LongTypeMapping
    {
        public PrestoLongTypeMapping(string storeType, DbType? dbType = null) : base(storeType, dbType)
        {
        }

        protected PrestoLongTypeMapping(RelationalTypeMappingParameters parameters) : base(parameters)
        {
        }
    }
}
