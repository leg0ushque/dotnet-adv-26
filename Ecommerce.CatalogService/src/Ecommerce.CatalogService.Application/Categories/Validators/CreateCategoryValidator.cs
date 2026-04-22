using Ecommerce.CatalogService.Application.Categories.DTOs;
using FluentValidation;

namespace Ecommerce.CatalogService.Application.Categories.Validators
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");

            RuleFor(x => x.ImageUrl)
                .Must(BeAValidUrl).When(x => !string.IsNullOrWhiteSpace(x.ImageUrl))
                .WithMessage("ImageUrl must be a valid URL.");
        }

        private bool BeAValidUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return true;

            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
