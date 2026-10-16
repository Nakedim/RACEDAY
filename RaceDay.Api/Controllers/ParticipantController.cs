using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts;
using System.Security.Claims;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/participants")] 
    [Authorize]
    public class ParticipantController : ControllerBase
    {
        private readonly RacedayDbContext _context;

        public ParticipantController(RacedayDbContext context)
        {
            _context = context;
        }

        // 1. GET BY ID: api/participants/1
        [HttpGet("{id:int}", Name = "GetParticipantById")]
        [Authorize(Roles = "Participant,Organiser")]
        [Produces("application/json")] 
        public async Task<ActionResult<ParticipantProfileDTO>> GetParticipant(int id)
        {
            var participant = await _context.Participants
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ParticipantID == id);

            if (participant == null)
            {
                return NotFound("Participant profile not found.");
            }

            var resultDto = new ParticipantProfileDTO
            {
                ParticipantID = participant.ParticipantID,
                FirstName = participant.FirstName,
                Surname = participant.Surname,
                Age = participant.Age,
                Location = participant.Location
            };

            return Ok(resultDto);
        }

        // 2. CREATE PROFILE: api/participants
        [HttpPost]
        //[Authorize(Roles = "Participant")]
        public async Task<ActionResult<ParticipantProfileDTO>> CreateParticipant([FromBody] ParticipantProfileDTO participantDto)
        {
            // FIX: Fallback lookup chain to handle different JWT token formats
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue(ClaimTypes.Name)
                         ?? User.FindFirstValue("sub"); // Standard OpenID Connect subject token claim

            if (string.IsNullOrWhiteSpace(userId))
            {
                // Change this temporary test response to a 400 BadRequest with an explicit message
                // so you can see exactly if the claim parsing is what fails!
                return BadRequest(new { error = "Auth identity token missing valid user ID claims." });
            }

            var profileExists = await _context.Participants
                .AnyAsync(p => p.ApplicationUserId == userId);

            if (profileExists)
            {
                return Conflict("A participant profile already exists.");
            }

            var participantEntity = new Participant
            {
                FirstName = participantDto.FirstName,
                Surname = participantDto.Surname,
                Age = participantDto.Age,
                Location = participantDto.Location,
                ApplicationUserId = userId
            };

            _context.Participants.Add(participantEntity);
            await _context.SaveChangesAsync();

            var resultDto = new ParticipantProfileDTO
            {
                ParticipantID = participantEntity.ParticipantID,
                FirstName = participantEntity.FirstName,
                Surname = participantEntity.Surname,
                Age = participantEntity.Age,
                Location = participantEntity.Location
            };

            // Safely returns the dynamic object link
            return CreatedAtRoute(
                 "GetParticipantById",
                 new { id = participantEntity.ParticipantID },
                 resultDto);
        }

        // 3. UPDATE OWN PROFILE: api/participants/profile
        [HttpPut("profile")]
        [Authorize(Roles = "Participant")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateParticipantProfile([FromBody] ParticipantProfileDTO participantDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var participant = await _context.Participants
                .FirstOrDefaultAsync(p => p.ApplicationUserId == userId);

            if (participant == null)
                return NotFound("Profile not found.");

            participant.FirstName = participantDto.FirstName;
            participant.Surname = participantDto.Surname;
            participant.Age = participantDto.Age;
            participant.Location = participantDto.Location;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 4. DELETE OWN PROFILE: api/participants/profile
        [HttpDelete("profile")]
        [Authorize(Roles = "Participant")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteParticipantProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var participant = await _context.Participants
                .FirstOrDefaultAsync(p => p.ApplicationUserId == userId);

            if (participant == null)
                return NotFound("Profile not found.");

            _context.Participants.Remove(participant);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
