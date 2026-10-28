

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

            if (!Enum.TryParse<ProfileRole>(model.ProfileRole, true, out var assignedRole)
                || !Enum.IsDefined(typeof(ProfileRole), assignedRole))
            {
                return BadRequest(new { Error = "Choose Organiser or Participant." });
            }

            string roleName = assignedRole.ToString();

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var identityRole = new IdentityRole<int>
                { 
                    Name = roleName
                };
                var roleResult = await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
                if (!roleResult.Succeeded)
                {
                    return StatusCode(500, roleResult.Errors);
                }
            }

            var newUser = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                Surname = model.Surname,
                Location = model.Location,
                profileRole = assignedRole,
                Age = model.Age
            };

            var createResult = await _userManager.CreateAsync(newUser, model.Password);
            if (!createResult.Succeeded)
            {
                return BadRequest(createResult.Errors);
            }

            try
            {
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
                        Age = model.Age,
                        Location = model.Location
                    };
                    _context.Participants.Add(participantEntity);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                await _userManager.DeleteAsync(newUser);
                return StatusCode(500, new { Error = "Failed to create database profile row.", Details = e.Message });
            }

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
            if (requestor.profileRole != ProfileRole.Organiser && requestorId != id)
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
                return Unauthorized(new { Error = "Invalid email or password. Looks like no Account has been created" });
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

            var jwtKey = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(jwtKey) || string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
            {
                return StatusCode(500, new { Error = "JWT configuration is incomplete." });
            }

            // --- 💡 AUTH OVERLAP FIXES ---

            // A. Trigger Identity Cookie creation for Session testing workflows
            await _signInManager.SignInAsync(user, isPersistent: true);

            // B. Populate Session State Cache string variable (Survives hot reload reboots)
            HttpContext.Session.SetString("UserId", user.Id.ToString());

            // C. Build out Standard structural claims array for JWT Token Generation
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
                authClaims.Add(new Claim("role", role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                expires: DateTime.UtcNow.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                User = new { user.Id, user.Email, user.FirstName, user.Surname, Role = roles.FirstOrDefault() }
            });
        }
    }
}
