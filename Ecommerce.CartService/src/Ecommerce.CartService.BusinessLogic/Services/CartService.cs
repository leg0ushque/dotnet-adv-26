using AutoMapper;
using Ecommerce.CartService.BusinessLogic.Dtos;
using Ecommerce.CartService.BusinessLogic.Results;
using Ecommerce.CartService.BusinessLogic.Validators;
using Ecommerce.CartService.DataAccess.Entities;
using Ecommerce.CartService.DataAccess.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<Result> AddItemToCartAsync(string cartKey, CartItemDto item)
        {
            var cart = await repository.GetByIdAsync(cartKey);

            if (cart == null)
            {
                cart = new Cart
                {
                    Id = cartKey,
                    Items = [ mapper.Map<CartItem>(item) ]
                };

                await repository.CreateAsync(cart);

                return Result.Success();
            }

            var existingItem = cart.Items.SingleOrDefault(i => i.Id == item.Id);

            if (existingItem == null)
            {
                cart.Items.Add(mapper.Map<CartItem>(item));
            }
            else
            {
                existingItem?.Quantity += item.Quantity;
            }

            await repository.UpdateAsync(cartKey, c => c.Items, cart.Items);

            return Result.Success();
        }

        public async Task<Result> DeleteItemFromCartAsync(string cartKey, string itemId, int? amount = null)
        {
            if(amount < default(int))
                return Result.Failure(Error.Validation(string.Empty, "The amount of items to delete must be positive"));

            var cart = await repository.GetByIdAsync(cartKey);

            if (cart == null)
                return Result.Failure(Error.NotFound("Cart", cartKey));

            var existingItem = cart.Items.SingleOrDefault(i => i.Id == itemId);

            if (existingItem == null)
                return Result.Failure(Error.NotFound("CartItem", itemId.ToString()));

            if (amount.HasValue)
            {
                existingItem.Quantity -= amount.Value;

                if (existingItem.Quantity <= default(int))
                {
                    cart.Items.Remove(existingItem);
                }
            }
            else
            {
                cart.Items.Remove(existingItem);
            }

            await repository.UpdateAsync(cartKey, c => c.Items, cart.Items);

            return Result.Success();
        }

        public async Task<Result<CartDto>> GetCartByKeyAsync(string cartKey)
        {
            var cart = await repository.GetByIdAsync(cartKey);

            if (cart == null)
                return Result.Failure<CartDto>(Error.NotFound("Cart", cartKey));

            return Result.Success(mapper.Map<CartDto>(cart));
        }
    }
}
