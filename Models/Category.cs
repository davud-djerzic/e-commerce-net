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
        public string? Description { get; set; }

        [JsonIgnore]
        public ICollection<Product> Products { get; set; } = new List<Product>(); // connection between products and category
    }
}

