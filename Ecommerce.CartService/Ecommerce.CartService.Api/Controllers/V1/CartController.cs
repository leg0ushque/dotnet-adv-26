using Asp.Versioning;
using Ecommerce.CartService.BusinessLogic.Dtos;
using Ecommerce.CartService.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.CartService.Api.Controllers.V1
{
    /// <summary>
    /// Cart API v1 - manage carts and items.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/cart")]
    [Authorize(Policy = Constants.AuthConstants.StoreCustomerManagerOnlyPolicy)]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>
        /// Get cart info by cart key (v1: returns cart model with items, v2: returns only items).
        /// </summary>
        /// <param name="cartKey">Cart unique key</param>
        /// <returns>Cart model (v1) or list of items (v2)</returns>
        [HttpGet("{cartKey}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(CartDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCartV1(string cartKey)
        {
            var result = await _cartService.GetCartByKeyAsync(cartKey);
            if (!result.IsSuccess)
                return NotFound(result.Error);
            return Ok(result.Value);
        }

        /// <summary>
        /// Get cart items by cart key (v2: returns only items).
        /// </summary>
        /// <param name="cartKey">Cart unique key</param>
        /// <returns>List of cart items</returns>
        [HttpGet("{cartKey}")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(List<CartItemDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCartV2(string cartKey)
        {
            var result = await _cartService.GetCartByKeyAsync(cartKey);
            if (!result.IsSuccess)
                return NotFound(result.Error);
            return Ok(result.Value.Items);
        }

        /// <summary>
        /// Add item to cart. Creates cart if not exists.
        /// </summary>
        /// <param name="cartKey">Cart unique key</param>
        /// <param name="item">Cart item model</param>
        [HttpPost("{cartKey}/item")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddItemToCart(string cartKey, [FromBody] CartItemDto item)
        {
            var result = await _cartService.AddItemToCartAsync(cartKey, item);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok();
        }

        /// <summary>
        /// Delete item from cart by item id.
        /// </summary>
        /// <param name="cartKey">Cart unique key</param>
        /// <param name="itemId">Item id (ProductId)</param>
        [HttpDelete("{cartKey}/items/{itemId}")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteItemFromCart(string cartKey, string itemId)
        {
            var result = await _cartService.DeleteItemFromCartAsync(cartKey, itemId);

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok();
        }
    }
}