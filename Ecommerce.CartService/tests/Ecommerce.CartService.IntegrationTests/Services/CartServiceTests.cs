using AutoFixture;
using Ecommerce.CartService.BusinessLogic.Dtos;
using Ecommerce.CartService.BusinessLogic.Mappings;
using Ecommerce.CartService.BusinessLogic.Services;
using Ecommerce.CartService.BusinessLogic.Validators;
using Ecommerce.CartService.DataAccess.Entities;
using Ecommerce.CartService.DataAccess.Repositories;
using FluentValidation;
using System.Threading.Tasks;
using Xunit;

namespace Ecommerce.CartService.IntegrationTests.Services
{
    public class CartServiceTests(DatabaseFixture dbFixture) 
        : IntegrationTestsBase<Cart, CartDto, CartDtoCreateValidator, CartDtoUpdateValidator>(dbFixture), IClassFixture<DatabaseFixture>
    {
        protected override IRepository<Cart> Repository => _dbFixture.CartRepositoryInstance!;

        [InlineData(-1d, 1)]
        [InlineData(0d, 1)]
        [InlineData(1d, -1)]
        [Theory]
        public async Task CreateAsync_InvalidDto_ReturnsValidationFailure(decimal price, int quantity)
        {
            // Arrange
            var invalidDto = _fixture.Build<CartDto>()
                .With(x => x.Id)
                .With(x => x.Name)
                .With(x => x.Price, price)
                .With(x => x.Quantity, quantity)
                .Create();

            // Act
            var result = await _service.CreateAsync(invalidDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Equal(Ecommerce.CartService.BusinessLogic.Results.ErrorType.Validation, result.Error.Type);
        }

        protected override IService<Cart, CartDto> CreateService()
            => new BusinessLogic.Services.CartService(Repository, _createValidator, _updateValidator, null!);
    }
}