using Ecommerce.CartService.BusinessLogic.Dtos;
using FluentValidation;

namespace Ecommerce.CartService.BusinessLogic.Validators;

public class CartDtoCreateValidator : AbstractValidator<CartDto>, ICreateValidator<CartDto>
{
    public const decimal MinPrice = default;
    public const int MinQuantity = default;

    public CartDtoCreateValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}
