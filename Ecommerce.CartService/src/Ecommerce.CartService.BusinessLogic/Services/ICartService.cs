using Ecommerce.CartService.BusinessLogic.Dtos;
using Ecommerce.CartService.DataAccess.Entities;

namespace Ecommerce.CartService.BusinessLogic.Services
{
    public interface ICartService : IService<Cart, CartDto>
    { }
}
