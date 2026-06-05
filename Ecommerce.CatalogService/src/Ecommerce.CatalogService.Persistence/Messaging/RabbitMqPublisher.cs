using Ecommerce.CatalogService.Application.Common.Interfaces;
using Ecommerce.CatalogService.Persistence.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;

namespace Ecommerce.CatalogService.Persistence.Messaging
{
    public class RabbitMqPublisher(
        RabbitMqConnectionFactory connectionFactory,
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqPublisher> logger) : IMessagePublisher
    {
        private readonly RabbitMqConnectionFactory _connectionFactory = connectionFactory;
        private readonly RabbitMqOptions _options = options.Value;
        private readonly ILogger<RabbitMqPublisher> _logger = logger;

        public async Task PublishAsync(string serializedBody, string eventTypeName, CancellationToken cancellationToken = default)
        {
            var connection = await _connectionFactory.GetConnectionAsync(cancellationToken);

            await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            var routingKey = _options.RoutingKeyPrefix + eventTypeName.ToLowerInvariant();

            var body = Encoding.UTF8.GetBytes(serializedBody);

            var props = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json",
                Type = eventTypeName,
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            };

            await channel.BasicPublishAsync(
                exchange: _options.ExchangeName,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: props,
                body: body,
                cancellationToken: cancellationToken);

            _logger.LogInformation(
                "Published message to RabbitMQ. Exchange: {Exchange}, RoutingKey: {RoutingKey}, Type: {EventType}",
                _options.ExchangeName,
                routingKey,
                eventTypeName);
        }
    }
}
