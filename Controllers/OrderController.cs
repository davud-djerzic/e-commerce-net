using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce.Exceptions;
using Ecommerce.Services;
using Ecommerce.Models.ResponseDto;
using Ecommerce.Models.RequestDto;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task <ActionResult<IEnumerable<OrderResponseDto>>> GetOrders()
        {
            try
            {
                var orders = await _orderService.GetOrdersAsync();
                return Ok(orders);
            } catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("getById"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetOrderById([FromQuery] int id)
        {
            try
            {
                var orders = await _orderService.GetOrderByIdAsync(id);
                return Ok(orders);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("your"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetYourOrders()
        {
            try
            {
                var orders = await _orderService.GetYourOrdersAsync();
                return Ok(orders);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("your/accepted"), Authorize(Roles = "Admin, User")]
        public async Task <ActionResult<IEnumerable<OrderResponseDto>>> GetYourAcceptedOrders()
        {
            try
            {
                var orders = await _orderService.GetYourAcceptedOrdersAsync();
                return Ok(orders);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("your/pending"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetYourPendingOrders()
        {
            try
            {
                var orders = await _orderService.GetYourPendingOrdersAsync();
                return Ok(orders);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("your/rejected"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetYourRejectedOrders()
        {
            try
            {
                var orders = await _orderService.GetYourRejectedOrdersAsync();
                return Ok(orders);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("accepted/"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetAcceptedOrders()
        {
            try
            {
                var orders = await _orderService.GetAcceptedOrdersAsync();
                return Ok(orders);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("pending"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetPendingOrders()
        {
            try
            {
                var orders = await _orderService.GetPendingOrdersAsync();
                return Ok(orders);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("rejected"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetRejectedOrders()
        {
            try
            {
                var orders = await _orderService.GetRejectedOrdersAsync();
                return Ok(orders);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost, Authorize(Roles = "Admin, User")]
        public async Task <ActionResult<OrderResponseDto>> CreateOrder(CreateOrderWithProductDto orderDto) 
        {
            try
            {
                var createdOrder = await _orderService.CreateOrderAsync(orderDto);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return BadRequest(ex.Message);  
            }
            
        }


        [HttpPatch("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageOrderPatch(int id, ManageOrderDto manageOrderDto)
        {
            var orderStatus = await _orderService.ManageOrderPatchAsync(id, manageOrderDto);
            if (orderStatus == -1) return NotFound("Order not found");
            if (orderStatus == 0) return BadRequest("Please type correct format of order status(-1 to 1");

            return NoContent();
        }

        [HttpDelete, Authorize(Roles = "Admin")]
        public async Task<ActionResult> SoftDeleteOrder(int id)
        {
            var isSoftDeletedOrder = await _orderService.SoftDeleteOrderAsync(id);
            if (!isSoftDeletedOrder) return NotFound("Order not found");

            return Ok("Order deleted");
        }

        [HttpGet("softDeleted"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Order>>> GetSoftDeletedOrders()
        {
            try
            {
                var softDeletedOrders = await _orderService.GetSoftDeletedOrdersAsync();
                return Ok(softDeletedOrders);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut, Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderWithPutMethod(int id, CreateOrderWithProductDto orderDto)
        {
            try
            {
                await _orderService.UpdateOrderWithPutMethod(id, orderDto);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                if (ex.Message == "Please change order quantity") return BadRequest(ex.Message);
                else return NotFound(ex.Message);
            }
        }
    }
}
