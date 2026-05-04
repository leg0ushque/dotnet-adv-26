using Ecommerce.CatalogService.Application.Categories.DTOs;
using Ecommerce.CatalogService.Application.Categories.Services;
using Ecommerce.CatalogService.Application.Categories.Validators;
using Ecommerce.CatalogService.Domain.Entities;
using Ecommerce.CatalogService.Domain.Exceptions;
using Ecommerce.CatalogService.IntegrationTests.Fixtures;
using Ecommerce.CatalogService.Persistence.Repositories;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.CatalogService.IntegrationTests.Categories
{
    public class CategoryIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;
        private readonly CategoryService _categoryService;

        public CategoryIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _fixture.ClearDatabase();

            var repository = new GenericRepository<Category>(_fixture.Context);
            var mapper = _fixture.Services.GetRequiredService<AutoMapper.IMapper>();
            var createValidator = new CreateCategoryValidator();
            var updateValidator = new UpdateCategoryValidator();
            _categoryService = new CategoryService(repository, createValidator, updateValidator, mapper); 
        }

        [Fact]
        public async Task CreateAsync_ShouldPersistCategoryToDatabase()
        {
            var dto = new CreateCategoryDto
            {
                Name = "Electronics",
                ImageUrl = "https://example.com/electronics.jpg",
                ParentCategoryId = null
            };

            var categoryId = await _categoryService.CreateAsync(dto);

            categoryId.Should().NotBeNullOrWhiteSpace();

            var savedCategory = await _fixture.Context.Categories.FindAsync(categoryId);
            savedCategory.Should().NotBeNull();
            savedCategory!.Name.Should().Be("Electronics");
            savedCategory.ImageUrl.Should().Be("https://example.com/electronics.jpg");
            savedCategory.ParentCategoryId.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnPersistedCategory()
        {
            var category = new Category(Guid.NewGuid().ToString(), "Books", "https://example.com/books.jpg", null);
            _fixture.Context.Categories.Add(category);
            await _fixture.Context.SaveChangesAsync();

            var result = await _categoryService.GetByIdAsync(category.Id);

            result.Should().NotBeNull();
            result.Id.Should().Be(category.Id);
            result.Name.Should().Be("Books");
            result.ImageUrl.Should().Be("https://example.com/books.jpg");
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistentId_ShouldReturnNull()
        {
            var nonExistentId = Guid.NewGuid().ToString();

            var entity = await _categoryService.GetByIdAsync(nonExistentId);

            entity.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCategories()
        {
            var categories = new[]
            {
                new Category(Guid.NewGuid().ToString(), "Electronics", null, null),
                new Category(Guid.NewGuid().ToString(), "Books", null, null),
                new Category(Guid.NewGuid().ToString(), "Clothing", null, null)
            };

            _fixture.Context.Categories.AddRange(categories);
            await _fixture.Context.SaveChangesAsync();

            var result = await _categoryService.GetAllAsync();

            result.Should().HaveCount(3);
            result.Should().Contain(c => c.Name == "Electronics");
            result.Should().Contain(c => c.Name == "Books");
            result.Should().Contain(c => c.Name == "Clothing");
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyExistingCategory()
        {
            var category = new Category(Guid.NewGuid().ToString(), "Old Name", null, null);
            _fixture.Context.Categories.Add(category);
            await _fixture.Context.SaveChangesAsync();

            var updateDto = new UpdateCategoryDto
            {
                Name = "Updated Name",
                ImageUrl = "https://example.com/updated.jpg",
                ParentCategoryId = null
            };

            await _categoryService.UpdateAsync(category.Id, updateDto);

            var updatedCategory = await _fixture.Context.Categories.FindAsync(category.Id);
            updatedCategory.Should().NotBeNull();
            updatedCategory!.Name.Should().Be("Updated Name");
            updatedCategory.ImageUrl.Should().Be("https://example.com/updated.jpg");
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistentId_ShouldThrowEntityNotFoundException()
        {
            var nonExistentId = Guid.NewGuid().ToString();
            var updateDto = new UpdateCategoryDto
            {
                Name = "Updated Name",
                ImageUrl = null,
                ParentCategoryId = null
            };

            var act = async () => await _categoryService.UpdateAsync(nonExistentId, updateDto);

            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveCategoryFromDatabase()
        {
            var category = new Category(Guid.NewGuid().ToString(), "To Delete", null, null);
            _fixture.Context.Categories.Add(category);
            await _fixture.Context.SaveChangesAsync();

            await _categoryService.DeleteAsync(category.Id);

            var deletedCategory = await _fixture.Context.Categories.FindAsync(category.Id);
            deletedCategory.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_WithParentCategory_ShouldCreateHierarchy()
        {
            var parentCategory = new Category(Guid.NewGuid().ToString(), "Electronics", null, null);
            _fixture.Context.Categories.Add(parentCategory);
            await _fixture.Context.SaveChangesAsync();

            var subCategoryDto = new CreateCategoryDto
            {
                Name = "Smartphones",
                ImageUrl = null,
                ParentCategoryId = parentCategory.Id
            };

            var subCategoryId = await _categoryService.CreateAsync(subCategoryDto);

            var savedSubCategory = await _fixture.Context.Categories.FindAsync(subCategoryId);
            savedSubCategory.Should().NotBeNull();
            savedSubCategory!.Name.Should().Be("Smartphones");
            savedSubCategory.ParentCategoryId.Should().Be(parentCategory.Id);
        }

        [Fact]
        public async Task GetAllAsync_WithEmptyDatabase_ShouldReturnEmptyList()
        {
            var result = await _categoryService.GetAllAsync();

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task UpdateAsync_ShouldChangeParentCategory()
        {
            var parentCategory1 = new Category(Guid.NewGuid().ToString(), "Electronics", null, null);
            var parentCategory2 = new Category(Guid.NewGuid().ToString(), "Gadgets", null, null);
            var childCategory = new Category(Guid.NewGuid().ToString(), "Smartphones", null, parentCategory1.Id);

            _fixture.Context.Categories.AddRange(parentCategory1, parentCategory2, childCategory);
            await _fixture.Context.SaveChangesAsync();

            var updateDto = new UpdateCategoryDto
            {
                Name = "Smartphones",
                ImageUrl = null,
                ParentCategoryId = parentCategory2.Id
            };

            await _categoryService.UpdateAsync(childCategory.Id, updateDto);

            var updatedCategory = await _fixture.Context.Categories.FindAsync(childCategory.Id);
            updatedCategory.Should().NotBeNull();
            updatedCategory!.ParentCategoryId.Should().Be(parentCategory2.Id);
        }
    }
}
