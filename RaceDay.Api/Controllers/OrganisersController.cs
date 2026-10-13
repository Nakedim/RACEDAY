using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;
using RaceDay.Api.Data;
using Microsoft.AspNetCore.Authorization;

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

 

        //both can view organiser

        [HttpGet]
        [Authorize (Roles = "Organiser, Participant")]
        public async Task<ActionResult<IEnumerable<Organiser>>> GetOrganisers()
        {
            return await _context.Organisers.ToListAsync();
        }

        //can update his own

        [HttpPut]
        [Authorize(Roles = "Organiser")]
        public IActionResult UpdateOrgProfile()
        {
            return Ok();
        }

        //can delete

        [HttpDelete]
        [Authorize(Roles = "Organiser")]
        public IActionResult DeleteOwnProfile()
        {
            return Ok();
        }
    }
}
