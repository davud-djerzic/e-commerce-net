using Ecommerce.Context;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Exceptions;
using System.Security.Claims;
using Ecommerce.Models.ResponseDto;
using Ecommerce.Models.RequestDto;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;
using Ecommerce.Services.ServiceInterfaces;
using Ecommerce.Caches;
using Microsoft.EntityFrameworkCore.Storage;
using Ecommerce.Caches.Interfaces;
//using StackExchange.Redis;

namespace Ecommerce.Services
{
    public class OrderService(ApplicationDbContext _context, IHttpContextAccessor _httpContextAccessor, ISendGridService sendGridService, IOrderCache orderCache, IGeneratePdfService generatePdfService) : IOrderService
    {
        public async Task<IEnumerable<OrderResponseDto>> GetOrdersAsync()
        {
            List<OrderResponseDto> orders = await _context.Orders
              .Where(o => !o.IsDeleted)
              .Include(o => o.OrderProducts)           // Include the link between Order and Product
                  .ThenInclude(op => op.Product)       // Include the Product entity
              .Include(o => o.User)                    // Include the User entity
              .Select(o => new OrderResponseDto
              {
                  OrderId = o.Id,
                  OrderDate = o.OrderDate,
                  OrderStatus = o.OrderStatus,
                  OrderStatusMessage = o.OrderStatusMessage,
                  Price = o.Price,
                  UserId = o.UserId,
                  FirstName = o.User.FirstName,
                  LastName = o.User.LastName,
                  Products = o.OrderProducts.Select(op => new ProductResponseDto
                  {
                      Id = op.ProductId,
                      ProductName = op.Product.ProductName,
                      ProductCode = op.Product.ProductCode,
                      Weight = op.Product.Weight,
                      Manufacturer = op.Product.Manufacturer,
                      CategoryId = op.Product.CategoryId,
                      CategoryName = op.Product.Category.CategoryName,
                      StockQuantity = op.Quantity,
                      Price = op.Product.Price
                  }).ToList() // Get the list of products with details for each order
              })
              .ToListAsync();

              if (orders.Count == 0) throw new NotFoundException("Orders not found");

            return orders;
        }

