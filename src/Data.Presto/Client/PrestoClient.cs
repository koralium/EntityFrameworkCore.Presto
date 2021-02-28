using Data.Presto.Models;
using Data.Presto.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Presto.Client
{
    class PrestoClient
    {
        private readonly HttpClient _httpClient;
        private readonly PrestoConnectionStringBuilder _connectionString;

        private bool? _useSsl = null;

        //Dictionary used to store which http protocol to use for connections where it is not marked explicitly.
        private static readonly ConcurrentDictionary<string, bool> _protocolLookup = new ConcurrentDictionary<string, bool>();

        public PrestoClient(PrestoConnectionStringBuilder prestoConnectionString)
        {
            _connectionString = prestoConnectionString;
            _httpClient = new HttpClient();

            if (_connectionString.Ssl.HasValue)
            {
                _useSsl = _connectionString.Ssl.Value;
            }
            else
            {
                //Check if another presto client has already done connections against the host
                if (_protocolLookup.TryGetValue(prestoConnectionString.Host, out var canUseSsl))
                {
                    _useSsl = canUseSsl;
                }
            }
        }

        private void AddHeaders(HttpRequestMessage httpRequestMessage)
        {
            if (_connectionString.Trino)
            {
                AddTrinoHeaders(httpRequestMessage);
            }
            else
            {
                AddPrestoHeaders(httpRequestMessage);
            }
        }

        private void AddTrinoHeaders(HttpRequestMessage httpRequestMessage)
        {
            httpRequestMessage.Headers.Add(TrinoHeaders.TRINO_USER, _connectionString.User);
            httpRequestMessage.Headers.Add(TrinoHeaders.TRINO_CATALOG, _connectionString.Catalog);
            httpRequestMessage.Headers.Add(TrinoHeaders.TRINO_SCHEMA, _connectionString.Schema);

            if (!string.IsNullOrEmpty(_connectionString.Password))
            {
                var byteArray = Encoding.ASCII.GetBytes($"{_connectionString.User}:${_connectionString.Password}");
                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("basic", Convert.ToBase64String(byteArray));
            }

            foreach (var extraCredential in _connectionString.ExtraCredentials)
            {
                httpRequestMessage.Headers.Add(TrinoHeaders.TRINO_EXTRA_CREDENTIAL, $"{extraCredential.Key}={extraCredential.Value}");
            }
        }

        private void AddPrestoHeaders(HttpRequestMessage httpRequestMessage)
        {
            httpRequestMessage.Headers.Add(PrestoHeaders.PRESTO_USER, _connectionString.User);
            httpRequestMessage.Headers.Add(PrestoHeaders.PRESTO_CATALOG, _connectionString.Catalog);
            httpRequestMessage.Headers.Add(PrestoHeaders.PRESTO_SCHEMA, _connectionString.Schema);

            if (!string.IsNullOrEmpty(_connectionString.Password))
            {
                var byteArray = Encoding.ASCII.GetBytes($"{_connectionString.User}:${_connectionString.Password}");
                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("basic", Convert.ToBase64String(byteArray));
            }

            foreach (var extraCredential in _connectionString.ExtraCredentials)
            {
                httpRequestMessage.Headers.Add(PrestoHeaders.PRESTO_EXTRA_CREDENTIAL, $"{extraCredential.Key}={extraCredential.Value}");
            }
        }

        public async Task<DecodeResult> FetchNext(string nextUri, CancellationToken cancellationToken)
        {
            using var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(nextUri)
            };

            var result = await _httpClient.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false);
            return await CheckResult(result, cancellationToken);
        }

        private async Task<DecodeResult> CheckResult(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken)
        {
            var content = await httpResponseMessage.Content.ReadAsByteArrayAsync();
            var contentMemory = content.AsMemory();

            var decodeResults = JsonDecoderHelper.NewPage(contentMemory);


            if (decodeResults.State == PrestoState.Queued ||
                decodeResults.State == PrestoState.Planning ||
                decodeResults.State == PrestoState.Starting ||
                (decodeResults.State == PrestoState.Running && decodeResults.RowCount == 0))
            {
                return await FetchNext(decodeResults.NextUri, cancellationToken);
            }

            return decodeResults;
        }

        private string GetProtocol()
        {
            if (!_useSsl.Value)
            {
                return "http://";
            }
            return "https://";
        }

        private void SetHostSsl(in string host, bool canUseSsl)
        {
            if (_useSsl == null)
            {
                _useSsl = canUseSsl;
                _protocolLookup.AddOrUpdate(host, canUseSsl, (key, old) => canUseSsl);
            }
        }

        private async Task<HttpResponseMessage> SendMessage(HttpMethod httpMethod, string path, CancellationToken cancellationToken, string content = null)
        {
            //Protocol has not yet been determined
            if (_useSsl == null)
            {
                try
                {
                    using var httpRequestMessage = new HttpRequestMessage()
                    {
                        Method = httpMethod,
                        RequestUri = new Uri($"https://{_connectionString.Host}{path}"),
                    };

                    if (content != null)
                    {
                        httpRequestMessage.Content = new StringContent(content);
                    }

                    AddHeaders(httpRequestMessage);

                    var response = await _httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
                    SetHostSsl(_connectionString.Host, true);
                    return response;
                }
                catch (HttpRequestException requestException)
                {
                    if (requestException?.InnerException?.Source == "System.Net.Security")
                    {
                        //Exception regarding security, test http:// instead
                        using var httpRequestMessage = new HttpRequestMessage()
                        {
                            Method = httpMethod,
                            RequestUri = new Uri($"http://{_connectionString.Host}{path}"),
                        };

                        if (content != null)
                        {
                            httpRequestMessage.Content = new StringContent(content);
                        }

                        AddHeaders(httpRequestMessage);

                        var response = await _httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
                        SetHostSsl(_connectionString.Host, false);
                        return response;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                using var httpRequestMessage = new HttpRequestMessage()
                {
                    Method = httpMethod,
                    RequestUri = new Uri($"{GetProtocol()}{_connectionString.Host}{path}"),
                };

                if (content != null)
                {
                    httpRequestMessage.Content = new StringContent(content);
                }

                AddHeaders(httpRequestMessage);

                return await _httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            }
        }

        public async Task<DecodeResult> Query(string statement, CancellationToken cancellationToken)
        {
            var result = await SendMessage(HttpMethod.Post, "/v1/statement", cancellationToken, statement);

            switch (result.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                case System.Net.HttpStatusCode.Accepted:
                case System.Net.HttpStatusCode.NoContent:
                    return await CheckResult(result, cancellationToken);
                default:
                    throw new InvalidOperationException(await result.Content.ReadAsStringAsync().ConfigureAwait(false));
            }
        }

        public async Task KillQuery(string queryId, CancellationToken cancellationToken)
        {
            await SendMessage(HttpMethod.Delete, $"/v1/query/{queryId}", cancellationToken).ConfigureAwait(false);
        }
    }
}
