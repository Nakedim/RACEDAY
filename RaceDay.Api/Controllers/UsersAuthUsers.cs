

using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;
using RaceDay.Api.Models;
using RaceDay.Contracts;

using System.Security.Claims;

namespace RACEDAY.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthUsers : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthUsers(
           UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager,
           RoleManager<IdentityRole> roleManager
         )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


     


        // POST: api/AuthUsers/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            var newUser = new ApplicationUser
            {
                UserName = model.Email,  //Identity defaults to matching UserName with Email
              
                Email = model.Email,
                FirstName = model.FirstName,
                Surname = model.Surname,
                profileRole = model.ProfileRole.ToLower() == "participant" ? ProfileRole.Participant : ProfileRole.Participant
                
            };

            var results = await _userManager.CreateAsync(newUser);

            if (!results.Succeeded)
            {
                return BadRequest(results.Errors);
            }
            return Ok(new { Message = "Registration successfull" });
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
              user: user,
                password: model.PasswordHashed, // Note: Ensure this is the plain-text password from the user input
                 isPersistent: model.RememberMe,
                 lockoutOnFailure: false 
  );

            if (result.Succeeded)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.Email ?? string.Empty),
                    new Claim(ClaimTypes.Role, user.profileRole.ToString())
                };
                return Ok(new
                {
                    Message = "Login successful!",
                    Role = user.profileRole.ToString()
                });
            }
            return Unauthorized(new { Message = "invalid email or password" });
          
        }

    }
}