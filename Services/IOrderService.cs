using Ecommerce.Models;
using Ecommerce.Models.ResponseDto;
using Ecommerce.Models.RequestDto;

namespace Ecommerce.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponseDto>> GetOrdersAsync();
        Task<OrderResponseDto> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderResponseDto>> GetYourOrdersAsync();
        Task<IEnumerable<OrderResponseDto>> GetYourAcceptedOrdersAsync();
        Task<IEnumerable<OrderResponseDto>> GetYourPendingOrdersAsync();
        Task<IEnumerable<OrderResponseDto>> GetYourRejectedOrdersAsync();
        Task<IEnumerable<OrderResponseDto>> GetAcceptedOrdersAsync();
        Task<IEnumerable<OrderResponseDto>> GetPendingOrdersAsync();
        Task<IEnumerable<OrderResponseDto>> GetRejectedOrdersAsync();
        Task<OrderResponseDto> CreateOrderAsync(CreateOrderWithProductDto orderDto);
        Task<int> ManageOrderPatchAsync(int id, ManageOrderDto manageOrderDto);
        Task<bool> SoftDeleteOrderAsync(int id);
        Task<IEnumerable<OrderResponseDto>> GetSoftDeletedOrdersAsync();
        Task UpdateOrderWithPutMethod(int id, CreateOrderWithProductDto orderDto);

    }
}
