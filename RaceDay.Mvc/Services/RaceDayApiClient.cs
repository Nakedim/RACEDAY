using System.Net.Http.Json;
using RaceDay.Mvc.Models;

namespace RaceDay.Mvc.Services
{
    public class RaceDayApiClient
    {
        private readonly HttpClient _httpClient;

        public RaceDayApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

            // Safety mechanism: If Program.cs fails to map the base address correctly, 
            // ensure the client fallback points directly to your verified running API port.
            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri("https://localhost:7087/");
            }
        }

        // ==========================================
        // CATEGORY & EVENT METHODS
        // ==========================================

        // POST /api/Categories
        public async Task<string?> CreateCategoryAsync(CategoryModel model)
        {
            try
            {
                var request = new
                {
                    CategoryName = model.CategoryName
                };
                Console.WriteLine(
    $"Sending category request to: " +
    new Uri(_httpClient.BaseAddress!, "api/Categories"));
                var response = await _httpClient.PostAsJsonAsync(
                    "api/Categories", request);

                if (response.IsSuccessStatusCode)
                    return null;

                var error = await response.Content.ReadAsStringAsync();

                return $"HTTP {(int)response.StatusCode}: {error}";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        // GET /api/Categories
        public async Task<IEnumerable<CategoryModel>> GetCategoriesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<CategoryModel>>("/api/Categories")
                       ?? new List<CategoryModel>();
            }
            catch
            {
                return new List<CategoryModel>();
            }
        }

        // POST /api/events
        public async Task<bool> CreateEventAsync(RaceEventModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/events", model);
            return response.IsSuccessStatusCode;
        }

        // GET /api/events
        public async Task<IEnumerable<RaceEventModel>> GetEventsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<RaceEventModel>>("/api/events")
                       ?? new List<RaceEventModel>();
            }
            catch
            {
                return new List<RaceEventModel>();
            }
        }

        // ==========================================
        // AUTHENTICATION METHODS
        // ==========================================


        public async Task<string?> RegisterAsync(RegisterView model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(
                    "api/authusers/register", model);

                var errorContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return null;
                }

                return $"HTTP {(int)response.StatusCode} " +
                       $"{response.StatusCode}: {errorContent}";
            }
            catch (Exception ex)
            {
                return $"Registration connection error: {ex.Message}";
            }
        }

        public async Task<LoginResultModel?> LoginAsync(LoginView model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(
                    "api/authusers/login", model);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Login failed: {error}");
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<LoginResultModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login exception: {ex.Message}");
                return null;
            }
        }


        //public async Task<LoginResultModel?> LoginAsync(LoginView model)
        //{
        //    try
        //    {
        //        var response = await _httpClient.PostAsJsonAsync("/api/usersauth/login", model);

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            return null;
        //        }

        //        return await response.Content.ReadFromJsonAsync<LoginResultModel>();
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}
    }
}
