﻿using Ecommerce.Models;

namespace Ecommerce.Services.ServiceInterfaces
{
    public interface IGeneratePdfService
    {
        Task GeneratePdfFile(List<OrderProduct> orderProduct);
    }
}
