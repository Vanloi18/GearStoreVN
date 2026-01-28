using GearStore.Application.DTOs.Cart;
using GearStore.Application.Interfaces;
using GearStore.Application.Interfaces.Services;
using GearStore.Domain.Entities;
using GearStore.Domain.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace GearStore.Application.Services;

/// <summary>
/// Service implementation for cart operations
/// </summary>
public class CartService : ICartService
{
    private readonly IUnitOfWork _unitOfWork;

    public CartService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CartDto?> GetCartAsync(string? userId, string? sessionId, CancellationToken cancellationToken = default)
    {
        Cart? cart = null;

        if (!string.IsNullOrEmpty(userId))
        {
            cart = await _unitOfWork.Carts.GetByUserIdAsync(userId, cancellationToken);
        }
        else if (!string.IsNullOrEmpty(sessionId))
        {
            cart = await _unitOfWork.Carts.GetBySessionIdAsync(sessionId, cancellationToken);
        }

        return cart == null ? null : MapToCartDto(cart);
    }

    public async Task<CartDto> AddToCartAsync(string? userId, string? sessionId, AddToCartDto dto, CancellationToken cancellationToken = default)
    {
        // Get or create cart
        var cart = await GetOrCreateCartAsync(userId, sessionId, cancellationToken);

        // Get product to validate and get current price
        // Use repository method to fetch with variants
        var product = await _unitOfWork.Products.GetByIdWithVariantsAsync(dto.ProductId, cancellationToken);
        
        if (product == null)
            throw new InvalidOperationException($"Product with ID {dto.ProductId} not found");

        if (!product.IsActive)
            throw new InvalidOperationException($"Product '{product.Name}' is not available");
            
        decimal price = product.Price;
        string? variantName = null;
        


        if (dto.VariantId.HasValue)
        {
             // If variants are not loaded, load them explicitly


             var variant = product.Variants.FirstOrDefault(v => v.Id == dto.VariantId.Value);
             
             if (variant == null)
                throw new InvalidOperationException($"Variant {dto.VariantId} not found");
             
             if (variant.Stock < dto.Quantity)
                throw new InvalidOperationException($"Insufficient stock for variant {variant.Name}");
             
             price = variant.Price;
             variantName = variant.Name;
        }
        else
        {
            if (product.Stock < dto.Quantity)
                throw new InvalidOperationException($"Insufficient stock for product {product.Name}");
        }
        
        cart.AddItem(product.Id, dto.VariantId, product.Name, variantName, price, dto.Quantity);

        // Save changes
        await _unitOfWork.Carts.UpdateAsync(cart, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToCartDto(cart);
    }

    public async Task<CartDto> UpdateCartItemAsync(string? userId, string? sessionId, int productId, UpdateCartItemDto dto, CancellationToken cancellationToken = default)
    {
        var cart = await GetOrCreateCartAsync(userId, sessionId, cancellationToken);

        // Update quantity using domain method
        cart.UpdateItemQuantity(productId, dto.Quantity);

        await _unitOfWork.Carts.UpdateAsync(cart, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToCartDto(cart);
    }

    public async Task<CartDto> RemoveFromCartAsync(string? userId, string? sessionId, int productId, CancellationToken cancellationToken = default)
    {
        var cart = await GetOrCreateCartAsync(userId, sessionId, cancellationToken);

        // Remove item using domain method
        cart.RemoveItem(productId);

        await _unitOfWork.Carts.UpdateAsync(cart, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToCartDto(cart);
    }

    public async Task ClearCartAsync(string? userId, string? sessionId, CancellationToken cancellationToken = default)
    {
        var cart = await GetOrCreateCartAsync(userId, sessionId, cancellationToken);

        // Clear cart using domain method
        cart.Clear();

        await _unitOfWork.Carts.UpdateAsync(cart, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<CartDto> MergeCartAsync(string userId, string sessionId, CancellationToken cancellationToken = default)
    {
        var guestCart = await _unitOfWork.Carts.GetBySessionIdAsync(sessionId, cancellationToken);
        if (guestCart == null)
             // If no guest cart, just return user cart
             return await GetCartAsync(userId, null, cancellationToken) ?? new CartDto();

        var userCart = await _unitOfWork.Carts.GetByUserIdAsync(userId, cancellationToken);
        
        if (userCart == null)
        {
            // Convert guest cart to user cart
            guestCart.ConvertToUserCart(userId);
            await _unitOfWork.Carts.UpdateAsync(guestCart, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return MapToCartDto(guestCart);
        }

        // Merge guest cart into user cart using domain method
        userCart.MergeWith(guestCart);

        await _unitOfWork.Carts.UpdateAsync(userCart, cancellationToken);
        await _unitOfWork.Carts.DeleteAsync(guestCart, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToCartDto(userCart);
    }

    // Private helper methods

    private async Task<Cart> GetOrCreateCartAsync(string? userId, string? sessionId, CancellationToken cancellationToken)
    {
        Cart? cart = null;

        if (!string.IsNullOrEmpty(userId))
        {
            cart = await _unitOfWork.Carts.GetByUserIdAsync(userId, cancellationToken);
        }
        else if (!string.IsNullOrEmpty(sessionId))
        {
            cart = await _unitOfWork.Carts.GetBySessionIdAsync(sessionId, cancellationToken);
        }

        if (cart == null)
        {
            // Create new cart
            if (!string.IsNullOrEmpty(userId))
            {
                cart = new Cart(userId);
            }
            else if (!string.IsNullOrEmpty(sessionId))
            {
                cart = Cart.CreateGuestCart(sessionId);
            }
            else
            {
                throw new InvalidOperationException("Either UserId or SessionId must be provided");
            }

            cart = await _unitOfWork.Carts.AddAsync(cart, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return cart;
    }

    private CartDto MapToCartDto(Cart cart)
    {
        return new CartDto
        {
            Id = cart.Id,
            UserId = cart.UserId,
            SessionId = cart.SessionId,
            Items = cart.Items.Select(item => new CartItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                ProductImageUrl = null, // Can be populated from Product if needed
                Price = item.Price,
                Quantity = item.Quantity,
                SubTotal = item.GetSubTotal(),
                MaxStock = 0 // Can be populated from Product if needed
            }).ToList(),
            ItemCount = cart.GetItemCount(),
            TotalAmount = cart.GetTotalAmount(),
            IsEmpty = cart.IsEmpty()
        };
    }
}
