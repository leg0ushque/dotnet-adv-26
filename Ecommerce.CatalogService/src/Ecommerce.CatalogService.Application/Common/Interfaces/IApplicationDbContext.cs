using Ecommerce.CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.CatalogService.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Category> Categories { get; }
        DbSet<Product> Products { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
