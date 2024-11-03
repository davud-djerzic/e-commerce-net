using Ecommerce.Context;
using Ecommerce.Models;
using Ecommerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Exceptions;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet, Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            try
            {
                var categories = await _categoryService.GetCategoriesAsync();
                return Ok(categories);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("getById"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<Category>> GetCategoryById([FromQuery] int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                return Ok(category);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult<Category>> AddCategory(CategoryDTO categoryDTO)
        {
            var category = await _categoryService.AddCategoryAsync(categoryDTO);
            return CreatedAtAction(nameof(AddCategory), new { id = category.Id }, category);
        }

        [HttpDelete, Authorize(Roles = "Admin")]
        public async Task <ActionResult<Category>> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result) return NotFound("Category not found");

            return Ok("Category deleted");
        }

        [HttpPut("{id}"), Authorize, Authorize(Roles = "Admin")]
        public async Task<ActionResult<Category>> UpdateCategory(int id, CategoryDTO categoryDTO)
        {
            var result = await _categoryService.UpdateCategoryAsync(id, categoryDTO);
            if (!result) return NotFound("Category not found");

            return NoContent();
        }
    }
}
