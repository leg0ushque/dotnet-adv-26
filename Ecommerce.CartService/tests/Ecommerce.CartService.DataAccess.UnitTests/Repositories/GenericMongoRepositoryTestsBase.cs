using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Ecommerce.CartService.DataAccess.Entities;
using Ecommerce.CartService.DataAccess.Factories;
using Ecommerce.CartService.DataAccess.Repositories;
using FluentAssertions;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace Ecommerce.CartService.DataAccess.UnitTests.Repositories;

public abstract class GenericMongoRepositoryTestsBase<TEntity>
    where TEntity : class, IEntity
{
    public required IFixture fixture;

    public required List<TEntity> entities;

    public required Mock<IAsyncCursor<TEntity>> asyncCursorMock;
    public required Mock<IMongoDbFactory> mongoDbFactoryMock;
    public required Mock<IMongoCollection<TEntity>> mongoCollectionMock;

    public required IRepository<TEntity> repository;

    public abstract IRepository<TEntity> CreateRepository(IMongoDbFactory factory);

    protected GenericMongoRepositoryTestsBase()
    {
        Setup();
    }

    public void Setup()
    {
        fixture = new Fixture();

        mongoDbFactoryMock = new Mock<IMongoDbFactory>();
        mongoCollectionMock = new Mock<IMongoCollection<TEntity>>();

        entities = [.. fixture.Build<TEntity>().With(x => x.Id, Guid.NewGuid().ToString()).CreateMany<TEntity>(5)];

        asyncCursorMock = new Mock<IAsyncCursor<TEntity>>();

        asyncCursorMock
            .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);
        asyncCursorMock
            .SetupGet(_ => _.Current)
            .Returns(entities);

        asyncCursorMock.SetupGet(_ => _.Current).Returns(entities);

        mongoCollectionMock.Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<TEntity>>(),
                It.IsAny<FindOptions<TEntity, TEntity>>(),
                It.IsAny<CancellationToken>()))
          .ReturnsAsync(asyncCursorMock.Object);

        mongoCollectionMock.Setup(x =>
            x.InsertOneAsync(
                It.IsAny<TEntity>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback<TEntity, InsertOneOptions, CancellationToken>((entity, options, ct) =>
            {
                entities.Add(entity);
            });

        var updatedEntity = entities.FirstOrDefault();

        mongoCollectionMock.Setup(x =>
            x.DeleteOneAsync(
                It.IsAny<FilterDefinition<TEntity>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Mock<DeleteResult>().Object)
            .Callback<FilterDefinition<TEntity>, CancellationToken>((filter, ct) =>
            {
                entities.Remove(updatedEntity!);
            });

        mongoDbFactoryMock.Setup(x => x.GetCollection<TEntity>(It.IsAny<string>()))
            .Returns(mongoCollectionMock.Object);

        repository = CreateRepository(mongoDbFactoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_WhenInvoked_ShouldCallFindAsync()
    {
        var result = await repository.GetAllAsync();

        mongoCollectionMock.Verify(x =>
            x.FindAsync(
                It.Is<ExpressionFilterDefinition<TEntity>>(_ => true),
                It.IsAny<FindOptions<TEntity, TEntity>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        result.Should().BeEquivalentTo(entities);
    }

    [Fact]
    public async Task CreateAsync_WhenInvoked_ShouldCallInsertOneAsync()
    {
        var beforeCountValue = entities.Count;

        var newEntity = fixture.Create<TEntity>();

        await repository.CreateAsync(newEntity);

        mongoCollectionMock.Verify(x =>
            x.InsertOneAsync(
                It.IsAny<TEntity>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        entities.Should().HaveCount(beforeCountValue + 1);

        entities.Should().ContainEquivalentOf(newEntity);
    }

    [Fact]
    public async Task DeleteAsync_WhenInvoked_ShouldCallDeleteOneAsync()
    {
        var existingEntity = entities.FirstOrDefault()!;

        await repository.DeleteAsync(existingEntity.Id);

        mongoCollectionMock.Verify(x =>
            x.DeleteOneAsync(
                It.IsAny<FilterDefinition<TEntity>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        entities.Should().NotContain(existingEntity);
    }
}
