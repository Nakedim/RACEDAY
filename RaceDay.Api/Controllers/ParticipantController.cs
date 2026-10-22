using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;
using RaceDay.Contracts.ProfileDTO;
using System.Security.Claims;
namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/participants")] 
    [Authorize]
    public class ParticipantController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        // Injected UserManager to handle all user accounts and roles directly
        public ParticipantController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateParticipantAccount([FromBody] ParticipantProfileDTO participantDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var participants = await _userManager.Users
                .Where(u => u.profileRole == ProfileRole.Participant)
                .Select(u => new ParticipantProfileDTO
                {
                    Id = Guid.Parse(u.Id), // Converts string Identity ID to Guid
                    FirstName = u.FirstName, // Assumes these fields exist on ApplicationUser
                    LastName = u.Surname,
                    Email = u.Email,
                }).ToListAsync();

            return Ok(participants);
        }


        // View all users who have the Participant role (Accessible only by Organisers)
        [HttpGet]
        public async Task<ActionResult<List<ApplicationUser>>> ViewAllParticipants()
        {
            // 1. Verify the caller is an Organiser
            var requestorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var requestor = await _userManager.FindByIdAsync(requestorId!);

            if (requestor == null || requestor.profileRole != ProfileRole.Organiser)
            {
                return Forbid();
            }

            // 2. Filter the users table to only return profiles marked as Participants
            return Ok();
        }

        // View a specific participant profile by their string Identity ID
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ApplicationUser>> ViewParticipantById(string id)
        {
            // 1. Verify the caller is an Organiser
            var requestorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var requestor = await _userManager.FindByIdAsync(requestorId!);

            if (requestor == null || requestor.profileRole != ProfileRole.Organiser)
            {
                return Forbid();
            }

            // 2. Find the target user profile
            var participant = await _userManager.FindByIdAsync(id);
            if (participant == null || participant.profileRole != ProfileRole.Participant)
            {
                return NotFound("Participant not found.");
            }
            var profileDto = new ParticipantProfileDTO
            {
                Id = Guid.Parse(participant.Id),
                FirstName = participant.FirstName,
                LastName = participant.Surname,
                Email = participant.Email!
              
            };


            return Ok(participant);
        }

        // View my own profile details (Accessible by logged-in Participants)
        [HttpGet("profile")]
        public async Task<ActionResult<ParticipantProfileDTO>> ViewOwnParticipantInfo()
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(authenticatedUserId))
            {
                return Unauthorized();
            }

            var userProfile = await _userManager.FindByIdAsync(authenticatedUserId);
            if (userProfile == null)
            {
                return NotFound();
            }

            var profileDto = new ParticipantProfileDTO
            {
                Id = Guid.Parse(userProfile.Id),
                FirstName = userProfile.FirstName,
                LastName = userProfile.Surname,
                Email = userProfile.Email!
            
            };

            return Ok(profileDto);
        }
    }
}
