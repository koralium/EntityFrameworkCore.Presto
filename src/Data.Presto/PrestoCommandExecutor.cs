using Data.Presto.Converter;
using Data.Presto.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Data.Presto
{
    public class PrestoCommandExecutor
    {
        private const int ChannelBoundry = 100000;
        private const int StatementNameLength = 32;
        private Channel<List<object>> _dataChannel;
        private Task _backgroundTask;
        private CancellationTokenSource _cancellationTokenSource;

        private Dictionary<string, int> _columnNameToOrdinal;
        private PrestoColumn[] _ordinalToColumn;
        private PrestoType[] _ordinalToTypeConverter;

        private PrestoCommandExecutor()
        {
            _dataChannel = Channel.CreateBounded<List<object>>(ChannelBoundry);
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void BuildColumnMetadata(List<PrestoColumn> columns)
        {
            if (_ordinalToColumn != null)
                return;

            _ordinalToColumn = columns.ToArray();
            _ordinalToTypeConverter = new PrestoType[columns.Count];
            _columnNameToOrdinal = new Dictionary<string, int>();
            for (int i = 0; i < columns.Count; i++)
            {
                if (_columnNameToOrdinal.ContainsKey(columns[i].Name))
                    continue;

                _columnNameToOrdinal.Add(columns[i].Name, i);
                _ordinalToTypeConverter[i] = PrestoTypeReader.ParseType(columns[i].Type);
            }
        }

        public T ConvertValue<T>(int ordinal, object obj)
        {
            if(_ordinalToTypeConverter == null)
            {
                throw new Exception("No data has been collected yet from Presto");
            }

            if(obj is JToken token)
            {
                var output = _ordinalToTypeConverter[ordinal].Convert(token);

                return output.ToObject<T>();
            }

            return (T)obj;
        }

        public ValueTask<int> GetFieldCount()
        {
            if(_ordinalToColumn != null)
            {
                return new ValueTask<int>(_ordinalToColumn.Length);
            }
            return new ValueTask<int>(WaitForMetadata().ContinueWith(t =>
            {
                return _ordinalToColumn.Length;
            }));
        }

        public ValueTask<int> GetOrdinal(string name)
        {
            if (_ordinalToColumn != null)
            {
                if(_columnNameToOrdinal.TryGetValue(name, out var ordinal))
                {
                    return new ValueTask<int>(ordinal);
                }
                return new ValueTask<int>(-1);
            }
            return new ValueTask<int>(WaitForMetadata().ContinueWith(t =>
            {
                if (_columnNameToOrdinal.TryGetValue(name, out var ordinal))
                {
                    return ordinal;
                }
                return -1;
            }));
        }

        public ValueTask<string> GetColumnName(int ordinal)
        {
            if (_ordinalToColumn != null)
            {
                return new ValueTask<string>(_ordinalToColumn[ordinal].Name);
            }
            return new ValueTask<string>(WaitForMetadata().ContinueWith(t =>
            {
                return _ordinalToColumn[ordinal].Name;
            }));
        }

        public ValueTask<string> GetDataTypeName(int ordinal)
        {
            if (_ordinalToColumn != null)
            {
                return new ValueTask<string>(_ordinalToColumn[ordinal].Type);
            }
            return new ValueTask<string>(WaitForMetadata().ContinueWith(t =>
            {
                return _ordinalToColumn[ordinal].Type;
            }));
        }

        public ValueTask<Type> GetFieldType(int ordinal)
        {
            if (_ordinalToColumn != null)
            {
                return new ValueTask<Type>(GetFieldTypeFromPresto(_ordinalToColumn[ordinal].Type));
            }
            return new ValueTask<Type>(WaitForMetadata().ContinueWith(t =>
            {
                return GetFieldTypeFromPresto(_ordinalToColumn[ordinal].Type);
            }));
        }

        private Type GetFieldTypeFromPresto(string prestoTypeName)
        {
            if(prestoTypeName.Equals("BOOLEAN", StringComparison.InvariantCultureIgnoreCase))
            {
                return typeof(bool);
            }
            if (prestoTypeName.Equals("TINYINT", StringComparison.InvariantCultureIgnoreCase))
            {
                return typeof(sbyte);
            }
            if (prestoTypeName.Equals("SMALLINT", StringComparison.InvariantCultureIgnoreCase))
            {
                return typeof(short);
            }
            if (prestoTypeName.Equals("INTEGER", StringComparison.InvariantCultureIgnoreCase))
            {
                return typeof(int);
            }
            if (prestoTypeName.Equals("BIGINT", StringComparison.InvariantCultureIgnoreCase))
            {
                return typeof(long);
            }
            if (prestoTypeName.Equals("REAL", StringComparison.InvariantCultureIgnoreCase))
            {
                return typeof(float);
            }
            if (prestoTypeName.Equals("DOUBLE", StringComparison.InvariantCultureIgnoreCase))
            {
                return typeof(double);
            }
            if (prestoTypeName.Equals("DECIMAL", StringComparison.InvariantCultureIgnoreCase))
            {
                return typeof(decimal);
            }
            if (prestoTypeName.StartsWith("VARCHAR", StringComparison.InvariantCultureIgnoreCase))
            {
                return typeof(string);
            }
            if (prestoTypeName.StartsWith("CHAR", StringComparison.InvariantCultureIgnoreCase))
            {
                return typeof(char);
            }
            if (prestoTypeName.StartsWith("DATE", StringComparison.InvariantCultureIgnoreCase))
            {
                return typeof(DateTime);
            }
            if (prestoTypeName.StartsWith("TIME", StringComparison.InvariantCultureIgnoreCase))
            {
                return typeof(TimeSpan);
            }
            if (prestoTypeName.StartsWith("TIMESTAMP", StringComparison.InvariantCultureIgnoreCase))
            {
                return typeof(DateTime);
            }
            throw new NotImplementedException();
        }

        private async Task WaitForMetadata()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested && _ordinalToColumn == null)
            {
                await Task.Delay(10);
            }
        }

        HttpClient HttpClient = new HttpClient();

        public int GetNumberOfColumns()
        {
            return 0;
        }

        public ChannelReader<List<object>> GetReader()
        {
            return _dataChannel.Reader;
        }

        public Task CloseAsync()
        {
            _cancellationTokenSource.Cancel();
            return _backgroundTask;
        }

        private static QueryParametersMetadata DeparameterizeStatement(PrestoCommand command, bool replaceAll = true)
        {
            QueryParametersMetadata output = new QueryParametersMetadata();
            var currentCommand = command.CommandText;

            //Replace newline with a whitespace
            currentCommand = currentCommand.Replace(System.Environment.NewLine, " ");
            //Replace multiple whitespaces with a single one
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            currentCommand = regex.Replace(currentCommand, " ");

            for (int i = 0; i < command.Parameters.Count; i++)
            {
                var parameter = command.Parameters[i];

                //Check if FETCH FIRST is used with parameter
                if (currentCommand.Contains($"FETCH FIRST {parameter.ParameterName}"))
                {
                    //replace that occurance of the parameter with only the value
                    currentCommand = currentCommand.Replace($"FETCH FIRST {parameter.ParameterName}", $"FETCH FIRST {parameter.Value}");
                }

                if (currentCommand.Contains($"FETCH NEXT {parameter.ParameterName}"))
                {
                    //replace that occurance of the parameter with only the value
                    currentCommand = currentCommand.Replace($"FETCH NEXT {parameter.ParameterName}", $"FETCH NEXT {parameter.Value}");
                }

                if (currentCommand.Contains($"OFFSET {parameter.ParameterName}"))
                {
                    //replace that occurance of the parameter with only the value
                    currentCommand = currentCommand.Replace($"OFFSET {parameter.ParameterName}", $"OFFSET {parameter.Value}");
                }

                if (!currentCommand.Contains(parameter.ParameterName))
                {
                    continue;
                }

                if (replaceAll)
                {
                    currentCommand = currentCommand.Replace(parameter.ParameterName, parameter.GenerateSqlValue());
                }
                else
                {
                    currentCommand = currentCommand.Replace(parameter.ParameterName, "?");
                    output.ParameterNameToIndex.Add(parameter.ParameterName, i);
                }
            }
            output.Query = currentCommand;

            return output;
        }

        private static async Task<string> PrepareStatement(PrestoCommandExecutor executor, PrestoCommand command, string statement)
        {
            var uniqueName = RandomString(StatementNameLength);
            statement = $"PREPARE {uniqueName} FROM {statement}";
            StringContent body = new StringContent(statement);
            executor.HttpClient.DefaultRequestHeaders.Add("X-Presto-User", command.PrestoConnection.ConnectionOptions.User);
            executor.HttpClient.DefaultRequestHeaders.Add("X-Presto-Catalog", command.PrestoConnection.ConnectionOptions.Catalog);
            executor.HttpClient.DefaultRequestHeaders.Add("X-Presto-Schema", command.PrestoConnection.ConnectionOptions.Schema);
            var response = await executor.HttpClient.PostAsync($"http://{command.PrestoConnection.ConnectionOptions.Host}/v1/statement", body, executor._cancellationTokenSource.Token);

            var content = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(content);
            }

            var prestoResponse = JsonConvert.DeserializeObject<PrestoStatementResponse>(content);

            Debug.Assert(prestoResponse.Id != null, "Query id is missing");
            Debug.Assert(prestoResponse.NextUri != null, "Next URI is missing");

            executor.HttpClient.DefaultRequestHeaders.Clear();

            bool continueLoop = true;
            while (continueLoop && !executor._cancellationTokenSource.Token.IsCancellationRequested)
            {
                response = await executor.HttpClient.GetAsync(prestoResponse.NextUri, executor._cancellationTokenSource.Token);

                if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(50), executor._cancellationTokenSource.Token);
                    continue;
                }

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception(content);
                }

                content = await response.Content.ReadAsStringAsync();
                prestoResponse = JsonConvert.DeserializeObject<PrestoStatementResponse>(content);

                switch (prestoResponse.Stats.State)
                {
                    case PrestoState.RUNNING:
                    case PrestoState.PLANNING:
                    case PrestoState.QUEUED:
                    case PrestoState.STARTING:
                        //Fetch again
                        break;
                    default:
                        continueLoop = false;
                        break;
                }
            }

            return uniqueName;
        }

        public static async Task<PrestoCommandExecutor> Execute(PrestoCommand prestoCommand)
        {
            PrestoCommandExecutor prestoCommandExecutor = new PrestoCommandExecutor();

            var parameterMetadata = DeparameterizeStatement(prestoCommand);
            var statement = parameterMetadata.Query;
            if (parameterMetadata.ParameterNameToIndex.Count > 0)
            {
                //Prepare the statement
                var statementName = await PrepareStatement(prestoCommandExecutor, prestoCommand, statement);

                string newStatement = $"EXECUTE {statementName} USING ";

                List<string> values = new List<string>();
                for(int i = 0; i < prestoCommand.Parameters.Count; i++)
                {
                    values.Add(prestoCommand.Parameters[i].GenerateSqlValue());
                }

                statement = $"{newStatement} {string.Join(',', values)}";
            }


            
            StringContent body = new StringContent(statement);
            prestoCommandExecutor.HttpClient.DefaultRequestHeaders.Add("X-Presto-User", prestoCommand.PrestoConnection.ConnectionOptions.User);
            prestoCommandExecutor.HttpClient.DefaultRequestHeaders.Add("X-Presto-Catalog", prestoCommand.PrestoConnection.ConnectionOptions.Catalog);
            prestoCommandExecutor.HttpClient.DefaultRequestHeaders.Add("X-Presto-Schema", prestoCommand.PrestoConnection.ConnectionOptions.Schema);
            var response = await prestoCommandExecutor.HttpClient.PostAsync($"http://{prestoCommand.PrestoConnection.ConnectionOptions.Host}/v1/statement", body, prestoCommandExecutor._cancellationTokenSource.Token);

            
            var content = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(content);
            }

            var prestoResponse = JsonConvert.DeserializeObject<PrestoStatementResponse>(content);

            Debug.Assert(prestoResponse.Id != null, "Query id is missing");
            Debug.Assert(prestoResponse.NextUri != null, "Next URI is missing");

            prestoCommandExecutor.HttpClient.DefaultRequestHeaders.Clear();

            prestoCommandExecutor._backgroundTask = Task.Run(async () =>
            {
                bool continueLoop = true;
                while (continueLoop && !prestoCommandExecutor._cancellationTokenSource.Token.IsCancellationRequested)
                {
                    response = await prestoCommandExecutor.HttpClient.GetAsync(prestoResponse.NextUri, prestoCommandExecutor._cancellationTokenSource.Token);

                    if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(50), prestoCommandExecutor._cancellationTokenSource.Token);
                        continue;
                    }

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        throw new Exception(content);
                    }

                    content = await response.Content.ReadAsStringAsync();
                    prestoResponse = JsonConvert.DeserializeObject<PrestoStatementResponse>(content);

                    if(prestoResponse.Columns != null)
                    {
                        prestoCommandExecutor.BuildColumnMetadata(prestoResponse.Columns);
                    }

                    if (prestoResponse.Data != null)
                    {
                        foreach (var row in prestoResponse.Data)
                        {
                            await prestoCommandExecutor._dataChannel.Writer.WriteAsync(row, prestoCommandExecutor._cancellationTokenSource.Token);
                        }
                    }

                    switch (prestoResponse.Stats.State)
                    {
                        case PrestoState.RUNNING:
                        case PrestoState.PLANNING:
                        case PrestoState.QUEUED:
                        case PrestoState.STARTING:
                            //Fetch again
                            break;
                        default:
                            continueLoop = false;
                            break;
                    }
                }

                prestoCommandExecutor._dataChannel.Writer.Complete();
            });

            return prestoCommandExecutor;
        }
    }
}
