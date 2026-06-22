using System.Collections.Generic;

namespace Ecommerce.CartService.BusinessLogic.Dtos;

public class CartDto : BaseDto
{

    public List<CartItemDto> Items { get; set; } = new();
}
