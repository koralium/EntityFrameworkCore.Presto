using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace EntityFrameworkCore.Presto.Storage.Internal
{
    public class PrestoTypeMappingSource : RelationalTypeMappingSource
    {
        private const string VarcharTypeName = "VARCHAR";
        private static readonly PrestoStringTypeMapping _text = new PrestoStringTypeMapping(VarcharTypeName);


        private readonly Dictionary<Type, RelationalTypeMapping> _clrTypeMappings
            = new Dictionary<Type, RelationalTypeMapping>
            {
                { typeof(string), _text },
                { typeof(long), new LongTypeMapping("bigint", System.Data.DbType.Int64) }
            };

        public PrestoTypeMappingSource(TypeMappingSourceDependencies dependencies, 
            RelationalTypeMappingSourceDependencies relationalDependencies) : base(dependencies, relationalDependencies)
        {
        }

        protected override RelationalTypeMapping FindMapping(in RelationalTypeMappingInfo mappingInfo)
        {
            var clrType = mappingInfo.ClrType;


            if(mappingInfo.StoreTypeName != null)
            {
                if(mappingInfo.StoreTypeName == "row")
                {
                    return new ObjectTypeMapping("row", clrType);
                }
                if(mappingInfo.StoreTypeName == "array")
                {
                    return new PrestoListTypeMapping("array", clrType);
                }
            }

            if (clrType != null
                && _clrTypeMappings.TryGetValue(clrType, out var mapping))
            {
                return mapping;
            }

            var o = base.FindMapping(mappingInfo);

            return o;
        }
    }
}
