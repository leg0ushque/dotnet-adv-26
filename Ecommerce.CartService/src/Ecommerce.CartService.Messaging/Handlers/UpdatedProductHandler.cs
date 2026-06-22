using Ecommerce.CartService.DataAccess.Repositories;
using Ecommerce.CartService.Messaging.Models;
using Microsoft.Extensions.Logging;

namespace Ecommerce.CartService.Messaging.Handlers;

public class UpdatedProductHandler : IMessageHandler<UpdatedProductMessage>
{
    private readonly IRepository<DataAccess.Entities.Cart> _cartRepository;
    private readonly ILogger<UpdatedProductHandler> _logger;

    public UpdatedProductHandler(
        IRepository<DataAccess.Entities.Cart> cartRepository,
        ILogger<UpdatedProductHandler> logger)
    {
        _cartRepository = cartRepository;
        _logger = logger;
    }

    public async Task HandleAsync(UpdatedProductMessage messageBody, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling product {ProductId} update", messageBody.Id);

        var cartsWithItem = await _cartRepository.FilterAsync(
            c => c.Items.Any(i => i.ItemId == messageBody.Id),
            cancellationToken);

        foreach (var cart in cartsWithItem)
        {
            var itemsToUpdate = cart.Items.Where(i => i.ItemId == messageBody.Id).ToList();

            foreach (var item in itemsToUpdate)
            {
                if(!string.IsNullOrEmpty(messageBody.Name))
                {
                    item.Name = messageBody.Name;
                }

                if (messageBody.Price.HasValue)
                {
                    item.Price = messageBody.Price;
                }
            }

            await _cartRepository.UpdateAsync(cart.Id, c => c.Items, cart.Items, cancellationToken);

            _logger.LogInformation(
                "Updated {Count} items in cart {CartId} for catalog item {ItemId}",
                itemsToUpdate.Count,
                cart.Id,
                messageBody.Id);
        }

        _logger.LogInformation(
            "Processed updated product {ProductId}. Updated {CartCount} carts",
            messageBody.Id,
            cartsWithItem.Count);
    }
}
