using AutoMapper;
using Ecommerce.CartService.BusinessLogic.Dtos;
using Ecommerce.CartService.DataAccess.Entities;

namespace Ecommerce.CartService.BusinessLogic.Mappings
{
    public class ApplicationMappingProfile : Profile
    {
        public ApplicationMappingProfile()
        {
            CreateMap<Cart, CartDto>().ReverseMap();
            CreateMap<Image, ImageDto>().ReverseMap();
            CreateMap<CartItem, CartItemDto>().ReverseMap();
        }
    }
}
