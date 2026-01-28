using GearStore.Application.Common;
using GearStore.Application.DTOs.Order;
using GearStore.Application.Interfaces.Services;
using GearStore.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GearStore.Web.Controllers.Admin;

[ApiController]
[Route("api/admin/orders")]
[Authorize(Roles = "Admin")]
[Produces("application/json")]
public class AdminOrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public AdminOrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderResponseDto>>>> GetAll()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(ApiResponse<IEnumerable<OrderResponseDto>>.Ok(orders));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> GetById(int id)
    {
        // Pass "Admin" or skip user check for Admin?
        // Service check: if userId provided, match. If null/empty? Service might fail or allow.
        // Let's rely on Service allowing null for Admin if designed, OR Create Admin Method.
        // GetOrderByIdAsync in Service: if (userId != null && order.UserId != userId) throw.
        // So passing null as userId should allow access (assuming implementation handles null).
        // Checking Service Step 1069:
        // "if (!string.IsNullOrEmpty(userId) && order.UserId != userId) ..."
        // So passing null userId bypasses the check!
        try 
        {
            var order = await _orderService.GetOrderByIdAsync(id, null); 
            return Ok(ApiResponse<OrderResponseDto>.Ok(order));
        }
        catch (Exception ex)
        {
            return NotFound(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
    {
        try
        {
            await _orderService.UpdateOrderStatusAsync(id, dto.Status);
            return Ok(ApiResponse<object>.Ok(null, "Order status updated"));
        }
        catch (InvalidOperationException ex) 
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }
}
