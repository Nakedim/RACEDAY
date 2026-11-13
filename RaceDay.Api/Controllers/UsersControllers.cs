

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts;
using RaceDay.Contracts.Logins;
using RaceDay.Contracts.RegisterRegquest;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RaceDay.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthUsersController : ControllerBase
    {
        private readonly RoleManager<IdentityRole <int>> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly RacedayDbContext _context;

        public AuthUsersController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole<int>> roleManager,
            IConfiguration configuration,
            RacedayDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (!Enum.TryParse<ProfileRole>(model.ProfileRole, true, out var assignedRole)
                    || !Enum.IsDefined(typeof(ProfileRole), assignedRole))
                {
                    return BadRequest(new { Error = "Choose Organiser or Participant." });
                }

                string roleName = assignedRole.ToString();

                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var identityRole = new IdentityRole<int> { Name = roleName };
                    var roleResult = await _roleManager.CreateAsync(identityRole);
                    if (!roleResult.Succeeded)
                    {
                        return StatusCode(500, new { Error = "Role creation failed.", Details = roleResult.Errors });
                    }
                }

                // 🌟 Double check property casing on 'ProfileRole' vs 'profileRole' here:
                var newUser = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    Surname = model.Surname,
                    Location = model.Location,
                    ProfileRole = assignedRole, // Ensure PascalCase matches your ApplicationUser class definition!
                    Age = 25 // Hardcode a valid number temporarily to test if model.Age is pulling 0
                };

                var createResult = await _userManager.CreateAsync(newUser, model.Password);
                if (!createResult.Succeeded)
                {
                    return BadRequest(new { Error = "UserManager failed.", Details = createResult.Errors });
                }

                if (assignedRole == ProfileRole.Organiser)
                {
                    var organiserEntity = new Organiser
                    {
                        ApplicationUserId = newUser.Id,
                        FirstName = newUser.FirstName,
                        Surname = newUser.Surname
                    };
                    _context.Organisers.Add(organiserEntity);
                }
                else if (assignedRole == ProfileRole.Participant)
                {
                    var participantEntity = new Participant
                    {
                        ApplicationUserId = newUser.Id,
                        FirstName = newUser.FirstName,
                        Surname = newUser.Surname,
                        Age = 25,
                        Location = model.Location
                    };
                    _context.Participants.Add(participantEntity);
                }

                await _context.SaveChangesAsync();

                var assignRoleResult = await _userManager.AddToRoleAsync(newUser, roleName);
                if (!assignRoleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(newUser);
                    return StatusCode(500, assignRoleResult.Errors);
                }

                return Ok(new { Message = "Registration successful.", UserId = newUser.Id });
            }
            catch (Exception ex)
            {
                // 🚨 This will surface the EXACT underlying database or runtime error!
                return StatusCode(500, new
                {
                    Error = "An unhandled exception occurred during registration.",
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet("{id}")]
        [Authorize] 
        public async Task<ActionResult<ApplicationUser>> GetParticipantById(string id)
        {
       
            var requestorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(requestorId))
            {
                requestorId = HttpContext.Session.GetString("UserId");
            }

            if (string.IsNullOrEmpty(requestorId))
            {
                return Unauthorized();
            }

            var requestor = await _userManager.FindByIdAsync(requestorId);
            if (requestor == null)
            {
                return Unauthorized();
            }

            // 2. Fetch the target participant
            var participant = await _userManager.FindByIdAsync(id);
            if (participant == null)
            {
                return NotFound("User profile not found.");
            }

            // Allow access if requestor is an Organiser or looking at their own profile data.
            if (requestor.ProfileRole != ProfileRole.Organiser && requestorId != id)
            {
                return Forbid();
            }

            return Ok(participant);
        }

        // POST: api/AuthUsers/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest(new { Error = "Invalid login data." });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new { Error = "Invalid email or password. Looks like no Account has been created." });
            }

            var passwordResult = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
            if (!passwordResult.Succeeded)
            {
                return Unauthorized(new { Error = "Invalid email or password." });
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Count == 0)
            {
                return StatusCode(403, new { Error = "This account has no assigned role." });
            }

            // 1. Create the Identity Auth Cookie (populates User.Claims and handles [Authorize])
            await _signInManager.SignInAsync(user, isPersistent: true);

            // 2. Store user details in Session
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("UserRole", roles.First());

            // 3. Return nested User object matching LoginResultModel
            return Ok(new
            {
                Message = "Login successful.",
                User = new
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    Role = roles.First()
                }
            });
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return Ok(new { Message = "Logged out successfully." });
        }
    }
}
