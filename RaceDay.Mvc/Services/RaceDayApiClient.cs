
using RaceDay.Mvc.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;


namespace RaceDay.Mvc.Services
{
    public class RaceDayApiClient
    {
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions JsonOptions =
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

        public RaceDayApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // ==========================================
        // AUTHENTICATION
        // ==========================================

        // Login user
        public async Task<RaceDay.Contracts.apiResponseprofile.ApiResponseProfile?> LoginAsync(
    LoginView loginModel)
        {
            try
            {
                var apiPayload = new
                {
                    Email = loginModel.Email,
                    Username = loginModel.Username,
                    Password = loginModel.Password
                };

                var response = await _httpClient.PostAsJsonAsync(
                    "api/authusers/login",
                    apiPayload);

                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<
                    RaceDay.Contracts.apiResponseprofile.ApiResponseProfile>(
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        // Register user
        public async Task<bool> RegisterAsync(
            RegisterView registerModel)
        {
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(
                    "api/authusers/register",
                    registerModel
                );

                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
            catch (TaskCanceledException)
            {
                return false;
            }
        }

        // ==========================================
        // RACE EVENTS
        // ==========================================

        // Create event
        public async Task<bool> CreateEventAsync(
            RaceEventModel newEvent)
        {
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(
                    "api/raceevents",
                    newEvent
                );

                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
            catch (TaskCanceledException)
            {
                return false;
            }
        }

        // Retrieve all events
        public async Task<List<RaceEventModel>> GetEventsAsync()
        {
            try
            {
                var events = await _httpClient.GetFromJsonAsync<
                    List<RaceEventModel>
                >("api/raceevents");

                return events ?? new List<RaceEventModel>();
            }
            catch (HttpRequestException)
            {
                return new List<RaceEventModel>();
            }
            catch (TaskCanceledException)
            {
                return new List<RaceEventModel>();
            }
            catch (JsonException)
            {
                return new List<RaceEventModel>();
            }
        }

        // ==========================================
        // CATEGORIES
        // ==========================================

        // Retrieve categories
        public async Task<IEnumerable<CategoryModel>?> GetCategoriesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<
                    IEnumerable<CategoryModel>
                >("api/categories");
            }
            catch (HttpRequestException)
            {
                return null;
            }
            catch (TaskCanceledException)
            {
                return null;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        // Create category
        public async Task<bool> CreateCategoryAsync(
            CategoryModel newCategory)
        {
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(
                    "api/categories",
                    newCategory
                );

                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
            catch (TaskCanceledException)
            {
                return false;
            }
        }
    }
}
