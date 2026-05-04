using Ecommerce.CartService.BusinessLogic.Dtos;
using Ecommerce.CartService.DataAccess.Entities;

namespace Ecommerce.CartService.BusinessLogic.Mappings
{
    public class CartMappingService : IMappingService<Cart, CartDto>
    {
        public Cart Map(CartDto src) => new()
        {
            Id = src.Id,
            Name = src.Name,
            Image = Map(src.Image),
            Price = src.Price,
            Quantity = src.Quantity
        };

        public CartDto Map(Cart src) => new()
        {
            Id = src.Id,
            Name = src.Name,
            Image = Map(src.Image),
            Price = src.Price,
            Quantity = src.Quantity
        };

        private static Image? Map(ImageDto? src) => src == null ? null : new()
        {
            ImageUrl = src.ImageUrl,
            ImageAltText = src.ImageAltText,
        };

        private static ImageDto? Map(Image? src) => src == null ? null : new()
        {
            ImageUrl = src.ImageUrl,
            ImageAltText = src.ImageAltText,
        };
    }
}
