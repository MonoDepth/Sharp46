using Sharp46.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sharp46.Rest
{
    public class RestClient : IRestClient
    {
        private bool _disposedValue;

        public HttpClient Client { get; private set; }
        public SocketsHttpHandler ClientHandler { get; private set; }
        public CookieContainer CookieContainer { get; private set; }

        public JsonSerializerOptions SerializerOptions { get; set; } = new()
        {
            PropertyNameCaseInsensitive = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public RestClient(string user, string password, TimeSpan? connectionTimeout = null, TimeSpan? requestTimeout = null, bool allowAutoRedirect = true)
        {
            CookieContainer = new();

            ClientHandler = new()
            {
                AllowAutoRedirect = allowAutoRedirect,
                CookieContainer = CookieContainer,
                UseCookies = true,
                ConnectTimeout = connectionTimeout ?? TimeSpan.FromSeconds(2)
            };

            Client = new(ClientHandler) { Timeout = requestTimeout ?? TimeSpan.FromSeconds(100) };

            //Defaults
            Client.DefaultRequestHeaders.Add("User-Agent", "Sharp46 Client Library");

            if (!string.IsNullOrWhiteSpace(user) && !string.IsNullOrEmpty(password))
            {
                var basicAuth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{user}:{password}"));
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);
            }
        }

        public async Task<RequestResponse> GetAsync(string endpoint, params object[] queryParams)
        {
            if (queryParams.Length > 0)
            {
                if (queryParams.Length % 2 != 0)
                {
                    throw new ArgumentException("Query parameters must be in pairs of key/value");
                }

                for (int i = 0; i < queryParams.Length; i += 2)
                {
                    var key = queryParams[i];
                    var value = queryParams[i + 1];

                    endpoint += $"{(i == 0 ? "?" : "&")}{key}={value}";
                }
            }

            var result = await Client.GetAsync(endpoint);

            return new RequestResponse()
            {
                Response = result
            };
        }

        public async Task<RequestResponse> PostAsync(string endpoint, IFormRequest body)
        {
            var result = await Client.PostAsync(endpoint, body.ToFormEncoded());

            return new RequestResponse()
            {
                Response = result
            };
        }

        public Task<RequestResponse> PutAsync(string endpoint, IFormRequest body)
        {
            throw new NotImplementedException();
        }

        public Task<RequestResponse> DeleteAsync(string endpoint)
        {
            throw new NotImplementedException();
        }

        public Task<RequestResponse> PatchAsync(string endpoint, IFormRequest body)
        {
            throw new NotImplementedException();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (!_disposedValue)
                    {
                        if (disposing)
                        {
                            ClientHandler.Dispose();
                            Client.Dispose();
                        }
                        _disposedValue = true;
                    }
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
