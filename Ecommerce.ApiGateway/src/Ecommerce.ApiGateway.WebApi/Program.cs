using Ecommerce.ApiGateway.WebApi.Aggregators;
using Microsoft.AspNetCore.Authentication;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace Ecommerce.ApiGateway.WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Console.Title = "Ecommerce ApiGateway";

            builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSwaggerForOcelot(builder.Configuration, opts =>
            {
                opts.GenerateDocsForAggregates = true;
            });

            builder.Services
                .AddOcelot(builder.Configuration)
                .AddCacheManager(x => x.WithDictionaryHandle())
                .AddSingletonDefinedAggregator<ProductDetailsAggregator>();

            builder.Services.AddAuthentication()
                .AddJwtBearer("KeycloakBearer", options =>
                {
                    options.Authority = builder.Configuration["Keycloak:Authority"];
                    options.Audience = builder.Configuration["Keycloak:Audience"];
                    options.RequireHttpsMetadata = false;

                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true
                    };
                });

            builder.Services.AddTransient<IClaimsTransformation, KeycloakRolesTransformer>();

            var app = builder.Build();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwaggerForOcelotUI(opts =>
            {
                opts.PathToSwaggerGenerator = "/swagger/docs";
            });

            app.UseHttpsRedirection();

            app.MapControllers();

            await app.UseOcelot();

            app.Run();
        }
    }
}
