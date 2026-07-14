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
using Microsoft.Extensions.Options;

namespace Ecommerce.CatalogService.Application.Products.Services;

public class ProductService(IRepository<Product> productRepository,
    IRepository<Category> categoryRepository,
    IValidator<CreateProductDto> createValidator,
    IValidator<UpdateProductDto> updateValidator,
    IMapper mapper,
    ITransactionManager transactionManager,
    IOutboxService outboxService,
    IOptions<JsonSerializerOptions> jsonSerializerOptions)
    : BaseService<Product, ProductDto, CreateProductDto, UpdateProductDto>(
        productRepository,
        createValidator,
        updateValidator,
        mapper,
        transactionManager), IProductService
{
    private readonly IRepository<Category> _categoryRepository = categoryRepository;
    private readonly IOutboxService _outboxService = outboxService;
    private readonly JsonSerializerOptions _jsonSerializerOptions = jsonSerializerOptions.Value;

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
            return Result.Failure(ErrorResult.NotFound(EntityName, id));
        }

        var validationResult = await _updateValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result.Failure(ErrorResult.Validation("Validation.Failed", errors));
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

    public async Task<Result<Dictionary<string, string>>> GetProductPropertiesAsync(string productId)
    {
        var product = await _repository.GetByIdAsync(productId);

        if (product == null)
        {
            return Result.Failure<Dictionary<string, string>>(ErrorResult.NotFound(EntityName, productId));
        }

        var properties = new Dictionary<string, string>
        {
            { "Id", product.Id },
            { "Name", product.Name },
            { "Price", product.Price.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) },
            { "Amount", product.Amount.ToString(System.Globalization.CultureInfo.InvariantCulture) }
        };

        if (!string.IsNullOrWhiteSpace(product.Description))
        {
            properties.Add("Description", product.Description);
        }

        if (!string.IsNullOrWhiteSpace(product.ImageUrl))
        {
            properties.Add("ImageUrl", product.ImageUrl);
        }

        var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
        if (category != null)
        {
            properties.Add("Category", category.Name);
            properties.Add("CategoryId", category.Id);

            if (!string.IsNullOrWhiteSpace(category.ParentCategoryId))
            {
                properties.Add("ParentCategoryId", category.ParentCategoryId);
            }
        }

        return Result.Success(properties);
    }

    private Task CreateOutboxMessageAsync(UpdatedProductMessage dto)
    {
        return _outboxService.AddOutboxMessageAsync(
            JsonSerializer.Serialize(dto, _jsonSerializerOptions),
            Constants.CatalogEventTypes.ProductUpdated);
    }
}
