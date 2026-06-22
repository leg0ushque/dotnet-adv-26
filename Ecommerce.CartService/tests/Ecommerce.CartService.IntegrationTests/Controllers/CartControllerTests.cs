using AutoFixture;
using Ecommerce.CartService.Api;
using Ecommerce.CartService.BusinessLogic.Dtos;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Ecommerce.CartService.IntegrationTests.Controllers;

public class CartControllerTests : ControllerTestsBase
{
    private readonly Fixture _fixture;

    public CartControllerTests(WebApplicationFactory<Program> factory) : base(factory)
    {
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetCartV1_WhenCartExists_ReturnsCartDto()
    {
        // Arrange
        var cartKey = _fixture.Create<string>();
        var item = CreateValidCartItem();
        await _client.PostAsJsonAsync($"/api/v1/cart/{cartKey}/item", item);

        // Act
        var response = await _client.GetAsync($"/api/v1/cart/{cartKey}");
        var cart = await response.Content.ReadFromJsonAsync<CartDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        cart.Should().NotBeNull();
        cart!.Id.Should().Be(cartKey);
        cart.Items.Should().ContainSingle(i => i.ItemId == item.ItemId);
    }

    [Fact]
    public async Task GetCartV1_WhenCartDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentCartKey = "non-existent-cart";

        // Act
        var response = await _client.GetAsync($"/api/v1/cart/{nonExistentCartKey}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCartV2_WhenCartExists_ReturnsOnlyItems()
    {
        // Arrange
        var cartKey = _fixture.Create<string>();
        var item = CreateValidCartItem();
        await _client.PostAsJsonAsync($"/api/v2/cart/{cartKey}/item", item);

        // Act
        var response = await _client.GetAsync($"/api/v2/cart/{cartKey}");
        var items = await response.Content.ReadFromJsonAsync<List<CartItemDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        items.Should().NotBeNull();
        items!.Should().ContainSingle(i => i.ItemId == item.ItemId);
    }

    [Fact]
    public async Task GetCartV2_WhenCartDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentCartKey = "non-existent-cart";

        // Act
        var response = await _client.GetAsync($"/api/v2/cart/{nonExistentCartKey}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddItemToCart_WhenValidItem_ReturnsOk()
    {
        // Arrange
        var cartKey = _fixture.Create<string>();
        var item = CreateValidCartItem();

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/cart/{cartKey}/item", item);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AddItemToCart_WhenDuplicateItem_ReturnsBadRequest()
    {
        // Arrange
        var cartKey = _fixture.Create<string>();
        var item = CreateValidCartItem();
        await _client.PostAsJsonAsync($"/api/v1/cart/{cartKey}/item", item);

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/cart/{cartKey}/item", item);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddItemToCart_WhenInvalidItem_ReturnsBadRequest()
    {
        // Arrange
        var cartKey = _fixture.Create<string>();
        var invalidItem = new CartItemDto
        {
            ItemId = "invalid-item",
            Name = "Invalid",
            Price = -1, // Invalid price
            Quantity = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/cart/{cartKey}/item", invalidItem);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteItemFromCart_WhenItemExists_ReturnsOk()
    {
        // Arrange
        var cartKey = _fixture.Create<string>();
        var item = CreateValidCartItem();
        await _client.PostAsJsonAsync($"/api/v1/cart/{cartKey}/item", item);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/cart/{cartKey}/items/{item.ItemId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteItemFromCart_WhenItemDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var cartKey = _fixture.Create<string>();
        var item = CreateValidCartItem();
        await _client.PostAsJsonAsync($"/api/v1/cart/{cartKey}/item", item);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/cart/{cartKey}/items/non-existent-item");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteItemFromCart_WhenCartDoesNotExist_ReturnsNotFound()
    {
        // Act
        var response = await _client.DeleteAsync($"/api/v1/cart/non-existent-cart/items/some-item");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("1.0")]
    [InlineData("2.0")]
    public async Task AddItemToCart_WorksForBothVersions(string version)
    {
        // Arrange
        var cartKey = _fixture.Create<string>();
        var item = CreateValidCartItem();

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v{version}/cart/{cartKey}/item", item);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("1.0")]
    [InlineData("2.0")]
    public async Task DeleteItemFromCart_WorksForBothVersions(string version)
    {
        // Arrange
        var cartKey = _fixture.Create<string>();
        var item = CreateValidCartItem();
        await _client.PostAsJsonAsync($"/api/v{version}/cart/{cartKey}/item", item);

        // Act
        var response = await _client.DeleteAsync($"/api/v{version}/cart/{cartKey}/items/{item.ItemId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private CartItemDto CreateValidCartItem()
    {
        return new CartItemDto
        {
            ItemId = _fixture.Create<string>(),
            Name = _fixture.Create<string>(),
            Price = 10.0m,
            Quantity = 1
        };
    }
}
