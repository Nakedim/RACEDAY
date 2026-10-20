
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RaceDay.Api.Filters;
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


        public AuthUsersController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration
            
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        // POST: api/AuthUsers/register
        [HttpPost("register")]
        [ValidateModel]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest model)
        {
            if (model == null)
            {
                return BadRequest(new
                {
                    Error = "Invalid registration data."
                });
            }

            // Validate the requested role
            if (!Enum.TryParse<ProfileRole>(
                model.ProfileRole,
                true,
                out var assignedRole))
            {
                return BadRequest(new
                {
                    Error = "Invalid role. Choose Organiser or Participant."
                });
            }

            var roleName = assignedRole.ToString();

            // Ensure the role exists before creating the user
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var createRoleResult =
                    await _roleManager.CreateAsync(
                        new IdentityRole(roleName));

                if (!createRoleResult.Succeeded)
                {
                    return StatusCode(500, createRoleResult.Errors);
                }
            }

            // Create the Identity user
            var newUser = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                Surname = model.Surname
            };

            var createUserResult =
                await _userManager.CreateAsync(
                    newUser,
                    model.Password);

            if (!createUserResult.Succeeded)
            {
                return BadRequest(createUserResult.Errors);
            }

            // Assign Organiser or Participant role
            var roleResult = await _userManager.AddToRoleAsync(
                newUser,
                roleName);

            if (!roleResult.Succeeded)
            {
                // Avoid leaving an unassigned user behind
                await _userManager.DeleteAsync(newUser);

                return StatusCode(500, roleResult.Errors);
            }

            return Ok(new
            {
                Message = "Registration successful.",
                UserId = newUser.Id,
                Email = newUser.Email,
                Role = roleName
            });
        }

        // POST: api/AuthUsers/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest(new { Error = "Invalid login payload." });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new { Error = "Invalid email or password." });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return Unauthorized(new { Error = "Invalid email or password." });
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Count == 0)
            {
                return StatusCode(403, new { Error = "This account has no assigned role." });
            }

            // --- ADDED FROM HERE TO GENERATE THE ACTUAL JWT TOKEN ---
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email ?? ""),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Vital step: Add roles into Claims so [Authorize(Roles = "...")] works
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.UtcNow.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                Message = "Login successful.",
                Token = new JwtSecurityTokenHandler().WriteToken(token), // Client needs this!
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
    }
}
