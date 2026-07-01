using Ecommerce.CartService.Messaging.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Ecommerce.CartService.Messaging.Infrastructure;

public class RabbitMqConnectionFactory(IOptions<RabbitMqOptions> options)
{
    private const int InitialCount = 1;
    private const int MaxCount = 1;

    private IConnection? _connection;
    private readonly RabbitMqOptions _options = options.Value;
    private readonly SemaphoreSlim _lock = new(InitialCount, MaxCount);

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
