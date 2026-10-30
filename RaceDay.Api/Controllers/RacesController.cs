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
using RaceDTO = RaceDay.Contracts.RaceRequestDTO.RaceDTO;


namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RacesController : ControllerBase
    {
        private readonly RacedayDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RacesController(RacedayDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet] 
        public async Task<ActionResult<List<RaceResponseDTO>>> GetAllRace()
        {
            return await _context.Races
               .Select(re => new RaceResponseDTO
               {
                   RaceName = re.RaceName,
                   StartTime = re.StartTime,
                   Location = re.Location,
                   Distance = re.Distance
               })
               .ToListAsync();
        }

        [HttpGet("{id:int}", Name = "GetRaceByID")]
        [Authorize]
        public async Task<ActionResult<Races>> GetRace(int id)
        {
            var race = await _context.Races.FindAsync(id);
            if (race == null)
            {
                return NotFound();
            }
            return Ok(race);
        }

        // Organisers create events
        [HttpPost]
        [Authorize]
        //[Authorize(Roles = "Organiser")] // Replaces manual userManager & role checks
        public async Task<IActionResult> CreateRace([FromBody] RaceDTO racedto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 1. Validate that the requesting user profile is an Organiser (Same logic applied)
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(authenticatedUserId)) return Unauthorized();

            var caller = await _userManager.FindByIdAsync(authenticatedUserId);
            if (caller == null || caller.profileRole != ProfileRole.Organiser)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { Error = "Access denied. Only organisers are allowed." });
            }

            var NewRace = new Races
            {
                RaceName = racedto.RaceName,
                CategoryID = racedto.CategoryID,
                StartTime = racedto.StartTime,
                EventID = racedto.EventID,
                Location = racedto.Location,
                Distance = racedto.Distance
            };

            _context.Races.Add(NewRace);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRace), new { id= NewRace.RaceID }, NewRace);
         

        }
    }
}
