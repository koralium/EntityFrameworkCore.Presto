using Data.Presto.Models;
using Data.Presto.Utils;
using System;
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

        public PrestoClient(PrestoConnectionStringBuilder prestoConnectionString)
        {
            _connectionString = prestoConnectionString;
            _httpClient = new HttpClient();
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

        public async Task<DecodeResult> Query(string statement, CancellationToken cancellationToken)
        {
            using var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"http://{_connectionString.Host}/v1/statement"),
                Content = new StringContent(statement)
            };

            AddHeaders(httpRequestMessage);

            var result = await _httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);

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
            using var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"http://{_connectionString.Host}/v1/query/{queryId}")
            };

            AddHeaders(httpRequestMessage);

            var result = await _httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
        }
    }
}
