using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Ecommerce.Models
{
    public class User
    {
        [Key] // primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // auto-increment
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]

        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string Role { get; set; } = string.Empty;

        [JsonIgnore]
        public List<Order> Orders { get; set; } = new List<Order>(); // list of orders
    }
}
