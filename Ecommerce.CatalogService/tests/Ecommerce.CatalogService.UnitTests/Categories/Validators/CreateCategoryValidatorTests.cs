using Ecommerce.CatalogService.Application.Categories.DTOs;
using Ecommerce.CatalogService.Application.Categories.Validators;
using FluentAssertions;

namespace Ecommerce.CatalogService.UnitTests.Categories.Validators;

public class CreateCategoryValidatorTests
{
    private readonly CreateCategoryValidator _validator;

    public CreateCategoryValidatorTests()
    {
        _validator = new CreateCategoryValidator();
    }

    [Fact]
    public void Validate_WithValidData_PassesValidation()
    {
        var dto = new CreateCategoryDto
        {
            Name = "Electronics",
            ImageUrl = "https://example.com/image.jpg",
            ParentCategoryId = null
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithEmptyName_FailsValidation()
    {
        var dto = new CreateCategoryDto
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
        var dto = new CreateCategoryDto
        {
            Name = new string('A', 51),
            ImageUrl = null,
            ParentCategoryId = null
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Name");
        result.Errors[0].ErrorMessage.Should().Be("Name must not exceed 50 characters.");
    }

    [Fact]
    public void Validate_WithNameExactly50Characters_PassesValidation()
    {
        var dto = new CreateCategoryDto
        {
            Name = new string('A', 50),
            ImageUrl = null,
            ParentCategoryId = null
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithNullImageUrl_PassesValidation()
    {
        var dto = new CreateCategoryDto
        {
            Name = "Books",
            ImageUrl = null,
            ParentCategoryId = null
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithValidHttpsUrl_PassesValidation()
    {
        var dto = new CreateCategoryDto
        {
            Name = "Electronics",
            ImageUrl = "https://example.com/image.jpg",
            ParentCategoryId = null
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithValidHttpUrl_PassesValidation()
    {
        var dto = new CreateCategoryDto
        {
            Name = "Electronics",
            ImageUrl = "http://example.com/image.jpg",
            ParentCategoryId = null
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithInvalidUrl_FailsValidation()
    {
        var dto = new CreateCategoryDto
        {
            Name = "Electronics",
            ImageUrl = "not-a-valid-url",
            ParentCategoryId = null
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "ImageUrl");
        result.Errors[0].ErrorMessage.Should().Be("ImageUrl must be a valid URL.");
    }

    [Fact]
    public void Validate_WithFtpUrl_FailsValidation()
    {
        var dto = new CreateCategoryDto
        {
            Name = "Electronics",
            ImageUrl = "ftp://example.com/image.jpg",
            ParentCategoryId = null
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "ImageUrl");
    }

    [Fact]
    public void Validate_WithParentCategoryId_PassesValidation()
    {
        var dto = new CreateCategoryDto
        {
            Name = "Smartphones",
            ImageUrl = null,
            ParentCategoryId = "parent-category-id"
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
    }
}
