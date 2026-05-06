namespace Ecommerce.CartService.BusinessLogic.Dtos
{
    public class CartItemDto : BaseDto
    {
        public required string Name { get; set; }

        public Image? Image { get; set; }

        public decimal? Price { get; set; }

        public int Quantity { get; set; }
    }
}
