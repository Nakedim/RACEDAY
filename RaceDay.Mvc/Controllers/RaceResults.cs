using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;

namespace RaceDay.Mvc.Controllers
{
    public class RaceResults : Controller
    {
        //
        [HttpPost]
        public async Task <IActionResult> SubmitRaceResults(EnterResultsModel results)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/RaceResults/SubmitRaceResults.cshtml", results);
            }
            // Logic to save race results to the database
            // ...



            return RedirectToAction("RaceResultsSubmitted");
        }
    }
}
