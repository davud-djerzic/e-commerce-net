namespace Ecommerce.Models.RequestDto
{
    public class CreateOrderWithProductDto
    {
        public List <AddProductsToOrderDto> Products { get; set; }
    }
}
