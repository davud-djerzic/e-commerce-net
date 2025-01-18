using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Exceptions;
using Ecommerce.Models.ResponseDto;
using Ecommerce.Models.RequestDto;
using Ecommerce.Services.ServiceInterfaces;
using Microsoft.AspNetCore.Cors;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[EnableCors("AllowAll")]
    public class CategoryController(ICategoryService _categoryService) : ControllerBase
    {
        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetCategories()
        {
            try
            {
                IEnumerable<CategoryResponseDto> categories = await _categoryService.GetCategoriesAsync();
                return Ok(categories);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new {ex = ex.Message});
            }
        }

        [HttpGet("getById"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryResponseDto>> GetCategoryById(int id)
        {
            try
            {
                CategoryResponseDto category = await _categoryService.GetCategoryByIdAsync(id);
                return Ok(category);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex = ex.Message });
            }
            catch(BadRequestException ex)
            {
                return BadRequest(new { ex = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex = ex.Message });
            }
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult<Category>> AddCategory(CategoryRequestDto categoryDto)
        {
            try
            {
                Category category = await _categoryService.AddCategoryAsync(categoryDto);
                return Ok(category);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { ex = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex = ex.Message });
            }
        }

        [HttpDelete, Authorize(Roles = "Admin")]
        public async Task <ActionResult<Category>> DeleteCategory(int id)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { ex = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex = ex.Message });
            }
        }

        [HttpPut("{id}"), Authorize, Authorize(Roles = "Admin")]
        public async Task<ActionResult<Category>> UpdateCategory(int id, CategoryRequestDto categoryDto)
        {
            try
            {
                await _categoryService.UpdateCategoryAsync(id, categoryDto);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { ex = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex = ex.Message });
            }
        }
    }
}
