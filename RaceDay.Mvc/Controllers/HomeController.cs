using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.Models;


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

        // Organiser dashboard
        [Authorize(Roles = "Organiser")]
        public IActionResult OrganiserDash()
        {
            return View("~/Views/Organiser/OrganiserDash.cshtml");
        }

        [Authorize(Roles = "Participant")]
        public IActionResult ParticipantDash()
        {
            return View();
        }

        // ==========================================
        // ORGANISER-ONLY PAGES (EVENTS & RACES)
        // ==========================================

        // 1. GET: Displays the creation form populated with categories from the API project
        [Authorize(Roles = "Organiser")]
        [HttpGet]
        public async Task<IActionResult> CreateEvent()
        {
            // Pull category lookup table from your external API project
            var categories = await _apiClient.GetCategoriesAsync();

            if (categories == null || !categories.Any())
            {
                ViewBag.ErrorMessage = "No categories could be loaded from the API server.";
                ViewBag.CategoriesList = new SelectList(categories, "CategoryId", "CategoryName");
            }
            else
            {
                // Binds the category Primary Key (CategoryId) to values, and CategoryName to what users read
                ViewBag.CategoriesList = new SelectList(categories, "CategoryId", "CategoryName");
            }

            return View("~/Views/Events/CreateEvent.cshtml");
        }

        // 2. POST: Processes the form submission when creating a new event
        [Authorize(Roles = "Organiser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(RaceEventModel newEvent)
        {
            if (!ModelState.IsValid)
            {
                // Reload categories dropdown list so the form doesn't crash on re-render
                var categories = await _apiClient.GetCategoriesAsync();
                ViewBag.CategoriesList = new SelectList(categories ?? Enumerable.Empty<CategoryModel>(), "CategoryId", "CategoryName");
                return View("~/Views/Events/CreateEvent.cshtml", newEvent);
            }

            // Fire data across HTTP to save into the database running on the API project
            bool isSaved = await _apiClient.CreateEventAsync(newEvent);

            if (isSaved)
            {
                return RedirectToAction("Events");
            }

            ModelState.AddModelError("", "The API server failed to save this event record.");
            var failedCategories = await _apiClient.GetCategoriesAsync();
            ViewBag.CategoriesList = new SelectList(failedCategories ?? Enumerable.Empty<CategoryModel>(), "CategoryId", "CategoryName");
            return View("~/Views/Events/CreateEvent.cshtml", newEvent);
        }

        [Authorize(Roles = "Organiser")]
        public IActionResult CreateRace()
        {
            return View("~/Views/Races/CreateRace.cshtml");
        }

        [Authorize(Roles = "Organiser")]
        public IActionResult Entries()
        {
            return View("~/Views/Entries/RaceEntries.cshtml");
        }

        [Authorize(Roles = "Organiser")]
        public IActionResult EnterResults()
        {
            return View("~/Views/Results/EnterResults.cshtml");
        }
        // GET: /Home/CreateCategory
        [Authorize(Roles = "Organiser")]
        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View("~/Views/Categories/CreateCategory.cshtml");
        }

        // POST: /Home/CreateCategory
        [Authorize(Roles = "Organiser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(CategoryModel newCategory)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Categories/CreateCategory.cshtml", newCategory);
            }

            // Pass the category data to your separate API project
            bool isSaved = await _apiClient.CreateCategoryAsync(newCategory);

            if (isSaved)
            {
                // Redirect somewhere useful, like the Event creation screen
                return RedirectToAction("CreateEvent");
            }

            ModelState.AddModelError("", "The API server failed to save this category.");
            return View("~/Views/Categories/CreateCategory.cshtml", newCategory);
        }

        // ==========================================
        // ANONYMOUS PUBLIC PAGES
        // ==========================================

        [AllowAnonymous]
        public IActionResult Results()
        {
            return View("~/Views/Results/Results.cshtml");
        }

        [AllowAnonymous]
        public IActionResult Races()
        {
            return View("~/Views/Races/Races.cshtml");
        }

        [AllowAnonymous]
        public IActionResult Events()
        {
            return View("~/Views/Events/RaceEvents.cshtml");
        }

        public IActionResult Create()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetallEvents()
        {
            var eventsList = await _apiClient.GetEventsAsync();

            if (eventsList == null || !eventsList.Any())
            {
                ViewBag.ErrorMessage = "No race events could be retrieved from the server.";
            }

            return View("~/Views/Events/RaceEvents.cshtml", eventsList);
        }
    }
}
