using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RACEDAY.Data;
using RACEDAY.Models;

namespace RACEDAY.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class OrganisersController: ControllerBase
    {
        private readonly RacedayDbContext _context;

        public OrganisersController(RacedayDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Organisers>>> GetOrganisers()
        {
            return await _context.Organisers.ToListAsync();
        }
    }
}
