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

namespace Ecommerce.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetOrdersAsync()
        {
            var orders = await _context.Orders
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

                    if (!orders.Any()) throw new NotFoundException("Orders not found");

            return orders;
        }

        public async Task<OrderResponseDto> GetOrderByIdAsync(int id)
        {
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
              

            if (order == null) throw new NotFoundException("Order not found");

            return order;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetYourOrdersAsync()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier); // get user ID from jwt token header
            int userId = int.Parse(userIdClaim.Value); // parse it to int

            if (userIdClaim == null) throw new NotFoundException("User not found in token");

            var orders = await _context.Orders
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


            if (!orders.Any()) throw new NotFoundException("Orders not found");

            return orders;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetYourAcceptedOrdersAsync()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier); // get user ID from jwt token header
            int userId = int.Parse(userIdClaim.Value); // parse it to int

            if (userIdClaim == null) throw new NotFoundException("User not found in token");

            var orders = await _context.Orders
              .Where(o => !o.IsDeleted && o.UserId == userId && o.OrderStatus == 1)
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


            if (!orders.Any()) throw new NotFoundException("Your accepted orders not found");

            return orders;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetYourPendingOrdersAsync()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier); // get user ID from jwt token header
            int userId = int.Parse(userIdClaim.Value); // parse it to int

            if (userIdClaim == null) throw new NotFoundException("User not found in token");

            var orders = await _context.Orders
              .Where(o => !o.IsDeleted && o.UserId == userId && o.OrderStatus == 0)
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


            if (!orders.Any()) throw new NotFoundException("Your pending orders not found");

            return orders;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetYourRejectedOrdersAsync()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier); // get user ID from jwt token header
            int userId = int.Parse(userIdClaim.Value); // parse it to int

            if (userIdClaim == null) throw new NotFoundException("User not found in token");

            var orders = await _context.Orders
              .Where(o => !o.IsDeleted && o.UserId == userId && o.OrderStatus == -1)
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


            if (!orders.Any()) throw new NotFoundException("Your rejected orders not found");

            return orders;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetAcceptedOrdersAsync()
        {
            var orders = await _context.Orders
              .Where(o => !o.IsDeleted && o.OrderStatus == 1)
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


            if (!orders.Any()) throw new NotFoundException("Accepted orders not found");

            return orders;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetPendingOrdersAsync()
        {
            var orders = await _context.Orders
              .Where(o => !o.IsDeleted && o.OrderStatus == 0)
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


            if (!orders.Any()) throw new NotFoundException("Pending orders not found");

            return orders;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetRejectedOrdersAsync()
        {
            var orders = await _context.Orders
              .Where(o => !o.IsDeleted && o.OrderStatus == -1)
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


            if (!orders.Any()) throw new NotFoundException("Rejected orders not found");

            return orders;
        }
        
        public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderWithProductDto orderDto)
        {
            decimal price = 0;

            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier); // get userId from jwt token header
            if (userIdClaim == null) throw new UnauthorizedAccessException("User not found in token.");

            int userId = int.Parse(userIdClaim.Value); // parse it to int

            var user = await _context.Users.FindAsync(userId); // get user with that id
            if (user == null) throw new NotFoundException("User not found.");

            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                OrderStatus = 0,
                OrderStatusMessage = "Pending",
                IsDeleted = false,
                UserId = userId,
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var productList = new List<ProductResponseDto>();

            foreach (var productDto in orderDto.Products)
            {
                var product = await _context.Products
                .Include(p => p.Category) // Uključuje kategoriju u upit
                .FirstOrDefaultAsync(p => p.Id == productDto.ProductId);

                if (product == null) throw new NotFoundException($"Product {productDto.ProductId} not found");

                if (productDto.Quantity > product.StockQuantity) throw new NotFoundException("There are not enough number of product");

                if (productDto.Quantity <= 0) throw new NotFoundException("Quantity must be over zero");

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

            await _context.SaveChangesAsync();

            var responseDto = new OrderResponseDto
            {
               // ProductId = orderDto.Products.FirstOrDefault()?.ProductId ?? 0,
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

        public async Task<int> ManageOrderPatchAsync(int id, ManageOrderDto manageOrderDto)
        {
            var adminIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (adminIdClaim == null) return -1;

            int adminId = int.Parse(adminIdClaim.Value);

            var admin = await _context.Users.FindAsync(adminId);
            if (admin == null) return -1;

            var order = await _context.Orders.Include(o => o.OrderProducts).FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted); // find created order with his ID
            if (order == null) return -1;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == order.UserId);
            if (user == null) return -1;

            if (manageOrderDto.OrderStatus > 1 || manageOrderDto.OrderStatus < -1) return 0; // if order status was typed wrongly

            // if the last state of order is rejected and now the order status changed to better, remove stock quantity from product 
            if (order.OrderStatus == -1 && (manageOrderDto.OrderStatus == 1 || manageOrderDto.OrderStatus == 0)) 
            {
                foreach (var product in order.OrderProducts)
                    {
                        var orderedProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.ProductId);
                        if (orderedProduct == null) throw new NotFoundException("Ordered product not found");

                        orderedProduct.StockQuantity -= product.Quantity;
                    }
                
            }

            order.OrderStatus = manageOrderDto.OrderStatus; // update status of order

            if (order.OrderStatus == 1) order.OrderStatusMessage = "Accepted";

            if (order.OrderStatus == -1) 
            {
                order.OrderStatusMessage = "Rejected";
                foreach (var product in order.OrderProducts) // get back stock quantity from orderProduct quantity to product quantity
                {
                    var orderedProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.ProductId);
                    if (orderedProduct == null) throw new NotFoundException("Ordered product not found");

                    orderedProduct.StockQuantity += product.Quantity;
                }
            }

            if (order.OrderStatus == 0) order.OrderStatusMessage = "Pending";

            string tekst = "Your order with " + order.Id + " id is " + order.OrderStatusMessage.ToLower();

            await Execute(admin.FirstName, user.FirstName, "Information about order", admin.Email, user.Email, tekst);

            await _context.SaveChangesAsync();

            return 1;
        }

        public async Task<bool> SoftDeleteOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null || order.IsDeleted) return false;

            order.IsDeleted = true;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetSoftDeletedOrdersAsync()
        {
            var orders = await _context.Orders
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

            return orders; // TODO: pogledati kasnije
        }

        public async Task UpdateOrderWithPutMethod(int id, CreateOrderWithProductDto orderDto)
        {
            decimal totalPrice = 0;

            var productList = new List<ProductResponseDto>();

             var order = await _context.Orders
                .Include(o => o.OrderProducts)          // Uključuje sve OrderProduct zapise povezane s narudžbom
                    .ThenInclude(op => op.Product)      // Uključuje Product za svaki OrderProduct
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) throw new NotFoundException("Order not found"); 

            order.OrderStatus = 0;
            order.OrderStatusMessage = "Pending";
            order.OrderDate = DateTime.UtcNow;

            foreach(var op in order.OrderProducts)
            {
                totalPrice += op.Quantity * op.Product.Price; // calculate old total price
            }

            foreach (var productDto in orderDto.Products)
            {
                var product = await _context.Products
                .Include(p => p.Category) // Uključuje kategoriju u upit
                .FirstOrDefaultAsync(p => p.Id == productDto.ProductId);

                if (product == null) throw new NotFoundException($"Product {productDto.ProductId} not found");

                //if (productDto.Quantity > product.StockQuantity) throw new NotFoundException("There are not enough number of product");

                //if (productDto.Quantity <= 0) throw new NotFoundException("Quantity must be over zero");

                var existingOrderProduct = order.OrderProducts.FirstOrDefault(op => op.ProductId == productDto.ProductId);

                if (existingOrderProduct == null) {
                    var newOrderProduct = new OrderProduct
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
                        throw new NotFoundException("Invalid product quantity");

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

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == order.UserId);

            if (user == null) throw new NotFoundException("User not found");

            var orderProduct = await _context.OrderProducts.FirstOrDefaultAsync(o => o.OrderId == id);
        }

        static async Task Execute(string senderName, string receiverName, string subjectText, string senderEmail, string receiverEmail, string text)
        {
            var apiKey = "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";
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("SendGrid API key is not configured.");
            }
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(senderEmail, senderName);
            var subject = subjectText;
            var to = new EmailAddress(receiverEmail, receiverEmail);
            var plainTextContent = text;
            var htmlContent = text;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            // Onemogućavanje sandbox moda ako je bio uključen
            msg.MailSettings = new MailSettings
            {
                SandboxMode = new SandboxMode { Enable = false }
            };
            var response = await client.SendEmailAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.OK &&
               response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                throw new Exception($"Failed to send email: {response.StatusCode} - {responseBody}");
            }
        }
    }
}
