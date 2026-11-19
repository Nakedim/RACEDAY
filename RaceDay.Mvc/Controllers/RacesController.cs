using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;
using System.Security.Claims;

namespace RaceDay.Mvc.Controllers
{
    [Route("[controller]/[action]")]
    public class RacesController : Controller
    {
        private readonly RaceDayApiClient _apiClient;

        public RacesController(RaceDayApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // GET: /Races/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var races = await _apiClient.GetRacesAsync();
            return View(races);
        }

        // GET: /Races/Create
        [HttpGet]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();
            return View(new RaceModel());
        }

        [HttpPost]
        [Authorize(Roles = "Organiser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RaceModel model)
        {
            var organiserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? HttpContext.Session.GetString("UserId");

            if (!int.TryParse(organiserIdClaim, out int organiserId))
            {
                ModelState.AddModelError(string.Empty, "Unable to identify the logged-in Organiser.");
                await LoadDropdownsAsync();
                return View(model);
            }

            model.OrganiserID = organiserId;
            ModelState.Remove(nameof(model.OrganiserID));

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return View(model);
            }

            // Calls POST api/Races and returns null on success, or an error string on failure
            var errorMessage = await _apiClient.CreateRaceAsync(model);

            if (errorMessage != null)
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                await LoadDropdownsAsync();
                return View(model);
            }

            // If we reach here, errorMessage is null (save succeeded!)
            TempData["SuccessMessage"] = "Race saved";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdownsAsync()
        {
            var events = await _apiClient.GetEventsAsync() ?? new List<RaceEventModel>();
            var categories = await _apiClient.GetCategoriesAsync() ?? new List<CategoryModel>();

            ViewBag.RaceEvents = events
                .Where(e => e != null)
                .Select(e => new SelectListItem
                {
                    Value = e.EventID.ToString(),
                    Text = e.Title ?? "Unnamed Event"
                })
                .ToList();

            ViewBag.Categories = categories
                .Where(c => c != null)
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName ?? "Unnamed Category"
                })
                .ToList();
        }
    }
}
