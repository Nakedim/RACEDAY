using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity; // <-- 1. CRITICAL: Make sure this namespace is here!
using Microsoft.AspNetCore.Mvc;
using RaceDay.Api.Models;
using RaceDay.Contracts;
using System.Threading.Tasks;

namespace RaceDay.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthUsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager _roleManager; // Fixed: Added generic parameter type

        public AuthUsersController(
           UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager,
           RoleManager roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpPost("register")]
        [ValidateModel]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            if (model == null)
            {
                return BadRequest("Invalid registration data.");
            }

            // Parse and validate the incoming role enum matching the DTO string input
            if (!Enum.TryParse<ProfileRole>(model.ProfileRole, true, out var assignedRole))
            {
                return BadRequest(new { Error = $"Invalid role provided. Valid roles are: {string.Join(", ", Enum.GetNames(typeof(ProfileRole)))}" });
            }

            var newUser = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                Surname = model.Surname,
            };

            var results = await _userManager.CreateAsync(newUser, model.Password);
            if (!results.Succeeded)
            {
                return BadRequest(results.Errors);
            }

            var roleResult = await _userManager.AddToRoleAsync(newUser, assignedRole.ToString());
            if (!roleResult.Succeeded)
            {
                return BadRequest(roleResult.Errors);
            }

            return Ok(new { Message = "Registration successful" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid email or password combination." });
            }

            var result = await _signInManager.PasswordSignInAsync(
                userName: user.UserName!,
                password: model.PasswordHashed, // Note: Ensure this contains cleartext since Identity hashes internally
                isPersistent: model.RememberMe,
                lockoutOnFailure: true
            );

            if (result.Succeeded)
            {
                // Fixed: Changed from GetUsersInRoleAsync to GetRolesAsync to fetch the user's roles
                var roles = await _userManager.GetRolesAsync(user);
                var defaultRole = roles.FirstOrDefault() ?? "Participant";

                // Save user identity details inside the server-side session store
                HttpContext.Session.SetString("UserId", user.Id);
                HttpContext.Session.SetString("Role", defaultRole); // Fixed: Replaced undeclared variable with defaultRole

                return Ok(new
                {
                    Message = "Login successful!",
                    User = new { Id = user.Id, Role = defaultRole }
                });
            }

            if (result.IsLockedOut)
            {
                return StatusCode(StatusCodes.Status423Locked, new
                {
                    Message = "Account locked due to too many failed login attempts. Please try again in 15 minutes."
                });
            }

            return Unauthorized(new { Message = "Invalid email or password combination." });
        }
    }
}
