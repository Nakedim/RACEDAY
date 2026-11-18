using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;
using System.Security.Claims;

namespace RaceDay.Mvc.Controllers
{
    [Authorize(Roles = "Participant")]
    [Route("[controller]/[action]")]
    public class RaceEntriesController : Controller
    {
        private readonly RaceDayApiClient _apiClient;

        public RaceEntriesController(RaceDayApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // GET: /RaceEntries/ViewEntries
        [HttpGet]
        public async Task<IActionResult> ViewEntries()
        {
            var entries = await _apiClient.GetEntriesAsync();
            return View(entries);
        }

        // GET: /RaceEntries/EnterRace
        [HttpGet]
        public async Task<IActionResult> EnterRace()
        {
            await LoadDropdownsAsync();
            // If your Razor view is inside Views/Entries/ instead of Views/RaceEntries/, 
            // change this to: return View("~/Views/Entries/CreateRaceEntries.cshtml", new EntriesModel());
            return View("~/Views/Entries/CreateRaceEntries.cshtml", new EntriesModel());
        }

        // POST: /RaceEntries/EnterRace
        [HttpGet]

       
        // GET: /RaceEntries/CreateEntries
        [HttpGet]
        public async Task<IActionResult> CreateEntries()
        {
            await LoadDropdownsAsync();
            return View("~/Views/Entries/CreateEntries.cshtml", new EntriesModel());
        }

        // POST: /RaceEntries/CreateEntries
        [HttpPost]
        [Authorize(Roles = "Participant")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEntries(EntriesModel model)
        {
            // 1. Get the logged-in Participant's ID from Claims or Session
            var participantIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? HttpContext.Session.GetString("UserId");

            if (!int.TryParse(participantIdClaim, out int participantId))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to identify the logged-in Participant.");
                await LoadDropdownsAsync();
                return View("~/Views/Entries/CreateEntries.cshtml", model);
            } // <-- Close the failure block right here!

            // 2. Assign ParticipantID and clear any prior ModelState error for it
            model.ParticipantID = participantId;
            ModelState.Remove(nameof(model.ParticipantID));

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return View("~/Views/Entries/CreateEntries.cshtml", model);
            }

            // 3. Send to the API
            var errorMessage = await _apiClient.CreateEntryAsync(model);

            if (errorMessage != null)
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                await LoadDropdownsAsync();
                return View("~/Views/Entries/CreateEntries.cshtml", model);
            }

            TempData["SuccessMessage"] = "Your race entry has been registered!";
            return RedirectToAction("ParticipantDash", "Home");
        }


        //help method
        private async Task LoadDropdownsAsync()
        {
            var events = await _apiClient.GetEventsAsync() ?? new List<RaceEventModel>();
            var categories = await _apiClient.GetCategoriesAsync() ?? new List<CategoryModel>();

            // Using SelectListItem directly avoids reflection errors in MultiSelectList!
            // If Visual Studio underlines EventName or CategoryID in red, accept IntelliSense's suggestion.
            ViewBag.RaceEvents = events
                .Where(e => e != null)
                .Select(e => new SelectListItem
                {
                    Value = e.EventID.ToString(),
                    Text = e.Title ?? "Unnamed Event",
                    
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
