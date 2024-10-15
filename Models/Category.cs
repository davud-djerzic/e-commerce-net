using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace E_commerce_API.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Dodaj ovo
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        // povezivanje s proizvodima
        [JsonIgnore]
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}

