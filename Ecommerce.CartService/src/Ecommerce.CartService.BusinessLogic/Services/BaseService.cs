using Ecommerce.CartService.BusinessLogic.Dtos;
using Ecommerce.CartService.BusinessLogic.Mappings;
using Ecommerce.CartService.DataAccess.Entities;
using Ecommerce.CartService.DataAccess.Repositories;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.CartService.BusinessLogic.Services
{
    public class BaseService<TEntity, TDto>(
        IRepository<TEntity> repository,
        IValidator<TDto> validator,
        IMappingService<TEntity, TDto> mapper) : IService<TEntity, TDto>
            where TEntity : IEntity
            where TDto : IDto
    {
        protected readonly IRepository<TEntity> _repository = repository;
        protected readonly IValidator<TDto> _validator = validator;
        protected readonly IMappingService<TEntity, TDto> _mapper = mapper;

        public virtual async Task CreateAsync(TDto dto, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var entity = _mapper.Map(dto);
            await _repository.CreateAsync(entity, cancellationToken);
        }

        public virtual async Task<List<TDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var entities = await _repository.GetAllAsync(cancellationToken);
            return [.. entities.Select(_mapper.Map)];
        }

        public virtual async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            await _repository.DeleteAsync(id, cancellationToken);
        }
    }
}
