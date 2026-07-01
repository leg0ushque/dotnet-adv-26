using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Ecommerce.CartService.BusinessLogic.Dtos;
using Ecommerce.CartService.BusinessLogic.Mappings;
using Ecommerce.CartService.BusinessLogic.Validators;
using Ecommerce.CartService.DataAccess.Entities;
using Ecommerce.CartService.DataAccess.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Ecommerce.CartService.IntegrationTests.Services;

public class CartServiceTests : IClassFixture<DatabaseFixture>
{
    private readonly Fixture _fixture;
    private readonly BusinessLogic.Services.CartService _cartService;
    private readonly IRepository<Cart> _cartRepository;

    public CartServiceTests(DatabaseFixture dbFixture)
    {
        _fixture = new Fixture();
        _cartRepository = dbFixture.CartRepositoryInstance!;
        _cartService = new BusinessLogic.Services.CartService(_cartRepository,
            new CartDtoCreateValidator(),
            new CartDtoUpdateValidator(),
            new CartItemDtoValidator(),
            new MapperConfiguration(cfg => cfg.AddProfile<ApplicationMappingProfile>(), NullLoggerFactory.Instance).CreateMapper());
    }

    [Fact]
    public async Task CreateAsync_WhenCartIsCreatedAndFetched_ReturnsCart()
    {
        var cartKey = _fixture.Create<string>();
        var cartDto = new CartDto { Id = cartKey, Items = [] };
        var createResult = await _cartService.CreateAsync(cartDto);
        createResult.IsSuccess.Should().BeTrue();

        var getResult = await _cartService.GetCartByKeyAsync(cartKey);
        getResult.IsSuccess.Should().BeTrue();
        getResult.Value.Id.Should().Be(cartKey);
    }

    [Fact]
    public async Task AddItemToCartAsync_WhenItemIsAdded_ReturnsItemInCart()
    {
        var cartKey = _fixture.Create<string>();
        var item = _fixture.Build<CartItemDto>()
            .With(x => x.ItemId, "item-1")
            .With(x => x.Name, "Test Product")
            .With(x => x.Price, 10.0m)
            .With(x => x.Quantity, 2)
            .Create();

        var addResult = await _cartService.AddItemToCartAsync(cartKey, item);
        var cartResult = await _cartService.GetCartByKeyAsync(cartKey);

        addResult.IsSuccess.Should().BeTrue();
        cartResult.IsSuccess.Should().BeTrue();
        cartResult.Value.Items.Should().Contain(i => i.ItemId == item.ItemId);
    }

    [Fact]
    public async Task AddItemToCartAsync_WhenDuplicateItem_ReturnsValidationFailure()
    {
        var cartKey = _fixture.Create<string>();
        var item = _fixture.Build<CartItemDto>()
            .With(x => x.ItemId, "item-2")
            .With(x => x.Name, "Duplicate")
            .With(x => x.Price, 5.0m)
            .With(x => x.Quantity, 1)
            .Create();
        await _cartService.AddItemToCartAsync(cartKey, item);

        var duplicateResult = await _cartService.AddItemToCartAsync(cartKey, item);
        duplicateResult.IsSuccess.Should().BeFalse();
        duplicateResult.Error.Should().NotBeNull();
        duplicateResult.Error.Type.Should().Be(Ecommerce.CartService.BusinessLogic.Results.ErrorType.Validation);
    }

    [Fact]
    public async Task DeleteItemFromCartAsync_WhenItemExists_RemovesItemFromCart()
    {
        var cartKey = _fixture.Create<string>();
        var item = _fixture.Build<CartItemDto>()
            .With(x => x.ItemId, "item-3")
            .With(x => x.Name, "ToRemove")
            .With(x => x.Price, 5.0m)
            .With(x => x.Quantity, 1)
            .Create();
        await _cartService.AddItemToCartAsync(cartKey, item);

        var deleteResult = await _cartService.DeleteItemFromCartAsync(cartKey, item.ItemId);
        deleteResult.IsSuccess.Should().BeTrue();
        var cartResultAfterDelete = await _cartService.GetCartByKeyAsync(cartKey);
        cartResultAfterDelete.Value.Items.Should().NotContain(i => i.ItemId == item.ItemId);
    }

    [Fact]
    public async Task DeleteItemFromCartAsync_WhenItemDoesNotExist_ReturnsNotFound()
    {
        var cartKey = _fixture.Create<string>();
        var deleteResult = await _cartService.DeleteItemFromCartAsync(cartKey, "not-exist");
        deleteResult.IsSuccess.Should().BeFalse();
        deleteResult.Error.Should().NotBeNull();
        deleteResult.Error.Type.Should().Be(Ecommerce.CartService.BusinessLogic.Results.ErrorType.NotFound);
    }


    [Theory]
    [InlineData(-1d, 1)]
    [InlineData(0d, 1)]
    [InlineData(1d, -1)]
    public async Task AddItemToCartAsync_WhenItemIsInvalid_ReturnsValidationFailure(decimal price, int quantity)
    {
        var cartKey = _fixture.Create<string>();
        var item = _fixture.Build<CartItemDto>()
            .With(x => x.ItemId, "item-4")
            .With(x => x.Name, "Invalid")
            .With(x => x.Price, price)
            .With(x => x.Quantity, quantity)
            .Create();

        var result = await _cartService.AddItemToCartAsync(cartKey, item);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Type.Should().Be(BusinessLogic.Results.ErrorType.Validation);
    }

    [Fact]
    public async Task DeleteAsync_WhenCartExists_RemovesCart()
    {
        var cartKey = _fixture.Create<string>();
        var cartDto = new CartDto { Id = cartKey, Items = [] };
        await _cartService.CreateAsync(cartDto);

        var deleteResult = await _cartService.DeleteAsync(cartKey);
        var getResult = await _cartService.GetCartByKeyAsync(cartKey);

        deleteResult.IsSuccess.Should().BeTrue();
        getResult.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_WhenCartDoesNotExist_ReturnsNotFound()
    {
        var result = await _cartService.DeleteAsync("not-exist");
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCartByKeyAsync_WhenCartDoesNotExist_ReturnsNotFound()
    {
        var result = await _cartService.GetCartByKeyAsync("not-exist");
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
    }
}
