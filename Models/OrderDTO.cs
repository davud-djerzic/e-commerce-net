using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models
{
    public class OrderDTO
    {
        [Required]
        [StringLength(20)]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; } = 0;
    }
}
