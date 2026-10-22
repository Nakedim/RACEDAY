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
            var eventsList = await _context.RaceEvents.FindAsync();
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
                CategoryID = eventDTO.CategoryID,
                EventType =eventDTO.EventType,
                OrganiserID = eventDTO.OrganiserID
            };
            return CreatedAtAction(nameof(GetRaceById),
                new {id = RaceEvents.EventID},
                RaceEvents
                );
        }
    }

}