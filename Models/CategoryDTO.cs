using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class CategoryDTO
    {

        [Required]
        [StringLength(50)]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
