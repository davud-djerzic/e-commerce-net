using Ecommerce.Models;
using Ecommerce.Models.RequestDto;
using Ecommerce.Models.ResponseDto;

namespace Ecommerce.Services.ServiceInterfaces
{
    public interface IProductServices
    {
        Task<IEnumerable<ProductResponseDto>> GetProductsAsync();
        Task<ProductResponseDto> GetProductByIdAsync(int id);
        Task<ProductResponseDto> GetProductByNameAsync(string productName);
        Task<Product> AddProductAsync(ProductRequestDto productDto);
        Task DeleteProductAsync(int id);
        Task UpdateProductAsync(int id, ProductRequestDto productDto);
        Task<IEnumerable<ProductResponseDto>> GetProductsByPriceAsync(decimal bottomPrice, decimal topPrice);
        Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryIdAsync(int categoryId);
        Task<IEnumerable<ProductResponseDto>> GetProductsByAvailability();
    }
}
