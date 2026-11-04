using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace RaceDay.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Organiser"))
                {
                    return RedirectToAction(nameof(OrganiserDash));
                }

                if (User.IsInRole("Participant"))
                {
                    return RedirectToAction(nameof(ParticipantDash));
                }
            }

            return View();
        }

        // Organiser dashboard
        [Authorize(Roles = "Organiser")]
        public IActionResult OrganiserDash()
        {
            return View("~/Views/Organiser/OrganiserDash.cshtml");
        }
  
        // Participant dashboard
        [Authorize(Roles = "Participant")]
        public IActionResult ParticipantDash()
        {
            return View("~/Views/Participant/ParticipantDash.cshtml");
        }


        public IActionResult Events()
        {
            return View("~/Views/Events/RaceEvents.cshtml");
        }
        public IActionResult CreateEvent()
        {
            return View("~/Views/Events/CreateEvent.cshtml");
        }
        public IActionResult CreateRace()
        {
            return View("~/Views/Races/CreateRace.cshtml");
        }
        public IActionResult Races()
        {
            return View("~/Views/Races/Race.cshtml");
        }
        public IActionResult Entries()
        {
            return View("~/Views/Entries/RaceEntries.cshtml");
        }
        public IActionResult Create()
        {
            // Populate selection options mapping text names to integer Database IDs
            //ViewBag.CategoriesList = new SelectList(_logger.Categories, "CategoryID", "CategoryName");
            //ViewBag.EventsList = new SelectList(_logger.RaceEvents, "EventID", "Title");

            return View();
        }
    }
}
    
