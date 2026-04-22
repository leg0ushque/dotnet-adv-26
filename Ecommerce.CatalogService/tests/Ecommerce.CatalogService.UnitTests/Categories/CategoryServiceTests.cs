using AutoMapper;
using Ecommerce.CatalogService.Application.Categories.DTOs;
using Ecommerce.CatalogService.Application.Categories.Services;
using Ecommerce.CatalogService.Application.Common.Interfaces;
using Ecommerce.CatalogService.Domain.Entities;
using Ecommerce.CatalogService.Domain.Exceptions;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace Ecommerce.CatalogService.UnitTests.Categories
{
    public class CategoryServiceTests
    {
        private readonly Mock<IRepository<Category>> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _mockRepository = new Mock<IRepository<Category>>();
            _mockMapper = new Mock<IMapper>();

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

            _service = new CategoryService(_mockRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCategoryExists_ReturnsCategoryDto()
        {
            var categoryId = "cat-123";
            var category = new Category(categoryId, "Electronics", "https://example.com/image.jpg", null);

            _mockRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(category);

            var result = await _service.GetByIdAsync(categoryId);

            result.Should().NotBeNull();
            result.Id.Should().Be(categoryId);
            result.Name.Should().Be("Electronics");
            result.ImageUrl.Should().Be("https://example.com/image.jpg");
            result.ParentCategoryId.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_WhenCategoryDoesNotExist_ReturnsNull()
        {
            var categoryId = "nonexistent-id";

            _mockRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Category?)null);

            var entity = await _service.GetByIdAsync(categoryId);

            entity.Should().BeNull();
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

            _mockRepository
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
            _mockRepository
                .Setup(r => r.GetAllAsync(null))
                .ReturnsAsync(new List<Category>());

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

            _mockRepository
                .Setup(r => r.CreateAsync(It.IsAny<Category>()))
                .ReturnsAsync(createdId);

            var result = await _service.CreateAsync(dto);

            result.Should().Be(createdId);

            _mockRepository.Verify(r => r.CreateAsync(It.Is<Category>(c =>
                c.Name == dto.Name &&
                c.ImageUrl == dto.ImageUrl &&
                c.ParentCategoryId == dto.ParentCategoryId
            )), Times.Once);
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

            _mockRepository
                .Setup(r => r.CreateAsync(It.IsAny<Category>()))
                .ReturnsAsync(createdId);

            var result = await _service.CreateAsync(dto);

            result.Should().Be(createdId);

            _mockRepository.Verify(r => r.CreateAsync(It.Is<Category>(c =>
                c.ParentCategoryId == "parent-cat-id"
            )), Times.Once);
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

            _mockRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(existingCategory);

            _mockRepository
                .Setup(r => r.UpdateAsync(It.IsAny<Category>()))
                .Returns(Task.CompletedTask);

            await _service.UpdateAsync(categoryId, updateDto);

            _mockRepository.Verify(r => r.UpdateAsync(It.Is<Category>(c =>
                c.Id == categoryId &&
                c.Name == updateDto.Name &&
                c.ImageUrl == updateDto.ImageUrl &&
                c.ParentCategoryId == updateDto.ParentCategoryId
            )), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenCategoryDoesNotExist_ThrowsEntityNotFoundException()
        {
            var categoryId = "nonexistent-id";
            var updateDto = new UpdateCategoryDto
            {
                Name = "Updated Name",
                ImageUrl = null,
                ParentCategoryId = null
            };

            _mockRepository
                .Setup(r => r.GetSingleAsync(It.IsAny<Expression<Func<Category, bool>>>()))
                .ReturnsAsync((Category?)null);

            var act = async () => await _service.UpdateAsync(categoryId, updateDto);

            await act.Should().ThrowAsync<EntityNotFoundException>();

            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_CallsRepositoryDelete()
        {
            var categoryId = "cat-to-delete";

            _mockRepository
                .Setup(r => r.DeleteByIdAsync(categoryId))
                .Returns(Task.CompletedTask);

            await _service.DeleteAsync(categoryId);

            _mockRepository.Verify(r => r.DeleteByIdAsync(categoryId), Times.Once);
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

            _mockRepository
                .Setup(r => r.CreateAsync(It.IsAny<Category>()))
                .ReturnsAsync("returned-id");

            var createdId = await _service.CreateAsync(dto);

            createdId.Should().NotBeNullOrWhiteSpace();
        }
    }
}
