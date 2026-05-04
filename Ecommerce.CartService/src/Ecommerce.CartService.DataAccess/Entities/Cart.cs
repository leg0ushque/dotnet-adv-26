namespace Ecommerce.CartService.DataAccess.Entities
{
    public class Cart : BaseEntity
    {
        public required string Name { get; set; }

        public Image? Image { get; set; }

        public decimal? Price { get; set; }

        public int Quantity { get; set; }
    }
}
