using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Ecommerce.Models
{
    public class Order
    {
        [Key] // primary key
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-increment
        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public int OrderStatus { get; set; } = 0;

        [Required]
        [StringLength(15)]
        public string OrderStatusMessage {  get; set; } = String.Empty;

        [Required]
        public decimal Price { get; set; } = 0;

        [ForeignKey("User")] // Foreign key to User
        [Required]
        public int UserId { get; set; }

        public User? User { get; set; } // instance of User

        [Required]
        public bool IsDeleted { get; set; } = false; // soft delete

        [JsonIgnore]
        public ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
