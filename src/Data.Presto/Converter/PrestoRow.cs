using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Presto.Converter
{
    internal class PrestoRow : PrestoType
    {
        public List<KeyValuePair<string, PrestoType>> Properties { get; set; }

        public PrestoRow()
        {
            Properties = new List<KeyValuePair<string, PrestoType>>();
        }

        public override JToken Convert(JToken token)
        {

            if (token is JArray jArray)
            {
                JObject output = new JObject();

                for (int i = 0; i < Properties.Count; i++)
                {
                    var kv = Properties[i];
                    output.Add(kv.Key, kv.Value.Convert(jArray[i]));
                }
                return output;
            }

            return token;
        }
    }
}
