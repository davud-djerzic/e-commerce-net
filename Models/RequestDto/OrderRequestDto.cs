using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models.RequestDto
{
    public class OrderRequestDto
    {
        [Required]
        [StringLength(20)]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; } = 0;
    }
}
