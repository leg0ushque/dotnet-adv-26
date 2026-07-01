using Ecommerce.CartService.DataAccess.Entities;
using Ecommerce.CartService.DataAccess.Factories;

namespace Ecommerce.CartService.DataAccess.Repositories;

public class CartRepository(IMongoDbFactory mongoDbFactory)
    : GenericMongoRepository<Cart>(mongoDbFactory)
{
    protected override string CollectionName => "Carts";
}
