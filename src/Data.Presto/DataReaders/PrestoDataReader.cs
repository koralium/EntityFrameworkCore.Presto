using Data.Presto.Client;
using Data.Presto.Internal;
using Data.Presto.Models;
using Data.Presto.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Data.Presto.DataReaders
{
    /// <summary>
    /// Reads a stream of presto data, this can either be used to read specific pages by using NextResult to switch to a new page.
    /// Or by iterating through all pages with Read().
    /// </summary>
    public class PrestoDataReader : DbDataReader
    {
        private static readonly DbDataReader NoReadDataReader = new ReadErrorDataReader(Resources.NoRead);

        private PrestoPageDataReader _dataReader;
        private DbDataReader _currentReader;

        private readonly PrestoClient _prestoClient;
        private readonly bool _streaming;

        private readonly Task _loadTask;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private Channel<DecodeResult> pagesChannel;
        private bool serverCallEnded = false;
        private readonly string _queryId;

        public string QueryId => _queryId;

        internal PrestoDataReader(DecodeResult decodeResult, PrestoClient prestoClient, bool streaming)
        {
            _queryId = decodeResult.Id;
            _dataReader = new PrestoPageDataReader(decodeResult.Columns, decodeResult.RowCount, decodeResult.ColumnDecoders);
            _currentReader = NoReadDataReader;
            _prestoClient = prestoClient;
            _streaming = streaming;

            
            if (decodeResult.State == PrestoState.Running)
            {
                pagesChannel = Channel.CreateBounded<DecodeResult>(1);
                //Create a long running task to fetch new results
                _loadTask = Task.Factory.StartNew(async () =>
                {
                    var nextUri = decodeResult.NextUri;
                    var lastState = decodeResult.State;
                    
                    while (nextUri != null && !_cancellationTokenSource.IsCancellationRequested)
                    {
                        var nextResult = await prestoClient.FetchNext(nextUri, _cancellationTokenSource.Token);

                        if (nextResult.RowCount > 0)
                        {
                            //Add to a stream
                            await pagesChannel.Writer.WriteAsync(nextResult, _cancellationTokenSource.Token);
                        }
                        nextUri = nextResult.NextUri;
                        lastState = nextResult.State;
                    }

                    if (nextUri == null)
                    {
                        //Mark that the server ran to completion
                        serverCallEnded = true;
                    }

                    //Complete the stream
                    pagesChannel.Writer.Complete();

                }, TaskCreationOptions.LongRunning)
                    .Unwrap();
            }
            
        }

        private enum State
        {
            NotStarted,
            Reading,
            ResultSetDone,
            Closed
        }

        private State _state;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

                AsyncHelper.RunSync(() => DisposeAsync().AsTask());
            }
        }

        private Task DisposeServerCall()
        {
            if (!serverCallEnded)
            {
                return _prestoClient.KillQuery(_queryId, default);
            }
            return Task.CompletedTask;
        }

        public override async ValueTask DisposeAsync()
        {
            _cancellationTokenSource.Cancel();
            var killServerCall = DisposeServerCall();
            await Task.WhenAll(Task.WhenAny(_loadTask), killServerCall);
        }

        private void CheckRead(in int ordinal)
        {
            if (ordinal >= _dataReader.FieldCount)
            {
                throw new InvalidOperationException(Resources.OrdinalNotFound(ordinal, _dataReader.FieldCount));
            }
        }

        public override object this[int ordinal] => GetValue(ordinal);

        public override object this[string name] => GetValue(GetOrdinal(name));

        public override int Depth => 0;

        public override int FieldCount => _dataReader.FieldCount;

        public override bool HasRows => _dataReader.HasRows;

        public override bool IsClosed => _state == State.Closed;

        public override int RecordsAffected => -1;

        public override bool GetBoolean(int ordinal)
        {
            CheckRead(ordinal);
            return _currentReader.GetBoolean(ordinal);
        }

        public override byte GetByte(int ordinal)
        {
            CheckRead(ordinal);
            return _currentReader.GetByte(ordinal);
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override char GetChar(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override string GetDataTypeName(int ordinal)
        {
            CheckRead(ordinal);

            //Use the data reader here, since this should work before calling Read()
            return _dataReader.GetDataTypeName(ordinal);
        }

        public override DateTime GetDateTime(int ordinal)
        {
            CheckRead(ordinal);
            return _currentReader.GetDateTime(ordinal);
        }

        public override decimal GetDecimal(int ordinal)
        {
            CheckRead(ordinal);
            return _currentReader.GetDecimal(ordinal);
        }

        public override double GetDouble(int ordinal)
        {
            CheckRead(ordinal);
            return _currentReader.GetDouble(ordinal);
        }

        public override IEnumerator GetEnumerator()
        {
            return new DbEnumerator(this, false);
        }

        public override Type GetFieldType(int ordinal)
        {
            CheckRead(ordinal);

            //Use the data reader here since it should give correct results before read()
            return _dataReader.GetFieldType(ordinal);
        }

        public override float GetFloat(int ordinal)
        {
            CheckRead(ordinal);
            return _currentReader.GetFloat(ordinal);
        }

        public override Guid GetGuid(int ordinal)
        {
            CheckRead(ordinal);
            return _currentReader.GetGuid(ordinal);
        }

        public override short GetInt16(int ordinal)
        {
            CheckRead(ordinal);
            return _currentReader.GetInt16(ordinal);
        }

        public override int GetInt32(int ordinal)
        {
            CheckRead(ordinal);
            return _currentReader.GetInt32(ordinal);
        }

        public override long GetInt64(int ordinal)
        {
            CheckRead(ordinal);
            return _currentReader.GetInt64(ordinal);
        }

        public override string GetName(int ordinal)
        {
            CheckRead(ordinal);
            //Use the data reader here since it should give correct results before read()
            return _dataReader.GetName(ordinal);
        }

        public override int GetOrdinal(string name)
        {
            //Use the data reader here since it should give correct results before read()
            return _dataReader.GetOrdinal(name);
        }

        public override string GetString(int ordinal)
        {
            CheckRead(ordinal);
            return _currentReader.GetString(ordinal);
        }

        public override object GetValue(int ordinal)
        {
            CheckRead(ordinal);
            return _currentReader.GetValue(ordinal);
        }

        public override int GetValues(object[] values)
        {
            return _currentReader.GetValues(values);
        }

        public override bool IsDBNull(int ordinal)
        {
            CheckRead(ordinal);
            return _currentReader.IsDBNull(ordinal);
        }

        public override T GetFieldValue<T>(int ordinal)
        {
            CheckRead(ordinal);
            return _currentReader.GetFieldValue<T>(ordinal);
        }

        public override bool NextResult()
        {
            if (_streaming)
            {
                return false;
            }
            else
            {
                return AsyncHelper.RunSync(() => NextResultAsync(default));
            }
        }

        public override Task<bool> NextResultAsync(CancellationToken cancellationToken)
        {
            if (_streaming)
            {
                return Task.FromResult(false);
            }
            else
            {
                return GetNextPage(cancellationToken);
            }
        }

        /// <summary>
        /// Takes the current row and writes it as a json object to a stream
        /// </summary>
        public void ToJsonStream(Stream stream)
        {
            if (_currentReader == _dataReader)
            {
                _dataReader.ToJsonStream(stream);
                return;
            }
            throw new InvalidOperationException(Resources.NoRead);
        }

        private async Task<bool> GetNextPage(CancellationToken cancellationToken)
        {
            if (pagesChannel != null)
            {
                
                if (pagesChannel.Reader.Count == 0 && pagesChannel.Reader.Completion.IsCompleted)
                {
                    return false;
                }
                await pagesChannel.Reader.WaitToReadAsync(cancellationToken);
                if (!pagesChannel.Reader.TryRead(out var decodeResult))
                {
                    return false;
                }

                _dataReader = new PrestoPageDataReader(decodeResult.Columns, decodeResult.RowCount, decodeResult.ColumnDecoders);
                _currentReader = NoReadDataReader;
                _state = State.NotStarted;
                return true;
            }
            return false;
        }

        public override bool Read()
        {
            if (_currentReader.Read())
            {
                return true;
            }

            if (_state == State.NotStarted)
            {
                //Set the current reader to the data reader
                _currentReader = _dataReader;
                _state = State.Reading;

                //Try and read from the initial record batch
                if (_currentReader.Read())
                {
                    return true;
                }
            }

            //Check if new pages should be loaded directly into the data stream
            if (_state == State.Reading)
            {
                if (_streaming)
                {
                    if (AsyncHelper.RunSync(() => GetNextPage(default)))
                    {
                        _currentReader = _dataReader;
                        _state = State.Reading;
                        _dataReader.Read(); //Read one time on the data reader to create a continous stream
                        return true;
                    }
                    else
                    {
                        Close();
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            //If the state is closed, the user should be notified
            if (_state == State.Closed)
            {
                throw new InvalidOperationException(Resources.DataReaderClosed);
            }

            throw new InvalidOperationException(Resources.UnknownDataReaderState(_state));
        }

        public override void Close()
            => Dispose(true);
    }
}
