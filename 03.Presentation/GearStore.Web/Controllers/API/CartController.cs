using System.Security.Claims;
using GearStore.Application.Common;
using GearStore.Application.DTOs.Cart;
using GearStore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GearStore.Web.Controllers.API;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly ILogger<CartController> _logger;

    public CartController(ICartService cartService, ILogger<CartController> logger)
    {
        _cartService = cartService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<CartDto>>> GetCart()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var cart = await _cartService.GetCartAsync(userId, null);
        
        if (cart == null)
            return Ok(ApiResponse<CartDto>.Ok(new CartDto { IsEmpty = true, Items = new List<CartItemDto>() }));

        return Ok(ApiResponse<CartDto>.Ok(cart));
    }

    [HttpPost("items")]
    public async Task<ActionResult<ApiResponse<CartDto>>> AddToCart(AddToCartDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
            var cart = await _cartService.AddToCartAsync(userId, null, dto);
            return Ok(ApiResponse<CartDto>.Ok(cart, "Item added to cart"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPut("items/{productId}")]
    public async Task<ActionResult<ApiResponse<CartDto>>> UpdateCartItem(int productId, UpdateCartItemDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
            var cart = await _cartService.UpdateCartItemAsync(userId, null, productId, dto);
            return Ok(ApiResponse<CartDto>.Ok(cart, "Cart item updated"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpDelete("items/{productId}")]
    public async Task<ActionResult<ApiResponse<CartDto>>> RemoveFromCart(int productId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
            var cart = await _cartService.RemoveFromCartAsync(userId, null, productId);
            return Ok(ApiResponse<CartDto>.Ok(cart, "Item removed from cart"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _cartService.ClearCartAsync(userId, null);
        return NoContent();
    }
}
