using MongoDB.Driver;

namespace Ecommerce.CartService.DataAccess.Factories;

public interface IMongoDbFactory
{
    public IMongoCollection<T> GetCollection<T>(string collectionName);
}
