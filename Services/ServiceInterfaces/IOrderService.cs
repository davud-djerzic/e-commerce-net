using Ecommerce.Models;
using Ecommerce.Models.ResponseDto;
using Ecommerce.Models.RequestDto;

namespace Ecommerce.Services.ServiceInterfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponseDto>> GetOrdersAsync();
        Task<OrderResponseDto> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderResponseDto>> GetYourOrdersAsync(int userId);
        Task<IEnumerable<OrderResponseDto>> GetYourAcceptedOrdersAsync(int userId);
        Task<IEnumerable<OrderResponseDto>> GetYourPendingOrdersAsync(int userId);
        Task<IEnumerable<OrderResponseDto>> GetYourCancelledOrdersAsync(int userId);
        Task<IEnumerable<OrderResponseDto>> GetAcceptedOrdersAsync();
        Task<IEnumerable<OrderResponseDto>> GetPendingOrdersAsync();
        Task<IEnumerable<OrderResponseDto>> GetCancelledOrdersAsync();
        Task<OrderResponseDto> CreateOrderAsync(CreateOrderWithProductDto orderDto, int userId);
        Task ManageOrderPatchAsync(int id, ManageOrderDto manageOrderDto);
        Task SoftDeleteOrderAsync(int id);
        Task<IEnumerable<OrderResponseDto>> GetSoftDeletedOrdersAsync();
        Task UpdateOrderAsync(int id, CreateOrderWithProductDto orderDto);

    }
}
