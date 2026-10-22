using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor handles Dependency Injection cleanly here
        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: api/Profile
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            // Fixed session key casing to match your Login setup ("UserId")
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                // Fixed typo from UnAuthorized -> Unauthorized
                return Unauthorized(new { Error = "You must be logged in to view your profile." });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { Error = "User profile not found." });
            }

            return Ok(new
            {
                user.Id,
                user.Email,
                user.FirstName,
                user.Surname,
                Role = user.profileRole.ToString()
            });
        }

    }
}
