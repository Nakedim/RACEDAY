using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;
using System.Security.Claims;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParticipantController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        // Injected UserManager to handle all user accounts and roles directly
        public ParticipantController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // View all users who have the Participant role (Accessible only by Organisers)
        [HttpGet]
        [Authorize] // Requires being logged in
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
            var participants = await _userManager.Users
                .Where(u => u.profileRole == ProfileRole.Participant)
                .ToListAsync();

            return Ok(participants);
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

            return Ok(participant);
        }

        // View my own profile details (Accessible by logged-in Participants)
        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<ApplicationUser>> ViewOwnParticipantInfo()
        {
            // 1. Get the current logged-in user's ID securely from the session token
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(authenticatedUserId))
            {
                return Unauthorized();
            }

            // 2. Fetch their clean account object directly
            var userProfile = await _userManager.FindByIdAsync(authenticatedUserId);
            if (userProfile == null)
            {
                return NotFound();
            }

            return Ok(userProfile);
        }

        // Delete/Remove a user profile (Accessible only by Organisers)
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteParticipant(string id)
        {
            // 1. Verify the caller is an Organiser
            var requestorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var requestor = await _userManager.FindByIdAsync(requestorId!);

            if (requestor == null || requestor.profileRole != ProfileRole.Organiser)
            {
                return Forbid();
            }

            // 2. Locate and delete the target profile
            var participant = await _userManager.FindByIdAsync(id);
            if (participant == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(participant);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }
    }
}
