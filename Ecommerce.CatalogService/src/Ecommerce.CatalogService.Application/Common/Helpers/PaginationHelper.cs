using Ecommerce.CatalogService.Application.Common.DTOs;

namespace Ecommerce.CatalogService.Application.Common.Helpers;

public static class PaginationHelper
{
    public static PaginatedResult<T> Paginate<T>(IEnumerable<T> source, PaginationOptions options)
    {
        options.VerifyValues();
        var totalCount = source.Count();
        var items = source
            .Skip((options.PageNumber - 1) * options.PageSize)
            .Take(options.PageSize)
            .ToList();

        return new PaginatedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = options.PageNumber,
            PageSize = options.PageSize
        };
    }
}