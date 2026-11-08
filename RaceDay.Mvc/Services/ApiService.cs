using System.Text;
using System.Text.Json;

namespace RaceDay.Mvc.Services
{
    public class ApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> SaveToApiAsync<T>(string endpointUrl, T entityData)
        {
            var client = _httpClientFactory.CreateClient();
            var json = JsonSerializer.Serialize(entityData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(endpointUrl, content);
            return response.IsSuccessStatusCode;
        }



    }
}
