using E_commerce_API.Context;
using E_commerce_API.Models;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet, Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<Category>>> getCategories()
        {
            var category = await _context.Categorys.ToListAsync();
            if (!category.Any())
            {
                return NotFound("Category not found");
            }

            return Ok(category);
        }

        [HttpGet("{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<Category>> getCategoryById(int id)
        {
            var category = await _context.Categorys.FindAsync(id);
            if (category == null)
            {
                return NotFound("Category not found");
            }

            return Ok(category);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult<Category>> addCategory(CategoryDTO categoryDTO)
        {
             var category = new Category { 
                    CategoryName = categoryDTO.CategoryName,
                    Description = categoryDTO.Description,
                };

                _context.Categorys.Add(category);
                await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(addCategory), category);
        }

        [HttpDelete, Authorize(Roles = "Admin")]
        public async Task <ActionResult<Category>> deleteCategory(int id)
        {
            var category = await _context.Categorys.FindAsync(id);
            if (category == null)
                return NotFound("Category not found");

            _context.Categorys.Remove(category);
            await _context.SaveChangesAsync();

            return Ok("Category deleted");
        }

        [HttpPut("{id}"), Authorize, Authorize(Roles = "Admin")]
        public async Task<ActionResult<Category>> updateCategory(int id, CategoryDTO categoryDTO)
        {
            /*if (id != categoryDTO.Id)
                return NotFound("Category not found");

            var categoryExists = await _context.Categorys.AnyAsync(c => c.Id == categoryDTO.Id);
            if (!categoryExists)
                return BadRequest("Category not exist");*/

            var categoryObject = await _context.Categorys.FindAsync(id);
            if (categoryObject == null)
                return NotFound("Category not exist");

            categoryObject.CategoryName = categoryDTO.CategoryName;
            categoryObject.Description = categoryDTO.Description;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
