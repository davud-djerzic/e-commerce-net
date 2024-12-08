using Ecommerce.Models.RequestDto;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Ecommerce.Models
{
    public class Category
    {
        [Key] // primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // auto increment
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<Product> Products { get; set; } = new List<Product>(); // connection between products and category

        public Category GetCategoryFromDto(Category category, CategoryRequestDto categoryDto)
        {
            category.CategoryName = categoryDto.CategoryName;
            category.Description = categoryDto.Description;

            return category;
        }
    }
}

