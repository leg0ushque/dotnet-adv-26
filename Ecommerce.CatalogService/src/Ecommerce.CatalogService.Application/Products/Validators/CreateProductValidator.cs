using Ecommerce.CatalogService.Application.Products.DTOs;
using Ecommerce.CatalogService.Domain;
using FluentValidation;

namespace Ecommerce.CatalogService.Application.Products.Validators;

public class CreateProductValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(Constants.MaxCategoryNameLength)
            .WithMessage($"Name must not exceed {Constants.MaxCategoryNameLength} characters.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("CategoryId is required.");

        RuleFor(x => x.Price)
            .GreaterThan(Constants.MinProductPrice).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.Amount)
            .GreaterThan(Constants.MinProductAmount).WithMessage("Amount must be a positive integer.");

        RuleFor(x => x.ImageUrl)
            .Must(BeAValidUrl).When(x => !string.IsNullOrWhiteSpace(x.ImageUrl))
            .WithMessage("ImageUrl must be a valid URL.");
    }

    private bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return true;
        }

        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
