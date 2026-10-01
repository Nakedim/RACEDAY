
﻿
using Microsoft.AspNetCore.Identity; 

﻿using Microsoft.AspNetCore.Identity; // Standard ASP.NET Core Identity

using Microsoft.AspNetCore.Mvc;
using RACEDAY.DTOs;
using RACEDAY.Models;
using System.Threading.Tasks;

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
            if (model.Password != model.ConfirmPassword)
            {
                return BadRequest(new { Message = "Passwords do not match." });
            }

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { Message = "User registered successfully!" });
            }

            return BadRequest(result.Errors);
        }
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _signInManager.PasswordSignInAsync(

                userName: model.Email,
                password: model.PasswordHashed,
                isPersistent: model.RememberMe,//system to remeber the user or user can choose no
                lockoutOnFailure: false

                );

            if (result.Succeeded)
            {
                return Ok(new { Message = "Login successful!" });
            }

            return Unauthorized(new { Message = "Invalid email or password combination." });
        }
    }
}