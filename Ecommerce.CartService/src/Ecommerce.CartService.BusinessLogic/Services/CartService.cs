using AutoMapper;
using Ecommerce.CartService.BusinessLogic.Dtos;
using Ecommerce.CartService.BusinessLogic.Results;
using Ecommerce.CartService.BusinessLogic.Validators;
using Ecommerce.CartService.DataAccess.Entities;
using Ecommerce.CartService.DataAccess.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.CartService.BusinessLogic.Services
{
    public class CartService(
        IRepository<Cart> repository,
        ICreateValidator<CartDto> createCartValidator,
        IUpdateValidator<CartDto> updateCartValidator,
        ICreateValidator<CartItemDto> createCartItemValidator,
        IMapper mapper) : BaseService<Cart, CartDto>(repository, createCartValidator, updateCartValidator, mapper), ICartService
    {
        private readonly ICreateValidator<CartItemDto> _createCartItemValidator = createCartItemValidator;

        protected override string EntityName => "Cart";

        public async Task<Result> AddItemToCartAsync(string cartKey, CartItemDto item)
        {
            var cart = await _repository.GetByIdAsync(cartKey);

            if (cart == null)
            {
                var validationResult = await _createCartItemValidator.ValidateAsync(item);

                if (!validationResult.IsValid)
                {
                    var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return Result.Failure(Error.Validation("Validation.Failed", errors));
                }

                cart = new Cart
                {
                    Id = cartKey,
                    Items = [ _mapper.Map<CartItem>(item) ]
                };

                await _repository.CreateAsync(cart);

                return Result.Success();
            }

            var existingItem = cart.Items.SingleOrDefault(i => i.ItemId == item.ItemId);

            if (existingItem != null)
            {
                return Result.Failure(Error.Validation("Cart.ItemExists", $"Item with id '{item.ItemId}' already exists in cart."));
            }

            var itemValidationResult = await _createCartItemValidator.ValidateAsync(item);
            if (!itemValidationResult.IsValid)
            {
                var errors = string.Join("; ", itemValidationResult.Errors.Select(e => e.ErrorMessage));

                return Result.Failure(Error.Validation("Validation.Failed", errors));
            }

            cart.Items.Add(_mapper.Map<CartItem>(item));

            await _repository.UpdateAsync(cartKey, c => c.Items, cart.Items);

            return Result.Success();
        }

        public async Task<Result> DeleteItemFromCartAsync(string cartKey, string itemId)
        {
            var cart = await _repository.GetByIdAsync(cartKey);

            if (cart == null)
                return Result.Failure(Error.NotFound("Cart", cartKey));

            var existingItem = cart.Items.SingleOrDefault(i => i.ItemId == itemId);

            if (existingItem == null)
                return Result.Failure(Error.NotFound("CartItem", itemId.ToString()));

            cart.Items.Remove(existingItem);

            await _repository.UpdateAsync(cartKey, c => c.Items, cart.Items);

            return Result.Success();
        }

        public async Task<Result<CartDto>> GetCartByKeyAsync(string cartKey)
        {
            var cart = await _repository.GetByIdAsync(cartKey);

            if (cart == null)
                return Result.Failure<CartDto>(Error.NotFound("Cart", cartKey));

            return Result.Success(_mapper.Map<CartDto>(cart));
        }
    }
}
