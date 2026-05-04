using Ecommerce.CatalogService.Application;
using Ecommerce.CatalogService.Application.Common.Interfaces;
using Ecommerce.CatalogService.Persistence.Data;
using Ecommerce.CatalogService.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.CatalogService.IntegrationTests.Fixtures
{
    public class DatabaseFixture : IDisposable
    {
        public EcommerceCatalogDbContext Context { get; private set; }
        public IServiceProvider Services { get; private set; }

        public DatabaseFixture()
        {
            var dbName = Guid.NewGuid().ToString();

            var services = new ServiceCollection();

            services.AddDbContext<EcommerceCatalogDbContext>(options =>
                options.UseInMemoryDatabase(dbName));

            services.AddLogging();

            services.AddApplication();

            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

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
