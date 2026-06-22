namespace Ecommerce.CartService.Messaging.Handlers;

public interface IMessageHandler<TMessageBody>
{
    public Task HandleAsync(TMessageBody messageBody, CancellationToken cancellationToken = default);
}
