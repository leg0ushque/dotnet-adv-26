using System.Globalization;
using System.Text.Json;
using AutoMapper;
using Ecommerce.CatalogService.Application.Common.Interfaces;
using Ecommerce.CatalogService.Application.Products.DTOs;
using Ecommerce.CatalogService.Application.Products.Services;
using Ecommerce.CatalogService.Domain.Entities;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using Moq;

namespace Ecommerce.CatalogService.UnitTests.Products;

public class ProductServiceTests
{
    private readonly Mock<IRepository<Product>> _mockRepository;
    private readonly Mock<IValidator<CreateProductDto>> _mockCreateValidator;
    private readonly Mock<IValidator<UpdateProductDto>> _mockUpdateValidator;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ITransactionManager> _mockTransactionManager;
    private readonly Mock<IOutboxService> _mockOutboxService;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _mockRepository = new Mock<IRepository<Product>>();
        _mockCreateValidator = new Mock<IValidator<CreateProductDto>>();
        _mockUpdateValidator = new Mock<IValidator<UpdateProductDto>>();
        _mockMapper = new Mock<IMapper>();
        _mockTransactionManager = new Mock<ITransactionManager>();
        _mockOutboxService = new Mock<IOutboxService>();

        var mockOptions = new Mock<IOptions<JsonSerializerOptions>>();
        mockOptions.Setup(o => o.Value).Returns(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
        });

        _productService = new ProductService(
            _mockRepository.Object,
            _mockCreateValidator.Object,
            _mockUpdateValidator.Object,
            _mockMapper.Object,
            _mockTransactionManager.Object,
            _mockOutboxService.Object,
            mockOptions.Object);
    }

    [Fact]
    public async Task GetProductsAsync_WithoutFilter_ReturnsAllProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new("1", "Product 1", "cat1", 10.0m, 5),
            new("2", "Product 2", "cat2", 20.0m, 10)
        };

        var productDtos = new List<ProductDto>
        {
            new() { Id = "1", Name = "Product 1", CategoryId = "cat1", Price = 10.0m, Amount = 5 },
            new() { Id = "2", Name = "Product 2", CategoryId = "cat2", Price = 20.0m, Amount = 10 }
        };

        _mockRepository.Setup(r => r.GetAllAsync(null)).ReturnsAsync(products);
        _mockMapper.Setup(m => m.Map<List<ProductDto>>(It.IsAny<List<Product>>())).Returns(productDtos);

        // Act
        var result = await _productService.GetProductsAsync(null, 1, 10);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task GetProductsAsync_WithCategoryFilter_ReturnsFilteredProducts()
    {
        // Arrange
        var categoryId = "cat1";
        var products = new List<Product>
        {
            new("1", "Product 1", "cat1", 10.0m, 5)
        };

        var productDtos = new List<ProductDto>
        {
            new() { Id = "1", Name = "Product 1", CategoryId = "cat1", Price = 10.0m, Amount = 5 }
        };

        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>()))
            .ReturnsAsync(products);
        _mockMapper.Setup(m => m.Map<List<ProductDto>>(It.IsAny<List<Product>>())).Returns(productDtos);

        // Act
        var result = await _productService.GetProductsAsync(categoryId, 1, 10);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().CategoryId.Should().Be(categoryId);
    }

    [Fact]
    public async Task GetProductsAsync_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var products = new List<Product>();
        for (int i = 1; i <= 25; i++)
        {
            products.Add(new Product(i.ToString(CultureInfo.InvariantCulture), $"Product {i}", "cat1", 10.0m * i, i));
        }

        var productDtos = products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            CategoryId = p.CategoryId,
            Price = p.Price,
            Amount = p.Amount
        }).Skip(10).Take(10).ToList();

        _mockRepository.Setup(r => r.GetAllAsync(null)).ReturnsAsync(products);

        _mockMapper
            .Setup(m => m.Map<List<ProductDto>>(It.IsAny<List<Product>>()))
            .Returns((List<Product> src) => [.. src.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                CategoryId = p.CategoryId,
                Price = p.Price,
                Amount = p.Amount
            })]);

        // Act
        var result = await _productService.GetProductsAsync(null, 2, 10);

        // Assert
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(25);
        result.PageNumber.Should().Be(2);
        result.PageSize.Should().Be(10);
        result.Items.Should().HaveCount(10);
        result.TotalPages.Should().Be(3);
        result.HasNext.Should().BeTrue();
        result.HasPrevious.Should().BeTrue();
    }
}
