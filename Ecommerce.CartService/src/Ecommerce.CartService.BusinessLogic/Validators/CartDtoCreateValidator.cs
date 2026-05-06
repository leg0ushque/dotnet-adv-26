using Ecommerce.CartService.BusinessLogic.Dtos;
using FluentValidation;

namespace Ecommerce.CartService.BusinessLogic.Validators
{
    public class CartDtoCreateValidator : AbstractValidator<CartDto>, ICreateValidator<CartDto>
    {
        public const decimal MinPrice = default;
        public const int MinQuantity = default;

        public CartDtoCreateValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.");

            /* some logic to verify no existing item with that name exists*/

            RuleFor(x => x.Price)
                .GreaterThan(MinPrice).When(x => x.Price.HasValue)
                .WithMessage("Price must be non-negative.");

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(MinQuantity)
                .NotEmpty().WithMessage("Quantity must be non-negative.");
        }
    }
}
