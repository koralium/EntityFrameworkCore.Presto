using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Data.Presto.Decoders
{
    class VarcharDecoder : ColumnDecoder
    {
        //private static readonly UTF8Encoding s_utf8Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
        //private List<ByteInterval> intervals = new List<ByteInterval>();
        //private Memory<byte> _memory;
        private List<string> values = new List<string>();

        public override int RowCount => values.Count;

        public override string GetDataTypeName()
        {
            return "varchar";
        }

        internal override void DecodeValue(ref Utf8JsonReader utf8JsonReader)
        {
            //var hasRead = utf8JsonReader.Read();
            values.Add(utf8JsonReader.GetString());
        }

        internal override void NewBatch(Memory<byte> memory)
        {
        }

        //private ReadOnlySpan<byte> GetSpan(ByteInterval interval)
        //{
        //    return _memory.Span.Slice((int)interval.Start, (int)(interval.Stop - interval.Start));
        //}

        public override string GetString(in int index)
        {
            return values[index];
            //var interval = values[index];

            //if (interval.IsNull)
            //{
            //    return null;
            //}

            //var span = _memory.Span.Slice((int)interval.Start, (int)(interval.Stop - interval.Start));
            //return s_utf8Encoding.GetString(span);
        }

        public override object GetFieldValue(in int index, Type type)
        {
            var val = GetString(index);
            if (Equals(type, typeof(string)))
            {
                return val;
            }
            return Convert.ChangeType(val, type);
        }

        public override object GetValue(in int index)
        {
            return GetString(index);
        }

        public override bool IsDbNull(in int index)
        {
            return values[index] == null;
        }

        /// <summary>
        /// Directly copies the raw utf8 value into a stream.
        /// This is useful when the value should be used directly in an output, and decoding from utf8 to utf16 may be wasteful.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="index"></param>
        public override void CopyUtf8Value(in Stream stream, in int index)
        {
            //stream.Write(GetSpan(intervals[index]));
        }

        public override Type GetFieldType()
        {
            return typeof(string);
        }
    }
}
