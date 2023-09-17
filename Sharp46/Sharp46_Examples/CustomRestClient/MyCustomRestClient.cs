using Sharp46.Models;
using Sharp46.Rest;
using System.Net.Http.Headers;
using System.Text;

namespace CustomRestClient
{
    internal class MyCustomRestClient : IRestClient
    {
        private readonly HttpClient _client = new();
        // This example reads from the environment variables
        private readonly string _user = Environment.GetEnvironmentVariable("46ELKS_USER") ?? "";
        private readonly string _password = Environment.GetEnvironmentVariable("46ELKS_PASSWORD") ?? "";

        public MyCustomRestClient()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_user}:{_password}")));
        }

        public async Task<RequestResponse> DeleteAsync(string endpoint)
        {
            var response = await _client.DeleteAsync(endpoint);

            return new RequestResponse
            {
                Response = response
            };
        }

        public async Task<RequestResponse> GetAsync(string endpoint, params object[] queryParams)
        {
            // Handle query params here...
            var response = await _client.GetAsync(endpoint);

            return new RequestResponse
            {
                Response = response
            };
        }

        public async Task<RequestResponse> PatchAsync(string endpoint, IFormRequest body)
        {
            var response = await _client.PatchAsync(endpoint, body.ToFormEncoded());

            return new RequestResponse
            {
                Response = response
            };
        }

        public async Task<RequestResponse> PostAsync(string endpoint, IFormRequest body)
        {
            var response = await _client.PostAsync(endpoint, body.ToFormEncoded());

            return new RequestResponse
            {
                Response = response
            };
        }

        public async Task<RequestResponse> PutAsync(string endpoint, IFormRequest body)
        {
            var response = await _client.PutAsync(endpoint, body.ToFormEncoded());

            return new RequestResponse
            {
                Response = response
            };
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
