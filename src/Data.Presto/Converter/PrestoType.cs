using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Presto.Converter
{
    internal class PrestoType
    {
        public virtual JToken Convert(JToken token)
        {
            return token;
        }
    }
}
