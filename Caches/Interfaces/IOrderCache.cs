using Ecommerce.Models;
using Ecommerce.Models.ResponseDto;

namespace Ecommerce.Caches.Interfaces
{
    public interface IOrderCache
    {
        Task AddOrderProduct(Order order);
        Task<ProductDataOrder> GetOrderProduct(int orderId, int productId);
        Task AddTotalPrice(Order order, decimal totalPrice);
        Task<decimal> GetTotalPrice(int orderId);
    }
}
