using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;
using System.Security.Claims;

namespace RaceDay.Mvc.Controllers
{
    public class RaceEventsController : Controller
    {
        private readonly RaceDayApiClient _apiClient;

        public RaceEventsController(RaceDayApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // ==========================================
        // LOAD CATEGORIES INTO DROPDOWN
        // ==========================================
        private async Task LoadCategoriesDropdownAsync(int? selectedCategoryId = null)
        {
            var categories = await _apiClient.GetCategoriesAsync();

            ViewBag.Categories = new SelectList(
                categories,
                "CategoryId",
                "CategoryName",
                selectedCategoryId
            );
        }

        // ==========================================
        // DISPLAY ALL RACE EVENTS
        // ==========================================
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var events = await _apiClient.GetEventsAsync()
                         ?? new List<RaceEventModel>();

            return View(events);
        }

        // ==========================================
        // CREATE EVENT - GET
        // Automatically loads categories
        // ==========================================
        [Authorize(Roles = "Organiser")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCategoriesDropdownAsync();

            return View(new RaceEventModel());
        }

        // ==========================================
        // CREATE EVENT - POST
        // ==========================================
        [Authorize(Roles = "Organiser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RaceEventModel model)
        {
            // 1. Get logged-in organiser ID from session or claims FIRST
            var organiserIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? HttpContext.Session.GetString("UserId");

            if (!int.TryParse(organiserIdClaim, out int organiserId))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to identify the logged-in organiser."
                );

                await LoadCategoriesDropdownAsync(model.CategoryId);
                return View(model);
            }

            // 2. Assign OrganiserID and remove it from ModelState validation
            model.OrganiserID = organiserId;
            ModelState.Remove(nameof(model.OrganiserID));

            // 3. Validate remaining form fields
            if (!ModelState.IsValid)
            {
                await LoadCategoriesDropdownAsync(model.CategoryId);
                return View(model);
            }

            // 4. Send event to API
            string? errorMessage = await _apiClient.CreateEventAsync(model);

            if (errorMessage == null)
            {
                TempData["SuccessMessage"] = "Race Event created successfully!";

                return RedirectToAction("OrganiserDash", "Home");
            }

            ModelState.AddModelError(
                string.Empty,
                $"Unable to create race event: {errorMessage}"
            );

            await LoadCategoriesDropdownAsync(model.CategoryId);

            return View(model);
        }
    }
}
