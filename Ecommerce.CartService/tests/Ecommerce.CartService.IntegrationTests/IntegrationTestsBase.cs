using AutoFixture;
using Ecommerce.CartService.BusinessLogic.Dtos;
using Ecommerce.CartService.BusinessLogic.Mappings;
using Ecommerce.CartService.BusinessLogic.Services;
using Ecommerce.CartService.DataAccess.Entities;
using Ecommerce.CartService.DataAccess.Repositories;
using FluentAssertions;
using FluentValidation;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Ecommerce.CartService.IntegrationTests
{
    public abstract class IntegrationTestsBase<TEntity, TDto, TMappingService, TValidator> : IAsyncLifetime
        where TEntity : IEntity
        where TDto : IDto
        where TMappingService : IMappingService<TEntity, TDto>, new()
        where TValidator : IValidator<TDto>, new()
    {
        protected readonly Fixture _fixture;

        protected readonly DatabaseFixture _dbFixture;

        public required IMappingService<TEntity, TDto> _mappingService;

        public required IValidator<TDto> _validator;

        public required IService<TEntity, TDto> _service;

        public IntegrationTestsBase(DatabaseFixture dbFixture)
        {
            _dbFixture = dbFixture;
            _fixture = new Fixture();

            EntitiesIds = [];

            _mappingService = new TMappingService();
            _validator = new TValidator();
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
}
