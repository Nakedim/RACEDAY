//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using RaceDay.Api.Data;
//using RaceDay.Api.Models;
//using RaceDay.Contracts;
//using RaceDay.Contracts.CreateEventRequest_;
//using RaceDay.Contracts.ResultsdTO;

//namespace RaceDay.Api.Controllers
//{

//    [ApiController]
//    [Route("api/[controller]")]
//    public class RaceEventsController : ControllerBase
//    {
//        private readonly RacedayDbContext _context;

//        private readonly UserManager<ApplicationUser> _userManager;

//        public RaceEventsController(RacedayDbContext context)
//        {
//            _context = context;
//            _userManager = _userManager;

//        }

//        //both view race events
//        [HttpGet]
//        [Authorize]
       
//        public async Task<ActionResult<List<RaceEvent>>> GetAllEvents()
//        {
//            var events = await _context.RaceEvents.ToListAsync();
//            return Ok(events);

//        }

//        [HttpGet("{RaceEventID}")]
//        [Authorize]
//        public async Task<IActionResult>GetEventsById(int RaceEventID)
//        {
//            var raceEvent = await _context.RaceEvents.FindAsync(RaceEventID);

//            if (raceEvent == null)
//            {
//                return NotFound();
//            }
//            return Ok(raceEvent);
//        }

//        //testing purpose only
        
//        [HttpPost]
//        [Authorize]
//        public async Task<IActionResult> CreateEvent([FromBody] EventsCreateDTO raceEventDTO)
//        {

//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }
//            var authenticatedUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
//            if (string.IsNullOrEmpty(authenticatedUserId))
//            {
//                return Unauthorized(new { Error = "User identification missing from authentication token." });
//            }
//            var user = await _userManager.FindByIdAsync(authenticatedUserId);
//            if (user == null)
//            {
//                return NotFound(new { Error = "User profile context not found." });
//            }
//            if (user.profileRole != ProfileRole.Organiser)
//            {
//            return Forbid(); // Blocks participants from accessing organiser tools
//            }
                
//            var NewEvents = new RaceEvent
//            {
//                Description = raceEventDTO.Description,
//                Title = raceEventDTO.Title,
//                Location = raceEventDTO.Location,
//                EventType = raceEventDTO.EventType,
//                OrganiserID = authenticatedUserId


//            };
//            _context.RaceEvents.Add(NewEvents);
//            await _context.SaveChangesAsync();
//            return CreatedAtAction(nameof(GetEventsById), new { RaceEventID = NewEvents.EventID }, NewEvents);

          
//        }
        

//    }
//}
