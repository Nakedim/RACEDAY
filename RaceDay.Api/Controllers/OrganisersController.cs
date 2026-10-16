using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;
using RaceDay.Api.Data;
using Microsoft.AspNetCore.Authorization;
using RaceDay.Contracts;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrganisersController : ControllerBase
    {
        private readonly RacedayDbContext _context;

        public OrganisersController(RacedayDbContext context)
        {
            _context = context;
        }

        //both can view organiser
        [HttpGet]
        //[Authorize(Roles = "Organiser, Participant")]
        public async Task<ActionResult<IEnumerable<OrganiserDTO>>> GetOrganisers()
        {
            return await _context.Organisers
                .Select(o => new OrganiserDTO
                {
                    OrganiserID = o.OrganiserID,
                    FirstName = o.FirstName,
                    Surname = o.Surname
                })
                .ToListAsync();
        }

        //can update his own
        [HttpPut("profile")]
        [Authorize(Roles = "Organiser")]
        public IActionResult UpdateOrgProfile([FromBody] OrganiserProfileDTO profileDto)
        {
            return Ok();
        }

        //can delete
        [HttpDelete("profile")]
        [Authorize(Roles = "Organiser")]
        public IActionResult DeleteOwnProfile()
        {
            return Ok();
        }
    }
}
