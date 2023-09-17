using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sharp46.Rest
{
    public class RequestResponse
    {
        public static JsonSerializerOptions SerializerOptions { get; set; } = new()
        {
            PropertyNameCaseInsensitive = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public HttpResponseMessage Response { get; set; } = new(System.Net.HttpStatusCode.RequestTimeout);

        public async Task<T> Deserialize<T>()
        {
            return JsonSerializer.Deserialize<T>(await Response.Content.ReadAsStreamAsync(), SerializerOptions) ?? throw new SerializationException("Failed to deserialize response");
        }

        public async Task<string> AsString()
        {
            if (Response == null)
                return "";
            return await Response.Content.ReadAsStringAsync();
        }
    }
}
