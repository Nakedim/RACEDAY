using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly RacedayDbContext _context;

        public CategoriesController(RacedayDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<CategoriesDTO>>> GetCategories(int id)
        {
            return await _context.Categories
                .Select(c => new CategoriesDTO
                {
                    CategoryID = c.CategoryID,
                    CategoryName = c.CategoryName
                })
                .ToListAsync();
        }

        //both view category
        [HttpGet("all")]
        [Authorize(Roles = "Organiser, Participant")]
        public async Task<ActionResult<IEnumerable<RaceEventDTO>>> GetAllCategories()
        {
            return await _context.RaceEvents
                .Select(re => new RaceEventDTO
                {
                    EventID = re.EventID,
                    Description = re.Description,
                    Title = re.Title,
                    location = re.location,
                    OrganiserID = re.OrganiserID,
                    CategoryID = re.CategoryID,
                    EventType = re.EventType
                })
                .ToListAsync();
        }

        //organisers create category
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateCategory(CategoriesDTO categoryDto)
        {
            var category = new Categories
            {
                CategoryName = categoryDto.CategoryName
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            categoryDto.CategoryID = category.CategoryID;

            return CreatedAtAction(
                nameof(GetAllCategories),
                new { id = category.CategoryID },
                categoryDto
            );
        }

        //organisers update category
        [HttpPut("{CategoryID}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateCategoryAsync(int CategoryID, CategoriesDTO categoryDto)
        {
            if (CategoryID != categoryDto.CategoryID)
            {
                return BadRequest("Category not found");
            }

            var existingCategory = await _context.Categories.FindAsync(CategoryID);
            if (existingCategory == null)
            {
                return NotFound("Category not found in database");
            }

            existingCategory.CategoryName = categoryDto.CategoryName;
            await _context.SaveChangesAsync();

            return Ok("Category Update successfull");
        }

        //organisers can delete category
        [HttpDelete("{CategoryID}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteCategory(int CategoryID)
        {
            var category = await _context.Categories.FindAsync(CategoryID);
            if (category == null)
            {
                return BadRequest("Category not found");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok("Category deleted successfully");
        }
    }
}
