using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using RaceDay.Mvc.Models;

namespace RaceDay.Mvc.Services
{


    public class RaceDayApiClient
    {
        private readonly HttpClient _httpClient;

        public RaceDayApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> LoginAsync(LoginView loginModel)
        {
            var response = await _httpClient.PostAsJsonAsync("api/login", loginModel);

            if (response.IsSuccessStatusCode)
            {
              
                var results = await response.Content.ReadFromJsonAsync<LoginResponse>();
                return results.Token;
            }
            return null;
        }
        public class LoginResponse
        {
            public string Token { get; set; } = string.Empty;
        }
    }
}
