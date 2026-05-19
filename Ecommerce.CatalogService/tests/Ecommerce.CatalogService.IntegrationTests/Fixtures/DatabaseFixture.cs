using Ecommerce.CatalogService.Application;
using Ecommerce.CatalogService.Persistence;
using Ecommerce.CatalogService.Persistence.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.CatalogService.IntegrationTests.Fixtures
{
    public class DatabaseFixture : IDisposable
    {
        public EcommerceCatalogDbContext Context { get; private set; }
        public IServiceProvider Services { get; private set; }

        public DatabaseFixture()
        {
            var services = new ServiceCollection();

            // Build configuration with InMemory flag
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["UseInMemoryDatabase"] = "true"
                })
                .Build();

            services.AddLogging();
            services.AddPersistence(configuration, useInMemoryDatabase: true);
            services.AddApplication();

            Services = services.BuildServiceProvider();

            Context = Services.GetRequiredService<EcommerceCatalogDbContext>();
            Context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
            Context.Dispose();
        }

        public void ClearDatabase()
        {
            Context.Categories.RemoveRange(Context.Categories);
            Context.Products.RemoveRange(Context.Products);
            Context.SaveChanges();
        }
    }
}
