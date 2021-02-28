using Data.Presto.Models;
using Data.Presto.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Data.Presto.Decoders
{
    class RowDecoder : ColumnDecoder
    {
        private readonly ColumnDecoder[] _columnDecoders;
        private readonly string[] _names;

        public override int RowCount => isNull.Count;
        private readonly List<bool> isNull = new List<bool>();

        //These fields are used when doing getFieldValue
        private TypeAccessor _lastUsedFieldValueTypeAccessor = null;
        private IReadOnlyList<PropertyAccessor> _lastUsedFieldValuePropertyAccessors = null;

        public RowDecoder(IReadOnlyList<PrestoColumn> columns)
        {
            _columnDecoders = columns.Select(x => x.PrestoType.CreateDecoder()).ToArray();
            _names = columns.Select(x => x.Name).ToArray();
        }

        public override string GetDataTypeName()
        {
            return "row";
        }

        public override Type GetFieldType()
        {
            return typeof(IReadOnlyDictionary<string, object>);
        }

        public override object GetFieldValue(in int index, Type type)
        {
            if (isNull[index])
            {
                return null;
            }

            if (!Equals(_lastUsedFieldValueTypeAccessor?.Type, type))
            {
                _lastUsedFieldValueTypeAccessor = TypeAccessors.GetTypeAccessor(type);
                _lastUsedFieldValuePropertyAccessors = _lastUsedFieldValueTypeAccessor.GetSetDelegates(_names);
            }

            var obj = Activator.CreateInstance(type);

            for (int i = 0; i < _names.Length; i++)
            {
                if (_lastUsedFieldValuePropertyAccessors[i] != null)
                {
                    _lastUsedFieldValuePropertyAccessors[i].SetValue(obj, _columnDecoders[i].GetFieldValue(index, _lastUsedFieldValuePropertyAccessors[i].PropertyType));
                }
            }

            return obj;
        }

        public override object GetValue(in int index)
        {
            if (isNull[index])
            {
                return null;
            }

            Dictionary<string, object> output = new Dictionary<string, object>(_columnDecoders.Length);
            for (int i = 0; i < _columnDecoders.Length; i++)
            {
                output.Add(_names[i], _columnDecoders[i].GetValue(index));
            }

            return output;
        }

        public override bool IsDbNull(in int index)
        {
            return isNull[index];
        }

        internal override void DecodeValue(ref Utf8JsonReader utf8JsonReader)
        {
            if (utf8JsonReader.TokenType == JsonTokenType.Null)
            {
                AppendOffset();
                return;
            }

            if (utf8JsonReader.TokenType != JsonTokenType.StartArray)
            {
                throw new InvalidOperationException($"Expected start array but got {utf8JsonReader.TokenType}");
            }

            var startDepth = utf8JsonReader.CurrentDepth;

            for (int i = 0; i < _columnDecoders.Length; i++)
            {
                var hasRead = utf8JsonReader.Read();
                _columnDecoders[i].DecodeValue(ref utf8JsonReader);
            }

            utf8JsonReader.Read();

            if (utf8JsonReader.TokenType != JsonTokenType.EndArray)
            {
                throw new InvalidOperationException($"Expected end array but got {utf8JsonReader.TokenType}");
            }

            isNull.Add(false);
        }

        internal override void NewBatch(Memory<byte> memory)
        {
        }

        public override void WriteJson(in int index, in Utf8JsonWriter jsonWriter)
        {
            if (IsDbNull(index))
            {
                jsonWriter.WriteNullValue();
            }
            else
            {
                jsonWriter.WriteStartObject();
                for(int i = 0; i < _names.Length; i++)
                {
                    jsonWriter.WritePropertyName(_names[i]);
                    _columnDecoders[i].WriteJson(index, jsonWriter);
                }
                jsonWriter.WriteEndObject();
            }
        }

        public override void AppendOffset()
        {
            isNull.Add(true);
            for (int i = 0; i < _columnDecoders.Length; i++)
            {
                _columnDecoders[i].AppendOffset();
            }
        }
    }
}
