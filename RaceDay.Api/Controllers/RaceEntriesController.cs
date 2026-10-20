using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Contracts.EntryRequest;

namespace RaceDay.Api.Models
{
    [ApiController]
    [Route("api/[controller]")]
    public class RaceEventsController : ControllerBase
    {
        private readonly RacedayDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public RaceEventsController(RacedayDbContext context)
        {
            _context = context;
            _userManager = _userManager;

        }

        [HttpPost]
        [Authorize]//Organiser

        public async Task<ActionResult<RaceEntries>> CreateEvent([FromBody] CreateRaceEntryDTO raceEntryDTO)
        {
            var raceEntry = await _context.RaceEvents.FindAsync(raceEntryDTO.RaceID);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var authenticatedUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(authenticatedUserId))
            {
                return Unauthorized(new { Error = "User identification missing from authentication token." });
            }
            var user = await _userManager.FindByIdAsync(authenticatedUserId);
            if (user == null)
            {
                return NotFound(new { Error = "User profile context not found." });
            }
            if (user.profileRole != ProfileRole.Organiser)
            {
                return Forbid(); // Blocks participants from accessing organiser tools
            }

            var newEntry = new RaceEntries
            {
                RaceEventID = raceEntryDTO.RaceID,
                CategoryID = raceEntryDTO.CategoryID,
                ParticipantID = raceEntryDTO.ParticipantID,
                RegistrationDate = raceEntryDTO.RegistrationDate
            };
            _context.RaceEntries.Add(newEntry);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEntriesById), new {RaceEntryID = newEntry.RaceEntryID}, newEntry);

        }

        [HttpGet("{all}")]
        [Authorize]
        public async Task<ActionResult<RaceEntries>> GetAllEntries()
        {
            
            var entries = await _context.RaceEntries.ToListAsync();
            return Ok(entries);
        }



        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<List<RaceEntries>>>    GetEntriesById(int id)
        {  
            var entry = await _context.RaceEntries.FindAsync(id);
            if (entry == null)
            {
                return NotFound();
            }
            return Ok(entry);
        }


    }
}