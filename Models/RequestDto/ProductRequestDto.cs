using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models.RequestDto
{
    public class ProductRequestDto
    {
        [Required]
        [StringLength(30)]
        public string ProductCode { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; }


        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive number.")]
        public int StockQuantity { get; set; } = 0;

        [Range(0.01, double.MaxValue, ErrorMessage = "Weight must be a positive value.")]
        public double Weight { get; set; }

        [StringLength(100)]
        public string Manufacturer { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; } // Foreign Key to Category
    }
}
