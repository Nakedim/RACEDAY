
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
        public async Task<string?> RegisterAsync(RegisterView view)
        {
            if (view == null)
                return "Registration information is missing.";

            // Calculate age from Date of Birth.
            int calculatedAge = DateTime.Today.Year - view.DateOfBirth.Year;

            if (view.DateOfBirth.Date > DateTime.Today.AddYears(-calculatedAge))
            {
                calculatedAge--;
            }

            // Match the API RegisterRequest DTO exactly.
            var apiPayload = new
            {
                Age = calculatedAge,
                FirstName = view.FirstName,
                Surname = view.Surname,
                Email = view.Email,
                PhoneNumber = view.PhoneNumber,
                Location = view.Location,
                Password = view.Password,
                ConfirmPassword = view.ConfirmPassword,
                ProfileRole = view.Role
            };

            try
            {
                using var response = await _httpClient.PostAsJsonAsync(
                    "api/AuthUsers/register",
                    apiPayload
                );

                if (response.IsSuccessStatusCode)
                {
                    return null;
                }

                // Read the actual validation error returned by the API.
                string errorMessage = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(errorMessage))
                {
                    return $"Registration failed. Server returned: {response.StatusCode}";
                }

                return errorMessage;
            }
            catch (HttpRequestException ex)
            {
                return $"Could not connect to the RaceDay API: {ex.Message}";
            }
            catch (TaskCanceledException)
            {
                return "The registration request timed out.";
            }
            catch (JsonException ex)
            {
                return $"There was a problem processing the API response: {ex.Message}";
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
