
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RaceDay.Mvc.Models;

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

        // GET: Display login page
        [HttpGet]
        public IActionResult Login()
        {
            string? username =
                HttpContext.Session.GetString("Username");

            string? role =
                HttpContext.Session.GetString("Role");

            if (!string.IsNullOrEmpty(username))
            {
                if (role == "Organiser")
                {
                    return RedirectToAction(
                        "OrganiserDash", "Home");
                }

                if (role == "Participant")
                {
                    return RedirectToAction(
                        "ParticipantDash", "Home");
                }
            }

            return View();
        }
    }
}
