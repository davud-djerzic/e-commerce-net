using StackExchange.Redis;
using Ecommerce.Models;
using Ecommerce.Context;
using Ecommerce.Exceptions;
using Order = Ecommerce.Models.Order;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.Json;
using Ecommerce.Caches.Interfaces;
using Ecommerce.Models.ResponseDto;

namespace Ecommerce.Caches
{
    public class OrderCache : IOrderCache
    {
        private readonly IDatabase _store = ConnectionMultiplexer.Connect("localhost").GetDatabase();

        public async Task AddOrderProduct(Order order)
        {
            if (order == null || order.OrderProducts == null) throw new NotFoundException("Order or orderProducts not found");

            foreach(var product in order.OrderProducts)
            {
                if (product == null) throw new NotFoundException($"Product {product.ProductId} not found");
                ProductDataOrder productData = new ProductDataOrder()
                {
                    ProductId = product.ProductId,
                    Name = product.Product.ProductName,
                    Price = product.Product.Price,
                    Quantity = product.Quantity
                };

                string jsonValue = JsonSerializer.Serialize(productData);

                string redisKey = $"order:{order.Id}:product:{product.ProductId}";

                await _store.StringSetAsync(redisKey, jsonValue, TimeSpan.FromMinutes(14400));
            }
        }

        public async Task<ProductDataOrder> GetOrderProduct(int orderId, int productId)
        {
            string redisKey = $"order:{orderId}:product:{productId}";

            string jsonValue = await _store.StringGetAsync(redisKey);
            if (jsonValue == null) throw new NotFoundException("Json value of key doesn't exist");

            ProductDataOrder productData = JsonSerializer.Deserialize<ProductDataOrder>(jsonValue);
            if (productData == null) throw new Exception("Error in json deserialize");
            return productData;
        }

        public async Task AddTotalPrice(Order order, decimal totalPrice)
        {
            string jsonValue = JsonSerializer.Serialize(totalPrice);

            string redisKey = $"order:{order.Id}:totalPrice";

            await _store.StringSetAsync(redisKey, jsonValue, TimeSpan.FromMinutes(14400));
        }
        public async Task<decimal> GetTotalPrice(int orderId)
        {
            string redisKey = $"order:{orderId}:totalPrice";

            string jsonValue = await _store.StringGetAsync(redisKey);
            if (jsonValue == null) throw new NotFoundException("Json value of key doesn't exist");

            decimal totalPrice = JsonSerializer.Deserialize<decimal>(jsonValue);
            if (totalPrice == null) throw new Exception("Error in json deserialize");

            return totalPrice;
        }

    }
}
