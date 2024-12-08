using System.ComponentModel.DataAnnotations;
using Ecommerce.Models;

namespace Ecommerce.Models.RequestDto
{
    public class ManageOrderDto
    {
        [Required]
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
    }
}
