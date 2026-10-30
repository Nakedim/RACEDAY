
﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using RaceDay.Api.Data;
using RaceDay.Api.Models;

using RaceDay.Contracts.ProfileDTO;
using System.Collections.Immutable;
using System.Security.Claims;
using OrganiserProfileDTO = RaceDay.Contracts.ProfileDTO.OrganiserProfileDTO;


namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrganisersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RacedayDbContext _context;

        public OrganisersController(UserManager<ApplicationUser> userManager,
            RacedayDbContext context)
        {
            _userManager = userManager;
            _context = context;
            
        }


        // POST: api/Organisers
        //[HttpPost("create-organiser/{id}")]
        //[Authorize]
        //public async Task<ActionResult<ApplicationUser>> CreateOrganiser(string id )
        //{
        //    var requestorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var requestor = await _userManager.FindByIdAsync(requestorId!);

        //    if (requestor == null || requestor.profileRole != ProfileRole.Organiser)
        //    {
        //        return Forbid();
        //    }

        //    // 2. Find the target user profile
        //    var participant = await _userManager.FindByIdAsync(id);
        //    if (participant == null || participant.profileRole != ProfileRole.Participant)
        //    {
        //        return NotFound("Organiser not found.");
        //    }
        //    participant.profileRole = ProfileRole.Organiser;
        //    await _userManager.UpdateAsync(participant);
        //    return Ok(participant);
        //}


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
        [HttpPut("Ownprofile-Update")]
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
            user.Surname = organiserProfileDTO.LastName;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { Message = "Profile updated successfully." });
        }

        //view
        [HttpGet("AllOrganisers")]
        [Authorize]
        public async Task<ActionResult<List<OrganiserDetailsDTO>>> GetAllOrganisers()
        {
            var organisers = await (
                from organiser in _context.Organisers
                join user in _context.Users
                    on organiser.ApplicationUserId equals user.Id
                select new OrganiserDetailsDTO
                {
                    Id = organiser.ApplicationUserId,
                    FirstName = user.FirstName,
                    Surname = user.Surname
                }
            ).ToListAsync();

            return Ok(organisers);
        }
        //create
        //[HttpPost]
        //public async Task<IActionResult> createOrganiser([FromBody] ViewOrganiserDetailsDTO OrganiserprofileDTO)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    var requestorId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        //    if (string.IsNullOrEmpty(requestorId)) return Unauthorized();


        //    var requestor = await _userManager.FindByIdAsync(requestorId);
        //    if (requestor == null || requestor.profileRole != ProfileRole.Organiser)
        //    {
        //        return Forbid("Only registered organisers are permitted to create new organiser profiles.");
        //    }
        //    var NewOrganiser = new ApplicationUser
        //    {

        //        UserName = OrganiserprofileDTO.Email, // Identity framework requires a unique username
        //        Email = OrganiserprofileDTO.Email,
        //        FirstName = OrganiserprofileDTO.FirstName,
        //        Surname = OrganiserprofileDTO.Surname,
        //        Location = OrganiserprofileDTO.Location, 
        //        profileRole = ProfileRole.Organiser
        //    };

        //   var result = await _userManager.CreateAsync(NewOrganiser, OrganiserprofileDTO.Password);
        //    if (!result.Succeeded)
        //    {
        //        return BadRequest(result.Errors);
        //    }
        //    var OrganiserEntity = new Organiser
        //    {
        //        ApplicationUserId = NewOrganiser.Id,
        //        FirstName = NewOrganiser.FirstName,
        //        Surname = NewOrganiser.Surname,
        //    };
        //    _context.Organisers.Add(OrganiserEntity);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction(nameof(GetOrganiserbyID),
        //        new { id = NewOrganiser.Id },
        //        OrganiserEntity
        //         );
        //}

    }






}
