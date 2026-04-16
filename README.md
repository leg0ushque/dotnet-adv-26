Self-check answers are available [here](/self-checks.md)
# Ecommerce.CartService

## Test runs

```bash
# Start MongoDB first!
docker-compose up -d

# Then run tests
dotnet test
```

**Important**: Integration tests won't work without Docker Compose running. MongoDB needs to be at `localhost:27017`.

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

