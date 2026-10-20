using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts.ResultsdTO;
using System.Security.Claims;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RaceResultsController : ControllerBase
    {
        private readonly RacedayDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // Injected UserManager to handle programmatic enum validation
        public RaceResultsController(RacedayDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Fetch a specific race result record by its numeric primary ID
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetRaceResults(int id)
        {
            // FIXED: Param variable renamed to 'id' to map route pattern and properly passed to FindAsync
            var raceResult = await _context.RaceResults.FindAsync(id);

            if (raceResult == null) return NotFound();

            return Ok(raceResult);
        }

        // Fetch all results recorded across the platform
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<RaceResults>>> GetAllResults()
        {
            var results = await _context.RaceResults.ToListAsync();
            return Ok(results);
        }

        // Organisers can securely create and post race results
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateResults([FromBody] ResultsRequestResponseDTO resultsRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 1. Validate that the requesting user profile is an Organiser
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var caller = await _userManager.FindByIdAsync(authenticatedUserId!);

            if (caller == null || caller.profileRole != ProfileRole.Organiser)
            {
                return Forbid();
            }

            // 2. Verify that the targeted participant account exists in the system
            var participantExists = await _userManager.Users
                .AnyAsync(u => u.Id == resultsRequestDTO.ParticipantID && u.profileRole == ProfileRole.Participant);

            if (!participantExists)
            {
                return BadRequest(new { Error = "The provided ParticipantID does not belong to a valid participant profile." });
            }

            var newResults = new RaceResults
            {
                RaceID = resultsRequestDTO.RaceID,
                ParticipantID = resultsRequestDTO.ParticipantID, // Safely maps string ID context now
                FinishTime = resultsRequestDTO.FinishTime.ToString("HH:mm:ss"),
                Position = resultsRequestDTO.Position
            };

            _context.RaceResults.Add(newResults);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRaceResults), new { id = newResults.ResultID }, newResults);
        }
    }
}
