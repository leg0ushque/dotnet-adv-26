namespace Ecommerce.CartService.DataAccess.Entities;

/// <summary>
/// Inline model for entities
/// </summary>
public class CartItem
{
    public required string ItemId { get; set; }

    public required string Name { get; set; }

    public Image? Image { get; set; }

    public decimal? Price { get; set; }

    public int Quantity { get; set; }
}
