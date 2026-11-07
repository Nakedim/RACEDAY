using Microsoft.AspNetCore.Mvc;

namespace RaceDay.Mvc.Controllers
{
    [Route("[controller]")]
    public class RaceEntriesController:Controller
    {
        public IActionResult CreateEntries()
        {
            return View("~/Views/Entries/CreateRaceEntries.cshtml");
        }

    }
}
