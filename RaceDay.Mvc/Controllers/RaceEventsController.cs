using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers
{
    [AllowAnonymous]
    public class RaceEventsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly RaceDayApiClient _apiClient;
        public RaceEventsController(IHttpClientFactory httpClientFactory, RaceDayApiClient apiClient)
        {
            _httpClientFactory = httpClientFactory;
            _apiClient = apiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var events = await _httpClientFactory.CreateClient("RaceDayApi")
                .GetFromJsonAsync<List<RaceEventModel>>("api/RaceEvents");

            // Pass the events array data model directly to your Razor View!
            return View(events);
        }

        [Authorize(Roles = "Organiser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RaceEventModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 1. Declare the variable at the method level so it is in scope below
            int loggedInOrganiserId;

            var organiserIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // 2. Try parsing the claim first
            if (string.IsNullOrEmpty(organiserIdClaim) || !int.TryParse(organiserIdClaim, out loggedInOrganiserId))
            {
                // 3. Fallback to session parsing if the claim fails
                var sessionUserId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(sessionUserId) || !int.TryParse(sessionUserId, out loggedInOrganiserId))
                {
                    ModelState.AddModelError(string.Empty, "Unable to resolve your Organiser session profile context.");
                    return View(model);
                }
            }

            //  NOW IN SCOPE: This will no longer be underlined!
            model.OrganiserID = loggedInOrganiserId;

            // Call your API Client (Using Option A from our previous step to handle the string error)
            string? errorMessage = await _apiClient.CreateEventAsync(model);

            if (errorMessage == null)
            {
                TempData["SuccessMessage"] = "Race Event created successfully under your profile name!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, $"The system was unable to create the race event. Error: {errorMessage}");
            return View(model);
        }



        [Authorize(Roles = "Organiser")]
        [HttpGet]
        public IActionResult Create()
        {
            // Returns the blank form view model to the user
            return View(new RaceEventModel());
        }
    }
}
