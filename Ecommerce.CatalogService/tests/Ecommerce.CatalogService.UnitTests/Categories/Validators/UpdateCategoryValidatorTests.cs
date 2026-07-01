using Ecommerce.CatalogService.Application.Categories.DTOs;
using Ecommerce.CatalogService.Application.Categories.Validators;
using FluentAssertions;

namespace Ecommerce.CatalogService.UnitTests.Categories.Validators;

public class UpdateCategoryValidatorTests
{
    private readonly UpdateCategoryValidator _validator;

    public UpdateCategoryValidatorTests()
    {
        _validator = new UpdateCategoryValidator();
    }

    [Fact]
    public void Validate_WithValidData_PassesValidation()
    {
        var dto = new UpdateCategoryDto
        {
            Name = "Updated Electronics",
            ImageUrl = "https://example.com/updated.jpg",
            ParentCategoryId = "parent-id"
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithEmptyName_FailsValidation()
    {
        var dto = new UpdateCategoryDto
        {
            Name = "",
            ImageUrl = null,
            ParentCategoryId = null
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Name");
        result.Errors[0].ErrorMessage.Should().Be("Name is required.");
    }

    [Fact]
    public void Validate_WithNameExceeding50Characters_FailsValidation()
    {
        var dto = new UpdateCategoryDto
        {
            Name = new string('B', 51),
            ImageUrl = null,
            ParentCategoryId = null
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Name");
        result.Errors[0].ErrorMessage.Should().Be("Name must not exceed 50 characters.");
    }

    [Fact]
    public void Validate_WithInvalidUrl_FailsValidation()
    {
        var dto = new UpdateCategoryDto
        {
            Name = "Electronics",
            ImageUrl = "invalid-url",
            ParentCategoryId = null
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "ImageUrl");
        result.Errors[0].ErrorMessage.Should().Be("ImageUrl must be a valid URL.");
    }

    [Fact]
    public void Validate_WithNullImageUrl_PassesValidation()
    {
        var dto = new UpdateCategoryDto
        {
            Name = "Electronics",
            ImageUrl = null,
            ParentCategoryId = null
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithValidHttpsUrl_PassesValidation()
    {
        var dto = new UpdateCategoryDto
        {
            Name = "Electronics",
            ImageUrl = "https://cdn.example.com/images/electronics.png",
            ParentCategoryId = null
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
    }
}
