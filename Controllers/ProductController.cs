using Ecommerce.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.OpenApi.Any;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet, Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _context.Products
                .Select(p => new
                {
                    p.Id,
                    p.ProductCode,
                    p.ProductName,
                    p.Price,
                    p.StockQuantity,
                    p.Weight,
                    p.Manufacturer,
                    p.Description,
                    p.CategoryId, 
                    p.Category.CategoryName, // get category name from category table
                })
                .ToListAsync();

            if (!products.Any())
                return NotFound("Products not found");

            return Ok(products);
        }


        [HttpGet("{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            var product = await _context.Products.Select(p => new
            {
                p.Id,
                p.ProductCode,
                p.ProductName,
                p.Price,
                p.StockQuantity,
                p.Weight,
                p.Manufacturer,
                p.Description,
                p.CategoryId, 
                p.Category.CategoryName,
            }).FirstOrDefaultAsync(p => p.Id == id); // get product by id

            if (product == null)
            {
                return NotFound("Product not found");
            }

            return Ok(product);
        }

        [HttpGet("/productName{productName}"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<Product>> GetProductById(string productName)
        {
            var product = await _context.Products.Select(p => new
            {
                p.Id,
                p.ProductCode,
                p.ProductName,
                p.Price,
                p.StockQuantity,
                p.Weight,
                p.Manufacturer,
                p.Description,
                p.CategoryId,
                p.Category.CategoryName,
            }).FirstOrDefaultAsync(p => p.ProductName == productName); // get product by name

            if (product == null)
            {
                return NotFound("Product not found");
            }

            return Ok(product);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult<Product>> AddProduct(ProductDTO productDto)
        {
            var categoryExists = await _context.Categorys.AnyAsync(c => c.Id == productDto.CategoryId); // check if typed category exist
            if (!categoryExists)
            {
                return BadRequest("Category does not exist.");
            }

            // Map DTO model to Product model
            var product = new Product
            {
                ProductCode = productDto.ProductCode,
                ProductName = productDto.ProductName,
                Price = productDto.Price,
                StockQuantity = productDto.StockQuantity,
                Weight = productDto.Weight,
                Manufacturer = productDto.Manufacturer,
                Description = productDto.Description,
                CategoryId = productDto.CategoryId 
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, productDto);
        }

        [HttpDelete, Authorize(Roles = "Admin")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var dbProduct = await _context.Products.FindAsync(id);
            if (dbProduct == null)
                return NotFound("Product not found");

            _context.Products.Remove(dbProduct);
            await _context.SaveChangesAsync();

            return Ok("Product deleted"); 
        }

        [HttpPut("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(int id, ProductDTO productDto)
        {
            var categoryExists = await _context.Categorys.AnyAsync(c => c.Id == productDto.CategoryId); 
            if (!categoryExists)
            {
                return BadRequest("Category does not exist.");
            }

            var product = await _context.Products.FindAsync(id); // find the product with input id
            if (product == null)
            {
                return NotFound("Product not found"); 
            }

            // Ažurirajte svojstva proizvoda
            product.ProductCode = productDto.ProductCode;
            product.ProductName = productDto.ProductName;
            product.Price = productDto.Price;
            product.StockQuantity = productDto.StockQuantity;
            product.Weight = productDto.Weight;
            product.Manufacturer = productDto.Manufacturer;
            product.Description = productDto.Description;
            product.CategoryId = productDto.CategoryId; 
            
            await _context.SaveChangesAsync();

            return NoContent(); // Vratite 204 No Content na uspješno ažuriranje
        }

        [HttpGet("price/{bottomPrice}, {upperPrice}"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByPrice(decimal bottomPrice, decimal upperPrice)
        {
            if (bottomPrice <= 0 || upperPrice <= 0)
            {
                return BadRequest("Price must be positive");
            }

            var products = await _context.Products.Where(p => p.Price >= bottomPrice && p.Price <= upperPrice).ToListAsync();

            if (!products.Any())
            {
                return NotFound("Products not found");
            }

            return Ok(products);
        }

        [HttpGet("category/{categoryId}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(int categoryId)
        {
            var categoryExists = await _context.Categorys.AnyAsync(c => c.Id == categoryId);
            if (!categoryExists)
            {
                return BadRequest("Category does not exist.");
            }

            var products = await _context.Products.Select(p => new
            {
                p.Id,
                p.ProductCode,
                p.ProductName,
                p.Price,
                p.StockQuantity,
                p.Weight,
                p.Manufacturer,
                p.Description,
                p.CategoryId, 
                p.Category.CategoryName,
            }).Where(p => p.CategoryId == categoryId).ToListAsync();

            if (!products.Any())
            {
                return NotFound("Products not found");
            }

            return Ok(products);
        }

        [HttpGet("availability"), Authorize(Roles = "Admin, User")]
        public async Task <ActionResult<IEnumerable<Product>>> GetProductsByAvailability()
        {
            var products = await _context.Products.Select(p => new
            {
                p.Id,
                p.ProductCode,
                p.ProductName,
                p.Price,
                p.StockQuantity,
                p.Weight,
                p.Manufacturer,
                p.Description,
                p.CategoryId, 
                p.Category.CategoryName,
            }).Where(p => p.StockQuantity > 0).ToListAsync();

            if (!products.Any())
            {
                return NotFound("Product not found");
            }

            return Ok(products);
        }
    }
}
