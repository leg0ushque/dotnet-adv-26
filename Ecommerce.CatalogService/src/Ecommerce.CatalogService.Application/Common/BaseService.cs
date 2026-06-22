using AutoMapper;
using Ecommerce.CatalogService.Application.Common.Interfaces;
using Ecommerce.CatalogService.Application.Common.Results;
using FluentValidation;

namespace Ecommerce.CatalogService.Application.Common;

public abstract class BaseService<TEntity, TDto, TCreateDto, TUpdateDto>(
    IRepository<TEntity> repository,
    IValidator<TCreateDto> createValidator,
    IValidator<TUpdateDto> updateValidator,
    IMapper mapper,
    ITransactionManager transactionManager) : IService<TDto, TCreateDto, TUpdateDto>
        where TEntity : class
        where TCreateDto : class
        where TUpdateDto : class
        where TDto : class
{
    protected readonly IRepository<TEntity> _repository = repository;
    protected readonly IValidator<TCreateDto> _createValidator = createValidator;
    protected readonly IValidator<TUpdateDto> _updateValidator = updateValidator;
    protected readonly IMapper _mapper = mapper;
    protected readonly ITransactionManager _transactionManager = transactionManager;

    protected abstract string EntityName { get; }

    public abstract void UpdateEntityDetails(TEntity entityToUpdate, TUpdateDto updateDto);

    public async Task<Result<TDto>> GetByIdAsync(string id)
    {
        var entity = await _repository.GetByIdAsync(id);

        if (entity == null)
        {
            return Result.Failure<TDto>(Error.NotFound(EntityName, id));
        }

        return Result.Success(_mapper.Map<TDto>(entity));
    }

    public async Task<List<TDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();

        return [.. entities.Select(_mapper.Map<TDto>)];
    }

    public async Task<Result<string>> CreateAsync(TCreateDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));

            return Result.Failure<string>(Error.Validation("Validation.Failed", errors));
        }

        var entity = _mapper.Map<TEntity>(dto);
        var id = await _repository.CreateAsync(entity);

        await _transactionManager.SaveChangesAsync();

        return Result.Success(id);
    }

    public virtual async Task<Result> UpdateAsync(string id, TUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);

        if (entity == null)
        {
            return Result.Failure(Error.NotFound(EntityName, id));
        }

        var validationResult = await _updateValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result.Failure(Error.Validation("Validation.Failed", errors));
        }

        UpdateEntityDetails(entity, dto);
        await _repository.UpdateAsync(entity);

        await _transactionManager.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(string id)
    {
        var entity = await _repository.GetByIdAsync(id);

        if (entity == null)
        {
            return Result.Failure(Error.NotFound(EntityName, id));
        }

        await _repository.DeleteByIdAsync(id);

        await _transactionManager.SaveChangesAsync();

        return Result.Success();
    }
}
