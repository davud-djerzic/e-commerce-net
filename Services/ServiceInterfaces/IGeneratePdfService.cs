using Ecommerce.Models;

namespace Ecommerce.Services.ServiceInterfaces
{
    public interface IGeneratePdfService
    {
        public void GeneratePdfFile(List<OrderProduct> orderProduct);
    }
}
