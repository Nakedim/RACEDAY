using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using RaceDay.Mvc.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RaceDay.Mvc.Services
{
    public class RaceDayApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RaceDayApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;

            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri("https://localhost:7087/");
            }
        }
        // ==========================================
        // CATEGORY METHODS
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
     

        public async Task<List<CategoryModel>> GetCategoriesAsync()
        {
            var response = await _httpClient.GetAsync("api/Categories");

            response.EnsureSuccessStatusCode();

            var categories = await response.Content
                .ReadFromJsonAsync<List<CategoryModel>>();

            return categories ?? new List<CategoryModel>();
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

        // ==========================================
        // login METHODS
        // ==========================================

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



        // ==========================================
        // RACE ENTRY METHODS
        // ==========================================

        // GET /api/RaceEntries
        public async Task<List<RaceEventModel>?> GetEventsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<RaceEventModel>>(
                "api/raceevents");
        }
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

       
        // ==========================================
        // RESULTS METHODS
        // ==========================================
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
        // ==========================================
        // RACE ENTRY METHODS
        // ==========================================
        public async Task<string?> CreateEntryAsync(EntriesModel entry)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "api/RaceEntries")
                {
                    Content = JsonContent.Create(entry)
                };

                var cookieHeader = _httpContextAccessor.HttpContext?.Request.Headers["Cookie"].ToString();
                if (!string.IsNullOrEmpty(cookieHeader))
                {
                    request.Headers.Add("Cookie", cookieHeader);
                }

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                    return null;

                var error = await response.Content.ReadAsStringAsync();
                return $"HTTP {(int)response.StatusCode} ({response.StatusCode}): {error}";
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }


        // ==========================================
        // RACES METHODS
        // ==========================================

        public async Task<List<RaceModel>> GetRacesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<RaceModel>>("api/Races") ?? new List<RaceModel>();
        }
        // POST /api/events

        public async Task<string?> CreateEventAsync(RaceEventModel model)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "api/raceevents")
                {
                    Content = JsonContent.Create(model)
                };

                // Forward the browser's cookie header to the API
                var cookieHeader = _httpContextAccessor.HttpContext?.Request.Headers["Cookie"].ToString();
                if (!string.IsNullOrEmpty(cookieHeader))
                {
                    request.Headers.Add("Cookie", cookieHeader);
                }

                var response = await _httpClient.SendAsync(request);
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

      
    }
    

}
