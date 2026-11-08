
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers
{
    public class UsersAuthController : Controller
    {
        private readonly RaceDayApiClient _apiClient;

        private const string OrganiserRole = "Organiser";
        private const string ParticipantRole = "Participant";

        public UsersAuthController(RaceDayApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // GET: Login
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToRoleDashboard();
            }

            return View();
        }

        // GET: Register
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register(string role = ParticipantRole)
        {
            if (role != OrganiserRole && role != ParticipantRole)
            {
                role = ParticipantRole;
            }

            return View(new RegisterView
            {
                Role = role
            });
        }

        // POST: Register
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterView model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Validate the selected role.
            if (model.Role != OrganiserRole &&
                model.Role != ParticipantRole)
            {
                ModelState.AddModelError(
                    nameof(model.Role),
                    "Please select a valid registration role."
                );

                return View(model);
            }

            // Validate Date of Birth.
            if (model.DateOfBirth.Date > DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(model.DateOfBirth),
                    "Date of birth cannot be in the future."
                );

                return View(model);
            }

            // Send registration details to the API.
            string? registrationError = await _apiClient.RegisterAsync(model);

            if (registrationError == null)
            {
                TempData["SuccessMessage"] =
                    "Registration successful! You can now log in.";

                return RedirectToAction(nameof(Login));
            }

            // Display the actual API error.
            ModelState.AddModelError(
                string.Empty,
                registrationError
            );

            return View(model);
        }

        // POST: Login
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginView model,
            string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _apiClient.LoginAsync(model);

            if (result?.User == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid username or password."
                );

                return View(model);
            }

            string role = result.User.Role?.Trim() ?? string.Empty;

            if (role != OrganiserRole &&
                role != ParticipantRole)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid user role returned by the API."
                );

                return View(model);
            }

            string email = result.User.Email ?? string.Empty;
            string userId = result.User.Id.ToString();

            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The API returned an invalid user account."
                );

                return View(model);
            }

            // Clear previous session information.
            HttpContext.Session.Clear();

            // Store authenticated user information.
            HttpContext.Session.SetString("Username", email);
            HttpContext.Session.SetString("UserId", userId);
            HttpContext.Session.SetString("Role", role);

            // Create authentication claims.
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );

            // Respect safe local return URLs.
            if (!string.IsNullOrWhiteSpace(returnUrl) &&
                Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Otherwise redirect according to the authenticated role.
            return RedirectToRoleDashboard(role);
        }

        // Redirect to the correct dashboard.
        private IActionResult RedirectToRoleDashboard(string? role = null)
        {
            role ??= User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == OrganiserRole)
            {
                return RedirectToAction(
                    "OrganiserDash",
                    "Home"
                );
            }

            if (role == ParticipantRole)
            {
                return RedirectToAction(
                    "ParticipantDash",
                    "Home"
                );
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: Access Denied
        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // POST: Logout
        [Authorize]
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
    }
}
