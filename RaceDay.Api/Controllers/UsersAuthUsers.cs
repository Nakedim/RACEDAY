
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        // POST: api/AuthUsers/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Enum.TryParse<ProfileRole>(
                model.ProfileRole, true, out var assignedRole)
                || !Enum.IsDefined(typeof(ProfileRole), assignedRole))
            {
                return BadRequest(new
                {
                    Error = "Choose Organiser or Participant."
                });
            }

            string roleName = assignedRole.ToString();

            // Ensure the role exists
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await _roleManager.CreateAsync(
                    new IdentityRole(roleName));

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
                profileRole = assignedRole
            };

            var createResult = await _userManager.CreateAsync(
                newUser, model.Password);

            if (!createResult.Succeeded)
            {
                return BadRequest(createResult.Errors);
            }

            var assignRoleResult = await _userManager.AddToRoleAsync(
                newUser, roleName);

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
