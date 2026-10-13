using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;
using RaceDay.Api.Data;

namespace RaceDay.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class RaceEventsController: ControllerBase
    {
        private readonly RacedayDbContext _context;

        public RaceEventsController(RacedayDbContext context)
        {
            _context = context;
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RaceEvent>>> GetAllEvents()
        {
            return await _context.RaceEvents.ToListAsync();
        }
    }
}
