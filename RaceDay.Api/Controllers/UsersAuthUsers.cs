
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly RacedayDbContext _context;


        public AuthUsersController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            RacedayDbContext context
            )
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

            if (!Enum.TryParse<ProfileRole>(model.ProfileRole, true, out var assignedRole)
                || !Enum.IsDefined(typeof(ProfileRole), assignedRole))
            {
                return BadRequest(new { Error = "Choose Organiser or Participant." });
            }

            string roleName = assignedRole.ToString();

            // Ensure the identity role exists
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (!roleResult.Succeeded)
                {
                    return StatusCode(500, roleResult.Errors);
                }
            }

            // 1. Instantiate and save the core Login user credentials
            var newUser = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                Surname = model.Surname,
                Location = model.Location,
                profileRole = assignedRole
            };

            var createResult = await _userManager.CreateAsync(newUser, model.Password);
            if (!createResult.Succeeded)
            {
                return BadRequest(createResult.Errors);
            }

            // 2. Build out specific domain extension table fields safely
            try
            {
                if (assignedRole == ProfileRole.Organiser)
                {
                    var organiserEntity = new Organiser
                    {
                        ApplicationUserId = newUser.Id, // Foreign Key link
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
                        Age = model.Age,       
                        Location = model.Location
                    };
                    _context.Participants.Add(participantEntity);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                // Fail-safe cleanup: deletes the user if profile assignment fails
                await _userManager.DeleteAsync(newUser);
                return StatusCode(500, new { Error = "Failed to create database profile row.", Details = e.Message });
            }

            // 3. Assign structural security roles
            var assignRoleResult = await _userManager.AddToRoleAsync(newUser, roleName);
            if (!assignRoleResult.Succeeded)
            {
                await _userManager.DeleteAsync(newUser);
                return StatusCode(500, assignRoleResult.Errors);
            }

            return Ok(new
            {
                Message = "Registration successful.",
                UserId = newUser.Id,
                Email = newUser.Email,
                Role = roleName
            });
        }
        //

        [HttpGet("{id}")]
        [Authorize] // Allows any authenticated user to enter
        public async Task<ActionResult<ApplicationUser>> GetParticipantById(string id)
        {
            // 1. Get the logged-in user making the request
            var requestorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var requestor = await _userManager.FindByIdAsync(requestorId!);

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

            // 💡 THE FIX: Allow the request if the requestor is an Organiser, 
            // OR if the requestor is looking at their own profile data.
            if (requestor.profileRole != ProfileRole.Organiser && requestorId != id)
            {
                return Forbid(); // Deny access if they are another participant trying to nose around
            }

            return Ok(participant);
        }


        // POST: api/AuthUsers/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Error = "Invalid login data."
                });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return Unauthorized(new
                {
                    Error = "Invalid email or password."
                });
            }

            var passwordResult =
                await _signInManager.CheckPasswordSignInAsync(
                    user,
                    model.Password,
                    lockoutOnFailure: false);

            if (!passwordResult.Succeeded)
            {
                return Unauthorized(new
                {
                    Error = "Invalid email or password."
                });
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Count == 0)
            {
                return StatusCode(403, new
                {
                    Error = "This account has no assigned role."
                });
            }

            var jwtKey = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(jwtKey) ||
                string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience))
            {
                return StatusCode(500, new
                {
                    Error = "JWT configuration is incomplete."
                });
            }

            var authClaims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier, user.Id),

                new Claim(
                    ClaimTypes.Name, user.Email ?? user.UserName ?? ""),

                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                authClaims.Add(
                    new Claim(ClaimTypes.Role, role));
            }

            var signingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey));

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                expires: DateTime.UtcNow.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(
                    signingKey,
                    SecurityAlgorithms.HmacSha256)
            );

            var tokenString =
                new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                Message = "Login successful.",
                Token = tokenString,
                Expiration = token.ValidTo,
                User = new
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    Surname = user.Surname,
                    Roles = roles
                }
            });
        }

        // POST: api/AuthUsers/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return Ok(new
            {
                Message = "Session cleared. Discard your JWT token."
            });
        }
    }
}
