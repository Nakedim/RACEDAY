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

        // GET: Category/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Views/Category/Create.cshtml");
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Category/Create.cshtml", model);
            }

            var error = await _apiClient.CreateCategoryAsync(model);

            if (error == null)
            {
                TempData["SuccessMessage"] = "Category saved successfully!";
                return RedirectToAction("OrganiserDash", "Home");
            }

            ViewBag.ErrorMessage = error;

            return View("~/Views/Category/Create.cshtml", model);
        }
    }
}
