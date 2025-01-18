namespace Ecommerce.Models
{
    public class ProductWithCategory
    {
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; } = decimal.Zero;
        public string CategoryName { get; set; } = string.Empty;
    }
}
