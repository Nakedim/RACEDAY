
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

        // GET: api/Categories
        [HttpGet]
        [Authorize(Roles = "Organiser,Participant")]
        public async Task<ActionResult<IEnumerable<CategoriesDTO>>>
            GetAllCategories()
        {
            return await _context.Categories
                .Select(c => new CategoriesDTO
                {
                    CategoryID = c.CategoryID,
                    CategoryName = c.CategoryName
                })
                .ToListAsync();
        }

        // GET: api/Categories/5
        [HttpGet("{id}", Name = "GetCategoryById")]
        public async Task<ActionResult<CategoriesDTO>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            return new CategoriesDTO { CategoryID = category.CategoryID, CategoryName = category.CategoryName };
        }

        // POST: api/Categories
        [HttpPost]
        //[Authorize(Roles = "Organiser")]
        public async Task<ActionResult<CategoriesDTO>>
            CreateCategory(CategoriesDTO categoryDto)
        {
            var category = new Categories
            {
                CategoryName = categoryDto.CategoryName
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            categoryDto.CategoryID = category.CategoryID;

            return CreatedAtRoute(
           "GetCategoryById",
           new { id = category.CategoryID },
           categoryDto
       );
        }
    }
}
