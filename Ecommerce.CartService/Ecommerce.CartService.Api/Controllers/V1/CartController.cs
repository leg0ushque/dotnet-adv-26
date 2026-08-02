using Asp.Versioning;
using Ecommerce.CartService.BusinessLogic.Dtos;
using Ecommerce.CartService.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.CartService.Api.Controllers.V1;



/// <summary>
/// Cart API v1 - manage carts and items.
/// </summary>
[ApiController]
[ApiVersion(Constants.V1)]
[ApiVersion(Constants.V2)]
[Route("api/v{version:apiVersion}/cart")]
public class CartController(ICartService cartService) : ControllerBase
{
    private readonly ICartService _cartService = cartService;

    /// <summary>
    /// Get cart info by cart key (v1: returns cart model with items, v2: returns only items).
    /// </summary>
    /// <param name="cartKey">Cart unique key</param>
    /// <returns>Cart model (v1) or list of items (v2)</returns>
    [HttpGet("{cartKey}")]
    [MapToApiVersion(Constants.V1)]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = Constants.AuthConstants.StoreCustomerManagerOnlyPolicy)]
    public async Task<IActionResult> GetCartV1(string cartKey)
    {
        var result = await _cartService.GetCartByKeyAsync(cartKey);
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get cart items by cart key (v2: returns only items).
    /// </summary>
    /// <param name="cartKey">Cart unique key</param>
    /// <returns>List of cart items</returns>
    [HttpGet("{cartKey}")]
    [MapToApiVersion(Constants.V2)]
    [ProducesResponseType(typeof(List<CartItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = Constants.AuthConstants.StoreCustomerManagerOnlyPolicy)]
    public async Task<IActionResult> GetCartV2(string cartKey)
    {
        var result = await _cartService.GetCartByKeyAsync(cartKey);
        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value.Items);
    }

    /// <summary>
    /// Add item to cart. Creates cart if not exists.
    /// </summary>
    /// <param name="cartKey">Cart unique key</param>
    /// <param name="item">Cart item model</param>
    [HttpPost("{cartKey}/item")]
    [MapToApiVersion(Constants.V1)]
    [MapToApiVersion(Constants.V2)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Policy = Constants.AuthConstants.StoreCustomerManagerOnlyPolicy)]
    public async Task<IActionResult> AddItemToCart(string cartKey, [FromBody] CartItemDto item)
    {
        var result = await _cartService.AddItemToCartAsync(cartKey, item);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    /// <summary>
    /// Delete item from cart by item id.
    /// </summary>
    /// <param name="cartKey">Cart unique key</param>
    /// <param name="itemId">Item id (ProductId)</param>
    [HttpDelete("{cartKey}/items/{itemId}")]
    [MapToApiVersion(Constants.V1)]
    [MapToApiVersion(Constants.V2)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = Constants.AuthConstants.StoreCustomerManagerOnlyPolicy)]
    public async Task<IActionResult> DeleteItemFromCart(string cartKey, string itemId)
    {
        var result = await _cartService.DeleteItemFromCartAsync(cartKey, itemId);

        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }

        return Ok();
    }
}
