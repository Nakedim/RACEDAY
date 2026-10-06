using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RACEDAY.Data;
using RACEDAY.Models;

namespace RACEDAY.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class EventsController: ControllerBase
    {
        private readonly RacedayDbContext _context;

        public EventsController(RacedayDbContext context)
        {
            _context = context;
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Events>>> GetAllEvents()
        {
            return await _context.Events.ToListAsync();
        }
    }
}
