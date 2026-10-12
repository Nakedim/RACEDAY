

using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;
using RaceDay.Api.Controllers;
using RaceDay.Api.Data;
using RaceDay.Api.Filters;
using RaceDay.Api.Models;
using RaceDay.Contracts;
using System.Security.Claims;

namespace RaceDay.Api.Controllers
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
            var results = await _userManager.CreateAsync(newUser, model.Password); // Note: Always pass the password!

            if (!results.Succeeded)
            {
                return BadRequest(results.Errors);
            }
            var roleResult = await _userManager.AddToRoleAsync(newUser, assignedRole.ToString());

            if(!roleResult.Succeeded)
                    return BadRequest(roleResult.Errors);
            {

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
                  
                };
                return Ok(new
                {
                    Message = "Login successful!",
                  
                });
            }
            return Unauthorized(new { Message = "invalid email or password" });
          
        }

    }
}