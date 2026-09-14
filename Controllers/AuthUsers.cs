using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RACEDAY.DTOs;
using System.Threading.Tasks;

namespace RACEDAY.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthUsers : ControllerBase
    {
    
        private readonly UserManager _userManager;
        private readonly SignInManager _signInManager;

        public AuthUsers(UserManager userManager, SignInManager signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // POST: api/AuthUsers
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
           
            if (model.Password != model.ConfirmPassword)
            {
                return BadRequest(new { Message = "Passwords do not match." });
            }

            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

       
            if (result.Succeeded)
            {
                return Ok(new { Message = "User registered successfully!" });
            }

            return BadRequest(result.Errors);
        }


    }
}
