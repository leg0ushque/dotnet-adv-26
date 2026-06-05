Self-check answers are available [here](/self-checks.md)

# docker compose

This solution contains docker compose file to setup all the vital components for all the services. They are  
* Mongo & Mongo Express (UI app for Mongo) - for Cart Service
* RabbitMQ (for interation between Catalog and Cart services)

Please, also make sure SQL Server is installed and ran. It's required for Catalog Service.

To run docker compose use the following command in console in the root of the repository: 
```
docker-compose up -d
```

# Ecommerce.CartService

## What This Project Does

Basic shopping cart service with:
- CRUD operations for cart items
- MongoDB for storage
- FluentValidation for input validation
- Both unit and integration tests

## Architecture

Two main layers:
- **BusinessLogic**: Services, DTOs, validators, mapping
- **DataAccess**: Repositories, entities, MongoDB factory

Everything uses interfaces and dependency injection, so it's testable.

## Testability

xUnit runner is used. Available Tests:
- Unit tests (using Moq)
- Integration tests (real MongoDB in Docker)

Key points
* Constructor injection everywhere - easy to mock
* Everything depends on interfaces (IRepository, IService, etc.)
* Generic base classes (BaseService, GenericMongoRepository) - less duplication
* Separate validation with FluentValidation
* DatabaseFixture for integration tests
* AutoFixture generates test data automatically

## Extensibility

The design allows extending at these points:

* Add new services inherited from generic `BaseService`
* Add new repositories inherited from generic `GenericMongoRepository
* Provide custom validation for entities using FluentValidation - inherit from `AbstractValidator`
* Custom mapping is available through implementation of `IMappingService<TEntity, TDto>` - it covers bi-directional mapping 
* New entities must be inherited from `BaseEntity` that implements `IEntity` with string ID (primary key)
* Customize MongoDB setup through implementation of `IMongoDbFactory.
## Project Structure

```
src/
  BusinessLogic/       - Services, DTOs, validators, mappings
  DataAccess/          - Repositories, entities, MongoDB factory
tests/
  UnitTests/           - Fast, mocked tests
  IntegrationTests/    - Real MongoDB tests
compose.yaml           - Docker config (MongoDB + Mongo Express)
```

## Test runs

You may run the tests using VS or `dotnet test` command.  

**Important**: Integration tests won't work without Docker Compose running. MongoDB needs to be at `localhost:27017`. Please, check settings.json for correctness (example file is provided)

# Ecommerce.CatalogService 

A service to manage Catalogs and Products.  
Solution is build following Clean Architecture.

Some mocked test data is available in test_data.sql, check it out.

## REST API

A REST-based Web API for managing product catalogs in an e-commerce system, built with .NET 10.

### API Versioning

- URL-based versioning: `/api/v1/...`
- Header-based versioning: `X-Api-Version: 1.0`
- Each API version has separate Swagger documentation

### OpenAPI/Swagger Documentation

- Interactive API documentation available at `/swagger`
- Separate documentation for each API version
- Available in Development environment

### HATEOAS Example Response

```json
{
  "data": {
    "id": "123",
    "name": "Laptop",
    "description": "High-performance laptop",
    "imageUrl": "https://example.com/laptop.jpg",
    "categoryId": "456",
    "price": 999.99,
    "amount": 10
  },
  "links": [
    {
      "href": "/api/v1/products/123",
      "rel": "self",
      "method": "GET"
    },
    {
      "href": "/api/v1/products/123",
      "rel": "update",
      "method": "PUT"
    },
    {
      "href": "/api/v1/products/123",
      "rel": "delete",
      "method": "DELETE"
    },
    {
      "href": "/api/v1/products",
      "rel": "all-products",
      "method": "GET"
    },
    {
      "href": "/api/v1/categories/456",
      "rel": "category",
      "method": "GET"
    }
  ]
}
```

---
## Running the Application

### Prerequisites

- .NET 10 SDK
- SQL Server (or use in-memory database for development)

Check if "ConnectionStrings" section of appsettings.json contains "DefaultConnection" connection string

Access Swagger UI at: `https://localhost:5001/swagger`

## Database Migrations

To use migrations please verify the dotnet-ef tool is installed:
```bash
dotnet tool install --global dotnet-ef
```

To create initial migration open Package Manager Console in VS and execute:
```bash
dotnet ef migrations add InitialCreate -c EcommerceCatalogDbContext --project src\Ecommerce.CatalogService.Persistence\Ecommerce.CatalogService.Persistence.csproj
```

To apply migrations open Package Manager Console in VS and execute:
```bash
dotnet ef database update -c EcommerceCatalogDbContext --project src\Ecommerce.CatalogService.Persistence\Ecommerce.CatalogService.Persistence.csproj
```

## Testing

The solution includes both Unit and Integration tests demonstrating testability.

