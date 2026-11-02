using Microsoft.AspNetCore.Mvc;

namespace RaceDay.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Simply returns the homepage containing the sidebar layout structure
            return View();
        }

        public IActionResult OrganiserDash() => View();
        public IActionResult ParticipantDash() => View();
    }
}
