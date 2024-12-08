using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce.Exceptions;
using Ecommerce.Models.ResponseDto;
using Ecommerce.Models.RequestDto;
using Ecommerce.Services.ServiceInterfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Cors;
using Ecommerce.Services;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class OrderController(IOrderService _orderService) : ControllerBase
    {
        [HttpGet, Authorize(Roles = "Admin")]
        public async Task <ActionResult<IEnumerable<OrderResponseDto>>> GetOrders()
        {
            try
            {
                IEnumerable<OrderResponseDto> orders = await _orderService.GetOrdersAsync();
                return Ok(orders);
            } catch (NotFoundException ex)
            {
                return NotFound(new { ex = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex = ex.Message });
            }
        }

        [HttpGet("getById"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<OrderResponseDto>> GetOrderById([FromQuery] int id)
        {
            try
            {
                OrderResponseDto orders = await _orderService.GetOrderByIdAsync(id);
                return Ok(orders);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { ex = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex = ex.Message });
            }
        }

        [HttpGet("your"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetYourOrders()
        {
            try
            {
                Claim? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier); // get user ID from jwt token header
                if (userIdClaim == null) throw new NotFoundException("User ID not found in token");
                int userId = int.Parse(userIdClaim!.Value); // parse it to int
                if (userIdClaim == null) throw new NotFoundException("User not found in token");

                IEnumerable<OrderResponseDto> orders = await _orderService.GetYourOrdersAsync(userId);
                return Ok(orders);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex = ex.Message });
            }
        }

        [HttpGet("your/accepted"), Authorize(Roles = "Admin, User")]
        public async Task <ActionResult<IEnumerable<OrderResponseDto>>> GetYourAcceptedOrders()
        {
            try
            {
                Claim? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier); // get user ID from jwt token header
                if (userIdClaim == null) throw new NotFoundException("User ID not found in token");
                int userId = int.Parse(userIdClaim!.Value); // parse it to int
                if (userIdClaim == null) throw new NotFoundException("User not found in token");

                IEnumerable<OrderResponseDto> orders = await _orderService.GetYourAcceptedOrdersAsync(userId);
                return Ok(orders);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex = ex.Message });
            }
        }

        [HttpGet("your/pending"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetYourPendingOrders()
        {
            try
            {
                Claim? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier); // get user ID from jwt token header
                if (userIdClaim == null) throw new NotFoundException("User ID not found in token");
                int userId = int.Parse(userIdClaim!.Value); // parse it to int
                if (userIdClaim == null) throw new NotFoundException("User not found in token");

                IEnumerable<OrderResponseDto> orders = await _orderService.GetYourPendingOrdersAsync(userId);
                return Ok(orders);
            }
            catch(NotFoundException ex)
            {
                return NotFound(new { ex = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex = ex.Message });
            }
        }

        [HttpGet("your/rejected"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetYourRejectedOrders()
        {
            try
            {
                Claim? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier); // get user ID from jwt token header
                if (userIdClaim == null) throw new NotFoundException("User ID not found in token");
                int userId = int.Parse(userIdClaim!.Value); // parse it to int
                if (userIdClaim == null) throw new NotFoundException("User not found in token");

                IEnumerable<OrderResponseDto> orders = await _orderService.GetYourCancelledOrdersAsync(userId);
                return Ok(orders);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex = ex.Message });
            }
        }

        [HttpGet("accepted/"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetAcceptedOrders()
        {
            try
            {
                IEnumerable<OrderResponseDto> orders = await _orderService.GetAcceptedOrdersAsync();
                return Ok(orders);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex = ex.Message });
            }
        }

        [HttpGet("pending"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetPendingOrders()
        {
            try
            {
                IEnumerable<OrderResponseDto> orders = await _orderService.GetPendingOrdersAsync();
                return Ok(orders);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex = ex.Message });
            }
        }

        [HttpGet("rejected"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetRejectedOrders()
        {
            try
            {
                IEnumerable<OrderResponseDto> orders = await _orderService.GetCancelledOrdersAsync();
                return Ok(orders);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex = ex.Message });
            }
        }

        [HttpPost, Authorize(Roles = "Admin, User")]
        public async Task <ActionResult<OrderResponseDto>> CreateOrder(CreateOrderWithProductDto orderDto) 
        {
            try
            {
                Claim? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier); // get user ID from jwt token header
                if (userIdClaim == null) throw new NotFoundException("User ID not found in token");
                int userId = int.Parse(userIdClaim!.Value); // parse it to int
                if (userIdClaim == null) throw new NotFoundException("User not found in token");

                OrderResponseDto createdOrder = await _orderService.CreateOrderAsync(orderDto, userId);
                return Ok(createdOrder);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { ex = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex = ex.Message });
            }
        }


        [HttpPatch("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageOrderPatch(int id, ManageOrderDto manageOrderDto)
        {
            try
            {
                await _orderService.ManageOrderPatchAsync(id, manageOrderDto);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { ex = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex = ex.Message });
            }
        }

        [HttpDelete, Authorize(Roles = "Admin")]
        public async Task<ActionResult> SoftDeleteOrder(int id)
        {
            try
            {
                await _orderService.SoftDeleteOrderAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { ex = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex = ex.Message });
            }
        }

        [HttpGet("softDeleted"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetSoftDeletedOrders()
        {
            try
            {
                IEnumerable<OrderResponseDto> softDeletedOrders = await _orderService.GetSoftDeletedOrdersAsync();
                return Ok(softDeletedOrders);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex = ex.Message });
            }
        }

        [HttpPut, Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrder(int id, CreateOrderWithProductDto orderDto)
        {
            try
            {
                await _orderService.UpdateOrderAsync(id, orderDto);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { ex = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { ex = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex = ex.Message });
            }
        }
    }
}
