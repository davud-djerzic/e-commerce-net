namespace Ecommerce.Models.ResponseDto
{
    public class OrderResponseDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int OrderStatus { get; set; }
        public string? OrderStatusMessage { get; set; }
        //public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public List <ProductResponseDto> Products { get; set; }
    }
}
