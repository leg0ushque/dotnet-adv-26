using AutoMapper;
using Ecommerce.CatalogService.Application.Categories.DTOs;
using Ecommerce.CatalogService.Application.Categories.Services;
using Ecommerce.CatalogService.Application.Common.Interfaces;
using Ecommerce.CatalogService.Domain.Entities;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace Ecommerce.CatalogService.UnitTests.Categories
{
    public class CategoryServiceTests
    {
        private readonly Mock<IValidator<CreateCategoryDto>> _createValidator;
        private readonly Mock<IValidator<UpdateCategoryDto>> _updateValidator;
        private readonly Mock<IRepository<Category>> _mockCategoryRepository;
        private readonly Mock<IRepository<Product>> _mockProductRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _mockCategoryRepository = new Mock<IRepository<Category>>();
            _mockProductRepository = new Mock<IRepository<Product>>();
            _mockMapper = new Mock<IMapper>();
            _createValidator = new();
            _updateValidator = new();

            _mockMapper.Setup(x => x.Map<CategoryDto>(It.IsAny<Category>()))
                .Returns((Category source) => new CategoryDto
                {
                    Id = source.Id,
                    Name = source.Name,
                    ImageUrl = source.ImageUrl,
                    ParentCategoryId = source.ParentCategoryId
                });

            _mockMapper.Setup(x => x.Map<Category>(It.IsAny<CategoryDto>()))
                .Returns((CategoryDto source) => new Category(
                    Guid.NewGuid().ToString(), 
                    source.Name,
                    source.ImageUrl, 
                    source.ParentCategoryId));

            _mockMapper.Setup(x => x.Map<Category>(It.IsAny<CreateCategoryDto>()))
                .Returns((CreateCategoryDto source) => new Category(
                    Guid.NewGuid().ToString(),
                    source.Name,
                    source.ImageUrl,
                    source.ParentCategoryId));

            _mockMapper.Setup(x => x.Map<Category>(It.IsAny<UpdateCategoryDto>()))
                .Returns((CreateCategoryDto source) => new Category(
                    Guid.NewGuid().ToString(),
                    source.Name,
                    source.ImageUrl,
                    source.ParentCategoryId));

            _createValidator.Setup(v => v.ValidateAsync(It.IsAny<CreateCategoryDto>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _updateValidator.Setup(v => v.ValidateAsync(It.IsAny<UpdateCategoryDto>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _service = new CategoryService(_mockCategoryRepository.Object, _mockProductRepository.Object, _createValidator.Object, _updateValidator.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCategoryExists_ReturnsCategoryDto()
        {
            var categoryId = "cat-123";
            var category = new Category(categoryId, "Electronics", "https://example.com/image.jpg", null);

            _mockCategoryRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(category);

            var result = await _service.GetByIdAsync(categoryId);

            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().Be(categoryId);
            result.Value.Name.Should().Be("Electronics");
            result.Value.ImageUrl.Should().Be("https://example.com/image.jpg");
            result.Value.ParentCategoryId.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_WhenCategoryDoesNotExist_ReturnsFailure()
        {
            var categoryId = "nonexistent-id";

            _mockCategoryRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Category?)null);

            var result = await _service.GetByIdAsync(categoryId);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllCategories()
        {
            var categories = new List<Category>
            {
                new ("cat-1", "Electronics", "https://example.com/1.jpg", null),
                new ("cat-2", "Books", "https://example.com/2.jpg", null),
                new ("cat-3", "Smartphones", null, "cat-1")
            };

            _mockCategoryRepository
                .Setup(r => r.GetAllAsync(null))
                .ReturnsAsync(categories);

            var result = await _service.GetAllAsync();

            result.Should().HaveCount(3);
            result[0].Name.Should().Be("Electronics");
            result[1].Name.Should().Be("Books");
            result[2].Name.Should().Be("Smartphones");
            result[2].ParentCategoryId.Should().Be("cat-1");
        }
        [Fact]
        public async Task GetAllAsync_WhenNoCategories_ReturnsEmptyList()
        {
            _mockCategoryRepository
                .Setup(r => r.GetAllAsync(null))
                .ReturnsAsync([]);

            var result = await _service.GetAllAsync();

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithValidData_CreatesCategory()
        {
            var dto = new CreateCategoryDto
            {
                Name = "Electronics",
                ImageUrl = "https://example.com/image.jpg",
                ParentCategoryId = null
            };

            var createdId = "new-cat-id";

            _mockCategoryRepository
                .Setup(r => r.CreateAsync(It.IsAny<Category>()))
                .ReturnsAsync(createdId);

            var result = await _service.CreateAsync(dto);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(createdId);

            _mockCategoryRepository.Verify(r => r.CreateAsync(It.Is<Category>(c =>
                c.Name == dto.Name &&
                c.ImageUrl == dto.ImageUrl &&
                c.ParentCategoryId == dto.ParentCategoryId
            )), Times.Once);

            _createValidator.Verify(v => v.ValidateAsync(
                It.Is<CreateCategoryDto>(d => 
                d.Name == dto.Name 
                && d.ImageUrl == dto.ImageUrl 
                && d.ParentCategoryId == dto.ParentCategoryId), default), 
                Times.Once());
        }

        [Fact]
        public async Task CreateAsync_WithParentCategory_CreatesSubCategory()
        {
            var dto = new CreateCategoryDto
            {
                Name = "Smartphones",
                ImageUrl = "https://example.com/smartphones.jpg",
                ParentCategoryId = "parent-cat-id"
            };

            var createdId = "new-subcategory-id";

            _mockCategoryRepository
                .Setup(r => r.CreateAsync(It.IsAny<Category>()))
                .ReturnsAsync(createdId);

            var result = await _service.CreateAsync(dto);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(createdId);

            _mockCategoryRepository.Verify(r => r.CreateAsync(It.Is<Category>(c =>
                c.ParentCategoryId == "parent-cat-id"
            )), Times.Once);

            _createValidator.Verify(v => v.ValidateAsync(
                It.Is<CreateCategoryDto>(d =>
                d.Name == dto.Name
                && d.ImageUrl == dto.ImageUrl
                && d.ParentCategoryId == dto.ParentCategoryId), default),
                Times.Once());
        }

        [Fact]
        public async Task UpdateAsync_WhenCategoryExists_UpdatesCategory()
        {
            var categoryId = "cat-123";
            var existingCategory = new Category(categoryId, "Old Name", null, null);

            var updateDto = new UpdateCategoryDto
            {
                Name = "Updated Name",
                ImageUrl = "https://example.com/updated.jpg",
                ParentCategoryId = "parent-id"
            };

            _mockCategoryRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(existingCategory);

            _mockCategoryRepository
                .Setup(r => r.UpdateAsync(It.IsAny<Category>()))
                .Returns(Task.CompletedTask);

            var result = await _service.UpdateAsync(categoryId, updateDto);

            result.IsSuccess.Should().BeTrue();

            _mockCategoryRepository.Verify(r => r.UpdateAsync(It.Is<Category>(c =>
                c.Id == categoryId &&
                c.Name == updateDto.Name &&
                c.ImageUrl == updateDto.ImageUrl &&
                c.ParentCategoryId == updateDto.ParentCategoryId
            )), Times.Once);

            _updateValidator.Verify(v => v.ValidateAsync(
                It.Is<UpdateCategoryDto>(d =>
                d.Name == updateDto.Name
                && d.ImageUrl == updateDto.ImageUrl
                && d.ParentCategoryId == updateDto.ParentCategoryId), default),
                Times.Once());
        }

        [Fact]
        public async Task UpdateAsync_WhenCategoryDoesNotExist_ReturnsFailure()
        {
            var categoryId = "nonexistent-id";
            var updateDto = new UpdateCategoryDto
            {
                Name = "Updated Name",
                ImageUrl = null,
                ParentCategoryId = null
            };

            _mockCategoryRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Category?)null);

            var result = await _service.UpdateAsync(categoryId, updateDto);

            result.IsFailure.Should().BeTrue();
            result.Error!.Type.Should().Be(Ecommerce.CatalogService.Application.Common.Results.ErrorType.NotFound);

            _mockCategoryRepository.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_CallsRepositoryDelete()
        {
            var categoryId = "cat-to-delete";
            var category = new Category(categoryId, "Test", null, null);

            _mockCategoryRepository
                .Setup(r => r.GetByIdAsync(categoryId))
                .ReturnsAsync(category);

            _mockCategoryRepository
                .Setup(r => r.DeleteByIdAsync(categoryId))
                .Returns(Task.CompletedTask);

            var result = await _service.DeleteAsync(categoryId);

            result.IsSuccess.Should().BeTrue();
            _mockCategoryRepository.Verify(r => r.DeleteByIdAsync(categoryId), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_GeneratesNewGuidForId()
        {
            var dto = new CreateCategoryDto
            {
                Name = "Test Category",
                ImageUrl = null,
                ParentCategoryId = null
            };

            _mockCategoryRepository
                .Setup(r => r.CreateAsync(It.IsAny<Category>()))
                .ReturnsAsync("returned-id");

            var result = await _service.CreateAsync(dto);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNullOrWhiteSpace();
        }
    }
}
