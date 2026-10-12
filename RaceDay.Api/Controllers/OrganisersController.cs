using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;
using RaceDay.Api.Data;

namespace RaceDay.Api.Controllers
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
        public async Task<ActionResult<IEnumerable<Organiser>>> GetOrganisers()
        {
            return await _context.Organisers.ToListAsync();
        }
    }
}
