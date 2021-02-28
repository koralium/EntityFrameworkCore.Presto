using Data.Presto.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Data.Presto.Decoders
{
    class ArrayDecoder : ColumnDecoder
    {
        private struct ListInterval
        {
            public int startIndex;
            public int endIndex;
            public bool isNull;
        }

        private List<ListInterval> intervals = new List<ListInterval>();

        private readonly Type _listType;
        private readonly ColumnDecoder _innerDecoder;

        public override int RowCount => intervals.Count;

        public ArrayDecoder(ColumnDecoder innerDecoder)
        {
            _innerDecoder = innerDecoder;
            _listType = typeof(List<>).MakeGenericType(_innerDecoder.GetFieldType());
        }

        public override void CopyUtf8Value(in Stream stream, in int index)
        {
            throw new NotImplementedException();
        }

        public override string GetDataTypeName() => $"array({_innerDecoder.GetDataTypeName()})";

        public override object GetFieldValue(in int index, Type type)
        {
            var interval = intervals[index];

            if (interval.isNull)
            {
                return null;
            }

            var listCreator = ListCreators.GetListCreator(type);


            for (int i = interval.startIndex; i < interval.endIndex; i++)
            {
                listCreator.AddElement(_innerDecoder.GetFieldValue(i, listCreator.ElementType));
            }

            return listCreator.Build();
        }

        public override object GetValue(in int index)
        {
            var interval = intervals[index];

            if (interval.isNull)
            {
                return null;
            }

            var list = (IList)Activator.CreateInstance(_listType);

            for (int i = interval.startIndex; i < interval.endIndex; i++)
            {
                list.Add(_innerDecoder.GetValue(i));
            }
            return list;
        }

        public override bool IsDbNull(in int index)
        {
            return intervals[index].isNull;
        }

        internal override void DecodeValue(ref Utf8JsonReader utf8JsonReader)
        {
            if (utf8JsonReader.TokenType == JsonTokenType.Null)
            {
                intervals.Add(new ListInterval()
                {
                    isNull = true
                });
                return;
            }
            if (utf8JsonReader.TokenType != JsonTokenType.StartArray)
            {
                throw new InvalidOperationException($"Expected start array but got {utf8JsonReader.TokenType}");
            }

            var startDepth = utf8JsonReader.CurrentDepth;

            var start = _innerDecoder.RowCount;
            while (utf8JsonReader.Read())
            {
                if (utf8JsonReader.TokenType == JsonTokenType.EndArray &&
                    startDepth == utf8JsonReader.CurrentDepth)
                {
                    break;
                }
               
                _innerDecoder.DecodeValue(ref utf8JsonReader);
            }
            var end = _innerDecoder.RowCount;
            intervals.Add(new ListInterval()
            {
                startIndex = start,
                endIndex = end
            });
        }

        internal override void NewBatch(Memory<byte> memory)
        {
        }

        public override Type GetFieldType()
        {
            return _listType;
        }
    }
}
