namespace Ecommerce.CartService.BusinessLogic.Dtos
{
    public class CartDto : BaseDto
    {
        public required string Name { get; set; }

        public ImageDto? Image { get; set; }

        public decimal? Price { get; set; }

        public int Quantity { get; set; }
    }
}
