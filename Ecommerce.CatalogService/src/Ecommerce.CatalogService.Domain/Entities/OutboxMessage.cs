using Ecommerce.CatalogService.Domain.Common;

namespace Ecommerce.CatalogService.Domain.Entities;

public class OutboxMessage : BaseEntity
{
    public string Type { get; set; } = string.Empty;

    public string Payload { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public bool IsProcessed { get; set; }

    public string? Error { get; set; }

    public int RetryCount { get; set; }

    public OutboxMessage()
    {
        Id = Guid.NewGuid().ToString();
        CreatedAt = DateTime.UtcNow;
        IsProcessed = false;
        RetryCount = Constants.DefaultMessageRetryCount;
    }

    public OutboxMessage(string type, string payload) : this()
    {
        Type = type;
        Payload = payload;
    }

    public void MarkAsProcessed()
    {
        IsProcessed = true;
        ProcessedAt = DateTime.UtcNow;
    }

    public void IncrementRetryCount(string? error = null)
    {
        RetryCount++;
        Error = error;
    }
}
