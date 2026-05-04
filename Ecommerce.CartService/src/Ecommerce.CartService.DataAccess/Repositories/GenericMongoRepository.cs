using Ecommerce.CartService.DataAccess.Entities;
using Ecommerce.CartService.DataAccess.Factories;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.CartService.DataAccess.Repositories
{
    public abstract class GenericMongoRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
        protected abstract string CollectionName { get; }

        protected readonly IMongoCollection<TEntity> _collection;

        protected GenericMongoRepository(IMongoDbFactory mongoDbFactory)
        {
            _collection = mongoDbFactory.GetCollection<TEntity>(CollectionName);
        }

        public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var items = await _collection.FindAsync(_ => true,
                cancellationToken: cancellationToken);

            return await items.ToListAsync(cancellationToken);
        }

        public Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return _collection.InsertOneAsync(entity, new InsertOneOptions(), cancellationToken);
        }

        public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            return _collection.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", id), cancellationToken);
        }
    }
}
