using Ecommerce.Models;

namespace Ecommerce.Models.ResponseDto
{
    public class OrderResponseDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string OrderStatusMessage { get; set; } = string.Empty;
        public int Quantity {  get; set; }
        public decimal Price { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<ProductResponseDto> Products { get; set; }
    }
}
