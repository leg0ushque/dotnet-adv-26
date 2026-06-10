using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using AutoMapper;
using Ecommerce.CartService.Api.Helpers;
using Ecommerce.CartService.Api.Middleware;
using Ecommerce.CartService.BusinessLogic;
using Ecommerce.CartService.BusinessLogic.Mappings;
using Ecommerce.CartService.Messaging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Claims;
using static Ecommerce.CartService.Api.Constants;

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

            builder.Services.AddMessagingServices();

            builder.Services.AddSingleton(SetupMapper());

            var authAuthority = builder.Configuration.GetValue<string>("Auth:Authority");
            var authAudience = builder.Configuration.GetValue<string>("Auth:Audience");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = authAuthority;
                    options.Audience = authAudience;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        RoleClaimType = ClaimTypes.Role,
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            if (context.Principal?.Identity is ClaimsIdentity claimsIdentity)
                            {
                                KeycloakRoleHelper.MapKeycloakRolesToClaims(claimsIdentity);
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthConstants.StoreCustomerManagerOnlyPolicy, policy =>
                    policy.RequireRole(AuthConstants.ManagerRole, AuthConstants.StoreCustomerRole));
            });

            builder.Services.AddAuthorization();

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<IdentityLoggingMiddleware>();

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