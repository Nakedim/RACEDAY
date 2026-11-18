using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RaceEntriesController : ControllerBase
    {
        private readonly RacedayDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RaceEntriesController(RacedayDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/RaceEntries (For Organisers or general listing)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllEntries()
        {
            var entries = await _context.RaceEntries
                .Select(e => new
                {
                    e.RaceEntryID,
                    e.RaceEventID,
                    EventTitle = e.RaceEvent.Title,
                    e.ParticipantID,
                    ParticipantName = e.Participant.FirstName + " " + e.Participant.Surname,
                    e.CategoryID,
                    CategoryName = e.Category.CategoryName,
                    e.RegistrationDate
                })
                .ToListAsync();

            return Ok(entries);
        }

        // GET: api/RaceEntries/my-entries (For the logged-in Participant)
        [HttpGet("my-entries")]
        public async Task<IActionResult> GetMyEntries()
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(authenticatedUserId)) return Unauthorized();

            var runner = await _userManager.FindByIdAsync(authenticatedUserId);
            if (runner == null || runner.ProfileRole != ProfileRole.Participant)
            {
                return Forbid("Only registered participants can view personal race entries.");
            }

            var myEntries = await _context.RaceEntries
                .Where(e => e.ParticipantID == runner.Id)
                .Select(e => new
                {
                    e.RaceEntryID,
                    e.RaceEventID,
                    EventTitle = e.RaceEvent.Title,
                    e.ParticipantID,
                    ParticipantName = runner.FirstName + " " + runner.Surname,
                    e.CategoryID,
                    CategoryName = e.Category.CategoryName,
                    e.RegistrationDate
                })
                .ToListAsync();

            return Ok(myEntries);
        }

        // POST: api/RaceEntries (Registers the logged-in Participant)
        [HttpPost]
        public async Task<IActionResult> CreateEntry([FromBody] CreateRaceEntryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 1. Verify caller is a logged-in Participant
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
           
            if (string.IsNullOrEmpty(authenticatedUserId)) return Unauthorized();

            var runner = await _userManager.FindByIdAsync(authenticatedUserId);
            if (runner == null || runner.ProfileRole != ProfileRole.Participant)
            {
                return Forbid("Only registered participants can enter a race event.");
            }

            // 2. Verify a Participant profile record exists (mirrors your Organiser check)
            var participantProfileExists = await _context.Participants
                .AnyAsync(p => p.ApplicationUserId == runner.Id);

            if (!participantProfileExists)
            {
                return BadRequest("No Participant profile record exists for this user account.");
            }

            // 3. Validate RaceEvent and Category
            if (!await _context.RaceEvents.AnyAsync(e => e.EventID == request.RaceEventID))
            {
                return BadRequest("Selected race event does not exist.");
            }

            if (!await _context.Categories.AnyAsync(c => c.CategoryId == request.CategoryID))
            {
                return BadRequest("Selected category does not exist.");
            }

            // 4. Prevent duplicate registration
            if (await _context.RaceEntries.AnyAsync(e => e.RaceEventID == request.RaceEventID && e.ParticipantID == runner.Id))
            {
                return BadRequest("You are already registered for this race event.");
            }

            // 5. Save using runner.Id (ApplicationUser.Id)
            var entry = new RaceEntries
            {
                RaceEventID = request.RaceEventID,
                ParticipantID = runner.Id,
                CategoryID = request.CategoryID,
                RegistrationDate = DateTime.UtcNow
            };

            _context.RaceEntries.Add(entry);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                entry.RaceEntryID,
                entry.RaceEventID,
                entry.ParticipantID,
                entry.CategoryID,
                entry.RegistrationDate
            });
        }
    }

    public class CreateRaceEntryRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int RaceEventID { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CategoryID { get; set; }
    }
}
