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

        //get all participant
        [HttpGet]
        //only organisers are allow to view all participant
        public async Task<ActionResult<IEnumerable<Participant>>> GetParticipant()
        {
            return await _context.Participants.ToListAsync();
        }

        //both view race events
        [HttpGet("{id}")]
        //[Authorize(Roles = "Organiser")]
        public async Task<ActionResult<Participant>> viewParticipant(int id)
        {
            var partipant = await _context.Participants.FindAsync();
            if(partipant == null)
            {
                return NotFound();
            }
            return Ok(partipant);
        }

        [HttpGet]
        [Authorize(Roles = "Participant")]
        public async Task<ActionResult<IEnumerable<RaceEvent>>> viewOwnParticipantInfo()
        {
            return await _context.RaceEvents.ToListAsync();

            //a user need to login with his own credentials
        }


        //update participant
        [HttpPut("{id}")]
        //[Authorize(Roles = "Organiser")]
        public async Task<ActionResult<Participant>> UpdateParticipant(int id)
        {
            var partipant = await _context.Participants.FindAsync();
            if (partipant == null)
            {
                return NotFound();
            }
            _context.Participants.Remove(partipant);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }

}
