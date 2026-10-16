using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
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
        // POST: api/Organisers
        [HttpPost]
        //[Authorize(Roles = "Organiser")] // or "Admin" — decide who can create organisers
        //[ProducesResponseType(StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrganiserDTO>> CreateOrganiser(
     [FromBody] OrganiserDTO organiserDto,
     CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var organiser = new Organiser
            {
                FirstName = organiserDto.FirstName,
                Surname = organiserDto.Surname
            };

            _context.Organiser.Add(organiser);
            await _context.SaveChangesAsync(cancellationToken);

            var result = new OrganiserDTO
            {
                OrganiserID = organiser.OrganiserID,
                FirstName = organiser.FirstName,
                Surname = organiser.Surname
            };

            return CreatedAtRoute(
           "GetOrganiserById",
           new { id = organiser.OrganiserID },
           result
       );
        }
        // GET: api/Organisers
        [HttpGet]
        [Authorize(Roles = "Organiser,Participant")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrganiserDTO>>> GetOrganisers(
            CancellationToken cancellationToken)
        {
            var organisers = await _context.Organiser
                .AsNoTracking()
                .Select(o => new OrganiserDTO
                {
                    OrganiserID = o.OrganiserID,
                    FirstName = o.FirstName,
                    Surname = o.Surname
                })
                .ToListAsync(cancellationToken);

            return Ok(organisers);
        }

        // PUT: api/Organisers/profile
        [HttpPut("profile")]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrgProfile(
            [FromBody] OrganiserProfileDTO profileDto,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!TryGetOrganiserId(out var organiserId))
                return Unauthorized();

            var organiser = await _context.Organiser
                .FirstOrDefaultAsync(o => o.OrganiserID == organiserId, cancellationToken);

            if (organiser == null)
                return NotFound();

            organiser.FirstName = profileDto.FirstName;
            organiser.Surname = profileDto.Surname;
            // ...any other editable fields

            await _context.SaveChangesAsync(cancellationToken);
            return NoContent();
        }
        // GET: api/Organisers/5
        [HttpGet("{id:int}", Name = "GetOrganiserById")]
        [Authorize(Roles = "Organiser,Participant")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrganiserDTO>> GetOrganiser(
            int id,
            CancellationToken cancellationToken)
        {
            var organiser = await _context.Organiser
                .AsNoTracking()
                .Where(o => o.OrganiserID == id)
                .Select(o => new OrganiserDTO
                {
                    OrganiserID = o.OrganiserID,
                    FirstName = o.FirstName,
                    Surname = o.Surname
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (organiser == null)
                return NotFound();

            return Ok(organiser);
        }



        // DELETE: api/Organisers/profile
        [HttpDelete("profile")]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteOwnProfile(CancellationToken cancellationToken)
        {
            if (!TryGetOrganiserId(out var organiserId))
                return Unauthorized();

            var organiser = await _context.Organiser
                .FirstOrDefaultAsync(o => o.OrganiserID == organiserId, cancellationToken);

            if (organiser == null)
                return NotFound();

            var hasRaces = await _context.Races
                .AnyAsync(r => r.OrganiserID == organiserId, cancellationToken);

            if (hasRaces)
                return Conflict("Cannot delete profile while races are still assigned.");

            _context.Organiser.Remove(organiser);
            await _context.SaveChangesAsync(cancellationToken);
            return NoContent();
        }

        private bool TryGetOrganiserId(out int organiserId)
        {
            organiserId = 0;
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("OrganiserID")?.Value;
            return int.TryParse(claim, out organiserId);
        }
    }
}