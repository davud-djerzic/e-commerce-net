namespace Ecommerce.Models.ResponseDto
{
    public class ProductDataOrder
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }  
        public int Quantity { get; set; }

    }
}
