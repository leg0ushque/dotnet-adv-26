using Asp.Versioning;
using Ecommerce.CatalogService.Api.Constants;
using Ecommerce.CatalogService.Api.Models;
using Ecommerce.CatalogService.Application.Categories.DTOs;
using Ecommerce.CatalogService.Application.Categories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.CatalogService.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class CategoriesController(ICategoryService categoryService) : ControllerBase
    {
        private readonly ICategoryService _categoryService = categoryService;

        [HttpGet]
        [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<CategoryDto>>> GetCategories()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Gets a specific category by ID with HATEOAS links (Level 3)
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Category with hypermedia links</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResourceResponse<CategoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResourceResponse<CategoryDto>>> GetCategory(string id)
        {
            var result = await _categoryService.GetByIdAsync(id);

            if (result.IsFailure)
                return NotFound(new { error = result.Error!.Message });

            var response = new ResourceResponse<CategoryDto>
            {
                Data = result.Value,
                Links =
                [
                    new Link { Href = Url.Action(nameof(GetCategory), new { id })!, Rel = "self", Method = HttpMethod.Get.Method },
                    new Link { Href = Url.Action(nameof(UpdateCategory), new { id })!, Rel = "update", Method = HttpMethod.Put.Method },
                    new Link { Href = Url.Action(nameof(DeleteCategory), new { id })!, Rel = "delete", Method = HttpMethod.Delete.Method },
                    new Link { Href = Url.Action(nameof(GetCategories))!, Rel = "all-categories", Method = HttpMethod.Get.Method }
                ]
            };

            return Ok(response);
        }

        /// <summary>
        /// Creates a new category
        /// </summary>
        /// <param name="createDto">Category creation data</param>
        /// <returns>Created category ID</returns>
        [HttpPost]
        [Authorize(Policy = AuthConstants.ManagerOnlyPolicy)]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<string>> CreateCategory([FromBody] CreateCategoryDto createDto)
        {
            var result = await _categoryService.CreateAsync(createDto);

            if (result.IsFailure)
                return BadRequest(new { error = result.Error!.Message });

            return CreatedAtAction(nameof(GetCategory), new { id = result.Value }, result.Value);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = AuthConstants.ManagerOnlyPolicy)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateCategory(string id, [FromBody] UpdateCategoryDto updateDto)
        {
            var result = await _categoryService.UpdateAsync(id, updateDto);

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

        /// <summary>
        /// Deletes a category and all related products
        /// </summary>
        /// <param name="id">Category ID</param>
        [HttpDelete("{id}")]
        [Authorize(Policy = AuthConstants.ManagerOnlyPolicy)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            var result = await _categoryService.DeleteCategoryWithProductsAsync(id);

            if (result.IsFailure)
                return NotFound(new { error = result.Error!.Message });

            return NoContent();
        }
    }
}
