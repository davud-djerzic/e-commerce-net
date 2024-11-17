using Ecommerce.Context;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Exceptions;
using Ecommerce.Models.ResponseDto;
using Ecommerce.Models.RequestDto;

namespace Ecommerce.Services
{
    public class CategoryServices : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetCategoriesAsync()
        {
            var categories = await _context.Categorys.Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                CategoryName = c.CategoryName,
                Description = c.Description,
            }).ToListAsync();

            if (!categories.Any())
                throw new NotFoundException("Category not found");

            return categories;
        }

        public async Task<CategoryResponseDto> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categorys.Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                CategoryName = c.CategoryName,
                Description = c.Description,
            }).FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                throw new NotFoundException("Category not found");

            return category;
        }

        public async Task<Category> AddCategoryAsync(CategoryRequestDto categoryDto)
        {
            var category = new Category
            {
                CategoryName = categoryDto.CategoryName,
                Description = categoryDto.Description,
            };

            _context.Categorys.Add(category);
            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categorys.FindAsync(id);
            if (category == null) return false;

            _context.Categorys.Remove(category);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task <bool> UpdateCategoryAsync(int id, CategoryRequestDto categoryDto)
        {
            var category = await _context.Categorys.FindAsync(id);
            if (category == null) return false;

            category.CategoryName = categoryDto.CategoryName;
            category.Description = categoryDto.Description;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
