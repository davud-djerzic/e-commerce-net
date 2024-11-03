using Ecommerce.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.OpenApi.Any;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
using Ecommerce.Migrations;
using Ecommerce.Services;
using Ecommerce.Exceptions;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductServices _productServices;

        public ProductController(IProductServices productServices)
        {
            _productServices = productServices;
        }

        [HttpGet, Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            try
            {
                var products = await _productServices.GetProductsAsync();
                return Ok(products);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpGet("getById"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<Product>> GetProductById([FromQuery] int id)
        {
            try
            {
                var product = await _productServices.GetProductByIdAsync(id);
                return Ok(product);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("getByProductName"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<Product>> GetProductByName([FromQuery] string productName)
        {
            try
            {
                var product = await _productServices.GetProductByNameAsync(productName);
                return Ok(product);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult<Product>> AddProduct(ProductDTO productDto)
        {
            try
            {
                var createdProduct = await _productServices.AddProductAsync(productDto);
                return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
            }
            catch (NotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpDelete, Authorize(Roles = "Admin")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _productServices.DeleteProductAsync(id);
            if (!product) return NotFound("Product not found");

            return Ok("Product deleted");

        }

        [HttpPut("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(int id, ProductDTO productDto)
        {
            var product = await _productServices.UpdateProductAsync(id, productDto);
            if (!product) return NotFound("Product not found");

            return NoContent();
        }

        [HttpGet("getByPrice"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByPrice([FromQuery] decimal bottomPrice,[FromQuery] decimal upperPrice)
        {
            try
            {
                var products = await _productServices.GetProductsByPriceAsync(bottomPrice, upperPrice);
                return Ok(products);
            } catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("getByCategoryId"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory([FromQuery] int categoryId)
        {
            try
            {
                var products = await _productServices.GetProductsByCategoryIdAsync(categoryId);
                return Ok(products);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("availability"), Authorize(Roles = "Admin, User")]
        public async Task <ActionResult<IEnumerable<Product>>> GetProductsByAvailability()
        {
            try
            {
                var products = await _productServices.GetProductsByAvailability();
                return Ok(products);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
