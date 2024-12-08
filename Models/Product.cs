using Ecommerce.Models.RequestDto;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Ecommerce.Models
{
    public class Product
    {
        [Key] // primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // auto-increment
        public int Id { get; set; }

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

        [ForeignKey("Category")] // Foreign key to Category
        [Required]
        public int CategoryId { get; set; }

        [JsonIgnore]
        [Required]
        public Category Category { get; set; } // instance of category

        [JsonIgnore]
        public ICollection<OrderProduct> OrderProducts { get; set;}

        public Product GetProductFromDto(Product product, ProductRequestDto productDto)
        {
            product.ProductCode = productDto.ProductCode;
            product.ProductName = productDto.ProductName;
            product.Price = productDto.Price;
            product.StockQuantity = productDto.StockQuantity;
            product.Weight = productDto.Weight;
            product.Manufacturer = productDto.Manufacturer;
            product.Description = productDto.Description;
            product.CategoryId = productDto.CategoryId;

            return product;
        }
    }
}
