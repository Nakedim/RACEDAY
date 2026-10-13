using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class RaceEntriesController: ControllerBase
    {
        private readonly RacedayDbContext _context;

        public RaceEntriesController(RacedayDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Authorize(Roles = "Organiser")]
        public async Task<ActionResult<IEnumerable<RaceEntries>>> GetRaceEntries()
        {
            return await _context.RaceEntries.ToListAsync();
        }


        [HttpPost]
        [Authorize(Roles = "Participant")]
        public async Task<ActionResult<IEnumerable<RaceEvent>>> EnterRace()
        {
            return await _context.RaceEvents.ToListAsync();
        }



        [HttpGet]
        [Authorize(Roles = "Organiser")]
        public async Task<ActionResult<IEnumerable<RaceEvent>>> ViewAllEntries()
        {
            return await _context.RaceEvents.ToListAsync();
        }

    }
}
