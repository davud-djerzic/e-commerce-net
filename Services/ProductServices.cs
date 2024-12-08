using Ecommerce.Context;
using Ecommerce.Models;
using Ecommerce.Models.RequestDto;
using Ecommerce.Models.ResponseDto;
using Ecommerce.Exceptions;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Services.ServiceInterfaces;

namespace Ecommerce.Services
{
    public class ProductServices(ApplicationDbContext _context) : IProductServices
    {
        public async Task<IEnumerable<ProductResponseDto>> GetProductsAsync()
        {
            List<ProductResponseDto> products = await _context.Products.Include(c => c.Category)
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

            if (products.Count == 0) throw new NotFoundException("Products not found");

            return products;
        }

        public async Task<ProductResponseDto> GetProductByIdAsync(int id)
        {
            if (id <= 0) throw new BadRequestException("Id must be over a zero");

            ProductResponseDto? product = await _context.Products.Include(c => c.Category)
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

            if (product == null) throw new NotFoundException($"Product {id} not found");

            return product;
        }

        public async Task<ProductResponseDto> GetProductByNameAsync(string productName)
        {
            ProductResponseDto? product = await _context.Products.Include(c => c.Category)
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

            if (product == null) throw new NotFoundException($"Product {productName} not found");

            return product;
        }

        public async Task<Product> AddProductAsync(ProductRequestDto productDto)
        {
            bool categoryExists = await _context.Categorys.AnyAsync(c => c.Id == productDto.CategoryId); // check if typed category exist
            if (!categoryExists) throw new NotFoundException("Category not found");

            bool productExists = await _context.Products.AnyAsync(p => p.ProductCode == productDto.ProductCode || p.ProductName == productDto.ProductName);
            if (productExists) throw new BadRequestException("Product already exists");

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

        public async Task DeleteProductAsync(int id)
        {
            if (id <= 0) throw new BadRequestException("Id must be over a zero");

            Product? product = await _context.Products.FindAsync(id);
            if (product == null) throw new NotFoundException($"Product {id} not found");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(int id, ProductRequestDto productDto)
        {
            if (id <= 0) throw new BadRequestException("Id must be over a zero");

            bool categoryExists = await _context.Categorys.AnyAsync(c => c.Id == productDto.CategoryId);
            if (!categoryExists) throw new NotFoundException($"Category {productDto.CategoryId} not found");

            Product? product = await _context.Products.FindAsync(id);
            if (product == null) throw new NotFoundException($"Product {id} not found");

            bool productExists = await _context.Products.AnyAsync(p => p.ProductCode == productDto.ProductCode || p.ProductName == productDto.ProductName);
            if (productExists) throw new BadRequestException("Product already exists");

            product.GetProductFromDto(product, productDto);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductResponseDto>> GetProductsByPriceAsync(decimal bottomPrice, decimal topPrice)
        {
            if (bottomPrice <= 0 || topPrice <= 0) throw new BadRequestException("Price must be positive");

            List<ProductResponseDto> products = await _context.Products.Include(c => c.Category)
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

            if (products.Count == 0) throw new NotFoundException("Product not found");

            return products;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryIdAsync(int categoryId)
        {
            if (categoryId <= 0) throw new BadRequestException("Id must be over a zero");

            bool categoryExist = await _context.Categorys.AnyAsync(p => p.Id == categoryId);
            if (!categoryExist) throw new NotFoundException($"Category {categoryId} not found");

            List<ProductResponseDto> products = await _context.Products.Include(c => c.Category)
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

            if (products.Count == 0) throw new NotFoundException("Products not found");

            return products;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetProductsByAvailability()
        {
            List<ProductResponseDto> products = await _context.Products.Include(c => c.Category)
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

            if (products.Count == 0) throw new NotFoundException("Products not found");

            return products;
        }
    }
}
