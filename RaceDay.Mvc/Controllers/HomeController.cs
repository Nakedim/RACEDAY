using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using System.Diagnostics;
namespace RaceDay.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

       
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("RaceDayApi");
           

        }
        [HttpGet]
        public IActionResult Index()
        {
            var model = new RegisterView();
         
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult register(RegisterView model)
        {
            if (!ModelState.IsValid)
            {
               
                return View("index", model);
            }
            return RedirectToAction("index", "Organiser Dashboard");
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult ContactUs()
        {
            ViewBag.Message = "Contact us if you have something";
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult organiserDash()
        {
            return View();
        }
        public IActionResult ParticipantDash()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
