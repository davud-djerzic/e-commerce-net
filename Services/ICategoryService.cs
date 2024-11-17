using Ecommerce.Models;
using Ecommerce.Models.ResponseDto;
using Ecommerce.Models.RequestDto;

namespace Ecommerce.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetCategoriesAsync();
        Task<CategoryResponseDto> GetCategoryByIdAsync(int id);
        Task<Category> AddCategoryAsync(CategoryRequestDto categoryDto);
        Task<bool> UpdateCategoryAsync(int id, CategoryRequestDto categoryDto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
