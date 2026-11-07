
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

        public HomeController(
            ILogger<HomeController> logger,
            RaceDayApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        // Public home page
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Organiser dashboard
        [Authorize(Roles = "Organiser")]
        [HttpGet]
        public IActionResult OrganiserDash()
        {
            return View("~/Views/Organiser/OrganiserDash.cshtml");
        }

        // Participant dashboard
        [Authorize(Roles = "Participant")]
        [HttpGet]
        public IActionResult ParticipantDash()
        {
            return View("~/Views/Participant/ParticipantDash.cshtml");
        }

        // ==========================================
        // ORGANISER PAGES
        // ==========================================

        // GET: Create Event
        [Authorize(Roles = "Organiser")]
        [HttpGet]
        public async Task<IActionResult> CreateEvent()
        {
            await LoadCategories();

            return View("~/Views/Events/CreateEvent.cshtml");
        }

        // POST: Create Event
        [Authorize(Roles = "Organiser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(
            RaceEventModel newEvent)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategories();

                return View(
                    "~/Views/Events/CreateEvent.cshtml",
                    newEvent
                );
            }

            bool isSaved = await _apiClient.CreateEventAsync(newEvent);

            if (isSaved)
            {
                TempData["SuccessMessage"] =
                    "Event created successfully.";

                return RedirectToAction(nameof(Events));
            }

            ModelState.AddModelError(
                string.Empty,
                "The API server failed to save this event."
            );

            await LoadCategories();

            return View(
                "~/Views/Events/CreateEvent.cshtml",
                newEvent
            );
        }

        // GET: Create Race
        [Authorize(Roles = "Organiser")]
        [HttpGet]
        public IActionResult CreateRace()
        {
            return View("~/Views/Races/CreateRace.cshtml");
        }

        // GET: Race Entries
        [Authorize(Roles = "Organiser")]
        [HttpGet]
        public IActionResult Entries()
        {
            return View("~/Views/Entries/RaceEntries.cshtml");
        }

        // GET: Enter Results
        [Authorize(Roles = "Organiser")]
        [HttpGet]
        public IActionResult EnterResults()
        {
            return View("~/Views/Results/EnterResults.cshtml");
        }

        // GET: Create Category
        [Authorize(Roles = "Organiser")]
        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View("~/Views/Categories/CreateCategory.cshtml");
        }

        // POST: Create Category
        [Authorize(Roles = "Organiser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(
            CategoryModel newCategory)
        {
            if (!ModelState.IsValid)
            {
                return View(
                    "~/Views/Categories/CreateCategory.cshtml",
                    newCategory
                );
            }

            bool isSaved =
                await _apiClient.CreateCategoryAsync(newCategory);

            if (isSaved)
            {
                TempData["SuccessMessage"] =
                    "Category created successfully.";

                return RedirectToAction(nameof(CreateEvent));
            }

            ModelState.AddModelError(
                string.Empty,
                "The API server failed to save this category."
            );

            return View(
                "~/Views/Categories/CreateCategory.cshtml",
                newCategory
            );
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
        public IActionResult Create()
        {
            return View();
        }

        // GET: Retrieve all events from API
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetallEvents()
        {
            var eventsList = await _apiClient.GetEventsAsync();

            if (eventsList == null || !eventsList.Any())
            {
                ViewBag.ErrorMessage =
                    "No race events could be retrieved from the server.";
            }

            return View(
                "~/Views/Events/RaceEvents.cshtml",
                eventsList
            );
        }

        // ==========================================
        // PRIVATE HELPER METHODS
        // ==========================================

        private async Task LoadCategories()
        {
            var categories = await _apiClient.GetCategoriesAsync();

            var categoryList =
                categories ?? Enumerable.Empty<CategoryModel>();

            if (!categoryList.Any())
            {
                ViewBag.ErrorMessage =
                    "No categories could be loaded from the API server.";
            }

            ViewBag.CategoriesList = new SelectList(
                categoryList,
                "CategoryId",
                "CategoryName"
            );
        }
    }
}
