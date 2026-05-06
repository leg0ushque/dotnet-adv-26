
# Catalog Service 

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

To create initial migration:
```bash
dotnet ef migrations add InitialCreate -c EcommerceCatalogDbContext --project src\Ecommerce.CatalogService.Persistence\Ecommerce.CatalogService.Persistence.csproj
```

To apply migrations:
```bash
dotnet ef database update -c EcommerceCatalogDbContext --project src\Ecommerce.CatalogService.Persistence\Ecommerce.CatalogService.Persistence.csproj
```

## Testing

The solution includes both Unit and Integration tests demonstrating testability.