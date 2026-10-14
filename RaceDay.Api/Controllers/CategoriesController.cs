using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController: ControllerBase
    {
        private readonly RacedayDbContext _context;

        public CategoriesController(RacedayDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categories>>> GetCategories(int id)
        {
            return await _context.Categories.ToListAsync();
        }
        //both view gategory
        [HttpGet]
        [Authorize(Roles = "Organiser, Participant")]
        public async Task<ActionResult<IEnumerable<RaceEvent>>> GetAllCategories()
        {
            return await _context.RaceEvents.ToListAsync();
        }
        //organisers create category
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateCategory(Categories category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetAllCategories),
                new {id = category.CategoryID},
                category
                
                );

        }

        //organisers update category
        [HttpPut]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateCategoryAsync(int CategoryID,Categories category)
        {
            
            if (CategoryID != category.CategoryID)
            {
                return BadRequest("Category not found");
            }
            var existingCategory = await _context.Categories
                .FindAsync();

            return Ok("Category Update successfull");
        }





        //organisers can delete category
        [HttpDelete]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteCategory(int CategoryID)
        {
            var category = await _context.Categories.FindAsync(CategoryID);
            if(category == null)
            {
                return BadRequest("Category not found");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok("Category deleted successfully");
        }
    }
}
