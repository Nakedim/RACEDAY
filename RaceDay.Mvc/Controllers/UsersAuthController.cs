using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RaceDay.Mvc.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers
{
    public class UsersAuthController : Controller
    {
        // FIXED: Inject your centralized client wrapper instead of raw HttpClient factory
        private readonly RaceDayApiClient _apiClient;

        public UsersAuthController(RaceDayApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Organiser"))
                {
                    return RedirectToAction("OrganiserDash", "Home");
                }

                if (User.IsInRole("Participant"))
                {
                    return RedirectToAction("ParticipantDash", "Home");
                }
            }

            return View();
        }

        // GET: Display registration form
        [HttpGet]
        public IActionResult Register(string role = "Participant")
        {
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

            if (model.Role != "Organiser" && model.Role != "Participant")
            {
                ModelState.AddModelError("Role", "Invalid user role.");
                return View(model);
            }

            if (model.DateOfBirth.Date > DateTime.Today)
            {
                ModelState.AddModelError("DateOfBirth", "Date of birth cannot be in the future.");
                return View(model);
            }

            // Calculate user's age safely
            int calculatedAge = DateTime.Today.Year - model.DateOfBirth.Year;
            if (model.DateOfBirth.Date > DateTime.Today.AddYears(-calculatedAge))
            {
                calculatedAge--;
            }

            // Route execution straight through your central ApiClient
            bool isRegistered = await _apiClient.RegisterAsync(model);

            if (isRegistered)
            {
                TempData["SuccessMessage"] = "Registration successful! Please log in.";
                return RedirectToAction("Login", "UsersAuth");
            }

            ModelState.AddModelError(string.Empty, "Registration failed. Please check your details or connection.");
            return View(model);
        }

        // POST: Authenticate user and issue Cookie state
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginView model, string? returnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _apiClient.LoginAsync(model);

            if (result?.User == null)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }

            string role = result.User.Role?.Trim() ?? "";

            if (role != "Organiser" && role != "Participant")
            {
                ModelState.AddModelError("", "Invalid user role returned by API.");
                return View(model);
            }

            // Clear any previous session information
            HttpContext.Session.Clear();

            // Store user information in Session
            HttpContext.Session.SetString("Username", result.User.Email);
            HttpContext.Session.SetString("UserId", result.User.Id.ToString());
            HttpContext.Session.SetString("Role", role);

            // Create authenticated identity
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, result.User.Email),
        new Claim(ClaimTypes.NameIdentifier, result.User.Id.ToString()),
        new Claim(ClaimTypes.Role, role)
    };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity)
            );

            // Redirect according to actual role
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            if (role == "Organiser")
                return RedirectToAction("OrganiserDash", "Home");

            return RedirectToAction("ParticipantDash", "Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }


        private string? GetRoleFromToken(string token)
        {
            try
            {
                var parts = token.Split('.');

                if (parts.Length != 3)
                    return null;

                string payload = parts[1];

                payload = payload.Replace('-', '+').Replace('_', '/');

                switch (payload.Length % 4)
                {
                    case 2:
                        payload += "==";
                        break;
                    case 3:
                        payload += "=";
                        break;
                }

                var jsonBytes = Convert.FromBase64String(payload);

                using var document = JsonDocument.Parse(jsonBytes);

                var root = document.RootElement;

                string[] roleClaims =
                {
            "role",
            "roles",
            ClaimTypes.Role
        };

                foreach (string claimName in roleClaims)
                {
                    if (root.TryGetProperty(claimName, out var role))
                    {
                        if (role.ValueKind == JsonValueKind.String)
                            return role.GetString();

                        if (role.ValueKind == JsonValueKind.Array &&
                            role.GetArrayLength() > 0)
                        {
                            return role[0].GetString();
                        }
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
