namespace Ecommerce.CartService.BusinessLogic.Dtos
{
    /// <summary>
    /// Inline model for DTOs
    /// </summary>
    public class CartItemDto 
    {
        public required string ItemId { get; set; }

        public required string Name { get; set; }

        public ImageDto? Image { get; set; }

        public decimal? Price { get; set; }

        public int Quantity { get; set; }
    }
}
