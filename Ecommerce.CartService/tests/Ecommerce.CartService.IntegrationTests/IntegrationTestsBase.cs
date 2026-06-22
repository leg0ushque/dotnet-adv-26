using AutoFixture;
using AutoMapper;
using Ecommerce.CartService.BusinessLogic.Dtos;
using Ecommerce.CartService.BusinessLogic.Mappings;
using Ecommerce.CartService.BusinessLogic.Services;
using Ecommerce.CartService.BusinessLogic.Validators;
using Ecommerce.CartService.DataAccess.Entities;
using Ecommerce.CartService.DataAccess.Repositories;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Ecommerce.CartService.IntegrationTests;

public abstract class IntegrationTestsBase<TEntity, TDto, TCreateValidator, TUpdateValidator> : IAsyncLifetime
    where TEntity : IEntity
    where TDto : IDto
    where TCreateValidator : ICreateValidator<TDto>, new()
    where TUpdateValidator : IUpdateValidator<TDto>, new()
{
    protected readonly Fixture _fixture;

    protected readonly DatabaseFixture _dbFixture;

    public required IMapper _mappingService;

    public required ICreateValidator<TDto> _createValidator;
    public required IUpdateValidator<TDto> _updateValidator;

    public required IService<TEntity, TDto> _service;

    public IntegrationTestsBase(DatabaseFixture dbFixture)
    {
        _dbFixture = dbFixture;
        _fixture = new Fixture();

        EntitiesIds = [];

        _mappingService = new MapperConfiguration(cfg => 
            cfg.AddProfile(new ApplicationMappingProfile()), NullLoggerFactory.Instance)
            .CreateMapper();

        _createValidator = new TCreateValidator();
        _updateValidator = new TUpdateValidator();

        _service = CreateService();
    }

    public List<string>? EntitiesIds { get; set; }

    protected abstract IRepository<TEntity> Repository { get; }

    protected abstract IService<TEntity, TDto> CreateService();

    public virtual Task InitializeAsync() => Task.CompletedTask;

    public virtual async Task DisposeAsync()
    {
        await RemoveEntities();
    }

    public async Task GenerateEntities(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        foreach (var entity in entities)
        {
            await Repository.CreateAsync(entity, ct);

            EntitiesIds!.Add(entity.Id);
        }
    }

    public async Task RemoveEntities(CancellationToken ct = default)
    {
        foreach (var id in EntitiesIds!)
        {
            await Repository.DeleteAsync(id, ct);
        }

        EntitiesIds.Clear();
    }

    [Fact]
    public virtual async Task CreateAsync_ValidDto_PersistsEntity()
    {
        // Arrange
        var dto = _fixture.Create<TDto>();

        // Act
        await _service.CreateAsync(dto);
        EntitiesIds!.Add(dto.Id);

        // Assert
        var allDtos = await _service.GetAllAsync();
        allDtos.Should().ContainEquivalentOf(dto, options => options.Excluding(x => x.Id));
    }
}
