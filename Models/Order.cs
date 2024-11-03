using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        
        [ForeignKey("Product")] // Foreign key to product
        [Required]
        //[StringLength(20)]
        public int ProductId { get; set; } = 0;

        public Product? Product { get; set; } // instance of Product

        [Required]
        public int Quantity { get; set; } = 0;

        [Required]
        public decimal Price { get; set; } = 0;

        [ForeignKey("User")] // Foreign key to User
        [Required]
        public int UserId { get; set; }

        public User? User { get; set; } // instance of User

        [Required]
        public bool IsDeleted { get; set; } = false; // soft delete
    }
}
