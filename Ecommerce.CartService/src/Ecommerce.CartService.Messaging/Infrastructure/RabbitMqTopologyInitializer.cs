using Ecommerce.CartService.Messaging.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Ecommerce.CartService.Messaging.Infrastructure;

public class RabbitMqTopologyInitializer(
    RabbitMqConnectionFactory connectionFactory,
    IOptions<RabbitMqOptions> options,
    ILogger<RabbitMqTopologyInitializer> logger) : IHostedService
{
    private readonly RabbitMqConnectionFactory _connectionFactory = connectionFactory;
    private readonly RabbitMqOptions _options = options.Value;
    private readonly ILogger<RabbitMqTopologyInitializer> _logger = logger;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var connection = await _connectionFactory.GetConnectionAsync(cancellationToken);
            await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await channel.ExchangeDeclareAsync(
                exchange: _options.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken);

            await channel.QueueDeclareAsync(
                queue: _options.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: cancellationToken);

            var productUpdatedRoutingKey = _options.RoutingKeyPrefix + 
                Constants.CatalogEventTypes.ProductUpdated.ToLowerInvariant();

            await channel.QueueBindAsync(
                queue: _options.QueueName,
                exchange: _options.ExchangeName,
                routingKey: productUpdatedRoutingKey,
                cancellationToken: cancellationToken);

            _logger.LogInformation(
                "RabbitMQ topology initialized. Exchange: {ExchangeName}, Queue: {QueueName}, RoutingKey: {RoutingKey}, Type: Topic",
                _options.ExchangeName,
                _options.QueueName,
                productUpdatedRoutingKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize RabbitMQ topology");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("RabbitMQ topology initializer stopped");
        return Task.CompletedTask;
    }
}
