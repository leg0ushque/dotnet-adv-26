using Ecommerce.CartService.BusinessLogic.Dtos;
using Ecommerce.CartService.DataAccess.Entities;

namespace Ecommerce.CartService.BusinessLogic.Mappings
{
    public interface IMappingService<TEntity, TDto>
        where TEntity : IEntity
        where TDto : IDto
    {
        public TEntity Map(TDto src);

        public TDto Map(TEntity src);
    }
}
