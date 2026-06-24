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

### Available Services

1. **[SQL Server](https://hub.docker.com/_/microsoft-mssql-server)**  
    - Microsoft SQL Server 2019 running on Linux container  
    - Exposed on port **1433**

2. **[Adminer](https://hub.docker.com/_/adminer)**  
    - Database management tool for SQL databases  
    - Accessible at **[http://localhost:8082](http://localhost:8082/)**  

3. **[MongoDB](https://hub.docker.com/_/mongo)**  
    - NoSQL document database  
    - Exposed on port **27017**  

4. **[Mongo Express](https://hub.docker.com/_/mongo-express)**  
    - Web-based MongoDB admin interface  
    - Accessible at **[http://localhost:8081](http://localhost:8081/)**  
    
5. **[RabbitMQ (with Management UI)](https://hub.docker.com/_/rabbitmq)**  
    - Message broker with management plugin enabled  
    - AMQP port: **5672**  
    - Management UI: **[http://localhost:15672](http://localhost:15672/)**  
    
6. **[Keycloak](https://hub.docker.com/r/quay.io/keycloak/keycloak)**  
    - Open-source identity and access management  
    - Accessible at **[http://localhost:8090](http://localhost:8090/)**  

7. SonarQube Analysis
    -  Code quality analysis & metrics  
    - Accessible at **[http://localhost:9000](http://localhost:9000/)**  
---
# Security. Auth

This project uses Keycloak as the Identity Management System (IMS) for authentication and authorization. Keycloak issues JWT tokens, manages user roles, and supports refresh tokens. All configuration is managed via JSON for reproducibility and automation - see `Ecommerce-realm.json` in the root of the repository. 

Access **Keycloak Admin Console** at [http://localhost:8090](http://localhost:8090/) (by default)

Warning!  
If database table doesn't exist - keycloak container will not work properly. Make sure SQL Server container works and contains `keycloakdb` database, use `docker restart keycloak` if needed.  

Please, do not forget to import realm (one-time action) using the following command

```
docker exec -it keycloak /opt/keycloak/bin/kc.sh import --file /opt/keycloak/data/import/Ecommerce-realm.json
```

Note that coldstart of Keycloak may take some time.  
## Token Management

### Obtain Access and Refresh Tokens

**Endpoint:**  
`POST http://localhost:8090/realms/Ecommerce/protocol/openid-connect/token`

**Parameters (x-www-form-urlencoded):**

- `client_id`: e.g., `catalog-service`
- `client_secret`: e.g., `catalog-secret`
- `grant_type`: `password`
- `username`: e.g., `manager1`
- `password`: e.g., `managerpass`

```
{
  "client_id": "catalog-service",
  "client_secret": "catalog-secret",
  "grant_type": "password",
  "username": "manager1",
  "password": "managerpass"
}
```

**Example (using curl):

```
curl -X POST "http://localhost:8090/realms/Ecommerce/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "client_id=catalog-service" \
  -d "client_secret=catalog-secret" \
  -d "grant_type=password" \
  -d "username=manager1" \
  -d "password=managerpass"
```

**Response example:**
```
{
  "access_token": "...",
  "refresh_token": "...",
  "expires_in": 300,
  "refresh_expires_in": 1800,
  ...
}
```

---
### Refresh Token

**Endpoint:**  
`POST http://localhost:8090/realms/Ecommerce/protocol/openid-connect/token`

**Parameters:**

- `grant_type`: `refresh_token`
- `refresh_token`: (your refresh token)
- `client_id`, `client_secret` as above

```
{
  "client_id": "catalog-service",
  "client_secret": "catalog-secret",
  "grant_type": "refresh_token",
  "refresh_token": "YOUR_REFRESH_TOKEN"
}
```

**Example:**
```
curl -X POST "http://localhost:8090/realms/Ecommerce/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "client_id=catalog-service" \
  -d "client_secret=catalog-secret" \
  -d "grant_type=refresh_token" \
  -d "refresh_token=YOUR_REFRESH_TOKEN"
```

Response example
```
{
  "access_token": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expires_in": 300,
  "refresh_expires_in": 1800,
  "refresh_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "token_type": "Bearer",
  "not-before-policy": 0,
  "session_state": "b1e2c3d4-5678-90ab-cdef-1234567890ab",
  "scope": "profile email"
}
```

---

### Logout (Invalidate Refresh Token)

**Endpoint:**  
`POST http://localhost:8090/realms/Ecommerce/protocol/openid-connect/logout`

**Parameters:**

- `client_id`
- `client_secret`
- `refresh_token`

```
{
  "client_id": "catalog-service",
  "client_secret": "catalog-secret",
  "refresh_token": "YOUR_REFRESH_TOKEN"
}
```

Response example: HTTP 204 (No content)

---
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


---
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

The solution includes both Unit and Integration tests demonstrating testability. Make sure Keycloak is running because some projects' API controllers will require authentication during test runs.

# SonarQube Analysis

#### Prerequisites:  

1) Install sonarscanner dotnet tool using  
```powershell
dotnet tool install --global dotnet-sonarscanner
```

2) Make sure sonarqube DB exists in SQL server (go to Adminer **[http://localhost:8082](http://localhost:8082/)** and check). Run `docker-compose up -d sql-init-sonarqube` if no database exists.

#### Startup

You can run SonarQube locally using
```powershell
docker compose --profile sonar up -d
```

"Sonar" profile allows to manipulate only sonar-related containers.

Then open  **[http://localhost:9000](http://localhost:9000/)** and login as admin/admin. Change password if required, remember about 12 symbols and other requirements.

To run analisys it's required to generate a token at **My Account → Security → Generate Token** (with option Global Analisys). Paste the token into both config files:
    - `sonar/sonar-cart.json`
    - `sonar/sonar-catalog.json`

#### Run Analysis

Use appropriate command to run analisys.
```powershell
.\sonar-analyze-cart.ps1

.\sonar-analyze-catalog.ps1
```

Results will be available at `http://localhost:9000/projects`

### Stop SonarQube When Done

After the analisys done and results are gathered, please, stop SonarQube as it uses min ~2.5GB of RAM, which is not really effective for development & debugging.

```powershell
docker compose --profile sonar stop
```

####  Known Issues & Fixes

**WSL2 Memory Usage**

You may see an error in SonarQube container logs like

```
bootstrap check failure [1] of [1]: max virtual memory areas vm.max_map_count [65530] is too low, increase to at least [262144]; for more information see [https://www.elastic.co/guide/en/elasticsearch/reference/8.19/bootstrap-checks-max-map-count.html]
```

This occurs because of SonarQube + full stack can consume **5.5GB+ RAM** and default memory setting (65k) is not enough. Temporary solution is to run the following command in powershell

```
wsl -d docker-desktop -u root sysctl -w vm.max_map_count=262144
```

Warning! This can increase your **vmmemwsl** RAM usage up to 8.7-10 GB. Stop SonarQube when it is not required.

Alternative way: cap WSL2 memory by creating/editing `C:\Users\<YourUser>\.wslconfig`:

```
[wsl2]
memory=5GB
processors=2
swap=2GB
kernelCommandLine = sysctl.vm.max_map_count=262144
```

Recommended  `memory=4GB` for at least 8GB RAM,  `memory=5GB` for 16GB RAM.

Then restart WSL:

```powershell
wsl --shutdown
# After the command, restart Docker / Rancher Desktop from system tray
```