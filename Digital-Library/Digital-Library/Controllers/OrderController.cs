using Digital_Library.Core.Enum;
using Digital_Library.Core.ViewModels.Requests;
using Digital_Library.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Digital_Library.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        
        [HttpPost]
        [Authorize] 
        public async Task<IActionResult> CreateOrder([FromBody] List<OrderDetailRequest> items)
        {
            var userId = User?.Identity?.Name; 
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not found.");

            var response = await _orderService.CreateOrderAsync(userId, items);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserOrders([FromQuery] string? userId = null)
        {
            var orders = await _orderService.GetUserOrdersAsync(userId);
            return Ok(orders);
        }

      
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrderById(string id)
        {
            var response = await _orderService.GetOrderByIdAsync(id);
            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }

       
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(string id, [FromQuery] Status status)
        {
            var response = await _orderService.UpdateOrderStatusAsync(id, status);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
