using Ecommerce.CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.CatalogService.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    public DbSet<Category> Categories { get; }
    public DbSet<Product> Products { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
