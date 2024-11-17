using Ecommerce.Models;
using Ecommerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Exceptions;
using Ecommerce.Models.ResponseDto;
using Ecommerce.Models.RequestDto;

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
        public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetCategories()
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
        public async Task<ActionResult<CategoryResponseDto>> GetCategoryById([FromQuery] int id)
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
        public async Task<ActionResult<Category>> AddCategory(CategoryRequestDto categoryDto)
        {
            var category = await _categoryService.AddCategoryAsync(categoryDto);
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
        public async Task<ActionResult<Category>> UpdateCategory(int id, CategoryRequestDto categoryDto)
        {
            var result = await _categoryService.UpdateCategoryAsync(id, categoryDto);
            if (!result) return NotFound("Category not found");

            return NoContent();
        }
    }
}
