using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts;
using RaceDay.Contracts.CreateEventRequest_;
using RaceDay.Contracts.RaceRequestDTO;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RaceEventsController : ControllerBase
    {

        private readonly RacedayDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public RaceEventsController(RacedayDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> ListAllEvents()
        {
            // Projecting with .Select prevents lazy-loading loops and hides ApplicationUser secrets
            var eventsList = await _context.RaceEvents
                .Select(e => new
                {
                    e.EventID,
                    e.Title,
                    e.Description,
                    e.Location,
                    e.CategoryID,
                    e.EventType,
                    e.OrganiserID,
                    OrganiserName = e.Organiser != null ? e.Organiser.FirstName + " " + e.Organiser.Surname : null
                })
                .ToListAsync();

            return Ok(eventsList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRaceById(int id)
        {
            var raceEvent = await _context.RaceEvents
                .Where(e => e.EventID == id)
                .Select(e => new
                {
                    e.EventID,
                    e.Title,
                    e.Description,
                    e.Location,
                    e.CategoryID,
                    e.EventType,
                    e.OrganiserID,
                    OrganiserName = e.Organiser != null ? e.Organiser.FirstName + " " + e.Organiser.Surname : null
                })
                .FirstOrDefaultAsync();

            if (raceEvent == null)
            {
                return NotFound();
            }

            return Ok(raceEvent);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateEvent([FromBody] EventsCreateDTO eventDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(authenticatedUserId)) return Unauthorized();

            var caller = await _userManager.FindByIdAsync(authenticatedUserId);
            if (caller == null || caller.ProfileRole != ProfileRole.Organiser)
            {
                return Forbid();
            }

            var organiser = await _context.Organisers
                .FirstOrDefaultAsync(o => o.ApplicationUserId == caller.Id);

            if (organiser == null)
            {
                return BadRequest("No Organiser record exists for this user account.");
            }

            var raceEvent = new RaceEvent
            {
                Title = eventDTO.Title,
                Description = eventDTO.Description,
                Location = eventDTO.Location,
                CategoryID = eventDTO.CategoryID,
                EventType = eventDTO.EventType,
                OrganiserID = organiser.OrganiserID
            };

            _context.RaceEvents.Add(raceEvent);
            await _context.SaveChangesAsync();

            // Return a clean DTO instead of the raw entity so CreatedAtAction doesn't trigger lazy loading
            return CreatedAtAction(nameof(GetRaceById),
                new { id = raceEvent.EventID },
                new
                {
                    raceEvent.EventID,
                    raceEvent.Title,
                    raceEvent.Description,
                    raceEvent.Location,
                    raceEvent.CategoryID,
                    raceEvent.EventType,
                    raceEvent.OrganiserID
                }
            );
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateEventsAsync(int id, [FromBody] UpdateEventDTO update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(authenticatedUserId))
            {
                return Unauthorized();
            }

            // 1. Verify the user has the Organiser role
            var user = await _userManager.FindByIdAsync(authenticatedUserId);
            if (user == null || user.ProfileRole != ProfileRole.Organiser)
            {
                return Forbid("Only registered organisers can modify events.");
            }

            // 2. Verify an Organiser profile exists for this account
            var organiser = await _context.Organisers
                .FirstOrDefaultAsync(o => o.ApplicationUserId == user.Id);

            if (organiser == null)
            {
                return BadRequest("No Organiser record exists for this user account.");
            }

            // 3. Find the event
            var eventLookup = await _context.RaceEvents.FindAsync(id);
            if (eventLookup == null)
            {
                return NotFound(new { message = "Event does not exist or wrong id provided." });
            }

            // 4. Ensure this organiser owns the event
            if (eventLookup.OrganiserID != organiser.OrganiserID)
            {
                return Forbid("You can only edit events created by your own organiser account.");
            }

            // 5. Update event details (keeping the original OrganiserID intact)
            eventLookup.Title = update.Title;
            eventLookup.Description = update.Description;
            eventLookup.Location = update.Location;
            eventLookup.CategoryID = update.CategoryID;
            eventLookup.EventType = update.EventType;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Event updated successfully." });
        }
    }
    }