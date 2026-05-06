using AutoMapper;
using Ecommerce.CatalogService.Application.Categories.DTOs;
using Ecommerce.CatalogService.Application.Categories.Services;
using Ecommerce.CatalogService.Application.Common.Interfaces;
using Ecommerce.CatalogService.Domain.Entities;
using FluentValidation;
using Moq;

namespace Ecommerce.CatalogService.UnitTests.Categories
{
    public class CategoryServiceCascadeDeleteTests
    {
        private readonly Mock<IRepository<Category>> _mockCategoryRepository;
        private readonly Mock<IRepository<Product>> _mockProductRepository;
        private readonly Mock<IValidator<CreateCategoryDto>> _mockCreateValidator;
        private readonly Mock<IValidator<UpdateCategoryDto>> _mockUpdateValidator;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CategoryService _categoryService;

        public CategoryServiceCascadeDeleteTests()
        {
            _mockCategoryRepository = new Mock<IRepository<Category>>();
            _mockProductRepository = new Mock<IRepository<Product>>();
            _mockCreateValidator = new Mock<IValidator<CreateCategoryDto>>();
            _mockUpdateValidator = new Mock<IValidator<UpdateCategoryDto>>();
            _mockMapper = new Mock<IMapper>();

            _categoryService = new CategoryService(
                _mockCategoryRepository.Object,
                _mockProductRepository.Object,
                _mockCreateValidator.Object,
                _mockUpdateValidator.Object,
                _mockMapper.Object);
        }

        [Fact]
        public async Task DeleteCategoryWithProductsAsync_DeletesCategoryAndRelatedProducts()
        {
            // Arrange
            var categoryId = "cat1";
            var products = new List<Product>
            {
                new Product("prod1", "Product 1", categoryId, 10.0m, 5),
                new Product("prod2", "Product 2", categoryId, 20.0m, 10)
            };

            var category = new Category(categoryId, "Electronics", null, null);

            _mockProductRepository
                .Setup(r => r.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>()))
                .ReturnsAsync(products);

            _mockCategoryRepository
                .Setup(r => r.GetByIdAsync(categoryId))
                .ReturnsAsync(category);

            // Act
            await _categoryService.DeleteCategoryWithProductsAsync(categoryId);

            // Assert
            _mockProductRepository.Verify(r => r.DeleteByIdAsync("prod1"), Times.Once);
            _mockProductRepository.Verify(r => r.DeleteByIdAsync("prod2"), Times.Once);
            _mockCategoryRepository.Verify(r => r.DeleteByIdAsync(categoryId), Times.Once);
        }

        [Fact]
        public async Task DeleteCategoryWithProductsAsync_WithNoProducts_DeletesOnlyCategory()
        {
            // Arrange
            var categoryId = "cat1";
            var products = new List<Product>();

            var category = new Category(categoryId, "Electronics", null, null);

            _mockProductRepository
                .Setup(r => r.GetAllAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>()))
                .ReturnsAsync(products);

            _mockCategoryRepository
                .Setup(r => r.GetByIdAsync(categoryId))
                .ReturnsAsync(category);

            // Act
            await _categoryService.DeleteCategoryWithProductsAsync(categoryId);

            // Assert
            _mockProductRepository.Verify(r => r.DeleteByIdAsync(It.IsAny<string>()), Times.Never);
            _mockCategoryRepository.Verify(r => r.DeleteByIdAsync(categoryId), Times.Once);
        }
    }
}
