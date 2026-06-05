using Ecommerce.CatalogService.Persistence.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Ecommerce.CatalogService.Persistence.Messaging
{
    public sealed class RabbitMqTopologyInitializer(
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

                // Declare exchange
                await channel.ExchangeDeclareAsync(
                    exchange: _options.ExchangeName,
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false,
                    cancellationToken: cancellationToken);

                _logger.LogInformation(
                    "RabbitMQ topology initialized. Exchange: {ExchangeName}, Type: Topic",
                    _options.ExchangeName);
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
}
