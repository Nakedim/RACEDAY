
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Contracts.EntryRequest;
using RaceDay.Api.Models;
using RaceDay.Contracts.RacesEntry;
using RaceDay.Api.Models.RaceDay.Api.Models;



namespace RaceDay.Api.Models
{
    [ApiController]
    [Route("api/[controller]")]
    public class RaceEntriesController : ControllerBase
    {
        private readonly RacedayDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // Injected UserManager to handle programmatic enum validation
        public RaceEntriesController(RacedayDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<RaceEntries>> CreateEvent([FromBody] CreateRaceEventEntryDTO raceEntryDTO)
        { 

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //check if the user is loggedin
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
            //if role is not participant disallow
            if (user.profileRole != ProfileRole.Participant)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Error = "Access denied. Only participants are allowed." });
            }
            
            var newEntry = new RaceEntries
            {
                RaceEventID = raceEntryDTO.RaceEventID,
                CategoryID = raceEntryDTO.CategoryID,
                ParticipantID = user.Id,
                RegistrationDate = DateTime.UtcNow
            };
            _context.RaceEntries.Add(newEntry);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEntriesById), new { id = newEntry.RaceEntryID }, newEntry);

        }

        [HttpGet("{all}")]
        [Authorize]
        public async Task<ActionResult<RaceEntries>> GetAllEntries()
        {

            var entries = await _context.RaceEntries.ToListAsync();
            return Ok(entries);
        }

        [HttpGet("{id}")]
     
        public async Task<ActionResult<List<RaceEntries>>> GetEntriesById(int id)
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