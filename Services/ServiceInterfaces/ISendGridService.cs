using Ecommerce.Models;

namespace Ecommerce.Services.ServiceInterfaces
{
    public interface ISendGridService
    {
        Task SendEmailAsync(SendGridModel sendGrid);
    }
}
