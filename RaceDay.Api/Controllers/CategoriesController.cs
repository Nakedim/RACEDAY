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
    public async Task<ActionResult> GetCategory(int id) 
        {
           
            var Category = await _context.Categories.FindAsync(id);

            if(Category == null)
            { 
                return NotFound();

            }
            return Ok(Category);
           
            
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
        //[Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoriesDTO categoryDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var Newcategory = new Categories()
            {
               
                CategoryName = categoryDTO.CategoryName

            };
            _context.Categories.Add(Newcategory);
            await _context.SaveChangesAsync();


            return CreatedAtAction(nameof(GetAllCategories), 
                new { id = Newcategory.CategoryID },Newcategory
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
