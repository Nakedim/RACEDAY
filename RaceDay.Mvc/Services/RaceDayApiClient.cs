using RaceDay.Mvc.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace RaceDay.Mvc.Services
{
    public class RaceDayApiClient
    {
        private readonly HttpClient _httpClient;

        public RaceDayApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CreateEventAsync(RaceEventModel newEvent)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/raceevents", newEvent);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        public async Task<ApiResponseProfile?> LoginAsync(LoginView loginModel)
        {
            try
            {
                var apiPayload = new
                {
                    Email = loginModel.Username,
                    Username = loginModel.Username,
                    Password = loginModel.Password
                };

                var response = await _httpClient.PostAsJsonAsync("api/authusers/login", apiPayload);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ApiResponseProfile>(
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        // FIXED: Removed the stray "Use code with caution." text that was causing syntax errors
        public async Task<bool> RegisterAsync(RegisterView registerModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/authusers/register", registerModel);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        public async Task<List<RaceEventModel>> GetEventsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<RaceEventModel>>("api/raceevents");
                return response ?? new List<RaceEventModel>();
            }
            catch
            {
                return new List<RaceEventModel>();
            }
        }

        public async Task<IEnumerable<CategoryModel>?> GetCategoriesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<CategoryModel>>("api/categories");
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<bool> CreateCategoryAsync(CategoryModel newCategory)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/categories", newCategory);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }
       

        public class LoginResponse
        {
            public string Token { get; set; } = string.Empty;
        }
    }
}
