using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;
using RaceDay.Api.Data;
using RaceDay.Contracts;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RaceEventsController : ControllerBase
    {
        private readonly RacedayDbContext _context;

        public RaceEventsController(RacedayDbContext context)
        {
            _context = context;
        }

        //both view race events
        [HttpGet]
        [Authorize(Roles = "Organiser, Participant")]
        public async Task<ActionResult<IEnumerable<RaceEventDTO>>> GetAllEvents()
        {
            return await _context.RaceEvents
                .Select(re => new RaceEventDTO
                {
                    EventID = re.EventID,
                    Description = re.Description,
                    Title = re.Title,
                    location = re.location,
                    OrganiserID = re.OrganiserID,
                    CategoryID = re.CategoryID,
                    EventType = re.EventType
                })
                .ToListAsync();
        }

        //organisers create events
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public IActionResult CreateEvent([FromBody] CreateRaceEventDTO createEventDto)
        {
            return Ok();
        }

        //organisers update events
        [HttpPut("{EventID}")]
        [Authorize(Roles = "Organiser")]
        public IActionResult UpdateEvent(int EventID, [FromBody] RaceEventDTO updateEventDto)
        {
            return Ok();
        }
    }
}
