using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;

namespace RaceDay.Mvc.Controllers
{ 
    
    [AllowAnonymous]
    public class RaceEventsController: Controller
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public RaceEventsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> index()
        {
            var events = await _httpClientFactory.CreateClient("RaceDayApi")
                .GetFromJsonAsync<List<RaceEventModel>>("api/RaceEvents");
            return View();
        }


    }
}
