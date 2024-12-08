using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models;
using Ecommerce.Models.RequestDto;
using Ecommerce.Models.ResponseDto;
using Microsoft.AspNetCore.Authorization;
using Ecommerce.Exceptions;
using Ecommerce.Services.ServiceInterfaces;
using Microsoft.AspNetCore.Cors;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class ProductController(IProductServices _productServices) : ControllerBase
    {
        [HttpGet, Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetProducts()
        {
            try
            {
                IEnumerable<ProductResponseDto> products = await _productServices.GetProductsAsync();
                return Ok(products);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex = ex.Message });
            }
        }


        [HttpGet("getById"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductResponseDto>> GetProductById([FromQuery] int id)
        {
            try
            {
                ProductResponseDto product = await _productServices.GetProductByIdAsync(id);
                return Ok(product);
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

        [HttpGet("getByProductName"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<ProductResponseDto>> GetProductByName([FromQuery] string productName)
        {
            try
            {
                ProductResponseDto product = await _productServices.GetProductByNameAsync(productName);
                return Ok(product);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex = ex.Message });
            }
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult<Product>> AddProduct(ProductRequestDto productDto)
        {
            try
            {
                Product createdProduct = await _productServices.AddProductAsync(productDto);
                return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
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

        [HttpDelete, Authorize(Roles = "Admin")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            try
            {
                await _productServices.DeleteProductAsync(id);
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

        [HttpPut("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(int id, ProductRequestDto productDto)
        {
            try
            {
                await _productServices.UpdateProductAsync(id, productDto);
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

        [HttpGet("getByPrice"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetProductsByPrice([FromQuery] decimal bottomPrice,[FromQuery] decimal upperPrice)
        {
            try
            {
                IEnumerable<ProductResponseDto> products = await _productServices.GetProductsByPriceAsync(bottomPrice, upperPrice);
                return Ok(products);
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

        [HttpGet("getByCategoryId"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetProductsByCategory([FromQuery] int categoryId)
        {
            try
            {
                IEnumerable<ProductResponseDto> products = await _productServices.GetProductsByCategoryIdAsync(categoryId);
                return Ok(products);
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

        [HttpGet("availability"), Authorize(Roles = "Admin, User")]
        public async Task <ActionResult<IEnumerable<ProductResponseDto>>> GetProductsByAvailability()
        {
            try
            {
                IEnumerable<ProductResponseDto> products = await _productServices.GetProductsByAvailability();
                return Ok(products);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex = ex.Message });
            }
        }
    }
}
