using Data.Presto.Decoders;
using Data.Presto.Models;
using Data.Presto.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Presto.Utils
{
    internal static class PrestoTypeHelper
    {
        internal static PrestoType GetPrestoType(string typeName)
        {
            if (typeName.StartsWith("varchar") || typeName.StartsWith("char") || typeName.Equals("ipaddress"))
            {
                return new VarcharType();
            }
            else if (typeName.Equals("boolean"))
            {
                return new BooleanType();
            }
            else if (typeName.Equals("tinyint"))
            {
                return new TinyintType();
            }
            else if (typeName.Equals("smallint"))
            {
                return new SmallintType();
            }
            else if (typeName.Equals("integer"))
            {
                return new IntegerType();
            }
            else if (typeName.Equals("bigint"))
            {
                return new BigintType();
            }
            else if (typeName.Equals("real"))
            {
                return new RealType();
            }
            else if (typeName.Equals("double"))
            {
                return new DoubleType();
            }
            else if (typeName.StartsWith("decimal"))
            {
                return new DecimalType();
            }
            else if (typeName.StartsWith("timestamp") || typeName.Equals("date"))
            {
                return new TimestampType();
            }
            else if (typeName.Equals("uuid"))
            {
                return new UuidType();
            }
            else if (typeName.StartsWith("array("))
            {
                var lastIndexOfParenthesis = typeName.LastIndexOf(')');
                var substring = typeName.Substring(6, lastIndexOfParenthesis - 6);
                var innerType = GetPrestoType(substring);
                return new ArrayType(innerType);
            }
            else if (typeName.StartsWith("row("))
            {
                var lastIndexOfParenthesis = typeName.LastIndexOf(')');
                var substring = typeName.Substring(4, lastIndexOfParenthesis - 4);
                List<PrestoColumn> columns = new List<PrestoColumn>();
                ParseRowSingleProperty(substring, columns);
                return new RowType(columns);
            }
            throw new NotImplementedException();
        }

        private static void ParseRowSingleProperty(string row, List<PrestoColumn> columns)
        {
            row = row.Trim();
            var whitespaceLocation = row.IndexOf(' ');

            var propertyName = row.Substring(0, whitespaceLocation);

            var nextCommaLocation = row.IndexOf(',');
            var lastClose = row.Length;

            if (nextCommaLocation == -1)
            {
                nextCommaLocation = int.MaxValue;
            }

            var nextLocation = (nextCommaLocation < lastClose) ? nextCommaLocation : lastClose;

            var propertyType = row.Substring(whitespaceLocation + 1, nextLocation - whitespaceLocation - 1);
            columns.Add(new PrestoColumn()
            {
                Name = propertyName,
                PrestoType = GetPrestoType(propertyType)
            });

            if (nextCommaLocation != int.MaxValue)
            {
                ParseRowSingleProperty(row.Substring(nextLocation + 1), columns);
            }
        }
    }
}
