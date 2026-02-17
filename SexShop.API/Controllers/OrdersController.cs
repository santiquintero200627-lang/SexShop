using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SexShop.Application.DTOs.Orders;
using SexShop.Application.Interfaces;
using System.Security.Claims;

namespace SexShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderDto createOrderDto)
        {
            var userId = User.FindFirst("uid")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _orderService.CreateOrderAsync(userId, createOrderDto);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.FindFirst("uid")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(result);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
             var result = await _orderService.GetOrderByIdAsync(id);
             if (!result.Succeeded) return NotFound(result);
             return Ok(result);
        }
    }
}
