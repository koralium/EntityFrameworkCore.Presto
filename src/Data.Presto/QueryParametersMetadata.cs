using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Presto
{
    public class QueryParametersMetadata
    {
        public Dictionary<string, int> ParameterNameToIndex { get; set; } = new Dictionary<string, int>();

        public string Query { get; set; }
    }
}
