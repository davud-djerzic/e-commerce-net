using Ecommerce.Context;
using Ecommerce.Models;
using Ecommerce.Models.RequestDto;
using Ecommerce.Models.ResponseDto;
using Ecommerce.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services
{
    public class ProductServices : IProductServices
    {
        private readonly ApplicationDbContext _context;

        public ProductServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetProductsAsync()
        {
            var products = await _context.Products.Include(c => c.Category)
                .Select(p => new ProductResponseDto
                {
                    Id = p.Id,
                    ProductCode = p.ProductCode,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    Weight = p.Weight,
                    Manufacturer = p.Manufacturer,
                    Description = p.Description,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName
                }).ToListAsync();

            if (!products.Any())
                throw new NotFoundException("Products not found");

            return products;
        }

        public async Task<ProductResponseDto> GetProductByIdAsync(int id)
        {
            var product = await _context.Products.Include(c => c.Category)
                .Select(p => new ProductResponseDto
                {
                    Id = p.Id,
                    ProductCode = p.ProductCode,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    Weight = p.Weight,
                    Manufacturer = p.Manufacturer,
                    Description = p.Description,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName
                }).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                throw new NotFoundException("Product not found");

            return product;
        }

        public async Task<ProductResponseDto> GetProductByNameAsync(string productName)
        {
            var product = await _context.Products.Include(c => c.Category)
                .Select(p => new ProductResponseDto
                {
                    Id = p.Id,
                    ProductCode = p.ProductCode,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    Weight = p.Weight,
                    Manufacturer = p.Manufacturer,
                    Description = p.Description,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName
                }).FirstOrDefaultAsync(p => p.ProductName == productName);

            if (product == null)
                throw new NotFoundException("Product not found");

            return product;
        }

        public async Task<Product> AddProductAsync(ProductRequestDto productDto)
        {
            var categoryExists = await _context.Categorys.AnyAsync(c => c.Id == productDto.CategoryId); // check if typed category exist
            if (!categoryExists) throw new NotFoundException("Category not found");


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

            return product;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return true;
        }
    
        public async Task<int> UpdateProductAsync(int id, ProductRequestDto productDto)
        {
            var categoryExists = await _context.Categorys.AnyAsync(c => c.Id == productDto.CategoryId);
            if (!categoryExists) return -1;

            var product = await _context.Products.FindAsync(id); // find the product with input id
            if (product == null) return 0;


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

            return 1;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetProductsByPriceAsync(decimal bottomPrice, decimal topPrice)
        {
            if (bottomPrice <= 0 || topPrice <= 0) throw new NotFoundException("Price must be positive");

            var products = await _context.Products.Include(c => c.Category)
                .Select(p => new ProductResponseDto
                {
                    Id = p.Id,
                    ProductCode = p.ProductCode,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    Weight = p.Weight,
                    Manufacturer = p.Manufacturer,
                    Description = p.Description,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName
                }).Where(p => p.Price >= bottomPrice && p.Price <= topPrice).ToListAsync();

            if (!products.Any()) throw new NotFoundException("Product not found");

            return products;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryIdAsync(int categoryId)
        {
            var categoryExist = await _context.Products.AnyAsync(p => p.CategoryId == categoryId);
            if (!categoryExist) throw new NotFoundException("Category not found");

            var products = await _context.Products.Include(c => c.Category)
                 .Select(p => new ProductResponseDto
                 {
                     Id = p.Id,
                     ProductCode = p.ProductCode,
                     ProductName = p.ProductName,
                     Price = p.Price,
                     StockQuantity = p.StockQuantity,
                     Weight = p.Weight,
                     Manufacturer = p.Manufacturer,
                     Description = p.Description,
                     CategoryId = p.CategoryId,
                     CategoryName = p.Category.CategoryName
                 }).Where(c => c.CategoryId == categoryId).ToListAsync();

            if (!products.Any()) throw new NotFoundException("Products not found");

            return products;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetProductsByAvailability()
        {
            var products = await _context.Products.Include(c => c.Category)
                .Select(p => new ProductResponseDto
                {
                    Id = p.Id,
                    ProductCode = p.ProductCode,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    Weight = p.Weight,
                    Manufacturer = p.Manufacturer,
                    Description = p.Description,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName
                }).Where(p => p.StockQuantity > 0).ToListAsync();

            if (!products.Any()) throw new NotFoundException("Products not found");

            return products;
        }
    }
}
