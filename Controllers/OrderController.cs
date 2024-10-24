using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Context;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task <ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Product) // include Product instance
                .Include(o => o.User)    // include User instance
                .Select(o => new
                {
                    o.Id,
                    o.OrderDate,
                    o.OrderStatus,
                    o.OrderStatusMessage,
                    o.Quantity,
                    o.Price,
                    o.Product.ProductName, // get productName from Product class
                    o.Product.ProductCode,  // get ProductCode from Product class
                    o.UserId,
                    o.User.FirstName, // get FirstName from User classs
                    o.User.LastName
                }).ToListAsync(); 

            if (!orders.Any())
            {
                return NotFound("Orders not found");
            }

            return Ok(orders);
        }

        [HttpGet("your"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<Order>>> GetYourOrders()
        {
            // Dobijanje korisničkog ID-a iz JWT tokena
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier); // get user ID from jwt token header
            int userId = int.Parse(userIdClaim.Value); // parse it to int

            if (userIdClaim == null)
            {
                return Unauthorized("User not found in token");
            }

            var orders = await _context.Orders.Select(o => new
            {
                o.OrderDate,
                o.OrderStatus,
                o.OrderStatusMessage,
                o.Quantity,
                o.Price,
                o.Product.ProductName,
                o.Product.ProductCode,
                o.UserId,
                o.User.FirstName,
                o.User.LastName,
            }).Where(o => o.UserId == userId).ToListAsync(); // get orders where userId from jwt token matches UserId from order table

            if (!orders.Any())
            {
                return NotFound("Orders not found");
            }
            
            return Ok(orders);
        }

        [HttpGet("your/accepted"), Authorize(Roles = "Admin, User")]
        public async Task <ActionResult<IEnumerable<Order>>> GetYourAcceptedOrders()
        {
            // Dobijanje korisničkog ID-a iz JWT tokena
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = int.Parse(userIdClaim.Value);

            if (userIdClaim == null)
            {
                return Unauthorized("User not found in token");
            }

            var orders = await _context.Orders.Select(o => new
            {
                o.OrderDate,
                o.OrderStatus,
                o.OrderStatusMessage,
                o.Quantity,
                o.Price,
                o.Product.ProductName,
                o.Product.ProductCode,
                o.UserId,
                o.User.FirstName,
                o.User.LastName,
            }).Where(p => (p.OrderStatus == 1) && (p.UserId == userId)).ToListAsync(); // get your order which was accepted

            if (!orders.Any())
            {
                return NotFound("Your accepted orders not found");
            }

            return Ok(orders);
        }

        [HttpGet("your/pending"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<Order>>> GetYourPendingOrders()
        {
            // Dobijanje korisničkog ID-a iz JWT tokena
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = int.Parse(userIdClaim.Value);

            if (userIdClaim == null)
            {
                return Unauthorized("User not found in token");
            }

            var orders = await _context.Orders.Select(o => new
            {
                o.OrderDate,
                o.OrderStatus,
                o.OrderStatusMessage,
                o.Quantity,
                o.Price,
                o.Product.ProductName,
                o.Product.ProductCode,
                o.UserId,
                o.User.FirstName,
                o.User.LastName,
            }).Where(p => (p.OrderStatus == 0) && (p.UserId == userId)).ToListAsync(); // get your order which is pending

            if (!orders.Any())
            {
                return NotFound("Your accepted orders not found");
            }

            return Ok(orders);
        }

        [HttpGet("your/rejected"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<Order>>> GetYourRejectedOrders()
        {
            // Dobijanje korisničkog ID-a iz JWT tokena
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = int.Parse(userIdClaim.Value);

            if (userIdClaim == null)
            {
                return Unauthorized("User not found in token");
            }

            var orders = await _context.Orders.Select(o => new
            {
                o.OrderDate,
                o.OrderStatus,
                o.OrderStatusMessage,
                o.Quantity,
                o.Price,
                o.Product.ProductName,
                o.Product.ProductCode,
                o.UserId,
                o.User.FirstName,
                o.User.LastName,
            }).Where(p => (p.OrderStatus == -1) && (p.UserId == userId)).ToListAsync(); // get your order which was rejected

            if (!orders.Any())
            {
                return NotFound("Your rejected orders not found");
            }

            return Ok(orders);
        }

        [HttpGet("accepted/"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Order>>> GetAcceptedOrders()
        {
            var orders = await _context.Orders.Select(o => new
            {
                o.OrderDate,
                o.OrderStatus,
                o.OrderStatusMessage,
                o.Quantity,
                o.Price,
                o.Product.ProductName,
                o.Product.ProductCode,
                o.UserId,
                o.User.FirstName,
                o.User.LastName,
            }).Where(p => p.OrderStatus == 1).ToListAsync(); // get all orders which was accepted

            if (!orders.Any())
            {
                return NotFound("Accepted orders not found");
            }

            return Ok(orders);
        }

        [HttpGet("pending"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Order>>> GetPendingOrders()
        {
            var orders = await _context.Orders.Select(o => new
            {
                o.OrderDate,
                o.OrderStatus,
                o.OrderStatusMessage,
                o.Quantity,
                o.Price,
                o.Product.ProductName,
                o.Product.ProductCode,
                o.UserId,
                o.User.FirstName,
                o.User.LastName,
            }).Where(p => p.OrderStatus == 0).ToListAsync(); // get all orders which is pending

            if (!orders.Any())
            {
                return NotFound("Accepted orders not found");
            }

            return Ok(orders);
        }

        [HttpGet("rejected"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Order>>> GetRejectedOrders()
        {
            var orders = await _context.Orders.Select(o => new
            {
                o.OrderDate,
                o.OrderStatus,
                o.OrderStatusMessage,
                o.Quantity,
                o.Price,
                o.Product.ProductName,
                o.Product.ProductCode,
                o.UserId,
                o.User.FirstName,
                o.User.LastName,
            }).Where(p => p.OrderStatus == -1).ToListAsync(); // get all orders which was rejected

            if (!orders.Any())
            {
                return NotFound("Rejected orders not found");
            }

            return Ok(orders);
        }

        [HttpPost, Authorize(Roles = "Admin, User")]
        public async Task <ActionResult<Order>> CreateOrder(OrderDTO orderDTO) 
        {
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier); // get userId from jwt token header

            int userId = int.Parse(userIdClaim.Value); // parse it to int

            var user = await _context.Users.FindAsync(userId); // get user with that id

            if (userIdClaim == null)
            {
                return Unauthorized("User not found in token");
            }

            // get product by product name
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductName == orderDTO.ProductName);
            if (product == null)
            {
                return BadRequest("Product not found");
            }
            if (orderDTO.Quantity > product.StockQuantity) // if quantity of order is over a procuct quantity
            {
                return BadRequest("There are not enough number of product");
            }
            if (orderDTO.Quantity <= 0) // if order quantity is below zero
            {
                return BadRequest("Quantity must be over a zero");
            }

            product.StockQuantity -= orderDTO.Quantity; 
            
            decimal price = product.Price * orderDTO.Quantity; // calculate the price

            // Mapirajte DTO u model proizvoda
            var order = new Order
            {
                OrderDate = DateTime.UtcNow, // get the time of making order
                OrderStatus = 0, // order pending
                OrderStatusMessage = "Pending", // order status pending
                Quantity = orderDTO.Quantity,
                ProductId = product.Id,
                UserId = userId,
                Price = price
            }; 

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            
            return Ok(order);
        }


        [HttpPatch("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageOrderPatch(int id, ManageOrderDTO manageOrderDTO)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id); // find created order with his ID
            if (order == null)
            {
                return NotFound("Order not found.");
            }

            order.OrderStatus = manageOrderDTO.OrderStatus; // update status of order

            if (order.OrderStatus > 1 || order.OrderStatus < -1) // if order status was typed wrongly
            {
                return BadRequest("Put the value 1 or -1");
            }

            if (order.OrderStatus == 1)
                order.OrderStatusMessage = "Accepted";

            if (order.OrderStatus == -1)
                order.OrderStatusMessage = "Rejected";

            order.OrderDate = DateTime.UtcNow; // update order date 

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
