using Ecommerce.CartService.BusinessLogic.Dtos;
using FluentValidation;

namespace Ecommerce.CartService.BusinessLogic.Validators
{
    public class CartDtoValidator : AbstractValidator<CartDto>
    {
        public const decimal MinPrice = 0m;
        public const int MinQuantity = 0;

        public CartDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.");

            RuleFor(x => x.Price)
                .GreaterThan(MinPrice).When(x => x.Price.HasValue)
                .WithMessage("Price must be non-negative.");

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(MinQuantity)
                .NotEmpty().WithMessage("Quantity must be non-negative.");
        }
    }
}
