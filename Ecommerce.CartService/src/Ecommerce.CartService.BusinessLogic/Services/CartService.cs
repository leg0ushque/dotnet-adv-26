using Ecommerce.CartService.BusinessLogic.Dtos;
using Ecommerce.CartService.BusinessLogic.Mappings;
using Ecommerce.CartService.DataAccess.Entities;
using Ecommerce.CartService.DataAccess.Repositories;
using FluentValidation;

namespace Ecommerce.CartService.BusinessLogic.Services
{
    public class CartService(
        IRepository<Cart> repository,
        IValidator<CartDto> validator,
        IMappingService<Cart, CartDto> mapper)
        : BaseService<Cart, CartDto>(repository, validator, mapper)
    {
    }
}
