using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Presto.Converter
{
    internal class PrestoArray : PrestoType
    {
        private readonly PrestoType _subType;
        public PrestoArray(PrestoType subType)
        {
            _subType = subType;
        }

        public override JToken Convert(JToken token)
        {
            if (token is JArray array)
            {
                JArray output = new JArray();

                for (int i = 0; i < array.Count; i++)
                {
                    output.Add(_subType.Convert(array[i]));
                }

                return output;
            }
            return base.Convert(token);
        }
    }
}
