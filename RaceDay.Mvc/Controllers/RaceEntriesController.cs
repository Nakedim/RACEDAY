
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

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


        [HttpGet]
        public async Task<IActionResult> ViewEntries()
        {

            //var entries = await _apiClient.GetEntriesAsync();

            //return View(entries);
            return Ok();
        }
        // GET: /RaceEntries/CreateEntries
        //[HttpGet]
        //public async Task<IActionResult> CreateEntries()
        //{
        //    await LoadDropdowns();

        //    return View("~/Views/Entries/CreateRaceEntries.cshtml",
        //        new EntriesModel());
        //}

        // POST: /RaceEntries/EnterRace
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> EnterRace(EntriesModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        await LoadDropdowns();

        //        return View(
        //            "~/Views/Entries/CreateRaceEntries.cshtml",
        //            model);
        //    }

        //    // API submission will be connected once
        //    // the Entries API endpoint and model are confirmed.

        //    ModelState.AddModelError(
        //        string.Empty,
        //        "Race entry submission has not yet been connected to the API."
        //    );

        //    await LoadDropdowns();

        //    return View(
        //        "~/Views/Entries/CreateRaceEntries.cshtml",
        //        model);
        //}

        // Populate event and category dropdowns.
        //    private async Task LoadDropdowns()
        //    {
        //        var events = await _apiClient.GetEventsAsync();

        //        var categories = await _apiClient.GetCategoriesAsync();

        //        ViewBag.RaceEvents = new SelectList(
        //            events,
        //            "RaceEventID",
        //            "Name"
        //        );

        //        ViewBag.Categories = new SelectList(
        //            categories ?? Enumerable.Empty<CategoryModel>(),
        //            "CategoryId",
        //            "CategoryName"
        //        );
        //    }
        //}
    }
}
