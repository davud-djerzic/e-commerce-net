using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models.RequestDto
{
    public class UserLoginDto
    {
        [Required]
        [StringLength(20)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string Password { get; set; } = string.Empty;
    }
}
