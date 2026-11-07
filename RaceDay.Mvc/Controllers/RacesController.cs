using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using System.Text;
using System.Text.Json;

namespace RaceDay.Mvc.Controllers
{
    public class RacesController :Controller
    {
        private readonly HttpClient _httpClient;

        public RacesController(IHttpClientFactory httpClientFactory)
        { 
            _httpClient = httpClientFactory.CreateClient("RaceDayApi");
        }

        // GET: /Races
        public async Task<IActionResult> Index()
        {
            List<RaceModel> races = new List<RaceModel>();

            try
            {
                // Update this path to match your actual backend API endpoint routing
                HttpResponseMessage response = await _httpClient.GetAsync("api/races");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    var apiData = JsonSerializer.Deserialize<List<RaceModel>>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (apiData != null)
                    {
                        races = apiData;
                    }
                }
                else
                {
                 ModelState.AddModelError(string.Empty, $"Server responded with an error: {response.ReasonPhrase}");
                }
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Unable to connect to the backend API registry.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An unexpected system exception occurred: {ex.Message}");
            }

            return View(races);
        }
        // GET: /Races/Create
        [HttpGet]
        public IActionResult Create()
        {
            // Initializes the form with a default start time set to right now
            var model = new RaceModel { StartTime = DateTime.Now };
            return View(model);
        }

        // POST: /Races/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RaceModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Simple validation rules
            if (model.StartTime < DateTime.Now)
            {
                ModelState.AddModelError("StartTime", "The start time cannot be in the past.");
                return View(model);
            }

            try
            {
                // Serialize model into a JSON string payload matching the API properties
                string jsonPayload = JsonSerializer.Serialize(model);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // Send POST request out to the API backend
                HttpResponseMessage response = await _httpClient.PostAsync("api/races", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Race created successfully!";
                    return RedirectToAction(nameof(Index));
                }

                // Parse server side API exception drops if transmission criteria fails
                string apiError = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"API Server Error: {apiError}");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Unable to establish communication connections with the RaceDay API.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An unexpected structural execution error occurred: {ex.Message}");
            }

            return View(model);
        }
    }
}
    
    

