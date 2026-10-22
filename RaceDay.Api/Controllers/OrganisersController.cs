
﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts;
using System.Security.Claims;


namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrganisersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RacedayDbContext _context;

        public OrganisersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            
        }


        // POST: api/Organisers
        [HttpPost]
    
        public async Task<ActionResult<ApplicationUser>> CreateOrganiser(string id )
        {
            var requestorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var requestor = await _userManager.FindByIdAsync(requestorId!);

            if (requestor == null || requestor.profileRole != ProfileRole.Organiser)
            {
                return Forbid();
            }

            // 2. Find the target user profile
            var participant = await _userManager.FindByIdAsync(id);
            if (participant == null || participant.profileRole != ProfileRole.Participant)
            {
                return NotFound("Participant not found.");
            }

            return Ok(participant);
        }










        // View my own profile details (Accessible by logged-in Participants)
        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<ApplicationUser>> ViewOwnParticipantInfo()
        {
            // 1. Get the current logged-in user's ID securely from the session token
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(authenticatedUserId))
            {
                return Unauthorized();
            }

            // 2. Fetch their clean account object directly
            var userProfile = await _userManager.FindByIdAsync(authenticatedUserId);
            if (userProfile == null)
            {
                return NotFound();
            }

            return Ok(userProfile);
        }
        // View an organiser's profile details by their alphanumeric Identity ID string
        [HttpGet("{id}")]
        [Authorize] // Requires being logged in
        public async Task<ActionResult> GetOrganiserbyID(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            // Verify the user exists AND their role is actually an Organiser
            if (user == null || user.profileRole != ProfileRole.Organiser)
            {
                return NotFound("Organiser profile not found.");
            }

            return Ok(new
            {
                user.Id,
                user.FirstName,
                user.Surname,
                user.Email
            });
        }

        // Updates the logged-in organiser's personal profile information
        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateOrgProfile([FromBody] OrganiserProfileDTO organiserProfileDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 1. Get the current logged-in user's ID from their token securely
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(authenticatedUserId))
            {
                return Unauthorized();
            }

            // 2. Fetch the profile record
            var user = await _userManager.FindByIdAsync(authenticatedUserId);
            if (user == null || user.profileRole != ProfileRole.Organiser)
            {
                return Forbid("Only registered organisers can modify this profile.");
            }

            // 3. Apply updates to the Identity model properties
            user.FirstName = organiserProfileDTO.FirstName;
            user.Surname = organiserProfileDTO.Surname;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { Message = "Profile updated successfully." });
        }

     
        // GET: api/Organisers
        [HttpGet]
        [Authorize(Roles = "Organiser,Participant")]
        public async Task<ActionResult<IEnumerable<OrganiserDTO>>> GetOrganisers()
        {
            return Ok();
        }
            
        
           
            

       
    }
}