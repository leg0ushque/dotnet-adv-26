using Ecommerce.CatalogService.Application.Common.Interfaces;
using Ecommerce.CatalogService.Domain.Common;
using Ecommerce.CatalogService.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ecommerce.CatalogService.Persistence.Repositories
{
    public class GenericRepository<TEntity>(EcommerceCatalogDbContext context) : IRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly EcommerceCatalogDbContext _context = context;

        public async Task<string> CreateAsync(TEntity item)
        {
            item.Id = Guid.NewGuid().ToString();

            await _context.Set<TEntity>().AddAsync(item);

            return item.Id;
        }

        public async Task DeleteByIdAsync(string id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<TEntity>().Remove(entity);
            }
        }

        public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }

        public Task<TEntity?> GetByIdAsync(string id)
            => GetSingleAsync(e => e.Id == id);

        public Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> filter)
            => _context.Set<TEntity>().FirstOrDefaultAsync(filter);

        public async Task UpdateAsync(TEntity item)
        {
            _context.Set<TEntity>().Update(item);

            await Task.CompletedTask;
        }
    }
}
