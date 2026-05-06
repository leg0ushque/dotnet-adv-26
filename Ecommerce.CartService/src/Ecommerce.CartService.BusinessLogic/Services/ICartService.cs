using Ecommerce.CartService.BusinessLogic.Dtos;
using Ecommerce.CartService.BusinessLogic.Results;
using Ecommerce.CartService.DataAccess.Entities;
using System.Threading.Tasks;

namespace Ecommerce.CartService.BusinessLogic.Services
{
    public interface ICartService : IService<Cart, CartDto>
    {
        Task<Result> AddItemToCartAsync(string cartKey, CartItemDto item);

        Task<Result> DeleteItemFromCartAsync(string cartKey, string itemId, int? amount = null);

        Task<Result<CartDto>> GetCartByKeyAsync(string cartKey);
    }
}
