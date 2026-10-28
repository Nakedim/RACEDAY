using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
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
    public class CategoriesController : ControllerBase
    {
        private readonly RacedayDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CategoriesController(RacedayDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            
        }



        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult> GetCategory(int id)
        {

            var Category = await _context.Categories.FindAsync(id);

            if (Category == null)
            {
                return NotFound();

            }
            return Ok(Category);


        }
        //both view gategory
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Categories>>> GetAllCategories()
        {
            var Categories = await _context.Categories.ToListAsync();
            if(Categories == null)
            {
                return NotFound();
            }
            return Ok(Categories);
        }

        [HttpPost]
        [Authorize] 
        public async Task<IActionResult> CreateCategory([FromBody] CategoriesDTO categoryDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Robust claim extraction to avoid standard remapping issues
            var authenticatedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(authenticatedUserId)) return Unauthorized();

            var caller = await _userManager.FindByIdAsync(authenticatedUserId);
            if (caller == null || caller.profileRole != ProfileRole.Organiser)
            {
                return Forbid();
            }
            var Newcategory = new Categories()
            {
                CategoryName = categoryDTO.CategoryName
            };
            _context.Categories.Add(Newcategory);
            await _context.SaveChangesAsync();
        
            return CreatedAtAction(nameof(GetCategory), new { id = Newcategory.CategoryID }, Newcategory);

        }

  


    }
}
