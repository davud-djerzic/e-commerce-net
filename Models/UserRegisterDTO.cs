using System.ComponentModel.DataAnnotations;

namespace E_commerce_API.Models
{
    public class UserRegisterDTO
    {
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
        [StringLength(30)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string Role { get; set; } = string.Empty;
    }
}
