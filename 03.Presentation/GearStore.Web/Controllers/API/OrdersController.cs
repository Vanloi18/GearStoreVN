using System.Security.Claims;
using GearStore.Application.Common;
using GearStore.Application.DTOs.Order;
using GearStore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GearStore.Web.Controllers.API;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> Checkout(OrderCreateDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
            var order = await _orderService.CheckoutAsync(userId, dto);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, ApiResponse<OrderResponseDto>.Ok(order, "Order placed successfully."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail($"Order failed: {ex.Message}"));
        }
    }

    [HttpPost("direct")]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> CreateDirectOrder(CreateOrderDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
            var order = await _orderService.CreateOrderAsync(userId, dto);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, ApiResponse<OrderResponseDto>.Ok(order, "Order placed successfully."));
        }
        catch (ArgumentException ex) { return BadRequest(ApiResponse<object>.Fail(ex.Message)); }
        catch (InvalidOperationException ex) { return BadRequest(ApiResponse<object>.Fail(ex.Message)); }
        catch (Exception ex) { return BadRequest(ApiResponse<object>.Fail($"Order failed: {ex.Message}")); }
    }

    [HttpGet("my")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderResponseDto>>>> GetMyOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var orders = await _orderService.GetMyOrdersAsync(userId);
        return Ok(ApiResponse<IEnumerable<OrderResponseDto>>.Ok(orders));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> GetOrderById(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id, userId);
            return Ok(ApiResponse<OrderResponseDto>.Ok(order));
        }
        catch (Exception ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
    }
}
