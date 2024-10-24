using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class ManageOrderDTO
    {
        [Required]
        public int OrderStatus { get; set; } = 0;
    }
}
