using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers
{
    [Authorize(Roles = "Organiser")]
    public class CategoryController : Controller
    {
        private readonly RaceDayApiClient _apiClient;

        public CategoryController(RaceDayApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // GET: Categories/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel model)
        {
            // Check MVC validation
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Send category to backend API
            bool success = await _apiClient.CreateCategoryAsync(model);

            if (success)
            {
                TempData["SuccessMessage"] =
                    "Category created successfully!";

                return RedirectToAction(
                    "OrganiserDash", "Home");
            }

            ModelState.AddModelError(
                string.Empty,
                "Unable to create category. Please try again."
            );

            return View(model);
        }
    }
}