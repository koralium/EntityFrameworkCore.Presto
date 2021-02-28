using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Presto.Utils
{
    internal static class JsonWriteUtils
    {
        public static readonly byte[] StartObjectCharacter = Encoding.UTF8.GetBytes("{");
        public static readonly byte[] Quote = Encoding.UTF8.GetBytes("\"");
        public static readonly byte[] Colon = Encoding.UTF8.GetBytes(":");
        public static readonly byte[] Comma = Encoding.UTF8.GetBytes(",");
        public static readonly byte[] EndObjectCharacter = Encoding.UTF8.GetBytes("}");
    }
}
