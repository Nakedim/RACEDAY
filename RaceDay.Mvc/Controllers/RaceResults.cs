using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;


namespace RaceDay.Mvc.Controllers
    {
        [Route("[controller]/[action]")]
        public class RaceResultsController : Controller
        {
            private readonly RaceDayApiClient _apiClient;

            public RaceResultsController(RaceDayApiClient apiClient)
            {
                _apiClient = apiClient;
            }

            // GET: /RaceResults/Index
            [HttpGet]
            public async Task<IActionResult> Index()
            {
                var results = await _apiClient.GetResultsAsync();
                return View(results);
            }

            // GET: /RaceResults/Create
            [HttpGet]
            [Authorize(Roles = "Organiser")]
            public async Task<IActionResult> Create()
            {
                await LoadRacesDropdownAsync();
                return View(new EnterResultsModel());
            }

            // POST: /RaceResults/Create
            [HttpPost]
            [Authorize(Roles = "Organiser")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(EnterResultsModel model)
            {
                if (!ModelState.IsValid)
                {
                    await LoadRacesDropdownAsync();
                    return View(model);
                }

                var errorMessage = await _apiClient.CreateResultAsync(model);

                if (errorMessage != null)
                {
                    ModelState.AddModelError(string.Empty, errorMessage);
                    await LoadRacesDropdownAsync();
                    return View(model);
                }

                TempData["SuccessMessage"] = "Race result recorded successfully!";
                return RedirectToAction(nameof(Index));
            }

            private async Task LoadRacesDropdownAsync()
            {
                var races = await _apiClient.GetRacesAsync() ?? new List<RaceModel>();

                ViewBag.Races = races
                    .Where(r => r != null)
                    .Select(r => new SelectListItem
                    {
                        Value = r.RaceID.ToString(),
                        Text = $"{r.RaceName} ({r.Distance} km)"
                    })
                    .ToList();
            }
        }
    }



