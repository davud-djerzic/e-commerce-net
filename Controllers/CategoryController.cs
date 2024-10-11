using E_commerce_API.Context;
using E_commerce_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> getCategories()
        {
            var category = await _context.Categorys.ToListAsync();
            if (!category.Any())
            {
                return NotFound("Category not found");
            }

            return Ok(category);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> getCategoryById(int id)
        {
            var category = await _context.Categorys.FindAsync(id);
            if (category == null)
            {
                return NotFound("Category not found");
            }

            return Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<Category>> addCategory(Category category)
        {
            // Provjerite da li kategorija sa datim ID-om postoji
            var categoryExists = await _context.Categorys.AnyAsync(c => c.Id == category.Id);
            if (categoryExists)
            {
                return Conflict("Category Exist");
            } else
            {
                _context.Categorys.Add(category);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(addCategory), category);
        }

        [HttpDelete]
        public async Task <ActionResult<Category>> deleteCategory(int id)
        {
            var category = await _context.Categorys.FindAsync(id);
            if (category == null)
                return NotFound("Category not found");

            _context.Categorys.Remove(category);
            await _context.SaveChangesAsync();

            return Ok("Category deleted");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Category>> updateCategory(int id, Category category)
        {
            if (id != category.Id)
                return NotFound("Category not found");

            var categoryExists = await _context.Categorys.AnyAsync(c => c.Id == category.Id);
            if (!categoryExists)
                return BadRequest("Category not exist");

            var categoryObject = await _context.Categorys.FindAsync(id);
            if (categoryObject == null)
                return NotFound("Category not found");

            categoryObject.CategoryName = category.CategoryName;
            categoryObject.Description = category.Description;

            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
