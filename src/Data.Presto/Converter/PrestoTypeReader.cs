using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Presto.Converter
{
    class PrestoTypeReader
    {
        public static PrestoType ParseType(string type)
        {
            if (type.StartsWith("row("))
            {
                var lastIndexOfParenthesis = type.LastIndexOf(')');
                var substring = type.Substring(4, lastIndexOfParenthesis - 4);
                PrestoRow prestoRow = new PrestoRow();
                ParseRowSingleProperty(substring, prestoRow);

                return prestoRow;
            }

            if (type.StartsWith("array("))
            {
                var lastIndexOfParenthesis = type.LastIndexOf(')');

                var substring = type.Substring(6, lastIndexOfParenthesis - 6);
                var subType = ParseType(substring);

                return new PrestoArray(subType);
            }

            return new PrestoType();
        }

        public static void ParseRowSingleProperty(string row, PrestoRow prestoRow)
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
            prestoRow.Properties.Add(new KeyValuePair<string, PrestoType>(propertyName, ParseType(propertyType)));

            if (nextCommaLocation != int.MaxValue)
            {
                ParseRowSingleProperty(row.Substring(nextLocation + 1), prestoRow);
            }
        }
    }
}
