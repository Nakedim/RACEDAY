using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts;
using RaceDay.Contracts.RaceRequestDTO;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using RaceDay.Contracts.CreateEventRequest_;

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
        public async Task<ActionResult<List<RaceEvent>>> ListAllEvents()
        {
            var eventsList = await _context.RaceEvents.ToListAsync();
            if (eventsList == null)
            {
                return NotFound();
            }
            return Ok(eventsList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RaceEvent>> GetRaceById(int id)
        {
            var eventsList = await _context.RaceEvents.FindAsync(id);
            if (eventsList == null)
            {
                return NotFound();
            }
            return Ok(eventsList);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<RaceEvent>> createEvent([FromBody] EventsCreateDTO eventDTO)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(authenticatedUserId)) return Unauthorized();

            var caller = await _userManager.FindByIdAsync(authenticatedUserId);
            if (caller == null || caller.profileRole != ProfileRole.Organiser)
            {
                return Forbid();
            }
            var RaceEvents = new RaceEvent
            {
                Title = eventDTO.Title,
                Description = eventDTO.Description,
                Location = eventDTO.Location,
                CategoryID = eventDTO.CategoryID,
                EventType = eventDTO.EventType,
                OrganiserID = eventDTO.OrganiserID
            };
            _context.RaceEvents.Add(RaceEvents);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRaceById),
                new { id = RaceEvents.EventID },
                RaceEvents
                );
        }

        [HttpPut]
        [Authorize]
       public async Task<IActionResult> updateEventsAsync(int id, [FromBody] UpdateEventDTO update)
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

            // 2. Fetch the profile record
            var user = await _userManager.FindByIdAsync(authenticatedUserId);
            if (user == null || user.profileRole != ProfileRole.Organiser)
            {
                return Forbid("Only registered organisers can modify this profile.");
            }

            var eventlookup = await _context.RaceEvents.FindAsync(id);
            if (eventlookup == null)
            {
                return NotFound(new { message = "Event not exits or wrong id provided" });
            }

            var newEvent = new UpdateEventDTO
            {
                Title = update.Title,
                Description = update.Description,
                Location = update.Location,
                CategoryID = update.CategoryID,
                EventType = update.EventType,
                OrganiserID = update.OrganiserID
            };
            return Ok();
            
        }
    }

}