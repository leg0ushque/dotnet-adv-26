using Ecommerce.CartService.DataAccess.Entities;
using Ecommerce.CartService.DataAccess.Factories;
using Ecommerce.CartService.DataAccess.Repositories;

namespace Ecommerce.CartService.DataAccess.UnitTests.Repositories;

public class CartRepositoryTests : GenericMongoRepositoryTestsBase<Cart>
{
    public override IRepository<Cart> CreateRepository(IMongoDbFactory factory)
        => new CartRepository(factory);
}
