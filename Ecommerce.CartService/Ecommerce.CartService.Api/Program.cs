using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using AutoMapper;
using Ecommerce.CartService.BusinessLogic;
using Ecommerce.CartService.BusinessLogic.Mappings;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Logging.Abstractions;

namespace Ecommerce.CartService.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));

            var databaseName = builder.Configuration.GetSection("DatabaseName").Value;
            ArgumentException.ThrowIfNullOrWhiteSpace(databaseName, nameof(databaseName));

            builder.Services.AddBusinessLogicServices(connectionString, databaseName);
            builder.Services.AddSingleton(SetupMapper());

            builder.Services.AddControllers();

            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("X-Api-Version"));
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            builder.Services.AddSwaggerGen(options =>
            {
                var provider = builder.Services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, new Microsoft.OpenApi.Models.OpenApiInfo
                    {
                        Title = $"Cart Service API {description.ApiVersion}",
                        Version = description.ApiVersion.ToString()
                    });
                }
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            });

            var app = builder.Build();

            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"Cart Service API {description.GroupName.ToUpperInvariant()}");
                }
            });

            app.UseHttpsRedirection();

            app.MapControllers();

            app.Run();
        }

        public static IMapper SetupMapper()
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ApplicationMappingProfile());
            }, NullLoggerFactory.Instance);

            return mapperConfig.CreateMapper();
        }
    }
}