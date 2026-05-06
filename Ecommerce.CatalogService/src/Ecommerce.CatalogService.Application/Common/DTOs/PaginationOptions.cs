namespace Ecommerce.CatalogService.Application.Common.DTOs
{
    public class PaginationOptions
    {
        public const int MinPageNumber = 1;

        public const int DefaultPageSize = 10;
        public const int MinPageSize = 1;
        public const int MaxPageSize = 100;

        public int PageNumber { get; set; } = MinPageNumber;
        public int PageSize { get; set; } = DefaultPageSize;

        public void VerifyValues()
        {
            if (PageNumber < MinPageNumber) PageNumber = MinPageNumber;

            if (PageSize < MinPageSize) PageSize = MinPageSize;

            if (PageSize > MaxPageSize) PageSize = MaxPageSize;
        }
    }
}