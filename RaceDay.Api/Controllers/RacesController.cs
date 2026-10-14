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
        [HttpPut("{id:int}")]
        [Authorize (Roles ="Organiser")]
        
        public async Task<ActionResult> UpdateRace( int id, [FromBody] RaceDTO updateRaceDto)
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
        [HttpGet]
        [Authorize(Roles = "Organiser, Participant")]
        public async Task<ActionResult<IEnumerable<RaceEvent>>> GetAllRace()        {
            return await _context.RaceEvents.ToListAsync();
        }
        [HttpGet]
        [Authorize(Roles = "Organiser, Participant")]
        public async Task<ActionResult<IEnumerable<RaceEvent>>> GetRace( int RaceID)
        {
            return Ok();
        }

        //organisers create events
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateRace([FromBody] RaceDTO racedto)
            
        {
            if (ModelState.IsValid)
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
            return CreatedAtAction("GetRaceByID", new {id = NewRace.RaceID}, NewRace);
          
        }
               
        }
    }
