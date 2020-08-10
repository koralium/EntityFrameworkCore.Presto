﻿using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;

namespace EntityFrameworkCore.Presto.Storage.Internal
{
    public class ObjectTypeMapping : RelationalTypeMapping
    {

        public ObjectTypeMapping([NotNullAttribute] string storeType, [NotNullAttribute] Type clrType, DbType? dbType = null, bool unicode = false, int? size = null) 
            : base(storeType, clrType, dbType, unicode, size)
        {
        }

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        {
            throw new NotImplementedException();
        }

    }
}
