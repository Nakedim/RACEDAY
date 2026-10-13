using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Api.Data;
using RaceDay.Api.Filters;
using RaceDay.Api.Models;
using RaceDay.Contracts;
using System.Security.Claims;

namespace RaceDay.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthUsersController : ControllerBase // Updated naming style to match standard Controller patterns
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthUsersController(
           UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager,
           RoleManager<IdentityRole> roleManager

         )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            roleManager = roleManager;

        }

        [HttpPost("register")]
        [ValidateModel]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            // 1. Guard clause against invalid input
            if (model == null)
            {
                return BadRequest("Invalid registration data.");
            }

            // 2. Parse and validate the incoming role
            if (!Enum.TryParse<ProfileRole>(model.ProfileRole, true, out var assignedRole))
            {
                return BadRequest(new { Error = $"Invalid role provided. Valid roles are: {string.Join(", ", Enum.GetNames(typeof(ProfileRole)))}" });
            }

            // 3. Map DTO to Identity model cleanly
            var newUser = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                Surname = model.Surname,

            };

            // 4. Create user via ASP.NET Identity UserManager
            var results = await _userManager.CreateAsync(newUser, model.Password);

            if (!results.Succeeded)
            {
                return BadRequest(results.Errors);
            }

            // 5. Assign user to the parsed role in security storage
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
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest(new { Error = "Invalid login payload." });
            }

            // 1. Find user by email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new { Error = "Invalid email or password." });
            }

            // 2. Validate password using SignInManager
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.PasswordHashed, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return Unauthorized(new { Error = "Invalid email or password." });
            }

            // 3. Retrieve the user's role (e.g., Organiser or Participant)
            var roles = await _userManager.GetRolesAsync(user);
            var primaryRole = roles.FirstOrDefault() ?? "Participant"; // Default fallback if no role assigned

            // 4. Establish Server-Side Session State (Matching your requirement)
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Role", primaryRole);

            return Ok(new
            {
                Message = "Login successful",
                User = new { Id = user.Id, Email = user.Email, Role = primaryRole }
            });
        }
    }
}
