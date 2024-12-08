using Ecommerce.Models;
using Ecommerce.Models.ResponseDto;
using Ecommerce.Models.RequestDto;

namespace Ecommerce.Services.ServiceInterfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetCategoriesAsync();
        Task<CategoryResponseDto> GetCategoryByIdAsync(int id);
        Task<Category> AddCategoryAsync(CategoryRequestDto categoryDto);
        Task UpdateCategoryAsync(int id, CategoryRequestDto categoryDto);
        Task DeleteCategoryAsync(int id);
    }
}
