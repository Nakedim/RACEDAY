using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RaceDayApiClient _apiClient;

        public HomeController(ILogger<HomeController> logger, RaceDayApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Organiser")]
        [HttpGet]
        public IActionResult OrganiserDash()
        {
            return View("~/Views/Organiser/OrganiserDash.cshtml");
        }

        [Authorize(Roles = "Participant")]
        [HttpGet]
        public IActionResult ParticipantDash()
        {
            return View("~/Views/Participant/ParticipantDash.cshtml");
        }

        // ==========================================
        // ORGANISER PAGES
        // ==========================================

        [Authorize(Roles = "Organiser")]
        [HttpGet]
        public async Task<IActionResult> CreateEvent()
        {
            await LoadCategories();
            return View("~/Views/Events/CreateEvent.cshtml");
        }

        [Authorize(Roles = "Organiser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(RaceEventModel newEvent)
        {
            if (newEvent == null)
            {
                return BadRequest("Event data cannot be null.");
            }

            if (!ModelState.IsValid)
            {
                await LoadCategories();
                return View("CreateEvent", newEvent);
            }

            //try
            //{
            //    bool isSaved = await _apiClient.CreateEventAsync(newEvent);
            //    if (isSaved)
            //    {
            //        TempData["SuccessMessage"] = "Event created successfully.";
            //        return RedirectToAction(nameof(Events));
            //    }

            //    ModelState.AddModelError(string.Empty, "The API server failed to save this event.");
            //}
            //catch (Exception ex)
            //{
            //    // Consider logging the actual exception (ex) here
            //    ModelState.AddModelError(string.Empty, "An error occurred while communicating with the server.");
            //}

            await LoadCategories();
            return View("CreateEvent", newEvent);
        }


        [Authorize(Roles = "Organiser")]
        [HttpGet]
        public IActionResult CreateRace()
        {
            return View("~/Views/Races/CreateRace.cshtml");
        }

        [Authorize(Roles = "Organiser")]
        [HttpGet]
        public IActionResult Entries()
        {
            return View("~/Views/Entries/RaceEntries.cshtml");
        }

        [Authorize(Roles = "Organiser")]
        [HttpGet]
        public IActionResult EnterResults()
        {
            return View("~/Views/Results/EnterResults.cshtml");
        }

        // ==========================================
        // PUBLIC PAGES
        // ==========================================

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Results()
        {
            return View("~/Views/Results/Results.cshtml");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Races()
        {
            return View("~/Views/Races/Races.cshtml");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Events()
        {
            return View("~/Views/Events/RaceEvents.cshtml");
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetallEvents()
        {
            try
            {
                var eventsList = await _apiClient.GetEventsAsync();

                if (eventsList == null || !eventsList.Any())
                {
                    ViewBag.ErrorMessage = "No race events are currently available.";

                    return View("~/Views/Events/RaceEvents.cshtml",
                        new List<RaceEventModel>());
                }

                return View("~/Views/Events/RaceEvents.cshtml", eventsList);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Unable to retrieve race events. Please try again later.";

                // Log the exception during development
                Console.WriteLine(ex.Message);

                return View("~/Views/Events/RaceEvents.cshtml",
                    new List<RaceEventModel>());
            }
        }
        [HttpGet]
        public async Task<IActionResult> ViewEntries()
        {
            var entries = await _apiClient.GetEntriesAsync();

            if (entries == null || !entries.Any())
            {
                ViewBag.ErrorMessage = "No race entries are currently available.";
            }

            return View(entries);
        }

        // ==========================================
        // PRIVATE HELPER METHODS
        // ==========================================

        private async Task LoadCategories()
        {
            var categories = await _apiClient.GetCategoriesAsync();

            Console.WriteLine($"Categories received: {categories.Count}");

            foreach (var category in categories)
            {
                Console.WriteLine(
                    $"ID: {category.CategoryId}, Name: {category.CategoryName}"
                );
            }

            ViewBag.Categories = new SelectList(
                categories,
                nameof(CategoryModel.CategoryId),
                nameof(CategoryModel.CategoryName)
            );
        }
    }
}
