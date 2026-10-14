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
    public class OrganisersController: ControllerBase
    {
        private readonly RacedayDbContext _context;

        public OrganisersController(RacedayDbContext context)
        {
            _context = context;
        }



        //Organiser create own Profile

        [HttpPost]
        //[Authorize (Roles = "Organiser, Participant" ) ]
        public async Task<ActionResult> CreateOrgProfile([FromBody]OrganiserProfileDTO organiserProfileDTO )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newOrganiser = new Organiser
            {
                FirstName = organiserProfileDTO.FirstName,
                Surname =  organiserProfileDTO.Surname
            };

            _context.Organisers.Add(newOrganiser);
            await _context.SaveChangesAsync();
            return
            CreatedAtAction(nameof(GetOrganiserbyID),
                 new { id = newOrganiser.OrganiserID }, newOrganiser
                 );

        }
        [HttpGet ("{id}")]
        //[Authorize (Roles = "Organiser, Participant")]
        public async Task<ActionResult>GetOrganiserbyID(int id)
        {
            var OrganiserID = await _context.Organisers.FindAsync(id);

            if (OrganiserID == null)
            {
                return NotFound();
            }
            return Ok(OrganiserID);
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
