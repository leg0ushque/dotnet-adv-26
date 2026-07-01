using Ecommerce.CatalogService.Persistence.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Ecommerce.CatalogService.Persistence.Messaging;

public class RabbitMqConnectionFactory(IOptions<RabbitMqOptions> options)
{
    private const int _initialCount = 1;
    private const int _maxCount = 1;

    private IConnection? _connection;

    private readonly RabbitMqOptions _options = options.Value;

    private readonly SemaphoreSlim _lock = new(_initialCount, _maxCount);

    public async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
    {
        if (_connection is not null && _connection.IsOpen)
        {
            return _connection;
        }

        await _lock.WaitAsync(cancellationToken);

        try
        {
            if (_connection is not null && _connection.IsOpen)
            {
                return _connection;
            }

            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                AutomaticRecoveryEnabled = _options.AutomaticRecoveryEnabled,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(_options.NetworkRecoveryIntervalSeconds)
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken: cancellationToken);
            return _connection;
        }
        finally
        {
            _lock.Release();
        }
    }
}
