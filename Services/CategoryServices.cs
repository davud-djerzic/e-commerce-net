using Ecommerce.Context;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Exceptions;

namespace Ecommerce.Services
{
    public class CategoryServices : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            var categories = await _context.Categorys.ToListAsync();
            if (!categories.Any())
                throw new NotFoundException("Category not found");

            return categories;
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categorys.FindAsync(id);
            if (category == null)
                throw new NotFoundException("Category not found");
              
            return category;
        }

        public async Task<Category> AddCategoryAsync(CategoryDTO categoryDTO)
        {
            var category = new Category
            {
                CategoryName = categoryDTO.CategoryName,
                Description = categoryDTO.Description,
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

        public async Task <bool> UpdateCategoryAsync(int id, CategoryDTO categoryDTO)
        {
            var category = await _context.Categorys.FindAsync(id);
            if (category == null) return false;

            category.CategoryName = categoryDTO.CategoryName;
            category.Description = categoryDTO.Description;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
