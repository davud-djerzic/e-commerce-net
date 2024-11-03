using Ecommerce.Models;

namespace Ecommerce.Services
{
    public interface IProductServices
    {
        Task <IEnumerable<Product>> GetProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task<Product> GetProductByNameAsync(string productName);
        Task<Product> AddProductAsync(ProductDTO productDTO);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> UpdateProductAsync(int id, ProductDTO productDTO);
        Task<IEnumerable<Product>> GetProductsByPriceAsync(decimal bottomPrice, decimal topPrice);
        Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int categoryId);
        Task<IEnumerable<Product>> GetProductsByAvailability();
    }
}
