using System.Net;
using System.Net.Http.Json;
using Ecommerce.CatalogService.Api.Models;
using Ecommerce.CatalogService.Application.Categories.DTOs;
using Ecommerce.CatalogService.IntegrationTests.Fixtures;
using FluentAssertions;

namespace Ecommerce.CatalogService.IntegrationTests.Categories
{
    public class CategoriesApiIntegrationTests : IClassFixture<CatalogWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public CategoriesApiIntegrationTests(CatalogWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateCategory_ReturnsCreatedCategory()
        {
            // Arrange
            var createCategoryDto = new
            {
                Name = "Test Category",
                ImageUrl = "https://example.com/test.jpg"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/categories", createCategoryDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var categoryId = await response.Content.ReadFromJsonAsync<string>();
            categoryId.Should().NotBeNullOrEmpty();
            response.Headers.Location.Should().NotBeNull();
        }

        [Fact]
        public async Task GetCategory_WithValidId_ReturnsCategoryWithHATEOASLinks()
        {
            // Arrange - Create a category first
            var createCategoryDto = new
            {
                Name = "HATEOAS Test Category",
                ImageUrl = "https://example.com/hateoas.jpg"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/v1/categories", createCategoryDto);
            var categoryId = await createResponse.Content.ReadFromJsonAsync<string>();

            // Act
            var response = await _client.GetAsync($"/api/v1/categories/{categoryId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var categoryResponse = await response.Content.ReadFromJsonAsync<ResourceResponse<CategoryDto>>();
            
            categoryResponse.Should().NotBeNull();
            categoryResponse!.Data.Should().NotBeNull();
            categoryResponse.Data.Name.Should().Be("HATEOAS Test Category");
            categoryResponse.Links.Should().NotBeEmpty();
            categoryResponse.Links.Should().Contain(l => l.Rel == "self");
            categoryResponse.Links.Should().Contain(l => l.Rel == "update");
            categoryResponse.Links.Should().Contain(l => l.Rel == "delete");
            categoryResponse.Links.Should().Contain(l => l.Rel == "all-categories");
        }

        [Fact]
        public async Task GetCategories_ReturnsAllCategories()
        {
            // Arrange - Create a few categories
            var category1 = new { Name = "Category 1", ImageUrl = "https://example.com/1.jpg" };
            var category2 = new { Name = "Category 2", ImageUrl = "https://example.com/2.jpg" };
            
            await _client.PostAsJsonAsync("/api/v1/categories", category1);
            await _client.PostAsJsonAsync("/api/v1/categories", category2);

            // Act
            var response = await _client.GetAsync("/api/v1/categories");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var categories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
            categories.Should().NotBeNull();
            categories!.Should().NotBeEmpty();
        }

        [Fact]
        public async Task UpdateCategory_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var createCategoryDto = new
            {
                Name = "Original Name",
                ImageUrl = "https://example.com/original.jpg"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/v1/categories", createCategoryDto);
            var categoryId = await createResponse.Content.ReadFromJsonAsync<string>();

            var updateCategoryDto = new
            {
                Name = "Updated Name",
                ImageUrl = "https://example.com/updated.jpg"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/v1/categories/{categoryId}", updateCategoryDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteCategory_WithProducts_DeletesCategoryAndProducts()
        {
            // Arrange - Create a category
            var createCategoryDto = new
            {
                Name = "Delete Test Category",
                ImageUrl = "https://example.com/delete.jpg"
            };

            var categoryResponse = await _client.PostAsJsonAsync("/api/v1/categories", createCategoryDto);
            var categoryId = await categoryResponse.Content.ReadFromJsonAsync<string>();

            // Create products in the category
            var product1 = new
            {
                Name = "Product 1",
                CategoryId = categoryId,
                Price = 10.0m,
                Amount = 5
            };
            var product2 = new
            {
                Name = "Product 2",
                CategoryId = categoryId,
                Price = 20.0m,
                Amount = 10
            };

            var product1Response = await _client.PostAsJsonAsync("/api/v1/products", product1);
            var product2Response = await _client.PostAsJsonAsync("/api/v1/products", product2);
            var product1Id = await product1Response.Content.ReadFromJsonAsync<string>();
            var product2Id = await product2Response.Content.ReadFromJsonAsync<string>();

            // Act - Delete the category
            var response = await _client.DeleteAsync($"/api/v1/categories/{categoryId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verify category is deleted
            var getCategoryResponse = await _client.GetAsync($"/api/v1/categories/{categoryId}");
            getCategoryResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

            // Verify products are also deleted
            var getProduct1Response = await _client.GetAsync($"/api/v1/products/{product1Id}");
            getProduct1Response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var getProduct2Response = await _client.GetAsync($"/api/v1/products/{product2Id}");
            getProduct2Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetCategory_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/v1/categories/nonexistent-id");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
