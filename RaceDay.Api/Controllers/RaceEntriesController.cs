
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
        [Authorize]//Organiser

        public async Task<ActionResult<RaceEntries>> CreateEvent([FromBody] CreateRaceEntryDTO raceEntryDTO)
        {
            var raceEntry = await _context.RaceEntries.FindAsync(raceEntryDTO.RaceEntryID);

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
            if (user.profileRole != ProfileRole.Participant)
            {
                return Forbid(); 
            }
            //var raceExists = await _context.RaceEvents.AnyAsync(r => r.RaceID == model.RaceID);
            //var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryID == model.CategoryID);


            var newEntry = new RaceEntries
            {
                RaceEventID = raceEntryDTO.RaceID,
                CategoryID = raceEntryDTO.CategoryID,
                ParticipantID = raceEntryDTO.ParticipantID,
                RegistrationDate = raceEntryDTO.RegistrationDate
            };
            _context.RaceEntries.Add(newEntry);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEntriesById), new { RaceEntryID = newEntry.RaceEntryID }, newEntry);

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
        public async Task<ActionResult<List<RaceEntries>>> GetEntriesById(int id)
        {
            var entry = await _context.RaceEntries.FindAsync(id);
            if (entry == null)
            {
                return NotFound();
            }
            return Ok(entry);
        }



        // GET: api/RaceEntries
        // Organisers can view all race entries.
        //[HttpGet]
        //[Authorize(Roles = "Organiser")]
        //public async Task<ActionResult<IEnumerable<RaceEntriesDTO>>> GetRaceEntries()
        //{
        //    var entries = await _context.RaceEntries
        //        .Select(re => new RaceEntriesDTO
        //        {
        //            RaceEntryID = re.RaceEntryID,

        //        })
        //        .ToListAsync();

        //    return Ok(entries);
        //}

        //// POST: api/RaceEntries/enter
        //// Participants can enter a race.
        //[HttpPost("enter")]
        //[Authorize(Roles = "Participant")]
        //public async Task<IActionResult> EnterRace(
        //    [FromBody] CreateRaceEntryDTO model)
        //{
        //    // Check that the race exists.
        //    var raceExists = await _context.Races
        //        .AnyAsync(r => r.RaceID == model.RaceID);

        //    if (!raceExists)
        //    {
        //        return NotFound("Race not found.");
        //    }

        //    // Check that the category exists.
        //    var categoryExists = await _context.Categories
        //        .AnyAsync(c => c.CategoryID == model.CategoryID);

        //    if (!categoryExists)
        //    {
        //        return NotFound("Category not found.");
        //    }

        //    return BadRequest(
        //        "Race and category are valid, but the participant " +
        //        "must be linked to their authenticated account " +
        //        "before the entry can be saved.");
        //}



    }
}