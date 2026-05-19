using System.Net;
using System.Net.Http.Json;
using Ecommerce.CatalogService.Api.Models;
using Ecommerce.CatalogService.Application.Products.DTOs;
using Ecommerce.CatalogService.Application.Common.DTOs;
using Ecommerce.CatalogService.IntegrationTests.Fixtures;
using FluentAssertions;

namespace Ecommerce.CatalogService.IntegrationTests.Products
{
    public class ProductsApiIntegrationTests : IClassFixture<CatalogWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ProductsApiIntegrationTests(CatalogWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateProduct_ReturnsCreatedProduct()
        {
            // Arrange - First create a category
            var createCategoryDto = new
            {
                Name = "Electronics",
                ImageUrl = "https://example.com/electronics.jpg"
            };

            var categoryResponse = await _client.PostAsJsonAsync("/api/v1/categories", createCategoryDto);
            categoryResponse.EnsureSuccessStatusCode();
            var categoryId = await categoryResponse.Content.ReadFromJsonAsync<string>();

            var createProductDto = new
            {
                Name = "Laptop",
                Description = "High-performance laptop",
                ImageUrl = "https://example.com/laptop.jpg",
                CategoryId = categoryId,
                Price = 999.99m,
                Amount = 10
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/products", createProductDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var productId = await response.Content.ReadFromJsonAsync<string>();
            productId.Should().NotBeNullOrEmpty();
            response.Headers.Location.Should().NotBeNull();
        }

        [Fact]
        public async Task GetProduct_WithValidId_ReturnsProductWithHATEOASLinks()
        {
            // Arrange - Create a category and product first
            var createCategoryDto = new
            {
                Name = "Books",
                ImageUrl = "https://example.com/books.jpg"
            };

            var categoryResponse = await _client.PostAsJsonAsync("/api/v1/categories", createCategoryDto);
            var categoryId = await categoryResponse.Content.ReadFromJsonAsync<string>();

            var createProductDto = new
            {
                Name = "Programming Book",
                Description = "Learn programming",
                ImageUrl = "https://example.com/book.jpg",
                CategoryId = categoryId,
                Price = 49.99m,
                Amount = 50
            };

            var createResponse = await _client.PostAsJsonAsync("/api/v1/products", createProductDto);
            var productId = await createResponse.Content.ReadFromJsonAsync<string>();

            // Act
            var response = await _client.GetAsync($"/api/v1/products/{productId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var productResponse = await response.Content.ReadFromJsonAsync<ResourceResponse<ProductDto>>();
            
            productResponse.Should().NotBeNull();
            productResponse!.Data.Should().NotBeNull();
            productResponse.Data.Name.Should().Be("Programming Book");
            productResponse.Links.Should().NotBeEmpty();
            productResponse.Links.Should().Contain(l => l.Rel == "self");
            productResponse.Links.Should().Contain(l => l.Rel == "update");
            productResponse.Links.Should().Contain(l => l.Rel == "delete");
            productResponse.Links.Should().Contain(l => l.Rel == "category");
        }

        [Fact]
        public async Task GetProducts_WithPagination_ReturnsPaginatedResults()
        {
            // Arrange - Create multiple products
            var createCategoryDto = new
            {
                Name = "Toys",
                ImageUrl = "https://example.com/toys.jpg"
            };

            var categoryResponse = await _client.PostAsJsonAsync("/api/v1/categories", createCategoryDto);
            var categoryId = await categoryResponse.Content.ReadFromJsonAsync<string>();

            for (int i = 1; i <= 15; i++)
            {
                var createProductDto = new
                {
                    Name = $"Toy {i}",
                    Description = $"Description {i}",
                    CategoryId = categoryId,
                    Price = 10.0m * i,
                    Amount = i
                };
                await _client.PostAsJsonAsync("/api/v1/products", createProductDto);
            }

            // Act
            var response = await _client.GetAsync("/api/v1/products?pageNumber=1&pageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var paginatedResult = await response.Content.ReadFromJsonAsync<PaginatedResult<ProductDto>>();
            paginatedResult.Should().NotBeNull();
        }

        [Fact]
        public async Task GetProducts_WithCategoryFilter_ReturnsFilteredProducts()
        {
            // Arrange
            var createCategoryDto = new
            {
                Name = "Sports",
                ImageUrl = "https://example.com/sports.jpg"
            };

            var categoryResponse = await _client.PostAsJsonAsync("/api/v1/categories", createCategoryDto);
            var categoryId = await categoryResponse.Content.ReadFromJsonAsync<string>();

            var createProductDto = new
            {
                Name = "Basketball",
                CategoryId = categoryId,
                Price = 29.99m,
                Amount = 20
            };
            await _client.PostAsJsonAsync("/api/v1/products", createProductDto);

            // Act
            var response = await _client.GetAsync($"/api/v1/products?categoryId={categoryId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task UpdateProduct_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var createCategoryDto = new
            {
                Name = "Clothing",
                ImageUrl = "https://example.com/clothing.jpg"
            };

            var categoryResponse = await _client.PostAsJsonAsync("/api/v1/categories", createCategoryDto);
            var categoryId = await categoryResponse.Content.ReadFromJsonAsync<string>();

            var createProductDto = new
            {
                Name = "T-Shirt",
                CategoryId = categoryId,
                Price = 19.99m,
                Amount = 100
            };

            var createResponse = await _client.PostAsJsonAsync("/api/v1/products", createProductDto);
            var productId = await createResponse.Content.ReadFromJsonAsync<string>();

            var updateProductDto = new
            {
                Name = "Updated T-Shirt",
                Description = "Comfortable cotton t-shirt",
                CategoryId = categoryId,
                Price = 24.99m,
                Amount = 80
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/v1/products/{productId}", updateProductDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteProduct_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var createCategoryDto = new
            {
                Name = "Home",
                ImageUrl = "https://example.com/home.jpg"
            };

            var categoryResponse = await _client.PostAsJsonAsync("/api/v1/categories", createCategoryDto);
            var categoryId = await categoryResponse.Content.ReadFromJsonAsync<string>();

            var createProductDto = new
            {
                Name = "Lamp",
                CategoryId = categoryId,
                Price = 39.99m,
                Amount = 15
            };

            var createResponse = await _client.PostAsJsonAsync("/api/v1/products", createProductDto);
            var productId = await createResponse.Content.ReadFromJsonAsync<string>();

            // Act
            var response = await _client.DeleteAsync($"/api/v1/products/{productId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verify deletion
            var getResponse = await _client.GetAsync($"/api/v1/products/{productId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
