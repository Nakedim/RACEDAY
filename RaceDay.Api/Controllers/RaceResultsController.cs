using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts.ResultsDTO;
using System.Security.Claims;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RaceResultsController : ControllerBase
    {
        private readonly RacedayDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RaceResultsController(RacedayDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetRaceResults(int id)
        {
            var raceResult = await _context.RaceResults.FindAsync(id);
            if (raceResult == null) return NotFound();
            return Ok(raceResult);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<RaceResults>>> GetAllResults()
        {
            var results = await _context.RaceResults.ToListAsync();
            return Ok(results);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EnterResultsAsync([FromBody] ResultsRequestDTO resultsDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 1. Validate that the requesting user profile is an Organiser
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                                      ?? HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(authenticatedUserId)) return Unauthorized();

            // 💡 FIX: FindByIdAsync requires a string token parameter argument, which it then parses internally to an integer under the hood.
            var caller = await _userManager.FindByIdAsync(authenticatedUserId);
            if (caller == null || caller.profileRole != ProfileRole.Organiser)
            {
                return Forbid();
            }


            // 2. Verify that the targeted participant account exists in the system
            var participantExists = await _context.Participants
               .Include(p => p.User) // Loads the ApplicationUser relation
               .AnyAsync(p => p.ParticipantID == resultsDto.ParticipantID
                && p.User != null
                && p.User.profileRole == ProfileRole.Participant);

            if (!participantExists)
            {
                return BadRequest(new { Error = "The provided ParticipantID does not belong to a valid participant profile." });
            }

            
            var newResults = new RaceResults
            {
                RaceID = resultsDto.RaceID,
                ParticipantID = resultsDto.ParticipantID,
                FinishTime = resultsDto.FinishTime,
                Position = resultsDto.Position
            };

            _context.RaceResults.Add(newResults);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRaceResults), new { id = newResults.ResultID }, newResults);
        }
    }
}
