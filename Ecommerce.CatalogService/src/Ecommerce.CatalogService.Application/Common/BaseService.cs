using AutoMapper;
using Ecommerce.CatalogService.Application.Common.Interfaces;
using Ecommerce.CatalogService.Domain.Entities;
using Ecommerce.CatalogService.Domain.Exceptions;
using FluentValidation;

namespace Ecommerce.CatalogService.Application.Common
{
    public abstract class BaseService<TEntity, TDto, TCreateDto, TUpdateDto>(
        IRepository<TEntity> repository, 
        IValidator<TCreateDto> createValidator,
        IValidator<TUpdateDto> updateValidator,
        IMapper mapper) : IService<TDto, TCreateDto, TUpdateDto>
            where TEntity : class
            where TCreateDto : class
            where TUpdateDto : class
            where TDto : class
    {
        private readonly IRepository<TEntity> _repository = repository;
        private readonly IValidator<TCreateDto> _createValidator = createValidator;
        private readonly IValidator<TUpdateDto> _updateValidator = updateValidator;
        private readonly IMapper _mapper = mapper;

        public abstract void UpdateEntityDetails(TEntity entityToUpdate, TUpdateDto updateDto);

        public async Task<TDto?> GetByIdAsync(string id)
        {
            var entity = await _repository.GetByIdAsync(id);

            return entity == null ? null : _mapper.Map<TDto>(entity);
        }

        public async Task<List<TDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();

            return [.. entities.Select(_mapper.Map<TDto>)];
        }

        public async Task<string> CreateAsync(TCreateDto dto)
        {
            await _createValidator.ValidateAndThrowAsync(dto);

            var entity = _mapper.Map<TEntity>(dto);

            return await _repository.CreateAsync(entity);
        }

        public async Task UpdateAsync(string id, TUpdateDto dto)
        {
            var entity = await _repository.GetByIdAsync(id) ??
                throw new EntityNotFoundException(nameof(Product), id);

            await _updateValidator.ValidateAndThrowAsync(dto);

            UpdateEntityDetails(entity, dto);

            await _repository.UpdateAsync(entity);
        }

        public Task DeleteAsync(string id)
        {
            return _repository.DeleteByIdAsync(id);
        }
    }
}