        public async Task<OrderResponseDto> GetOrderByIdAsync(int id)
        {
            if (id <= 0) throw new BadRequestException("Id must be over a zero");

            var order = await _context.Orders
              .Where(o => !o.IsDeleted && o.Id == id)
              .Include(o => o.OrderProducts)           // Include the link between Order and Product
                  .ThenInclude(op => op.Product)       // Include the Product entity
              .Include(o => o.User)                    // Include the User entity
              .Select(o => new OrderResponseDto
              {
                  OrderId = o.Id,
                  OrderDate = o.OrderDate,
                  OrderStatus = o.OrderStatus,
                  OrderStatusMessage = o.OrderStatusMessage,
                  Price = o.Price,
                  UserId = o.UserId,
                  FirstName = o.User.FirstName,
                  LastName = o.User.LastName,
                  Products = o.OrderProducts.Select(op => new ProductResponseDto
                  {
                      Id = op.ProductId,
                      ProductName = op.Product.ProductName,
                      ProductCode = op.Product.ProductCode,
                      Weight = op.Product.Weight,
                      Manufacturer = op.Product.Manufacturer,
                      CategoryId = op.Product.CategoryId,
                      CategoryName = op.Product.Category.CategoryName,
                      StockQuantity = op.Quantity,
                      Price = op.Product.Price
                  }).ToList() // Get the list of products with details for each order
              }).FirstOrDefaultAsync();
              

            if (order == null) throw new NotFoundException($"Order with {id} id not found");

            return order;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetYourOrdersAsync(int userId)
        {
            List<OrderResponseDto> orders = await _context.Orders
              .Where(o => !o.IsDeleted && o.UserId == userId)
              .Include(o => o.OrderProducts)           // Include the link between Order and Product
                  .ThenInclude(op => op.Product)       // Include the Product entity
              .Include(o => o.User)                    // Include the User entity
              .Select(o => new OrderResponseDto
              {
                  OrderId = o.Id,
                  OrderDate = o.OrderDate,
                  OrderStatus = o.OrderStatus,
                  OrderStatusMessage = o.OrderStatusMessage,
                  Price = o.Price,
                  UserId = o.UserId,
                  FirstName = o.User.FirstName,
                  LastName = o.User.LastName,
                  Products = o.OrderProducts.Select(op => new ProductResponseDto
                  {
                      Id = op.ProductId,
                      ProductName = op.Product.ProductName,
                      ProductCode = op.Product.ProductCode,
                      Weight = op.Product.Weight,
                      Manufacturer = op.Product.Manufacturer,
                      CategoryId = op.Product.CategoryId,
                      CategoryName = op.Product.Category.CategoryName,
                      StockQuantity = op.Quantity,
                      Price = op.Product.Price
                  }).ToList() // Get the list of products with details for each order
              }).ToListAsync();


            if (orders.Count == 0) throw new NotFoundException("Orders not found");

            return orders;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetYourAcceptedOrdersAsync(int userId)
        {
            List<OrderResponseDto> orders = await _context.Orders
              .Where(o => !o.IsDeleted && o.UserId == userId && o.OrderStatus == OrderStatus.Accepted)
              .Include(o => o.OrderProducts)           // Include the link between Order and Product
                  .ThenInclude(op => op.Product)       // Include the Product entity
              .Include(o => o.User)                    // Include the User entity
              .Select(o => new OrderResponseDto
              {
                  OrderId = o.Id,
                  OrderDate = o.OrderDate,
                  OrderStatus = o.OrderStatus,
                  OrderStatusMessage = o.OrderStatusMessage,
                  Price = o.Price,
                  UserId = o.UserId,
                  FirstName = o.User.FirstName,
                  LastName = o.User.LastName,
                  Products = o.OrderProducts.Select(op => new ProductResponseDto
                  {
                      Id = op.ProductId,
                      ProductName = op.Product.ProductName,
                      ProductCode = op.Product.ProductCode,
                      Weight = op.Product.Weight,
                      Manufacturer = op.Product.Manufacturer,
                      CategoryId = op.Product.CategoryId,
                      CategoryName = op.Product.Category.CategoryName,
                      StockQuantity = op.Quantity,
                      Price = op.Product.Price
                  }).ToList() // Get the list of products with details for each order
              }).ToListAsync();


            if (orders.Count == 0) throw new NotFoundException("Your accepted orders not found");

            return orders;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetYourPendingOrdersAsync(int userId)
        {
            List<OrderResponseDto> orders = await _context.Orders
              .Where(o => !o.IsDeleted && o.UserId == userId && o.OrderStatus == OrderStatus.Pending)
              .Include(o => o.OrderProducts)           // Include the link between Order and Product
                  .ThenInclude(op => op.Product)       // Include the Product entity
              .Include(o => o.User)                    // Include the User entity
              .Select(o => new OrderResponseDto
              {
                  OrderId = o.Id,
                  OrderDate = o.OrderDate,
                  OrderStatus = o.OrderStatus,
                  OrderStatusMessage = o.OrderStatusMessage,
                  Price = o.Price,
                  UserId = o.UserId,
                  FirstName = o.User.FirstName,
                  LastName = o.User.LastName,
                  Products = o.OrderProducts.Select(op => new ProductResponseDto
                  {
                      Id = op.ProductId,
                      ProductName = op.Product.ProductName,
                      ProductCode = op.Product.ProductCode,
                      Weight = op.Product.Weight,
                      Manufacturer = op.Product.Manufacturer,
                      CategoryId = op.Product.CategoryId,
                      CategoryName = op.Product.Category.CategoryName,
                      StockQuantity = op.Quantity,
                      Price = op.Product.Price
                  }).ToList() // Get the list of products with details for each order
              }).ToListAsync();


            if (orders.Count == 0) throw new NotFoundException("Your pending orders not found");

            return orders;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetYourCancelledOrdersAsync(int userId)
        {
            List<OrderResponseDto> orders = await _context.Orders
              .Where(o => !o.IsDeleted && o.UserId == userId && o.OrderStatus == OrderStatus.Cancelled)
              .Include(o => o.OrderProducts)           // Include the link between Order and Product
                  .ThenInclude(op => op.Product)       // Include the Product entity
              .Include(o => o.User)                    // Include the User entity
              .Select(o => new OrderResponseDto
              {
                  OrderId = o.Id,
                  OrderDate = o.OrderDate,
                  OrderStatus = o.OrderStatus,
                  OrderStatusMessage = o.OrderStatusMessage,
                  Price = o.Price,
                  UserId = o.UserId,
                  FirstName = o.User.FirstName,
                  LastName = o.User.LastName,
                  Products = o.OrderProducts.Select(op => new ProductResponseDto
                  {
                      Id = op.ProductId,
                      ProductName = op.Product.ProductName,
                      ProductCode = op.Product.ProductCode,
                      Weight = op.Product.Weight,
                      Manufacturer = op.Product.Manufacturer,
                      CategoryId = op.Product.CategoryId,
                      CategoryName = op.Product.Category.CategoryName,
                      StockQuantity = op.Quantity,
                      Price = op.Product.Price
                  }).ToList() // Get the list of products with details for each order
              }).ToListAsync();


            if (orders.Count == 0) throw new NotFoundException("Your cancelled orders not found");

            return orders;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetAcceptedOrdersAsync()
        {
            List<OrderResponseDto> orders = await _context.Orders
              .Where(o => !o.IsDeleted && o.OrderStatus == OrderStatus.Accepted)
              .Include(o => o.OrderProducts)           // Include the link between Order and Product
                  .ThenInclude(op => op.Product)       // Include the Product entity
              .Include(o => o.User)                    // Include the User entity
              .Select(o => new OrderResponseDto
              {
                  OrderId = o.Id,
                  OrderDate = o.OrderDate,
                  OrderStatus = o.OrderStatus,
                  OrderStatusMessage = o.OrderStatusMessage,
                  Price = o.Price,
                  UserId = o.UserId,
                  FirstName = o.User.FirstName,
                  LastName = o.User.LastName,
                  Products = o.OrderProducts.Select(op => new ProductResponseDto
                  {
                      Id = op.ProductId,
                      ProductName = op.Product.ProductName,
                      ProductCode = op.Product.ProductCode,
                      Weight = op.Product.Weight,
                      Manufacturer = op.Product.Manufacturer,
                      CategoryId = op.Product.CategoryId,
                      CategoryName = op.Product.Category.CategoryName,
                      StockQuantity = op.Quantity,
                      Price = op.Product.Price
                  }).ToList() // Get the list of products with details for each order
              }).ToListAsync();


            if (orders.Count == 0) throw new NotFoundException("Accepted orders not found");

            return orders;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetPendingOrdersAsync()
        {
            List<OrderResponseDto> orders = await _context.Orders
              .Where(o => !o.IsDeleted && o.OrderStatus == OrderStatus.Pending)
              .Include(o => o.OrderProducts)           // Include the link between Order and Product
                  .ThenInclude(op => op.Product)       // Include the Product entity
              .Include(o => o.User)                    // Include the User entity
              .Select(o => new OrderResponseDto
              {
                  OrderId = o.Id,
                  OrderDate = o.OrderDate,
                  OrderStatus = o.OrderStatus,
                  OrderStatusMessage = o.OrderStatusMessage,
                  Price = o.Price,
                  UserId = o.UserId,
                  FirstName = o.User.FirstName,
                  LastName = o.User.LastName,
                  Products = o.OrderProducts.Select(op => new ProductResponseDto
                  {
                      Id = op.ProductId,
                      ProductName = op.Product.ProductName,
                      ProductCode = op.Product.ProductCode,
                      Weight = op.Product.Weight,
                      Manufacturer = op.Product.Manufacturer,
                      CategoryId = op.Product.CategoryId,
                      CategoryName = op.Product.Category.CategoryName,
                      StockQuantity = op.Quantity,
                      Price = op.Product.Price
                  }).ToList() // Get the list of products with details for each order
              }).ToListAsync();


            if (orders.Count == 0) throw new NotFoundException("Pending orders not found");

            return orders;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetCancelledOrdersAsync()
        {
            List<OrderResponseDto> orders = await _context.Orders
              .Where(o => !o.IsDeleted && o.OrderStatus == OrderStatus.Cancelled)
              .Include(o => o.OrderProducts)           // Include the link between Order and Product
                  .ThenInclude(op => op.Product)       // Include the Product entity
              .Include(o => o.User)                    // Include the User entity
              .Select(o => new OrderResponseDto
              {
                  OrderId = o.Id,
                  OrderDate = o.OrderDate,
                  OrderStatus = o.OrderStatus,
                  OrderStatusMessage = o.OrderStatusMessage,
                  Price = o.Price,
                  UserId = o.UserId,
                  FirstName = o.User.FirstName,
                  LastName = o.User.LastName,
                  Products = o.OrderProducts.Select(op => new ProductResponseDto
                  {
                      Id = op.ProductId,
                      ProductName = op.Product.ProductName,
                      ProductCode = op.Product.ProductCode,
                      Weight = op.Product.Weight,
                      Manufacturer = op.Product.Manufacturer,
                      CategoryId = op.Product.CategoryId,
                      CategoryName = op.Product.Category.CategoryName,
                      StockQuantity = op.Quantity,
                      Price = op.Product.Price
                  }).ToList() // Get the list of products with details for each order
              }).ToListAsync();


            if (orders.Count == 0) throw new NotFoundException("Cancelled orders not found");

            return orders;
        }
        
        public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderWithProductDto orderDto, int userId)
        {
            var productList = new List<ProductResponseDto>();

            if (orderDto.Products == null || !orderDto.Products.Any())
            {
                throw new BadRequestException("No products selected for the order.");
            }

            decimal price = 0;
            var user = await _context.Users.FindAsync(userId); // get user with that id
            if (user == null) throw new NotFoundException("User not found.");

            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                OrderStatus = OrderStatus.Pending, 
                OrderStatusMessage = OrderStatus.Pending.ToString(),
                IsDeleted = false,
                UserId = userId,
            };

            _context.Orders.Add(order);

            await _context.SaveChangesAsync();

            foreach (var productDto in orderDto.Products)
            {
                var product = await _context.Products
                    .Include(p => p.Category) // Include category in the query
                    .FirstOrDefaultAsync(p => p.Id == productDto.ProductId);

                if (product == null) {
                    await removeOrderNullProduct(order);
                    throw new NotFoundException($"Product {productDto.ProductId} not found");
                }


                if (productDto.Quantity > product.StockQuantity)
                {
                    await removeOrderNullProduct(order);
                    throw new BadRequestException("There are not enough number of product");
                }

                if (productDto.Quantity <= 0) {
                    await removeOrderNullProduct(order);
                    throw new BadRequestException("Quantity must be over zero");
                }
                

                product.StockQuantity -= productDto.Quantity;

                price += product.Price * productDto.Quantity;

                var orderProduct = new OrderProduct
                {
                    OrderId = order.Id, 
                    ProductId = productDto.ProductId,
                    Quantity = productDto.Quantity,
                };

                _context.OrderProducts.Add(orderProduct);

                productList.Add(new ProductResponseDto
                {
                    Id = product.Id,
                    ProductCode = product.ProductCode,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    Weight = product.Weight,
                    Manufacturer = product.Manufacturer,
                    Description = product.Description,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category?.CategoryName,
                });
            }

            order.Price = price;

            if (productList.Count == 0)
            {
                await removeOrderNullProduct(order);
                throw new BadRequestException("Zero product selected");
            }

            await _context.SaveChangesAsync();

            await orderCache.AddOrderProduct(order);

            decimal totalPrice = 0;
            foreach(var product in productList)
            {
                ProductDataOrder productDataOrder = await orderCache.GetOrderProduct(order.Id, product.Id);
                totalPrice += productDataOrder.Price * productDataOrder.Quantity;
            }

            await orderCache.AddTotalPrice(order, totalPrice);

            var responseDto = new OrderResponseDto
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                OrderStatus = order.OrderStatus, 
                OrderStatusMessage = order.OrderStatusMessage,
                Price = order.Price,
                UserId = order.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Products = productList,
            };

            return responseDto;
        }

        private async Task removeOrderNullProduct(Order order)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }

        public async Task ManageOrderPatchAsync(int id, ManageOrderDto manageOrderDto)
        {
            if (id <= 0) throw new BadRequestException("Id must be over a zero");

            Order? order = await _context.Orders.Include(o => o.OrderProducts).FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted); // find created order with his ID
            if (order == null) throw new NotFoundException($"Order {id} not found");

            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == order.UserId);
            if (user == null) throw new NotFoundException($"User {order.UserId} not found");

           // if (manageOrderDto.OrderStatus > 1 || manageOrderDto.OrderStatus < -1) return 0; // if order status was typed wrongly

            // if the last state of order is rejected and now the order status changed to better, remove stock quantity from product 
            if (order.OrderStatus == OrderStatus.Cancelled && (manageOrderDto.OrderStatus == OrderStatus.Accepted || manageOrderDto.OrderStatus == OrderStatus.Pending)) 
            {
                foreach (var product in order.OrderProducts)
                    {
                        var orderedProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.ProductId);
                        if (orderedProduct == null) throw new NotFoundException("Ordered product not found");

                        orderedProduct.StockQuantity -= product.Quantity;
                    }
                
            }

            order.OrderStatus = manageOrderDto.OrderStatus; // update status of order

            if (order.OrderStatus == OrderStatus.Accepted) order.OrderStatusMessage = OrderStatus.Accepted.ToString();

            if (order.OrderStatus == OrderStatus.Cancelled) 
            {
                order.OrderStatusMessage = OrderStatus.Cancelled.ToString();
                foreach (var product in order.OrderProducts) // get back stock quantity from orderProduct quantity to product quantity
                {
                    var orderedProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.ProductId);
                    if (orderedProduct == null) throw new NotFoundException("Ordered product not found");

                    orderedProduct.StockQuantity += product.Quantity;
                }
            }

