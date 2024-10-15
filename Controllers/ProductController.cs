using E_commerce_API.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using E_commerce_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.OpenApi.Any;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;

namespace E_commerce_API.Controllers
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
        public async Task<ActionResult<IEnumerable<Product>>> getProducts()
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
                    p.CategoryId, // Vraćamo samo ID kategorije
                    p.Category.CategoryName,
                })
                .ToListAsync();

            return Ok(products);
        }


        [HttpGet("{id}"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<Product>> getProductById(int id)
        {
            // Prvo tražimo proizvod po ID-u
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
                p.CategoryId, // Vraćamo samo ID kategorije
                p.Category.CategoryName,
            }).FirstOrDefaultAsync(p => p.Id == id);

            // Ako proizvod nije pronađen, vraćamo NotFound status
            if (product == null)
            {
                return NotFound("Product not found");
            }

            // Ako je pronađen, vraćamo proizvod
            return Ok(product);
        }

        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult<Product>> addProduct(ProductDTO productDto)
        {
            // Proverite da li kategorija sa datim ID-om postoji
            var categoryExists = await _context.Categorys.AnyAsync(c => c.Id == productDto.CategoryId);
            if (!categoryExists)
            {
                return BadRequest("Category does not exist.");
            }

            // Mapirajte DTO u model proizvoda
            var product = new Product
            {
                ProductCode = productDto.ProductCode,
                ProductName = productDto.ProductName,
                Price = productDto.Price,
                StockQuantity = productDto.StockQuantity,
                Weight = productDto.Weight,
                Manufacturer = productDto.Manufacturer,
                Description = productDto.Description,
                CategoryId = productDto.CategoryId // Povezivanje sa kategorijom
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(getProductById), new { id = product.Id }, productDto);
        }

        [HttpDelete, Authorize(Roles = "Admin")]
        public async Task<ActionResult<Product>> deleteProduct(int id)
        {
            var dbProduct = await _context.Products.FindAsync(id);
            if (dbProduct == null)
                return NotFound("Product not found");

            _context.Products.Remove(dbProduct);
            await _context.SaveChangesAsync();

            return Ok("Product deleted");
                   
        }

        [HttpPut("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> updateProduct(int id, ProductDTO productDto)
        {
            // Provjerite da li kategorija sa datim ID-om postoji
            var categoryExists = await _context.Categorys.AnyAsync(c => c.Id == productDto.CategoryId);
            if (!categoryExists)
            {
                return BadRequest("Category does not exist.");
            }
            // Pronađite proizvod koji treba ažurirati
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound("Product not found"); // Ako proizvod ne postoji
            }

            // Ažurirajte svojstva proizvoda
            product.ProductCode = productDto.ProductCode;
            product.ProductName = productDto.ProductName;
            product.Price = productDto.Price;
            product.StockQuantity = productDto.StockQuantity;
            product.Weight = productDto.Weight;
            product.Manufacturer = productDto.Manufacturer;
            product.Description = productDto.Description;
            product.CategoryId = productDto.CategoryId; // Povezivanje sa kategorijom
            
            await _context.SaveChangesAsync();

            return NoContent(); // Vratite 204 No Content na uspješno ažuriranje
        }

        [HttpGet("{bottomPrice}, {upperPrice}"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<Product>>> getProductsByPrice(decimal bottomPrice, decimal upperPrice)
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

        [HttpGet("/category/{categoryId}"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<Product>>> getProductsByCategory(int categoryId)
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
                p.CategoryId, // Vraćamo samo ID kategorije
                p.Category.CategoryName,
            }).Where(p => p.CategoryId == categoryId).ToListAsync();

            if (!products.Any())
            {
                return NotFound("Products not found");
            }

            return Ok(products);
        }

        [HttpGet("/availability"), Authorize(Roles = "Admin, User")]
        public async Task <ActionResult<IEnumerable<Product>>> getProductsByAvailability()
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
                p.CategoryId, // Vraćamo samo ID kategorije
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
