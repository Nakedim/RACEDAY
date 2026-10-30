using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts.ParticipantDTO;
using RaceDay.Contracts.ProfileDTO;
using System.Security.Claims;
using ParticipantProfileDTO = RaceDay.Contracts.ProfileDTO.ParticipantProfileDTO;
namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/participants")]
    [Authorize]
    public class ParticipantController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RacedayDbContext _context;

        // Injected UserManager to handle all user accounts and roles directly
        public ParticipantController(UserManager<ApplicationUser> userManager,

            RacedayDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateParticipantAccount([FromBody] ParticipanregisterDTO participantDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = new ApplicationUser
            {
                UserName = participantDTO.Email,
                Email = participantDTO.Email,
                FirstName = participantDTO.FirstName,
                Surname = participantDTO.Surname,
                Location = participantDTO.Location,
                Age = participantDTO.Age,
                profileRole = ProfileRole.Participant
            };

            var result = await _userManager.CreateAsync(user, participantDTO.Password); // Assumes Password is in your DTO

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // 3. Assign the security framework role
            await _userManager.AddToRoleAsync(user, "Participant");

            // 4. Create your custom domain Entity tracking record
            var participantEntity = new Participant
            {
                ApplicationUserId = user.Id, // Link them together via Foreign Key
                FirstName = user.FirstName,
                Surname = user.Surname,
                Age = user.Age,
                Location = user.Location


            };

            _context.Participants.Add(participantEntity);
            await _context.SaveChangesAsync();

            var responseData = new
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                Age = user.Age,
                Location = user.Location,
                Surname = user.Surname
            };

            return CreatedAtAction(nameof(CreateParticipantAccount), new { id = user.Id }, responseData);
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
                return Forbid("Not authorised");
            }
            var AllParticipants = await _userManager.Users
                .Where(u => u.profileRole == ProfileRole.Participant)
                .ToListAsync();
            // 2. Filter the users table to only return profiles marked as Participants
            return Ok(AllParticipants);
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
                Id = participant.Id,
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
                Id = userProfile.Id,
                FirstName = userProfile.FirstName,
                LastName = userProfile.Surname,
                Email = userProfile.Email!

            };

            return Ok(profileDto);
        }

     
    }
}
