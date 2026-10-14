using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts;

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

        //organisers create Raceevents
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task <IActionResult> CreateEvent([FromBody ] RaceEventDTO raceEventDTO )
        {

                if (ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var NewEvent = new RaceEvent 
                {
                   Title = raceEventDTO.Title,
                   Description = raceEventDTO.Title,
                   location = raceEventDTO.location,
                   Distance = raceEventDTO.Distance,
                   EventType = raceEventDTO.EventType,
                   OrganiserID = raceEventDTO.OrganiserID,
                   
                };
               _context.RaceEvents.Add(NewEvent);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetRaceByID", new { id = NewEvent.EventID },NewEvent);
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
