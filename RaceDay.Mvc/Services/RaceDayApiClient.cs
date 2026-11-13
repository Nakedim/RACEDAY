using Microsoft.AspNetCore.Mvc.Rendering;
using RaceDay.Mvc.Models;
using System.Net.Http.Json;

namespace RaceDay.Mvc.Services
{
    public class RaceDayApiClient
    {
        private readonly HttpClient _httpClient;

        public RaceDayApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

         
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
        // GET /api/Categories

        public async Task<List<CategoryModel>> GetCategoriesAsync()
        {
            var response = await _httpClient.GetAsync("api/Categories");

            response.EnsureSuccessStatusCode();

            var categories = await response.Content
                .ReadFromJsonAsync<List<CategoryModel>>();

            return categories ?? new List<CategoryModel>();
        }



        // POST /api/events

        public async Task<string?> CreateEventAsync(RaceEventModel model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(
                    "api/raceevents", model);

                var error = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    return null;

                return $"HTTP {(int)response.StatusCode}: {error}";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        // ==========================================
        // AUTHENTICATION METHODS
        // ==========================================

        public async Task<string?> RegisterAsync(RegisterView model)
        {
            try
            {
                var today = DateTime.Today;
                int age = today.Year - model.DateOfBirth.Year;
                if (model.DateOfBirth.Date > today.AddYears(-age))
                {
                    age--;
                }

                var request = new
                {
                    Age = age,
                    Location = model.Location,
                    FirstName = model.FirstName,
                    Surname = model.Surname,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Password = model.Password,
                    ConfirmPassword = model.ConfirmPassword,
                    ProfileRole = model.ProfileRole
                };

                var response = await _httpClient.PostAsJsonAsync(
                    "api/authusers/register", request);

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

        public async Task<List<RaceEventModel>?> GetEventsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<RaceEventModel>>(
                "api/raceevents");
        }

        // ==========================================
        // RACE ENTRY METHODS
        // ==========================================

        // GET /api/RaceEntries
        public async Task<List<EntriesModel>> GetEntriesAsync()
        {
            try
            {
                var entries = await _httpClient.GetFromJsonAsync<List<EntriesModel>>(
                    "api/RaceEntries");

                return entries ?? new List<EntriesModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving race entries: {ex.Message}");

                return new List<EntriesModel>();
            }
        }

        public async Task LogoutAsync() => await _httpClient.PostAsync("api/authusers/logout", null);

        public async Task<List<EnterResultsModel>> GetResultsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<EnterResultsModel>>(

                    "api/RaceResults") ?? new List<EnterResultsModel>();
            }
            catch
            {
                return new List<EnterResultsModel>();
            }
        }
      

    }
}
