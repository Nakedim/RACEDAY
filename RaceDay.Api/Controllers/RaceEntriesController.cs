
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts;
using RaceDay.Contracts.EntryRequest;
using RaceEntriesDTO = RaceDay.Contracts.EntryRequest.RaceEntriesDTO;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RaceEntriesController : ControllerBase
    {
        private readonly RacedayDbContext _context;

        public RaceEntriesController(RacedayDbContext context)
        {
            _context = context;
        }

        // GET: api/RaceEntries
        // Organisers can view all race entries.
        [HttpGet]
        [Authorize(Roles = "Organiser")]
        public async Task<ActionResult<IEnumerable<RaceEntriesDTO>>>
        GetRaceEntries()
        {
            var entries = await _context.RaceEntries
                .Select(re => new RaceEntriesDTO
                {
                    RaceEntryID = re.RaceEntryID,
                    RaceID = re.RaceID,
                    ParticipantID = re.ParticipantID,
                    CategoryID = re.CategoryID,
                    EntryDate = re.EntryDate
                })
                .ToListAsync();

            return Ok(entries);
        }

        // POST: api/RaceEntries/enter
        // Participants can enter a race.
        [HttpPost("enter")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> EnterRace(
            [FromBody] CreateRaceEntryDTO model)
        {
            // Check that the race exists.
            var raceExists = await _context.Races
                .AnyAsync(r => r.RaceID == model.RaceID);

            if (!raceExists)
            {
                return NotFound("Race not found.");
            }

            // Check that the category exists.
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.CategoryID == model.CategoryID);

            if (!categoryExists)
            {
                return NotFound("Category not found.");
            }

            return BadRequest(
                "Race and category are valid, but the participant " +
                "must be linked to their authenticated account " +
                "before the entry can be saved.");
        }

        // GET: api/RaceEntries/all
        // Organisers can view all entries.
        [HttpGet("all")]
        [Authorize(Roles = "Organiser")]
        public async Task<ActionResult<IEnumerable<RaceEntriesDTO>>>
            ViewAllEntries()
        {
            return await GetRaceEntries();
        }
    }
}
