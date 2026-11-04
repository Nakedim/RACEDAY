
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RaceDay.Mvc.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace RaceDay.Mvc.Controllers
{
    public class UsersAuthController : Controller
    {
        private readonly HttpClient _httpClient;

        public UsersAuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("RaceDayApi");
        }

        // GET: Display registration form
        [HttpGet]
        public IActionResult Register(string role = "Participant")
        {
            // Only allow the two registered roles
            if (role != "Organiser" && role != "Participant")
            {
                role = "Participant";
            }

            var model = new RegisterView
            {
                Role = role
            };

            return View(model);
        }

        // POST: Register user through RaceDay API
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterView model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Validate role on the server
            if (model.Role != "Organiser" &&
                model.Role != "Participant")
            {
                ModelState.AddModelError("Role", "Invalid user role.");
                return View(model);
            }

            // Prevent future dates of birth
            if (model.DateOfBirth.Date > DateTime.Today)
            {
                ModelState.AddModelError(
                    "DateOfBirth",
                    "Date of birth cannot be in the future.");

                return View(model);
            }

            try
            {
                // Calculate user's age
                int calculatedAge = DateTime.Today.Year -
                                    model.DateOfBirth.Year;

                if (model.DateOfBirth.Date >
                    DateTime.Today.AddYears(-calculatedAge))
                {
                    calculatedAge--;
                }

                // Prepare registration information for the API
                var apiPayload = new
                {
                    FirstName = model.FirstName,
                    Surname = model.Surname,
                    Email = model.Email,
                    Password = model.Password,
                    ConfirmPassword = model.ConfirmPassword,
                    PhoneNumber = model.PhoneNumber,
                    Age = calculatedAge,
                    Location = "Not Specified",
                    ProfileRole = model.Role
                };

                string jsonPayload =
                    JsonSerializer.Serialize(apiPayload);

                var content = new StringContent(
                    jsonPayload,
                    Encoding.UTF8,
                    "application/json");

                // Send registration request to API
                HttpResponseMessage response =
                    await _httpClient.PostAsync(
                        "api/authusers/register",
                        content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] =
                        "Registration successful! Please log in.";

                    // Both roles must log in before accessing dashboards
                    return RedirectToAction("Login", "UsersAuth");
                }

                // Handle API errors
                string apiError =
                    await response.Content.ReadAsStringAsync();

                ModelState.AddModelError(
                    string.Empty,
                    $"Registration failed: {apiError}");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to connect to the RaceDay API. Please try again.");
            }
            catch (TaskCanceledException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The API request timed out. Please try again.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    $"An unexpected error occurred: {ex.Message}");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            string? cachedRole = HttpContext.Session.GetString("Role");

            if (!string.IsNullOrEmpty(cachedRole))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginView model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var apiPayload = new
                {
                    Email = model.Username,
                    username = model.Username,
                    Password = model.Password
                    
                };

                string jsonPayload = JsonSerializer.Serialize(apiPayload);

                var content = new StringContent(
                    jsonPayload,
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync(
                    "api/authusers/login",
                    content);

                if (!response.IsSuccessStatusCode)
                {
                    string apiError = await response.Content.ReadAsStringAsync();

                    ModelState.AddModelError(
                        string.Empty,
                        $"Login failed: {apiError}");

                    return View(model);
                }

                string apiResponseJson =
                    await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<ApiResponseProfile>(
                    apiResponseJson,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (result?.User == null ||
                    string.IsNullOrWhiteSpace(result.User.Email) ||
                    string.IsNullOrWhiteSpace(result.User.Role))
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Login succeeded, but user information is missing.");

                    return View(model);
                }

                // Store logged-in user information in MVC session
                HttpContext.Session.SetString("UserId", result.User.Id.ToString());
                HttpContext.Session.SetString("Username", result.User.Email);
                HttpContext.Session.SetString("Role", result.User.Role);
                HttpContext.Session.SetString("Token", result.Token);
                // 2. Create the Identity Claims (Crucial for [Authorize(Roles = "...")] to work)
                var claims = new List<Claim>
            {
               new Claim(ClaimTypes.Name, result.User.Email),
               new Claim(ClaimTypes.Role, result.User.Role) // This maps "Organiser" or "Participant"
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                string userRole = result.User.Role?.Trim() ?? string.Empty;

                if (string.Equals(userRole, "Organiser", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("OrganiserDash", "Home");
                }

                if (string.Equals(userRole, "Participant", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("ParticipantDash", "Home");
                }

                ModelState.AddModelError(
                    string.Empty,
                    "Unrecognised user role.");

                return View(model);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to connect to the RaceDay API.");
            }
            catch (TaskCanceledException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The API request timed out.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            // Clear all session variables (UserId, Username, Role, Token)
            HttpContext.Session.Clear();

            // Send them back to the login screen
            return RedirectToAction("Login", "UsersAuth");
        }

        private class ApiResponseProfile
        {
            public string Token { get; set; } = string.Empty;

            public DateTime Expiration { get; set; }

            public ApiUserProfile? User { get; set; }
        }

        private class ApiUserProfile
        {
            public int Id { get; set; }

            public string Email { get; set; } = string.Empty;

            public string FirstName { get; set; } = string.Empty;

            public string Surname { get; set; } = string.Empty;

            public string Role { get; set; } = string.Empty;
        }

    }
}
