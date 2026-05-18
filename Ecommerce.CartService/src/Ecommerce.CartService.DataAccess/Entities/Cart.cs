using System.Collections.Generic;

namespace Ecommerce.CartService.DataAccess.Entities
{
    public class Cart : BaseEntity
    {
        public List<CartItem> Items { get; set; } = [];
    }
}
