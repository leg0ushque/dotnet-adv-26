using MongoDB.Bson.Serialization.Attributes;

namespace Ecommerce.CartService.DataAccess.Entities
{
    public abstract class BaseEntity : IEntity
    {
        [BsonId]
        public required string Id { get; set; }
    }
}
