using Ecommerce.CartService.DataAccess.Entities;
using Ecommerce.CartService.DataAccess.Factories;
using Ecommerce.CartService.DataAccess.Repositories;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Ecommerce.CartService.IntegrationTests
{
    public class DatabaseFixture
    {
        public IRepository<Cart>? CartRepositoryInstance { get; private set; }

        public DatabaseFixture()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("settings.json")
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection")!;
            var databaseName = config.GetSection("DatabaseName").Value!;

            var mongoDbFactory = new MongoDbFactory(connectionString, databaseName);

            CartRepositoryInstance = new CartRepository(mongoDbFactory);
        }
    }
}
