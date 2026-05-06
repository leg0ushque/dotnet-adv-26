using FluentValidation;

namespace Ecommerce.CartService.BusinessLogic.Validators
{
    public interface ICreateValidator<TDto> : IValidator<TDto>
    {
    }
}
