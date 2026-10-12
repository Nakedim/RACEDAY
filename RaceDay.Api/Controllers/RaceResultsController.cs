using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;
using RaceDay.Api.Data;

namespace RaceDay.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class RaceResultsController: ControllerBase
    {
        private readonly RacedayDbContext _context;

        public RaceResultsController(RacedayDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RaceResults>>> GetRaceResults()
        {
            return await _context.RaceResults.ToListAsync();
        }
    }
}
