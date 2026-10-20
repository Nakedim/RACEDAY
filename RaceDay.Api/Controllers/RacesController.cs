using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts;
using RaceDay.Contracts.RaceRequestDTO;

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
        public async Task<ActionResult<IEnumerable<Races>>> GetParticipant()
        {
            return await _context.Races.ToListAsync();
        }

        //update race
        //test purpose only
        [HttpPut("{id:int}")]
        //[Authorize(Roles = "Organiser")]

        public async Task<ActionResult> UpdateRace(int id, [FromBody] RaceDTO updateRaceDto)
        {
            if (ModelState.IsValid)
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

            await _context.SaveChangesAsync();
            return NoContent();
        }



        //both view race events
        [HttpGet("{ALL}")]
        [Authorize(Roles = "Organiser, Participant")]
        public async Task<ActionResult<List<RaceEvent>>> GetAllRace()
        {
            var races = await _context.Races.ToListAsync();
            return Ok(races);
        }

        [HttpGet("{RaceID}")]
        //[Authorize(Roles = "Organiser, Participant")]
        public async Task<ActionResult<IEnumerable<RaceEvent>>> GetRace(int RaceID)
        {
            var race = await _context.Races.FindAsync(RaceID);
            if (race == null)
            {
                return NotFound();
            }
            return Ok(race);
        }

        //organisers create events
        [HttpPost]
        //[Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateRace([FromBody] RaceRequestDTO racedto)

        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var NewRace = new Races
            {
                RaceName = racedto.RaceName,
                StartTime = racedto.StartTime,
                EventID = racedto.EventID,
                Location = racedto.Location,
                Distance = racedto.Distance

            };
            _context.Races.Add(NewRace);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRace),
                new { RaceID = NewRace.RaceID }, NewRace);

        }

    }
}
