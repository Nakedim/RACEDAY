using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RACEDAY.Data;
using RACEDAY.Models;

namespace RACEDAY.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class RacesController: ControllerBase
    {
        private readonly RacedayDbContext _context;

        public RacesController(RacedayDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Races>>> GetParticipant()
        {
            return await _context.Races.ToListAsync();
        }
    }
}
