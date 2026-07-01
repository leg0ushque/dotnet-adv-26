using Ecommerce.CatalogService.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ecommerce.CatalogService.Persistence.BackgroundServices;

public class OutboxProcessorBackgroundService(
    IServiceProvider serviceProvider,
    ILogger<OutboxProcessorBackgroundService> logger) : BackgroundService
{
    public const int DelayIntervalSeconds = 10;

    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<OutboxProcessorBackgroundService> _logger = logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(DelayIntervalSeconds);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox Processor Background Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var outboxService = scope.ServiceProvider.GetRequiredService<IOutboxService>();

                await outboxService.ProcessPendingMessagesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing outbox messages");
            }

            await Task.Delay(_interval, stoppingToken);
        }

        _logger.LogInformation("Outbox Processor Background Service stopped");
    }
}
