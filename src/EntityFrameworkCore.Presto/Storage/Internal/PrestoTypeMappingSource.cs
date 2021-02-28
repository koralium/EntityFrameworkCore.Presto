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
                { typeof(long), new LongTypeMapping("bigint", System.Data.DbType.Int64) },
                { typeof(double), new DoubleTypeMapping("double", System.Data.DbType.Double) },
                { typeof(short), new ShortTypeMapping("short", System.Data.DbType.Int16) },
                { typeof(int), new IntTypeMapping("integer", System.Data.DbType.Int32)},
                { typeof(sbyte), new SByteTypeMapping("sbyte", System.Data.DbType.SByte) },
                { typeof(byte), new ByteTypeMapping("byte", System.Data.DbType.Byte) },
                { typeof(DateTime), new DateTimeTypeMapping("timestamp", System.Data.DbType.DateTime) },
                { typeof(bool), new BoolTypeMapping("boolean", System.Data.DbType.Boolean) },
                { typeof(decimal), new DecimalTypeMapping("decimal", System.Data.DbType.Decimal) },
                { typeof(float), new FloatTypeMapping("real", System.Data.DbType.Single) },
                { typeof(Guid), new GuidTypeMapping("uuid", System.Data.DbType.Guid) }
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
