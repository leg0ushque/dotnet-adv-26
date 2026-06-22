using Asp.Versioning;
using Ecommerce.CatalogService.Api.Constants;
using Ecommerce.CatalogService.Api.Models;
using Ecommerce.CatalogService.Application.Common.DTOs;
using Ecommerce.CatalogService.Application.Products.DTOs;
using Ecommerce.CatalogService.Application.Products.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.CatalogService.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class ProductsController(IProductService productService) : ControllerBase
{
    private readonly IProductService _productService = productService;

    /// <summary>
    /// Gets products with optional filtering by category and pagination
    /// </summary>
    /// <param name="categoryId">Optional category ID filter</param>
    /// <param name="pagination">Pagination options</param>
    /// <returns>Paginated list of products</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<ProductDto>>> GetProducts(
        [FromQuery] string? categoryId,
        [FromQuery] PaginationOptions pagination)
    {
        pagination.VerifyValues();

        var result = await _productService.GetProductsAsync(categoryId, pagination.PageNumber, pagination.PageSize);

        return Ok(result);
    }

    /// <summary>
    /// Gets a specific product by ID with HATEOAS links (Level 3)
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product with hypermedia links</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResourceResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResourceResponse<ProductDto>>> GetProduct(string id)
    {
        var result = await _productService.GetByIdAsync(id);

        if (result.IsFailure)
        {
            return NotFound(new { error = result.Error!.Message });
        }

        var response = new ResourceResponse<ProductDto>
        {
            Data = result.Value,
            Links =
            [
                new Link { Href = Url.Action(nameof(GetProduct), new { id })!, Rel = "self", Method = HttpMethod.Get.Method },
                new Link { Href = Url.Action(nameof(UpdateProduct), new { id })!, Rel = "update", Method = HttpMethod.Put.Method },
                new Link { Href = Url.Action(nameof(DeleteProduct), new { id })!, Rel = "delete", Method = HttpMethod.Delete.Method },
                new Link { Href = Url.Action(nameof(GetProducts))!, Rel = "all-products", Method = HttpMethod.Get.Method },
                new Link { Href = Url.Action("GetCategory", "Categories", new { id = result.Value.CategoryId })!, Rel = "category", Method = HttpMethod.Get.Method }
            ]
        };

        return Ok(response);
    }

    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="createDto">Product creation data</param>
    /// <returns>Created product ID</returns>
    [HttpPost]
    [Authorize(Policy = AuthConstants.ManagerOnlyPolicy)]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<string>> CreateProduct([FromBody] CreateProductDto createDto)
    {
        var result = await _productService.CreateAsync(createDto);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.Error!.Message });
        }

        return CreatedAtAction(nameof(GetProduct), new { id = result.Value }, result.Value);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = AuthConstants.ManagerOnlyPolicy)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateProduct(string id, [FromBody] UpdateProductDto updateDto)
    {
        var result = await _productService.UpdateAsync(id, updateDto);

        if (result.IsFailure)
        {
            return result.Error!.Type switch
            {
                Application.Common.Results.ErrorType.NotFound => NotFound(new { error = result.Error.Message }),
                Application.Common.Results.ErrorType.Validation => BadRequest(new { error = result.Error.Message }),
                _ => StatusCode(500, new { error = result.Error.Message })
            };
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = AuthConstants.ManagerOnlyPolicy)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteProduct(string id)
    {
        var result = await _productService.DeleteAsync(id);

        if (result.IsFailure)
        {
            return NotFound(new { error = result.Error!.Message });
        }

        return NoContent();
    }
}
