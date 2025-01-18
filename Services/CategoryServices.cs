using Ecommerce.Context;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Exceptions;
using Ecommerce.Models.ResponseDto;
using Ecommerce.Models.RequestDto;
using Ecommerce.Services.ServiceInterfaces;

namespace Ecommerce.Services
{
    public class CategoryServices(ApplicationDbContext _context) : ICategoryService
    {
        public async Task<IEnumerable<CategoryResponseDto>> GetCategoriesAsync()
        {
            List<CategoryResponseDto> categories= await _context.Categorys.Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                CategoryName = c.CategoryName,
                Description = c.Description,
            }).ToListAsync();

            if (categories.Count == 0) throw new NotFoundException("Categories not found");

            return categories;
        }

        public async Task<CategoryResponseDto> GetCategoryByIdAsync(int id)
        {
            if (id <= 0) throw new BadRequestException("Id must be over a zero");

            CategoryResponseDto? category = await _context.Categorys.Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                CategoryName = c.CategoryName,
                Description = c.Description,
            }).FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                throw new NotFoundException($"Category {id} id not found");

            return category;
        }

        public async Task<Category> AddCategoryAsync(CategoryRequestDto categoryDto)
        {
            bool categoryNameExist = await _context.Categorys.AnyAsync(c => c.CategoryName == categoryDto.CategoryName);
            if (categoryNameExist) throw new BadRequestException("Category name already exist");

            Category category = new Category
            {
                CategoryName = categoryDto.CategoryName,
                Description = categoryDto.Description,
            };

            _context.Categorys.Add(category);
            await _context.SaveChangesAsync();

            return category;
        }

        public async Task DeleteCategoryAsync(int id)
        {
            if (id <= 0) throw new BadRequestException("Id must be over a zero");

            Category? category = await _context.Categorys.FindAsync(id);
            if (category == null) throw new NotFoundException($"Category with {id} id not found");

            _context.Categorys.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(int id, CategoryRequestDto categoryDto)
        {
            Category? category = await _context.Categorys.FindAsync(id);
            if (category == null) throw new NotFoundException($"Category {id} not found");

            string categoryName = category.CategoryName;
            List<Category> existedCategories = await _context.Categorys.Where(c => c.CategoryName != categoryName).ToListAsync();
            foreach (Category cat in existedCategories)
            {
                if (cat.CategoryName == categoryDto.CategoryName) throw new BadRequestException($"Category with name {categoryDto.CategoryName} already exists"); 
            }

            category.GetCategoryFromDto(category, categoryDto);

            await _context.SaveChangesAsync();
        }
    }
}
