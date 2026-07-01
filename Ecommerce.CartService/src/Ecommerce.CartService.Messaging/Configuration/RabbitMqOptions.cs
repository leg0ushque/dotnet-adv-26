namespace Ecommerce.CartService.Messaging.Configuration;

public class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    public string HostName { get; set; } = "localhost";

    public int Port { get; set; } = 5672;

    public string UserName { get; set; } = "guest";

    public string Password { get; set; } = "guest";

    public string ExchangeName { get; set; } = "catalog_events";

    public string QueueName { get; set; } = "catalog_queue";

    public string RoutingKeyPrefix { get; set; } = "catalog.";

    public bool AutomaticRecoveryEnabled { get; set; } = true;

    public int NetworkRecoveryIntervalSeconds { get; set; } = 10;
}
