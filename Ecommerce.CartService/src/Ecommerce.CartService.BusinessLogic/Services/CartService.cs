using AutoMapper;
using Ecommerce.CartService.BusinessLogic.Dtos;
using Ecommerce.CartService.BusinessLogic.Validators;
using Ecommerce.CartService.DataAccess.Entities;
using Ecommerce.CartService.DataAccess.Repositories;

namespace Ecommerce.CartService.BusinessLogic.Services
{

    public class CartService(
        IRepository<Cart> repository,
        ICreateValidator<CartDto> createValidator,
        IUpdateValidator<CartDto> updateValidator,
        IMapper mapper)
        : BaseService<Cart, CartDto>(repository, createValidator, updateValidator, mapper), ICartService
    {
        protected override string EntityName => "Cart";
    }
}
