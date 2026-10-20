using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Api.Models;
using RaceDay.Contracts;
using System.Security.Claims;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrganisersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        
        public OrganisersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // View an organiser's profile details by their alphanumeric Identity ID string
        [HttpGet("{id}")]
        [Authorize] // Requires being logged in
        public async Task<ActionResult> GetOrganiserbyID(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            // Verify the user exists AND their role is actually an Organiser
            if (user == null || user.profileRole != ProfileRole.Organiser)
            {
                return NotFound("Organiser profile not found.");
            }

            return Ok(new
            {
                user.Id,
                user.FirstName,
                user.Surname,
                user.Email
            });
        }

        // Updates the logged-in organiser's personal profile information
        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateOrgProfile([FromBody] OrganiserProfileDTO organiserProfileDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 1. Get the current logged-in user's ID from their token securely
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(authenticatedUserId))
            {
                return Unauthorized();
            }

            // 2. Fetch the profile record
            var user = await _userManager.FindByIdAsync(authenticatedUserId);
            if (user == null || user.profileRole != ProfileRole.Organiser)
            {
                return Forbid("Only registered organisers can modify this profile.");
            }

            // 3. Apply updates to the Identity model properties
            user.FirstName = organiserProfileDTO.FirstName;
            user.Surname = organiserProfileDTO.Surname;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { Message = "Profile updated successfully." });
        }

        // Deletes the logged-in organiser's account profile
        [HttpDelete("profile")]
        [Authorize]
        public async Task<IActionResult> DeleteOwnProfile()
        {
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(authenticatedUserId))
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(authenticatedUserId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }
    }
}
