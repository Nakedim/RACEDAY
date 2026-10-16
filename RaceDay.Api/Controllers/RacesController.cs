using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RacesController : ControllerBase
    {
        private readonly RacedayDbContext _context;

        public RacesController(RacedayDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RaceDTO>>> GetParticipant()
        {
            return await _context.Races
                .Select(r => new RaceDTO
                {
                    RaceID = r.RaceID,
                    RaceName = r.RaceName,
                    StartTime = r.StartTime,
                    Location = r.Location
                    // Map Distance or EventID here if they exist on your database model
                })
                .ToListAsync();
        }

        //update race
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<ActionResult> UpdateRace(int id, [FromBody] RaceDTO updateRaceDto)
        {
            // FIXED: Flipped to !ModelState.IsValid to block bad payloads instead of good ones
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var exisitingRace = await _context.Races.FindAsync(id);
            if (exisitingRace == null)
            {
                return NotFound($"Race with ID: {id} was not found");
            }

            exisitingRace.RaceName = updateRaceDto.RaceName;
            exisitingRace.Location = updateRaceDto.Location;
            // Map remaining parameters (e.g., StartTime, Distance) if supported by your database model

            await _context.SaveChangesAsync();
            return Ok();
        }

        //both view race events
        [HttpGet("all")]
        [Authorize(Roles = "Organiser, Participant")]
        public async Task<ActionResult<IEnumerable<RaceEventDTO>>> GetAllRace()
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

        // FIXED: Added path definition & ActionName to satisfy the target within CreatedAtAction
        [HttpGet("{RaceID:int}", Name = "GetRaceByID")]
        [Authorize(Roles = "Organiser, Participant")]
        public async Task<ActionResult<IEnumerable<RaceEvent>>> GetRace(int RaceID)
        {
            return Ok();
        }

        //organisers create events
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateRace([FromBody] RaceDTO racedto)
        {
            // FIXED: Flipped to !ModelState.IsValid to block bad payloads instead of good ones
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var NewRace = new Races
            {
                RaceName = racedto.RaceName,
                StartTime = racedto.StartTime,
                Location = racedto.Location
            };

            _context.Races.Add(NewRace);
            await _context.SaveChangesAsync();

            // The DTO gets updated with the newly generated primary key
            racedto.RaceID = NewRace.RaceID;

            return CreatedAtAction("GetRaceByID", new { RaceID = NewRace.RaceID }, racedto);
        }
    }
}
