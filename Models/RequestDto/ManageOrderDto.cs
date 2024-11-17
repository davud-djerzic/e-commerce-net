using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models.RequestDto
{
    public class ManageOrderDto
    {
        [Required]
        public int OrderStatus { get; set; } = 0;
    }
}
