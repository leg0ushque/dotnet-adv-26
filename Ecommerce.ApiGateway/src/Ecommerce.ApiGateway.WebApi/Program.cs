using Ecommerce.ApiGateway.WebApi.Aggregators;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.OpenApi;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace Ecommerce.ApiGateway.WebApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Console.Title = "Ecommerce ApiGateway";

        builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                { new OpenApiSecuritySchemeReference("Bearer", document), new() }
            });
        });

        builder.Services.AddSwaggerForOcelot(builder.Configuration, opts =>
        {
            opts.GenerateDocsForAggregates = true;
            opts.AggregateDocsGeneratorPostProcess = (aggregateRoute, routesDocs, pathItemDoc, documentation) =>
            {
                if (aggregateRoute.UpstreamPathTemplate == "/catalog/products/{productId}/full")
                {
                    var operation = pathItemDoc.Operations?[HttpMethod.Get];
                    if (operation != null)
                    {
                        operation.Parameters?.Add(new OpenApiParameter()
                        {
                            Name = "productId",
                            Schema = new OpenApiSchema() { Type = JsonSchemaType.String, Format = "string" },
                            In = ParameterLocation.Path,
                            Required = true,
                            Description = "The product identifier"
                        });

                        operation.Security =
                        [
                            new OpenApiSecurityRequirement
                            { { new OpenApiSecuritySchemeReference("Bearer", documentation), new List<string>() } }
                        ];
                    }
                }
            };
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
