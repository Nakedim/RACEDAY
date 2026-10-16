using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParticipantController : ControllerBase
    {
        private readonly RacedayDbContext _context;

        public ParticipantController(RacedayDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Participant>>> GetParticipant()
        {
            return await _context.Participants.ToListAsync();
        }

        //both view race events
        [HttpGet("info/all")]
        [Authorize(Roles = "Organiser")]
        public async Task<ActionResult<IEnumerable<RaceEvent>>> viewAllParticipantInfo()
        {
            return await _context.RaceEvents.ToListAsync();
        }

        [HttpGet("info/me")]
        [Authorize(Roles = "Participant")]
        public async Task<ActionResult<IEnumerable<RaceEvent>>> viewOwnParticipantInfo()
        {
            return await _context.RaceEvents.ToListAsync();
            //a user need to login with his own credentials
        }

        //organisers create events
        [HttpPost("event")]
        [Authorize(Roles = "Organiser")]
        public IActionResult CreateEvent()
        {
            return Ok();
        }
    }
}
