namespace Ecommerce.CartService.Messaging.Handlers
{
    public interface IMessageHandler<TMessageBody>
    {
        Task HandleAsync(TMessageBody messageBody, CancellationToken cancellationToken = default);
    }
}
