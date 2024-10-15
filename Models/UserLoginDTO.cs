using System.ComponentModel.DataAnnotations;

namespace E_commerce_API.Models
{
    public class UserLoginDTO
    {
        [Required]
        [StringLength(20)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string Password { get; set; } = string.Empty;
    }
}
