using Ecommerce.CartService.DataAccess.Entities;
using Ecommerce.CartService.DataAccess.Factories;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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

        public async Task<string> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(entity, new InsertOneOptions(), cancellationToken);

            return entity.Id;
        }

        public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            return _collection.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", id), cancellationToken);
        }

        public async Task<TEntity> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var item = await _collection.FindAsync(Builders<TEntity>.Filter.Eq("_id", id),
                cancellationToken: cancellationToken);

            return await item.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<TEntity>> FilterAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        {
            var items = await _collection.FindAsync(Builders<TEntity>.Filter.Where(expression),
                cancellationToken: cancellationToken);

            return await items.ToListAsync(cancellationToken);
        }

        public async Task<List<TEntity>> FilterAsync<TField>(Expression<Func<TEntity, TField>> field, IEnumerable<TField> values, CancellationToken cancellationToken = default)
        {
            var items = await _collection.FindAsync(Builders<TEntity>.Filter.In(field, values),
                cancellationToken: cancellationToken);

            return await items.ToListAsync(cancellationToken);
        }
    }
}
