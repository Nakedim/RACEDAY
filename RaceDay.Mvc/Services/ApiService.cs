using System.Text;
using System.Text.Json;

namespace RaceDay.Mvc.Services
{
    public class ApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            // Configured to automatically match camelCase API responses to PascalCase C# models
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // Generic POST Method
        public async Task<bool> SaveToApiAsync<T>(string endpointUrl, T entityData)
        {
            var client = _httpClientFactory.CreateClient();
            var json = JsonSerializer.Serialize(entityData, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(endpointUrl, content);
            return response.IsSuccessStatusCode;
        }

        // Generic GET Method
        public async Task<IEnumerable<T>> GetFromApiAsync<T>(string endpointUrl)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(endpointUrl);

                if (!response.IsSuccessStatusCode)
                    return Enumerable.Empty<T>();

                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<T>>(jsonString, _jsonOptions)
                       ?? Enumerable.Empty<T>();
            }
            catch
            {
                return Enumerable.Empty<T>();
            }
        }
    }
}
