using Ecommerce.CatalogService.Application.Common.Interfaces;
using Ecommerce.CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.CatalogService.Persistence.Data;

public class EcommerceCatalogDbContext(DbContextOptions<EcommerceCatalogDbContext> options) : DbContext(options), IApplicationDbContext
{
    public DbSet<Category> Categories { get; set; } = null!;

    public DbSet<Product> Products { get; set; } = null!;

    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EcommerceCatalogDbContext).Assembly);
    }
}
