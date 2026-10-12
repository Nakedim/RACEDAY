using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;
using RaceDay.Api.Data;

namespace RaceDay.Api.Controllers
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
