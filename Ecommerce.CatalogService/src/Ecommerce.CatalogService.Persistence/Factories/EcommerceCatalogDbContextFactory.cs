using Ecommerce.CatalogService.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.CatalogService.Persistence.Factories;

public class EcommerceCatalogDbContextFactory : IDesignTimeDbContextFactory<EcommerceCatalogDbContext>
{
    public EcommerceCatalogDbContext CreateDbContext(string[] args)
    {
        var assemblyPath = Path.Combine(Directory.GetCurrentDirectory(), "..\\Ecommerce.CatalogService.Api");

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(assemblyPath)
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<EcommerceCatalogDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new EcommerceCatalogDbContext(optionsBuilder.Options);
    }
}
