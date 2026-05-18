using FluentValidation;

namespace Ecommerce.CartService.BusinessLogic.Validators
{
    public interface IUpdateValidator<TDto> : IValidator<TDto>
    {
    }
}
