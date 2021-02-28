using Data.Presto.Decoders;
using Data.Presto.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Data.Presto.Utils
{
    internal static class JsonDecoderHelper
    {
        public static IReadOnlyList<ColumnDecoder> GetColumnDecoders(IReadOnlyList<PrestoColumn> columns)
        {
            return columns.Select(x => x.PrestoType.CreateDecoder()).ToImmutableList();
        }

        internal enum PrestoResultPropertyType
        {
            None,
            NextUri,
            Columns,
            Data,
            Stats,
            Error
        }
        
        /// <summary>
        /// Reads the id property, which is expected to always be the first property in the results
        /// </summary>
        /// <param name="utf8JsonReader"></param>
        /// <returns></returns>
        internal static string ReadResultId(ref Utf8JsonReader utf8JsonReader)
        {
            bool hasRead = utf8JsonReader.Read();
            CheckHasRead(hasRead);

            if (utf8JsonReader.TokenType != JsonTokenType.StartObject)
            {
                throw new Exception($"Expected 'StartObject' got {utf8JsonReader.TokenType.ToString()}");
            }

            hasRead = utf8JsonReader.Read();
            CheckHasRead(hasRead);

            if (utf8JsonReader.TokenType != JsonTokenType.PropertyName)
            {
                throw new Exception($"Expected 'propertyName' got {utf8JsonReader.TokenType.ToString()}");
            }

            var propertyName = utf8JsonReader.GetString();

            if (propertyName != "id")
            {
                throw new Exception($"Expected 'id' property got '{propertyName}'");
            }

            hasRead = utf8JsonReader.Read();
            CheckHasRead(hasRead);

            if (utf8JsonReader.TokenType != JsonTokenType.String)
            {
                throw new Exception($"Expected 'id' property to be of type 'string' got '{utf8JsonReader.TokenType.ToString()}'");
            }

            string id = utf8JsonReader.GetString();
            return id;
        }

        /// <summary>
        /// Helper to find start of either nextUri, columns, data or stats
        /// </summary>
        /// <param name="reader"></param>
        internal static PrestoResultPropertyType FindNextUriColumnsDataOrStats(ref Utf8JsonReader reader)
        {
            while(reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName:
                        var propertyName = reader.GetString();
                        switch (propertyName)
                        {
                            case "nextUri":
                                return PrestoResultPropertyType.NextUri;
                            case "columns":
                                return PrestoResultPropertyType.Columns;
                            case "data":
                                return PrestoResultPropertyType.Data;
                            case "stats":
                                return PrestoResultPropertyType.Stats;
                            case "error":
                                return PrestoResultPropertyType.Error;
                        }
                        break;
                }
            }
            return PrestoResultPropertyType.None;
        }

        private static void CheckHasRead(in bool hasRead)
        {
            if (!hasRead)
            {
                new Exception("Could not read result");
            }
        }

        private static PrestoColumn ParseColumn(ref Utf8JsonReader reader)
        {
            ExpectType(ref reader, JsonTokenType.StartObject);

            int startDepth = reader.CurrentDepth;
            int innerDepth = startDepth + 1;

            string columnName = null;
            string typeName = null;

            while (reader.Read())
            {
                //Stop condition
                if (reader.TokenType == JsonTokenType.EndObject &&
                    reader.CurrentDepth == startDepth)
                {
                    break;
                }

                if (reader.TokenType == JsonTokenType.PropertyName &&
                    reader.CurrentDepth == innerDepth)
                {
                    var propertyName = reader.GetString();
                    switch (propertyName)
                    {
                        case "name":
                            columnName = ParseStringValue(ref reader);
                            break;
                        case "type":
                            typeName = ParseStringValue(ref reader);
                            break;
                    }
                }
            }
            return new PrestoColumn()
            {
                Name = columnName,
                PrestoType = PrestoTypeHelper.GetPrestoType(typeName)
            };
        }

        private static void ExpectType(ref Utf8JsonReader reader, JsonTokenType jsonTokenType)
        {
            if (reader.TokenType != jsonTokenType)
            {
                throw new Exception($"Expected '{jsonTokenType.ToString()}' but got '{reader.TokenType.ToString()}'");
            }
        }

        private static IReadOnlyList<PrestoColumn> ParseColumns(ref Utf8JsonReader reader)
        {
            List<PrestoColumn> columns = new List<PrestoColumn>();
            var hasRead = reader.Read();
            CheckHasRead(hasRead);

            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new Exception($"Expected 'StartArray' but got {reader.TokenType}");
            }

            int arrayStartDepth = reader.CurrentDepth;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray &&
                    reader.CurrentDepth == arrayStartDepth)
                {
                    break;
                }
                columns.Add(ParseColumn(ref reader));
            }
            return columns;
        }

        private static string ParseStringValue(ref Utf8JsonReader reader)
        {
            var hasRead = reader.Read();
            CheckHasRead(hasRead);

            if (reader.TokenType != JsonTokenType.String)
            {
                throw new Exception($"Expected string, but was '{reader.TokenType}'");
            }
            
            return reader.GetString();
        }

        private static int DecodeData(IReadOnlyList<ColumnDecoder> decoders, ref Utf8JsonReader reader)
        {
            var hasRead = reader.Read();
            CheckHasRead(hasRead);

            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new Exception($"Expected start array for decoding data, but was '{reader.TokenType}'");
            }

            var startDepth = reader.CurrentDepth;
            var rowDepth = reader.CurrentDepth + 1;

            int rowCount = 0;
            while(reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray && 
                    startDepth == reader.CurrentDepth)
                {
                    break;
                }
                if (reader.TokenType == JsonTokenType.StartArray &&
                    rowDepth == reader.CurrentDepth) //New row
                {
                    rowCount++;
                    //Go through each decoder where they decode their own values to the format that suites them
                    for (int i = 0; i < decoders.Count; i++)
                    {
                        hasRead = reader.Read();
                        CheckHasRead(hasRead);
                        decoders[i].DecodeValue(ref reader);
                    }
                }
            }

            return rowCount;
        }

        private static PrestoState DecodeStats(ref Utf8JsonReader reader)
        {
            var hasRead = reader.Read();
            CheckHasRead(hasRead);
            ExpectType(ref reader, JsonTokenType.StartObject);

            int startDepth = reader.CurrentDepth;
            var statsDepth = startDepth + 1;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject &&
                    startDepth == reader.CurrentDepth)
                {
                    break;
                }
                if (reader.TokenType == JsonTokenType.PropertyName &&
                    statsDepth == reader.CurrentDepth)
                {
                    var propertyName = reader.GetString();
                    if (propertyName == "state")
                    {
                        var state = ParseStringValue(ref reader);
                        switch (state)
                        {
                            case "QUEUED":
                                return PrestoState.Queued;
                            case "PLANNING":
                                return PrestoState.Planning;
                            case "STARTING":
                                return PrestoState.Starting;
                            case "RUNNING":
                                return PrestoState.Running;
                            case "FINISHED":
                                return PrestoState.Finished;
                            case "CANCELED":
                                return PrestoState.Canceled;
                            case "FAILED":
                                return PrestoState.Failed;
                        }
                    }
                }
            }
            throw new InvalidOperationException("Could not find state");
        }

        private static string ReadErrorMessage(ref Utf8JsonReader reader)
        {
            var hasRead = reader.Read();
            CheckHasRead(hasRead);
            ExpectType(ref reader, JsonTokenType.StartObject);

            var startDepth = reader.CurrentDepth;
            var messageDepth = startDepth + 1;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject &&
                    startDepth == reader.CurrentDepth)
                {
                    break;
                }

                if (reader.TokenType == JsonTokenType.PropertyName &&
                    messageDepth == reader.CurrentDepth)
                {
                    var propertyName = reader.GetString();
                    if (propertyName == "message")
                    {
                        return ParseStringValue(ref reader);
                    }
                }
            }
            return "";
        }

        public static DecodeResult NewPage(Memory<byte> memory)
        {
            Utf8JsonReader utf8JsonReader = new Utf8JsonReader(memory.Span);
            DecodeResult.Builder builder = new DecodeResult.Builder();

            builder.Id(ReadResultId(ref utf8JsonReader));

            IReadOnlyList<ColumnDecoder> decoders = null;
            
            PrestoResultPropertyType prestoResultPropertyType;
            while ((prestoResultPropertyType = FindNextUriColumnsDataOrStats(ref utf8JsonReader)) != PrestoResultPropertyType.None)
            {
                switch (prestoResultPropertyType)
                {
                    case PrestoResultPropertyType.NextUri:
                        var nextUri = ParseStringValue(ref utf8JsonReader);
                        builder.NextUri(nextUri);
                        break;
                    case PrestoResultPropertyType.Columns:
                        var columns = ParseColumns(ref utf8JsonReader);
                        builder.Columns(columns);
                        decoders = columns.Select(x => x.PrestoType.CreateDecoder()).ToImmutableArray();
                        for (int i = 0; i < decoders.Count; i++)
                        {
                            decoders[i].SetOrdinal((uint)i);
                        }
                        builder.ColumnDecoders(decoders);
                        break;
                    case PrestoResultPropertyType.Data:
                        foreach(var decoder in decoders)
                        {
                            decoder.NewBatch(memory);
                        }
                        builder.RowCount(DecodeData(decoders, ref utf8JsonReader));
                        break;
                    case PrestoResultPropertyType.Stats:
                        builder.State(DecodeStats(ref utf8JsonReader));
                        break;
                    case PrestoResultPropertyType.Error:
                        builder.ErrorMessage(ReadErrorMessage(ref utf8JsonReader));
                        break;
                }
            }
            return builder.Build();
        }
    }
}
