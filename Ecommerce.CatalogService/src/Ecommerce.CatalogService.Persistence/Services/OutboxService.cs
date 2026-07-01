using Ecommerce.CatalogService.Application.Common.Interfaces;
using Ecommerce.CatalogService.Domain.Entities;
using Ecommerce.CatalogService.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ecommerce.CatalogService.Persistence.Services;

public class OutboxService(
    EcommerceCatalogDbContext context,
    IMessagePublisher messagePublisher,
    ILogger<OutboxService> logger) : IOutboxService
{
    public const int MaxRetryCount = 5;
    public const int MessagesBatchSize = 5;

    private readonly EcommerceCatalogDbContext _context = context;
    private readonly IMessagePublisher _messagePublisher = messagePublisher;
    private readonly ILogger<OutboxService> _logger = logger;

    public async Task AddOutboxMessageAsync(string payload, string eventType)
    {
        var outboxMessage = new OutboxMessage(eventType, payload);
        await _context.OutboxMessages.AddAsync(outboxMessage);
    }

    public async Task ProcessPendingMessagesAsync(CancellationToken cancellationToken)
    {
        var pendingMessages = await _context.OutboxMessages
            .Where(m => !m.IsProcessed && m.RetryCount < MaxRetryCount)
            .OrderBy(m => m.CreatedAt)
            .Take(MessagesBatchSize)
            .ToListAsync(cancellationToken);

        foreach (var message in pendingMessages)
        {
            try
            {
                await PublishMessageAsync(message, cancellationToken);

                message.MarkAsProcessed();

                _logger.LogInformation(
                    "Successfully processed outbox message {MessageId} of type {MessageType}",
                    message.Id,
                    message.Type);
            }
            catch (Exception ex)
            {
                message.IncrementRetryCount(ex.Message);

                _logger.LogError(
                    ex,
                    "Failed to process outbox message {MessageId} of type {MessageType}. Retry count: {RetryCount}",
                    message.Id,
                    message.Type,
                    message.RetryCount);
            }
        }

        if (pendingMessages.Count != default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task PublishMessageAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        await _messagePublisher.PublishAsync(message.Payload, message.Type, cancellationToken);
    }
}
