using Data.Presto.Decoders;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Data.Presto.Models
{
    class DecodeResult
    {
        public class Builder
        {
            private string _id;
            private string _nextUri;
            private IReadOnlyList<ColumnDecoder> _columnDecoders;
            private int _rowCount;
            private IReadOnlyList<PrestoColumn> _columns;
            private PrestoState _prestoState;
            private string _errorMessage;

            public Builder NextUri(string nextUri)
            {
                _nextUri = nextUri;
                return this;
            }

            public Builder Id(string id)
            {
                _id = id;
                return this;
            }

            public Builder ColumnDecoders(IReadOnlyList<ColumnDecoder> columnDecoders)
            {
                _columnDecoders = columnDecoders;
                return this;
            }

            public Builder RowCount(int rowCount)
            {
                _rowCount = rowCount;
                return this;
            }

            public Builder Columns(IReadOnlyList<PrestoColumn> columns)
            {
                _columns = columns;
                return this;
            }

            public Builder State(PrestoState prestoState)
            {
                _prestoState = prestoState;
                return this;
            }

            public Builder ErrorMessage(string errorMessage)
            {
                _errorMessage = errorMessage;
                return this;
            }

            public DecodeResult Build()
            {
                return new DecodeResult(_id, _nextUri, _columnDecoders, _rowCount, _columns ?? ImmutableList.Create<PrestoColumn>(), _prestoState, _errorMessage);
            }
        }

        public string Id { get; }

        public string NextUri { get; }

        public IReadOnlyList<ColumnDecoder> ColumnDecoders { get; }

        public int RowCount { get; }

        public IReadOnlyList<PrestoColumn> Columns { get; }

        public PrestoState State { get; }

        public string ErrorMessage { get; }

        public DecodeResult(string id, string nextUri, IReadOnlyList<ColumnDecoder> columnDecoders, int rowCount, IReadOnlyList<PrestoColumn> columns, PrestoState prestoState, string errorMessage)
        {
            Id = id;
            NextUri = nextUri;
            ColumnDecoders = columnDecoders;
            RowCount = rowCount;
            Columns = columns;
            State = prestoState;
            ErrorMessage = errorMessage;
        }
    }
}
