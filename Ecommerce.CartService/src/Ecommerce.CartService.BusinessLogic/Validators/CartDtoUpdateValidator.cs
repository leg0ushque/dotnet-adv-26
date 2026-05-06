using Ecommerce.CartService.BusinessLogic.Dtos;
using FluentValidation;

namespace Ecommerce.CartService.BusinessLogic.Validators
{
    public class CartDtoUpdateValidator : AbstractValidator<CartDto>, IUpdateValidator<CartDto>
    {
        public const decimal MinPrice = default;
        public const int MinQuantity = default;

        public CartDtoUpdateValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");
        }
    }
}
