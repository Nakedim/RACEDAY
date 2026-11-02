
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
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



        // GET: Displays the registration form and catches incoming role properties
        [HttpGet]
        public IActionResult Register(string role)
        {
            var model = new RegisterView
            {
                Role = string.IsNullOrEmpty(role) ? "Participant" : role
            };
            return View(model);
        }

        // POST: Validates parameters and handles navigation logic shifts cleanly
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterView model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // If invalid data input, refresh structural elements showing alerts
            }
            try
            {
                int calculatedAge = DateTime.Today.Year - model.DateOfBirth.Year;
                if (model.DateOfBirth.Date > DateTime.Today.AddYears(-calculatedAge)) calculatedAge--;


                var apiPayload = new
                {
                    FirstName = model.FirstName,
                    Surname = model.Surname,
                    Email = model.Email,
                    Password = model.Password,
                    PhoneNumber = model.PhoneNumber,
                    Age = calculatedAge,
                    Location = "Not Specified", // Placeholder (or capture from a form field)
                    Role = model.Role
                };
               string jsonPayload = JsonSerializer.Serialize(apiPayload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync("api/authusers/register", content); ;

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    // 3. If saving succeeds, redirect based on their role
                    if (model.Role.Equals("Organiser", StringComparison.OrdinalIgnoreCase))
                    {
                        return RedirectToAction("Login", "Home");
                    }
                    return RedirectToAction("ParticipantDash", "Home");
                }
                else
                {
                    // Read the error message returned from the API backend if any
                    string apiError = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Server Error: {apiError}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Could not connect to API: {ex.Message}");
            }

            // Return to view with error state alerts if request fails
            return View(model);
        }

        [HttpGet]
        public IActionResult Login() => View();
    }
}
