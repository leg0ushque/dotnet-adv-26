using Ecommerce.CartService.Messaging.Configuration;
using Ecommerce.CartService.Messaging.Handlers;
using Ecommerce.CartService.Messaging.Infrastructure;
using Ecommerce.CartService.Messaging.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Ecommerce.CartService.Messaging.Consumers
{
    public class UpdatedProductConsumer(
        IMessageHandler<UpdatedProductMessage> messageHandler,
        RabbitMqConnectionFactory connectionFactory,
        IOptions<RabbitMqOptions> options,
        ILogger<UpdatedProductConsumer> logger) : BackgroundService
    {
        private readonly IMessageHandler<UpdatedProductMessage> _messageHandler = messageHandler;
        private readonly RabbitMqConnectionFactory _connectionFactory = connectionFactory;
        private readonly RabbitMqOptions _options = options.Value;
        private readonly ILogger<UpdatedProductConsumer> _logger = logger;

        private IChannel? _channel;
        private CancellationToken _cancellationToken;

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;

            _logger.LogInformation("Updated Product Consumer started. Listening for {EventType} events only.", 
                Constants.CatalogEventTypes.ProductUpdated);

            var connection = await _connectionFactory.GetConnectionAsync(cancellationToken);

            _channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            var routingKey = _options.RoutingKeyPrefix + Constants.CatalogEventTypes.ProductUpdated.ToLowerInvariant();

            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: cancellationToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += HandleMessageAsync;

            try
            {
                await _channel.BasicConsumeAsync(
                    queue: _options.QueueName,
                    autoAck: false,
                    consumer: consumer,
                    cancellationToken: cancellationToken);

                _logger.LogInformation("Consumer registered for queue: {QueueName}", _options.QueueName);

                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            finally
            {
                consumer.ReceivedAsync -= HandleMessageAsync;

                if (_channel != null)
                {
                    await _channel.CloseAsync();
                }

                _logger.LogInformation("Updated Product Consumer stopped");
            }
        }

        private async Task HandleMessageAsync(object sender, BasicDeliverEventArgs ea)
        {
            if (_channel == null)
            {
                _logger.LogError("Channel is not initialized");
                return;
            }

            var deliveryTag = ea.DeliveryTag;

            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var eventType = ea.BasicProperties.Type;
                var routingKey = ea.RoutingKey;

                _logger.LogInformation(
                    "Received message. EventType: {EventType}, RoutingKey: {RoutingKey}", 
                    eventType, 
                    routingKey);

                if (eventType != Constants.CatalogEventTypes.ProductUpdated)
                {
                    _logger.LogWarning("Unexpected event type: {EventType}. Expected: {ExpectedType}. Ignoring message.", 
                        eventType, 
                        Constants.CatalogEventTypes.ProductUpdated);

                    await _channel.BasicAckAsync(deliveryTag: deliveryTag, multiple: false, cancellationToken: _cancellationToken);

                    return;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                };

                var messageBody = JsonSerializer.Deserialize<UpdatedProductMessage>(message, options);

                if (messageBody == null)
                {
                    _logger.LogError("Failed to deserialize message to UpdatedProductMessage");
                    await _channel.BasicAckAsync(deliveryTag: deliveryTag, multiple: false, cancellationToken: _cancellationToken);
                    return;
                }

                await _messageHandler.HandleAsync(messageBody, _cancellationToken);

                await _channel.BasicAckAsync(deliveryTag: deliveryTag, multiple: false, cancellationToken: _cancellationToken);

                _logger.LogInformation("Message processed successfully for ItemId: {ItemId}", messageBody.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message. DeliveryTag: {DeliveryTag}", deliveryTag);

                await _channel.BasicNackAsync(deliveryTag: deliveryTag, multiple: false, requeue: true, cancellationToken: _cancellationToken);
            }
        }
    }
}
