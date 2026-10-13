using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;
using RaceDay.Api.Data;

namespace RaceDay.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class RaceEventsController: ControllerBase
    {
        private readonly RacedayDbContext _context;

        public RaceEventsController(RacedayDbContext context)
        {
            _context = context;
        }
        
        //both view race events
        [HttpGet]
        [Authorize(Roles = "Organiser, Participant")]
        public async Task<ActionResult<IEnumerable<RaceEvent>>> GetAllEvents()
        {
            return await _context.RaceEvents.ToListAsync();
        }

        //organisers create events
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public IActionResult CreateEvent()
        {
            return Ok();
        }
        //organisers update events
        [HttpPut]
        [Authorize(Roles = "Organiser")]
        public IActionResult UpdateEvent(int EventID)
        {
            return Ok();
        }

    }
}
