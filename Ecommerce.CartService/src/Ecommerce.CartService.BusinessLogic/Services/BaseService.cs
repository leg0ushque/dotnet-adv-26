using AutoMapper;
using Ecommerce.CartService.BusinessLogic.Dtos;
using Ecommerce.CartService.BusinessLogic.Results;
using Ecommerce.CartService.BusinessLogic.Validators;
using Ecommerce.CartService.DataAccess.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.CartService.BusinessLogic.Services
{
    public abstract class BaseService<TEntity, TDto>(
        IRepository<TEntity> repository,
        ICreateValidator<TDto> createValidator,
        IUpdateValidator<TDto> updateValidator,
        IMapper mapper) : IService<TEntity, TDto>
            where TEntity : class
            where TDto : class, IDto
    {
        private readonly IRepository<TEntity> _repository = repository;
        private readonly ICreateValidator<TDto> _createValidator = createValidator;
        private readonly IUpdateValidator<TDto> _updateValidator = updateValidator;
        private readonly IMapper _mapper = mapper;

        protected abstract string EntityName { get; }

        public async Task<Result<string>> CreateAsync(TDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));

                return Result.Failure<string>(Error.Validation("Validation.Failed", errors));
            }

            var entity = _mapper.Map<TEntity>(dto);

            var id = await _repository.CreateAsync(entity);

            return Result.Success(id);
        }

        public async Task<List<TDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();

            return [.. entities.Select(_mapper.Map<TDto>)];
        }

        public async Task<Result<TDto>> GetByIdAsync(string id)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null)
                return Result.Failure<TDto>(Error.NotFound(EntityName, id));

            return Result.Success(_mapper.Map<TDto>(entity));
        }

        public async Task<Result> UpdateAsync(string id, TDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null)
                return Result.Failure(Error.NotFound(EntityName, id));

            var validationResult = await _updateValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return Result.Failure(Error.Validation("Validation.Failed", errors));
            }

            await _repository.UpdateAsync(id, entity);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(string id)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null)
                return Result.Failure(Error.NotFound(EntityName, id));

            await _repository.DeleteAsync(id);

            return Result.Success();
        }

    }
}