using System.Text.Json;
using AutoMapper;
using Ecommerce.CatalogService.Application.Common;
using Ecommerce.CatalogService.Application.Common.DTOs;
using Ecommerce.CatalogService.Application.Common.DTOs.QueueMessages;
using Ecommerce.CatalogService.Application.Common.Helpers;
using Ecommerce.CatalogService.Application.Common.Interfaces;
using Ecommerce.CatalogService.Application.Common.Results;
using Ecommerce.CatalogService.Application.Products.DTOs;
using Ecommerce.CatalogService.Application.Products.Interfaces;
using Ecommerce.CatalogService.Domain;
using Ecommerce.CatalogService.Domain.Entities;
using FluentValidation;

namespace Ecommerce.CatalogService.Application.Products.Services;

public class ProductService(IRepository<Product> productRepository,
    IValidator<CreateProductDto> createValidator,
    IValidator<UpdateProductDto> updateValidator,
    IMapper mapper,
    ITransactionManager transactionManager,
    IOutboxService outboxService)
    : BaseService<Product, ProductDto, CreateProductDto, UpdateProductDto>(
        productRepository,
        createValidator,
        updateValidator,
        mapper,
        transactionManager), IProductService
{
    private readonly IOutboxService _outboxService = outboxService;

    protected override string EntityName => "Product";

    public override void UpdateEntityDetails(Product entityToUpdate, UpdateProductDto updateDto)
        => entityToUpdate.UpdateDetails(
            updateDto.Name,
            updateDto.CategoryId,
            updateDto.Price,
            updateDto.Amount,
            updateDto.Description,
            updateDto.ImageUrl);

    public override async Task<Result> UpdateAsync(string id, UpdateProductDto dto)
    {
        var product = await _repository.GetByIdAsync(id);

        if (product == null)
        {
            return Result.Failure(Error.NotFound(EntityName, id));
        }

        var validationResult = await _updateValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result.Failure(Error.Validation("Validation.Failed", errors));
        }

        try
        {
            await _transactionManager.BeginTransactionAsync();

            UpdateEntityDetails(product, dto);
            await _repository.UpdateAsync(product);

            var message = _mapper.Map<UpdatedProductMessage>(dto);
            message.Id = id;
            await CreateOutboxMessageAsync(message);

            await _transactionManager.SaveChangesAsync();
            await _transactionManager.CommitTransactionAsync();

            return Result.Success();
        }
        catch
        {
            await _transactionManager.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<PaginatedResult<ProductDto>> GetProductsAsync(string? categoryId, int pageNumber, int pageSize)
    {
        var allProducts = string.IsNullOrWhiteSpace(categoryId)
            ? await _repository.GetAllAsync()
            : await _repository.GetAllAsync(p => p.CategoryId == categoryId);

        var productDtos = _mapper.Map<List<ProductDto>>(allProducts);

        var paginated = PaginationHelper.Paginate(productDtos,
            new PaginationOptions { PageNumber = pageNumber, PageSize = pageSize });

        return paginated;
    }

    private Task CreateOutboxMessageAsync(UpdatedProductMessage dto)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        return _outboxService.AddOutboxMessageAsync(
            JsonSerializer.Serialize(dto, options),
            Constants.CatalogEventTypes.ProductUpdated);
    }
}