            if (order.OrderStatus == OrderStatus.Pending) order.OrderStatusMessage = OrderStatus.Pending.ToString();

            List<OrderProduct> orderProductList = await _context.OrderProducts.Where(op => op.OrderId == id).ToListAsync();
            if (orderProductList.Count == 0) throw new NotFoundException("Order products not found");
            
            generatePdfService.GeneratePdfFile(orderProductList);
            //var questPdf = new GeneratePdfService(_context);
            //questPdf.GeneratePdfFile(orderProductList);

            string path = Path.Combine(Directory.GetCurrentDirectory() + "/pdfs/", $"Order {orderProductList[0].OrderId}.pdf");

            if (!File.Exists(path))
            {
                throw new Exception("PDF generation failed or file not found.");
            }

            string tekst = "Your order with is " + order.OrderStatusMessage.ToLower();

            await sendGridService.SendEmailAsync("Admin", user.FirstName, "Information about order", user.Email, tekst, path);
            
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteOrderAsync(int id)
        {
            if (id <= 0) throw new BadRequestException("Id must be over a zero");

            Order? order = await _context.Orders.FirstOrDefaultAsync(o => !o.IsDeleted && o.Id == id);
            if (order == null || order.IsDeleted) throw new NotFoundException($"Order {id} not found");

            order.IsDeleted = true;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderResponseDto>> GetSoftDeletedOrdersAsync()
        {
            List<OrderResponseDto> orders = await _context.Orders
              .Where(o => o.IsDeleted)
              .Include(o => o.OrderProducts)           // Include the link between Order and Product
                  .ThenInclude(op => op.Product)       // Include the Product entity
              .Include(o => o.User)                    // Include the User entity
              .Select(o => new OrderResponseDto
              {
                  OrderId = o.Id,
                  OrderDate = o.OrderDate,
                  OrderStatus = o.OrderStatus,
                  OrderStatusMessage = o.OrderStatusMessage,
                  Price = o.Price,
                  UserId = o.UserId,
                  FirstName = o.User.FirstName,
                  LastName = o.User.LastName,
                  Products = o.OrderProducts.Select(op => new ProductResponseDto
                  {
                      Id = op.ProductId,
                      ProductName = op.Product.ProductName,
                      ProductCode = op.Product.ProductCode,
                      Weight = op.Product.Weight,
                      Manufacturer = op.Product.Manufacturer,
                      CategoryId = op.Product.CategoryId,
                      CategoryName = op.Product.Category.CategoryName,
                      StockQuantity = op.Quantity,
                      Price = op.Product.Price
                  }).ToList() // Get the list of products with details for each order
              }).ToListAsync();


            if (!orders.Any()) throw new NotFoundException("Soft deleted orders not found");

            return orders; 
        }

        public async Task UpdateOrderAsync(int id, CreateOrderWithProductDto orderDto)
        {
            if (id <= 0) throw new BadRequestException("Id must be over a zero");

            decimal totalPrice = 0;

            List<ProductResponseDto> productList = new List<ProductResponseDto>();

             Order? order = await _context.Orders
                .Include(o => o.OrderProducts)         
                    .ThenInclude(op => op.Product)      
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) throw new NotFoundException("Order not found"); 

            order.OrderStatus = 0;
            order.OrderStatusMessage = OrderStatus.Pending.ToString();
            order.OrderDate = DateTime.UtcNow;

            foreach(var op in order.OrderProducts)
            {
                totalPrice += op.Quantity * op.Product.Price; // calculate old total price
            }

            foreach (var productDto in orderDto.Products)
            {
                Product? product = await _context.Products
                .Include(p => p.Category) 
                .FirstOrDefaultAsync(p => p.Id == productDto.ProductId);

                if (product == null) throw new NotFoundException($"Product {productDto.ProductId} not found");

                //if (productDto.Quantity > product.StockQuantity) throw new NotFoundException("There are not enough number of product");

                //if (productDto.Quantity <= 0) throw new NotFoundException("Quantity must be over zero");

                OrderProduct? existingOrderProduct = order.OrderProducts.FirstOrDefault(op => op.ProductId == productDto.ProductId);

                if (existingOrderProduct == null) {
                    OrderProduct newOrderProduct = new OrderProduct
                    {
                        OrderId = order.Id,
                        ProductId = productDto.ProductId,
                        Quantity = productDto.Quantity,
                    };
                    _context.OrderProducts.Add(newOrderProduct);

                    totalPrice += product.Price * productDto.Quantity;
                    product.StockQuantity -= productDto.Quantity;
                }
                else // already exist
                {
                    product.StockQuantity += existingOrderProduct.Quantity; // get back the last stock quantity

                    totalPrice -= existingOrderProduct.Quantity * existingOrderProduct.Product.Price;

                    existingOrderProduct.ProductId = productDto.ProductId; // change ProductId 

                    if (productDto.Quantity < 0 || productDto.Quantity > product.StockQuantity)
                        throw new BadRequestException("Invalid product quantity");

                    totalPrice += productDto.Quantity * product.Price;

                    existingOrderProduct.Quantity = productDto.Quantity;

                    product.StockQuantity -= productDto.Quantity;
                }


                productList.Add(new ProductResponseDto
                {
                    Id = product.Id,
                    ProductCode = product.ProductCode,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    Weight = product.Weight,
                    Manufacturer = product.Manufacturer,
                    Description = product.Description,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category?.CategoryName,
                });
            }

            order.Price = totalPrice;

            await _context.SaveChangesAsync();

            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == order.UserId);

            if (user == null) throw new NotFoundException("User not found");

            OrderProduct? orderProduct = await _context.OrderProducts.FirstOrDefaultAsync(o => o.OrderId == id);
        }

    }
}
